using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Serialization;
using UnityEngine;
using static Define;

public class Freeze : CCBase
{
    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }

        EffectType = EEffectType.CrowdControl;

        return true;
    }

    public override void ApplyEffect()
    {
        Loop = false;
        base.ApplyEffect();
    }
}
