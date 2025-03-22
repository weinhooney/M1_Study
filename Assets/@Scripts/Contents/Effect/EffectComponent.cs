using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static Define;

public class EffectComponent : MonoBehaviour
{
    public List<EffectBase> ActiveEffects = new List<EffectBase>();
    private Creature _owner;

    public void SetInfo(Creature owner)
    {
        _owner = owner;
    }

    public List<EffectBase> GenerateEffects(IEnumerable<int> effectIds, EEffectSpawnType spawnType)
    {
        List<EffectBase> generatedEffects = new List<EffectBase>();

        foreach (int id in effectIds)
        {
            string className = Managers.Data.EffectDict[id].ClassName;
            Type effectType = Type.GetType(className);

            if (effectType == null)
            {
                Debug.LogError($"Effect Type not found: {className}");
                return null;
            }

            GameObject go = Managers.Object.SpawnGameObject(_owner.CenterPosition, "EffectBase");
            go.name = Managers.Data.EffectDict[id].ClassName;
            EffectBase effect = go.AddComponent(effectType) as EffectBase;
            effect.transform.parent = _owner.Effects.transform;
            effect.transform.localPosition = Vector2.zero;
            Managers.Object.Effects.Add(effect);
            
            ActiveEffects.Add(effect);
            generatedEffects.Add(effect);
            
            effect.SetInfo(id, _owner, spawnType);
            effect.ApplyEffect();
        }

        return generatedEffects;
    }

    public void RemoveEffect(EffectBase effect)
    {
        
    }

    public void ClearDebuffBySkill()
    {
        foreach (var buff in ActiveEffects.ToArray())
        {
            if (buff.EffectType != Define.EEffectType.Buff)
            {
                buff.ClearEffect(Define.EEffectClearType.ClearSkill);
            }
        }
    }
}
