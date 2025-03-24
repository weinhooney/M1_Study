using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBuff : BuffBase
{
    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }

        return true;
    }

    public override void ApplyEffect()
    {
        base.ApplyEffect();
        AddModifier(Owner.Atk, this);
    }

    public override bool ClearEffect(Define.EEffectClearType clearType)
    {
        if (base.ClearEffect(clearType) == true)
        {
            RemoveModifier(Owner.Atk, this);
        }

        return true;
    }
}
