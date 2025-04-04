using System.Collections;
using System.Collections.Generic;
using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Rendering;
using static Define;

public class BaseObject : InitBase
{
    public int ExtraCells { get; set; } = 0;
    public EObjectType ObjectType { get; protected set; } = EObjectType.None;
    public CircleCollider2D Collider { get; private set; }
    public SkeletonAnimation SkeletonAnim { get; private set; }
    public Rigidbody2D RigidBody { get; private set; }
    private HurtFlashEffect HurtFlash;
    public float ColliderRadius { get { return Collider != null ? Collider.radius : 0.0f; } }
    public Vector3 CenterPosition { get { return transform.position + Vector3.up * ColliderRadius; } }
    public int DataTemplateID { get; set; }
    
    private bool _lookLeft = true;

    public bool LookLeft
    {
        get { return _lookLeft; }
        set
        {
            _lookLeft = value;
            Flip(!value);
        }
    }
    
    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }

        Collider = gameObject.GetOrAddComponent<CircleCollider2D>();
        SkeletonAnim = GetComponent<SkeletonAnimation>();
        RigidBody = GetComponent<Rigidbody2D>();
        HurtFlash = gameObject.GetOrAddComponent<HurtFlashEffect>();

        return true;
    }

    protected virtual void OnDisable()
    {
        if (SkeletonAnim == null)
        {
            return;
        }

        if (SkeletonAnim.AnimationState == null)
        {
            return;
        }

        SkeletonAnim.AnimationState.Event -= OnAnimEventHandler;
    }
    
    public void LookAtTarget(BaseObject target)
    {
        Vector2 dir = target.transform.position - transform.position;
        if (dir.x < 0)
        {
            LookLeft = true;
        }
        else
        {
            LookLeft = false;
        }
    }

    public static Vector3 GetLookAtRotation(Vector3 dir)
    {
        // Mathf.Atan2를 사용해 각도를 계산하고, 라디안에서 도로 변환
        float angle = Mathf.Atan2(-dir.x, dir.y) * Mathf.Rad2Deg;
        
        // Z축을 기준으로 회전하는 Vector3 값을 리턴
        return new Vector3(0, 0, angle);
    }
    
    #region Battle

    public virtual void OnDamaged(BaseObject attacker, SkillBase skill)
    {
        HurtFlash.Flash();
    }

    public virtual void OnDead(BaseObject attacker, SkillBase skill)
    {
        
    }
    
    #endregion
    
    #region Spine

    protected virtual void SetSpineAnimation(string dataLabel, int sortingOrder)
    {
        if (SkeletonAnim == null)
        {
            return;
        }

        SkeletonAnim.skeletonDataAsset = Managers.Resource.Load<SkeletonDataAsset>(dataLabel);
        SkeletonAnim.Initialize(true);
        
        // Register AnimEvent
        if (SkeletonAnim.AnimationState != null)
        {
            SkeletonAnim.AnimationState.Event -= OnAnimEventHandler;
            SkeletonAnim.AnimationState.Event += OnAnimEventHandler;
        }
        
        // Spine SkeletonAnimation은 SpriteRenderer를 사용하지 않고 MeshRenderer를 사용함
        // 그렇기 때문에 2D Sort Axis가 안 먹히게 되는데, SortingGroup을 SpriteRenderer, MeshRenderer를 같이 계산함
        SortingGroup sg = Util.GetOrAddComponent<SortingGroup>(gameObject);
        sg.sortingOrder = sortingOrder;
    }
    
    protected virtual void UpdateAnimation() { }
    
    public TrackEntry PlayAnimation(int trackIndex, string animName, bool loop)
    {
        if (SkeletonAnim == null)
        {
            return null;
        }

        TrackEntry entry = SkeletonAnim.AnimationState.SetAnimation(trackIndex, animName, loop);

        if (animName == AnimName.DEAD)
        {
            entry.MixDuration = 0;
        }
        else
        {
            entry.MixDuration = 0.2f;
        }
        
        return entry;
    }

    public void AddAnimation(int trackIndex, string animName, bool loop, float delay)
    {
        if (SkeletonAnim == null)
        {
            return;
        }

        SkeletonAnim.AnimationState.AddAnimation(trackIndex, animName, loop, delay);
    }

    public void Flip(bool flag)
    {
        if (SkeletonAnim == null)
        {
            return;
        }

        SkeletonAnim.skeleton.ScaleX = flag ? -1 : 1;
    }

    public virtual void OnAnimEventHandler(TrackEntry trackEntry, Spine.Event e)
    {
        Debug.Log("OnAnimEventHandler");
    }
    #endregion

    #region Map
    public bool LerpCellPosCompleted { get; protected set; }

    private Vector3Int _cellPos;

    public Vector3Int CellPos
    {
        get { return _cellPos; }
        protected set
        {
            _cellPos = value;
            LerpCellPosCompleted = false;
        }
    }

    public void SetCellPos(Vector3Int cellPos, bool forceMove = false)
    {
        CellPos = cellPos;
        LerpCellPosCompleted = false;

        if (forceMove)
        {
            transform.position = Managers.Map.Cell2World(CellPos);
            LerpCellPosCompleted = true;
        }
    }

    public void LerpToCellPos(float moveSpeed)
    {
        if (LerpCellPosCompleted)
        {
            return;
        }

        Vector3 destPos = Managers.Map.Cell2World(CellPos);
        Vector3 dir = destPos - transform.position;

        if (dir.x < 0)
        {
            LookLeft = true;
        }
        else
        {
            LookLeft = false;
        }

        if (dir.magnitude < 0.01f)
        {
            transform.position = destPos;
            LerpCellPosCompleted = true;
            return;
        }

        float moveDist = Mathf.Min(dir.magnitude, moveSpeed * Time.deltaTime);
        transform.position += dir.normalized * moveDist;
    }
    #endregion
}
