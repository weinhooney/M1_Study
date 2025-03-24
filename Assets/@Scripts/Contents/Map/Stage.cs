using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public struct ObjectSpawnInfo
{
    public ObjectSpawnInfo(string name, int dataId, int x, int y, Vector3 worldPos, EObjectType type)
    {
        Name = name;
        DataId = dataId;
        Vector3Int pos = new Vector3Int(x, y, 0);
        CellPos = pos;
        WorldPos = worldPos;
        ObjectType = type;
    }

    public string Name;
    public int DataId;
    public Vector3Int CellPos;
    public Vector3 WorldPos;
    public EObjectType ObjectType;
}

public class Stage : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
