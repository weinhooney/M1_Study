using System;
using System.Collections;
using System.Collections.Generic;
using Spine;
using UnityEngine;
using static Define;
using Event = Spine.Event;

public class Hero : Creature
{
    public bool NeedArrange { get; set; }
    
    public override ECreatureState CreatureState
    {
        get { return _creatureState; }
        set
        {
            if (_creatureState != value)
            {
                base.CreatureState = value;
            }
        }
    }

    private EHeroMoveState _heroMoveState = EHeroMoveState.None;
    public EHeroMoveState HeroMoveState
    {
        get { return _heroMoveState; }
        private set
        {
            _heroMoveState = value;
            switch (value)
            {
                case EHeroMoveState.CollectEnv:
                    NeedArrange = true;
                    break;
                
                case EHeroMoveState.TargetMonster:
                    NeedArrange = true;
                    break;
                
                case EHeroMoveState.ForceMove:
                    NeedArrange = true;
                    break;
            }
        }
    }
    
    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }

        CreatureType = Define.ECreatureType.Hero;
        
        Managers.Game.OnJoystickStateChanged -= HandleOnJoystickStateChanged;
        Managers.Game.OnJoystickStateChanged += HandleOnJoystickStateChanged;

        // Map
        Collider.isTrigger = true;
        RigidBody.simulated = false;
        
        StartCoroutine(CoUpdateAI());
        
        return true;
    }

    public override void SetInfo(int templateID)
    {
        base.SetInfo(templateID);
        
        // State
        CreatureState = ECreatureState.Idle;
        
        // Skill
        Skills = gameObject.GetOrAddComponent<SkillComponent>();
        Skills.SetInfo(this, CreatureData.SkillIdList);
    }

    public Transform HeroCampDest
    {
        get
        {
            HeroCamp camp = Managers.Object.Camp;
            if (HeroMoveState == EHeroMoveState.ReturnToCamp)
            {
                return camp.Pivot;
            }

            return camp.Destination;
        }
    }
    
    #region AI
    protected override void UpdateIdle()
    {
        // 0. 이동 상태라면 강제 변경
        if (HeroMoveState == EHeroMoveState.ForceMove)
        {
            CreatureState = ECreatureState.Move;
            return;
        }
        
        // 0. 너무 멀어졌다면 강제로 이동
        
        // 1. 몬스터
        Creature creature = FindClosestInRange(HERO_SEARCH_DISTANCE, Managers.Object.Monsters) as Creature;
        if (creature != null)
        {
            Target = creature;
            CreatureState = ECreatureState.Move;
            HeroMoveState = EHeroMoveState.TargetMonster;
            return;
        }

        // 2. 주변 Env 채굴
        Env env = FindClosestInRange(HERO_SEARCH_DISTANCE, Managers.Object.Envs) as Env;
        if (env != null)
        {
            Target = env;
            CreatureState = ECreatureState.Move;
            HeroMoveState = EHeroMoveState.CollectEnv;
            return;
        }

        // 3. Camp 주변으로 모이기
        if(NeedArrange)
        {
            CreatureState = ECreatureState.Move;
            HeroMoveState = EHeroMoveState.ReturnToCamp;
            return;
        }
    }

    protected override void UpdateMove()
    {
        // 0. 누르고 있다면 강제 이동
        if (HeroMoveState == EHeroMoveState.ForceMove)
        {
            EFindPathResult result = FindPathAndMoveToCellPos(HeroCampDest.position, HERO_DEFAULT_MOVE_DEPTH);
            return;
        }
        
        // 1. 주변 몬스터 서치
        if (HeroMoveState == EHeroMoveState.TargetMonster)
        {
            // 몬스터가 죽었으면 포기
            if (Target.IsValid() == false)
            {
                HeroMoveState = EHeroMoveState.None;
                CreatureState = ECreatureState.Move;
                return;
            }

            SkillBase skill = Skills.GetReadySkill();
            ChaseOrAttackTarget(HERO_SEARCH_DISTANCE, skill);
            return;
        }
        
        // 2. 주변 Env 채굴
        if (HeroMoveState == EHeroMoveState.CollectEnv)
        {
            // 몬스터가 있으면 포기
            Creature creature = FindClosestInRange(HERO_SEARCH_DISTANCE, Managers.Object.Monsters) as Creature;
            if (creature != null)
            {
                Target = creature;
                HeroMoveState = EHeroMoveState.TargetMonster;
                CreatureState = ECreatureState.Move;
                return;
            }
            
            // Env 이미 채집했으면 포기
            if (Target.IsValid() == false)
            {
                HeroMoveState = EHeroMoveState.None;
                CreatureState = ECreatureState.Move;
                return;
            }

            SkillBase skill = Skills.GetReadySkill();
            ChaseOrAttackTarget(HERO_SEARCH_DISTANCE, skill);
            return;
        }
        
        // 3. Camp 주변으로 모이기
        if (HeroMoveState == EHeroMoveState.ReturnToCamp)
        {
            Vector3 destPos = HeroCampDest.position;
            if (FindPathAndMoveToCellPos(destPos, HERO_DEFAULT_MOVE_DEPTH) == EFindPathResult.Success)
            {
                return;
            }
            
            // 실패 사유 검사
            BaseObject obj = Managers.Map.GetObject(destPos);
            if (obj.IsValid())
            {
                // 내가 그 자리를 차지하고 있다면
                if (obj == this)
                {
                    HeroMoveState = EHeroMoveState.None;
                    NeedArrange = false;
                    return;
                }
                
                // 다른 영웅이 멈춰있다면
                Hero hero = obj as Hero;
                if (hero != null && hero.CreatureState == ECreatureState.Idle)
                {
                    HeroMoveState = EHeroMoveState.None;
                    NeedArrange = false;
                    return;
                }
            }
        }
        
        // 4. 기타 (눌렀다 뗐을 때)
        if (LerpCellPosCompleted)
        {
            CreatureState = ECreatureState.Idle;   
        }
    }

    protected override void UpdateSkill()
    {
        if (HeroMoveState == EHeroMoveState.ForceMove)
        {
            CreatureState = ECreatureState.Move;
            return;
        }

        if (Target.IsValid() == false)
        {
            CreatureState = ECreatureState.Move;
            return;
        }
    }

    protected override void UpdateDead()
    {
        
    }
    #endregion
    
    void HandleOnJoystickStateChanged(EJoystickState joystickState)
    {
        switch (joystickState)
        {
            case EJoystickState.PointerDown:
                HeroMoveState = EHeroMoveState.ForceMove;
                break;
            
            case EJoystickState.Drag:
                HeroMoveState = EHeroMoveState.ForceMove;
                break;
            
            case EJoystickState.PointerUp:
                HeroMoveState = EHeroMoveState.None;
                break;
            
            default:
                break;
        }
    }
    
    public override void OnAnimEventHandler(TrackEntry trackEntry, Event e)
    {
        base.OnAnimEventHandler(trackEntry, e);
    }
}
