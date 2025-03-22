using System.Collections;
using System.Collections.Generic;
using Data;
using Newtonsoft.Json;
using UnityEngine;

public interface ILoader<Key, Value>
{
    Dictionary<Key, Value> MakeDict();
}

public class DataManager
{
    public Dictionary<int, MonsterData> MonsterDict { get; private set; } = new Dictionary<int, MonsterData>();
    public Dictionary<int, HeroData> HeroDict { get; private set; } = new Dictionary<int, HeroData>();
    public Dictionary<int, SkillData> SkillDict { get; private set; } = new Dictionary<int, SkillData>();
    public Dictionary<int, ProjectileData> ProjectileDict { get; private set; } = new Dictionary<int, ProjectileData>();
    public Dictionary<int, EnvData> EnvDict { get; private set; } = new Dictionary<int, EnvData>();
    public Dictionary<int, EffectData> EffectDict { get; private set; } = new Dictionary<int, EffectData>();

    public void Init()
    {
        MonsterDict = LoadJson<MonsterDataLoader, int, Data.MonsterData>("MonsterData").MakeDict();
        HeroDict = LoadJson<HeroDataLoader, int, Data.HeroData>("HeroData").MakeDict();
        SkillDict = LoadJson<SkillDataLoader, int, Data.SkillData>("SkillData").MakeDict();
        ProjectileDict = LoadJson<ProjectileDataLoader, int, Data.ProjectileData>("ProjectileData").MakeDict();
        EnvDict = LoadJson<EnvDataLoader, int, EnvData>("EnvData").MakeDict();
        EffectDict = LoadJson<EffectDataLoader, int, EffectData>("EffectData").MakeDict();
    }

    private Loader LoadJson<Loader, Key, Value>(string path) where Loader : ILoader<Key, Value>
    {
        TextAsset textAsset = Managers.Resource.Load<TextAsset>(path);
        return JsonConvert.DeserializeObject<Loader>(textAsset.text);
    }
}
