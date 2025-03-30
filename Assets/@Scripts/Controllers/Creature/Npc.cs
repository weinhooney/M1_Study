using System.Collections;
using System.Collections.Generic;
using Data;
using Spine.Unity;
using UnityEngine;
using static Define;

public class Npc : BaseObject
{
    public NpcData Data { get; set; }
    private SkeletonAnimation _skeletonAnim;
    private UI_NpcInteraction _ui;

    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }

        ObjectType = EObjectType.Npc;
        return true;
    }

    public void SetInfo(int dataId)
    {
        Data = Managers.Data.NpcDict[dataId];
        gameObject.name = $"{Data.SkeletonDataID}_{Data.Name}";

        #region Spine Animation
        SetSpineAnimation(Data.SkeletonDataID, SortingLayers.NPC);
        PlayAnimation(0, AnimName.IDLE, true);
        #endregion
        
        // Npc 상호작용을 위한 버튼
        GameObject button = Managers.Resource.Instantiate("UI_NpcInteraction", gameObject.transform);
        button.transform.localPosition = new Vector3(0, 3f);

        _ui = button.GetComponent<UI_NpcInteraction>();
        _ui.SetInfo(DataTemplateID, this);
    }
}
