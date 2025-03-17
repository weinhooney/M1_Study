using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using Data;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;
using TypeConverter = Unity.VisualScripting.YamlDotNet.Serialization.Utilities.TypeConverter;

public class DataTransformer : EditorWindow
{
#if UNITY_EDITOR
    [MenuItem("Tools/ParseExcel %#K")]
    public static void ParseExcelDataToJson()
    {
        ParseExcelDataToJson<MonsterDataLoader, MonsterData>("Monster");
        ParseExcelDataToJson<HeroDataLoader, HeroData>("Hero");
        ParseExcelDataToJson<SkillDataLoader, SkillData>("skill");
        ParseExcelDataToJson<ProjectileDataLoader, ProjectileData>("Projectile");
        ParseExcelDataToJson<EnvDataLoader, EnvData>("Env");

        Debug.Log("DataTransformer Completed");
    }

    #region Helpers

    private static void ParseExcelDataToJson<Loader, LoaderData>(string fileName) where Loader : new() where LoaderData : new()
    {
        Loader loader = new Loader();
        FieldInfo field = loader.GetType().GetFields()[0];
        field.SetValue(loader, ParseExcelDataToList<LoaderData>(fileName));

        string jsonStr = JsonConvert.SerializeObject(loader, Formatting.Indented);
        File.WriteAllText($"{Application.dataPath}/@Resources/Data/JsonData/{fileName}Data.json", jsonStr);
        AssetDatabase.Refresh();
    }

    private static List<LoaderData> ParseExcelDataToList<LoaderData>(string fileName) where LoaderData : new()
    {
        List<LoaderData> loaderDatas = new List<LoaderData>();

        string[] lines = File.ReadAllText($"{Application.dataPath}/@Resources/Data/ExcelData/{fileName}Data.csv").Split("\n");

        for (int l = 1; l < lines.Length; ++l)
        {
            string[] row = lines[l].Replace("\r", "").Split(',');
            if (row.Length == 0)
            {
                continue;
            }

            if (string.IsNullOrEmpty(row[0]))
            {
                continue;
            }

            LoaderData loaderData = new LoaderData();
            var fields = GetFieldsInBase(typeof(LoaderData));
           
            for (int f = 0; f < fields.Count; ++f)
            {
                FieldInfo field = loaderData.GetType().GetField(fields[f].Name);
                Type type = field.FieldType;

                if (type.IsGenericType)
                {
                    object value = ConvertList(row[f], type);
                    field.SetValue(loaderData, value);
                }
                else
                {
                    object value = ConvertValue(row[f], field.FieldType);
                    field.SetValue(loaderData, value);
                }
            }

            loaderDatas.Add(loaderData);
        }

        return loaderDatas;
    }

    private static object ConvertValue(string value, Type type)
    {
        if (string.IsNullOrEmpty(value))
        {
            return null;
        }

        var converter = TypeDescriptor.GetConverter(type);
        try
        {
            return converter.ConvertFromString(value);
        }
        catch (Exception e)
        {
            int temp = 0;
            return null;
        }
    }

    private static object ConvertList(string value, Type type)
    {
        if (string.IsNullOrEmpty(value))
        {
            return null;
        }
        
        // Reflection
        Type valueType = type.GetGenericArguments()[0];
        Type genericListType = typeof(List<>).MakeGenericType(valueType);
        var genericList = Activator.CreateInstance(genericListType) as IList;
        
        // Parse Excel
        var list = value.Split('&').Select(x => ConvertValue(x, valueType)).ToList();

        foreach (var item in list)
        {
            genericList.Add(item);
        }

        return genericList;
    }

    public static List<FieldInfo> GetFieldsInBase(Type type, BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
    {
        List<FieldInfo> fields = new List<FieldInfo>();
        HashSet<string> fieldNames = new HashSet<string>(); // 중복 방지
        Stack<Type> stack = new Stack<Type>();

        while (type != typeof(object))
        {
            stack.Push(type);
            type = type.BaseType;
        }

        while (stack.Count > 0)
        {
            Type currentType = stack.Pop();

            foreach (var field in currentType.GetFields(bindingFlags))
            {
                if (fieldNames.Add(field.Name))
                {
                    fields.Add(field);
                }
            }
        }

        return fields;
    }
    #endregion
#endif
}