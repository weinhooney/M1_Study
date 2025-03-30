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
    public Dictionary<int, Data.HeroInfoData> HeroInfoDict { get; private set; } = new Dictionary<int, Data.HeroInfoData>();
    public Dictionary<int, SkillData> SkillDict { get; private set; } = new Dictionary<int, SkillData>();
    public Dictionary<int, ProjectileData> ProjectileDict { get; private set; } = new Dictionary<int, ProjectileData>();
    public Dictionary<int, EnvData> EnvDict { get; private set; } = new Dictionary<int, EnvData>();
    public Dictionary<int, EffectData> EffectDict { get; private set; } = new Dictionary<int, EffectData>();
    public Dictionary<int, AoEData> AoEDict { get; private set; } = new Dictionary<int, AoEData>();
    public Dictionary<int, NpcData> NpcDict { get; private set; } = new Dictionary<int, NpcData>();
    public Dictionary<string, Data.TextData> TextDict { get; private set; } = new Dictionary<string, Data.TextData>();

    public void Init()
    {
        MonsterDict = LoadJson<MonsterDataLoader, int, Data.MonsterData>("MonsterData").MakeDict();
        HeroDict = LoadJson<HeroDataLoader, int, Data.HeroData>("HeroData").MakeDict();
        HeroInfoDict = LoadJson<Data.HeroInfoDataLoader, int, Data.HeroInfoData>("HeroInfoData").MakeDict();
        SkillDict = LoadJson<SkillDataLoader, int, Data.SkillData>("SkillData").MakeDict();
        ProjectileDict = LoadJson<ProjectileDataLoader, int, Data.ProjectileData>("ProjectileData").MakeDict();
        EnvDict = LoadJson<EnvDataLoader, int, EnvData>("EnvData").MakeDict();
        EffectDict = LoadJson<EffectDataLoader, int, EffectData>("EffectData").MakeDict();
        AoEDict = LoadJson<AoEDataLoader, int, AoEData>("AoEData").MakeDict();
        NpcDict = LoadJson<NpcDataLoader, int, NpcData>("NpcData").MakeDict();
        TextDict = LoadJson<Data.TextDataLoader, string, Data.TextData>("TextData").MakeDict();
    }

    private Loader LoadJson<Loader, Key, Value>(string path) where Loader : ILoader<Key, Value>
    {
        TextAsset textAsset = Managers.Resource.Load<TextAsset>(path);
        return JsonConvert.DeserializeObject<Loader>(textAsset.text);
    }
}
