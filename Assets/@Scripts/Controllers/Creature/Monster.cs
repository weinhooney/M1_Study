using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Vector3 = UnityEngine.Vector3;
using static Define;
using Random = UnityEngine.Random;
using Vector2 = UnityEngine.Vector2;

public class Monster : Creature
{
    public override ECreatureState CreatureState
    {
        get { return base.CreatureState; }
        set
        {
            base.CreatureState = value;
            switch (value)
            {
                case ECreatureState.Idle:
                    UpdateAITick = 0.5f;
                    break;
                
                case ECreatureState.Move:
                    UpdateAITick = 0.0f;
                    break;
                    
                case ECreatureState.Skill:
                    UpdateAITick = 0.0f;
                    break;
                
                case ECreatureState.Dead:
                    UpdateAITick = 1.0f;
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

        ObjectType = EObjectType.Monster;

        StartCoroutine(CoUpdateAI());
        
        return true;
    }

    public override void SetInfo(int templateID)
    {
        base.SetInfo(templateID);
        
        // State
        CreatureState = ECreatureState.Idle;
    }

    private void Start()
    {
        _initPos = transform.position;
    }

    #region AI
    private Vector3 _destPos;
    private Vector3 _initPos;
    
    protected override void UpdateIdle()
    {
        // Debug.Log("Idle");

        // Patrol
        {
            int patrolPercent = 10;
            int rand = Random.Range(0, 100);
            if (rand <= patrolPercent)
            {
                _destPos = _initPos + new Vector3(Random.Range(-2f, 2f), Random.Range(-2f, 2f));
                CreatureState = ECreatureState.Move;
                return;
            }
        }
        
        // Search Player
        Creature creature = FindClosestInRange(Define.MONSTER_SEARCH_DISTANCE, Managers.Object.Heroes, func : IsValid) as Creature;
        if (creature != null)
        {
            Target = creature;
            CreatureState = ECreatureState.Move;
            return;
        }
    }

    protected override void UpdateMove()
    {
        // Debug.Log("Move");

        if (Target.IsValid() == false)
        {
            Creature creature = FindClosestInRange(MONSTER_SEARCH_DISTANCE, Managers.Object.Heroes, func: IsValid) as Creature;
            if (creature != null)
            {
                Target = creature;
                CreatureState = ECreatureState.Move;
                return;
            }
            
            // Move
            FindPathAndMoveToCellPos(_destPos, MONSTER_DEFAULT_MOVE_DEPTH);
            if (LerpCellPosCompleted)
            {
                CreatureState = ECreatureState.Idle;
                return;
            }
        }
        else
        {
            // Chase
            ChaseOrAttackTarget(MONSTER_SEARCH_DISTANCE, AttackDistance);
            
            // 너무 멀어지면 포기
            if (Target.IsValid() == false)
            {
                Target = null;
                _destPos = _initPos;
                return;
            }
        }
    }

    protected override void UpdateSkill()
    {
        base.UpdateSkill();
        
        if (Target.IsValid() == false)
        {
            Target = null;
            _destPos = _initPos;
            CreatureState = ECreatureState.Move;
            return;
        }
    }

    protected override void UpdateDead()
    {
       
    }
    
    #endregion

    #region Battle

    public override void OnDamaged(BaseObject attacker, SkillBase skill)
    {
        base.OnDamaged(attacker, skill);
    }

    public override void OnDead(BaseObject attacker, SkillBase skill)
    {
        base.OnDead(attacker, skill);
        
        // TODO : Drop Item
        
        Managers.Object.Despawn(this);
    }

    #endregion
}
