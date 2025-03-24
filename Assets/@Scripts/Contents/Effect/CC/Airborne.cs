using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Define;

public class Airborne : CCBase
{
    [SerializeField] private float _airborneDistance = 5;

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
        
        StopCoroutine(DoAirborne(_lastState));
        StartCoroutine(DoAirborne(_lastState));
    }
    
    // TODO
    // 에어본 중에 또 에어본을 맞는 경우
    // 에어본 중에 넉백 당하는 경우
    // 넉백 중에 에어본 하는 경우
    IEnumerator DoAirborne(ECreatureState lastState)
    {
        Vector3 originalPosition = Owner.SkeletonAnim.transform.localPosition;
        Vector3 upPosition = originalPosition + Vector3.up * _airborneDistance;

        float halfTickTime = EffectData.TickTime * 0.5f;
        
        // 위로 올라갈 때
        for (float t = 0; t < halfTickTime; t += Time.deltaTime)
        {
            float normalzedTime = t / halfTickTime;
            Owner.SkeletonAnim.transform.localPosition = Vector3.Lerp(originalPosition, upPosition, normalzedTime);
            yield return null;
        }
        
        // 아래로 내려갈 때
        for (float t = 0; t < halfTickTime; t += Time.deltaTime)
        {
            float normailzedTime = t / halfTickTime;
            Owner.SkeletonAnim.transform.localPosition = Vector3.Lerp(upPosition, originalPosition, normailzedTime);
            yield return null;
        }

        Owner.SkeletonAnim.transform.localPosition = originalPosition;
        
        // 에어본 상태가 끝난 후 상태 복귀
        if(Owner.CreatureState == ECreatureState.OnDamaged)
        {
            // Owner.CreatureState = _lastState;
            Owner.CreatureState = ECreatureState.Idle;
        }

        ClearEffect(EEffectClearType.EndOfAirborne);
    }

    protected override IEnumerator CoStartTimer()
    {
        // Airborne는 타이머 없음
        yield break;
    }
}
