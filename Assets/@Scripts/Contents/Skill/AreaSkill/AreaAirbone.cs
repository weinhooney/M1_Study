using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class AreaAirbone : AreaSkill
{
    public override void SetInfo(Creature owner, int skillTemplateID)
    {
        base.SetInfo(owner, skillTemplateID);

        _angleRange = 360;

        if (_indicator != null)
        {
            _indicator.SetInfo(Owner, SkillData, EIndicatorType.Cone);
        }

        _indicatorType = EIndicatorType.Cone;
    }

    public override void DoSkill()
    {
        base.DoSkill();
    }
}
