using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class AttackModule : CharacterModule
{
    float damage;
    HitPointModule hpModule;
    
    public bool isAttack = false;

    public sealed override System.Type RegistrationType => typeof(AttackModule);

    public override void OnRegistration(CharacterBase newOwner)
    {
        base.OnRegistration(newOwner);
        hpModule = Owner.GetModule<HitPointModule>();
    }

    public override void OnUnregistration(CharacterBase oldOwner)
    {
        base.OnUnregistration(oldOwner);
    }

    public void AttackTarget(GameObject target, float damage)
    {
        
        if (Owner is null || target is null) return;

        CharacterBase targetChar = target.GetComponentInParent<CharacterBase>();
        if (targetChar == null) return;

        if (Owner.CharType == CharacterType.Chaser && targetChar.CharType == CharacterType.Chaser) return;
        if (Owner.CharType == CharacterType.Runner && targetChar.CharType == CharacterType.Chaser) return;

        float finalDamage;
        if (Owner.CharType == CharacterType.Chaser)
            finalDamage = ChaserAttack(targetChar);
        else
            finalDamage = RunnerAttack(targetChar);

        targetChar?.DamageNotify(Owner.gameObject, Owner, finalDamage);
    }

    /// <summary> 술래의 공격 : 맞으면 죽는다 </summary>
    // instigator가 술래일때 나를 떄리면 발동한다 instigator가 나를 때리고 나는 데미지를 받는다
    public float ChaserAttack(CharacterBase target)
    {
        if (target == null) return 0f;
        HitPointModule targetHp = target.GetModule<HitPointModule>();
        if (targetHp != null) return targetHp.HP > 0f ? targetHp.HP : 4f;
        return 4f;
    }
    ///<summary>생존자의 공격 : 술래에게는 통하지 않고 같은 생존자에게만 통한다</summary>
    public float RunnerAttack(CharacterBase target)
    {
        if (target.CharType != CharacterType.Runner) return 0f;
        //if (target == Owner) return 0f;
        // 기본 데미지(조절 가능)
        return 1f;
    }

    public void StartAttack()
    {
        isAttack = true;
    }

    public void EndAttack()
    {
        isAttack = false;
    }

    //공격

    //AttackModule에서 공격을 하면 HitPointModule에서 HP가 다는 방식

    //술래 : 생존자들을 처치하는 용도, 한대 맞으면 바로 죽는다. 술래끼리는 통하지 않아야 한다.
    //데미지를 입히는 스크립트, 술래 생존자에 따라 데미지 강도 변경, 
    //생존자 : 다른 생존자를 공격해 체력이 0이되면 잠시 기절 기절과 기절이 끝난후 잠시 동안은 생존자의 공격으로 부터 무적

    //컨트롤러에서 공격키를 누른다 >
    //일단 공격 애니메이션(애니메이션 모듈 존재)이 나가면서 공격 성공 여부 체크하는 스크립트(어택 모듈에 추가 생각중) 실행 >
    //체크 해서 공격에 성공하면 AttackTarget을 실행 >
    //AttackTarget에서 데미지를 입힐 수 있는 상대면 내가 술래인지 생존자인지 체크 >
    //나의 상태에 따라 ChaserAttack 혹은 RunnerAttack을 실행하여 데미지 계산 >
    //Attack에서 최종적으로 계산된 데미지를 반환 > AttackTarget에서 DamageNotify 실행 > 
    //HitPointModule의 OnDamageReceived를 실행하여 체력을 감소 시킨다.

    ///<summary> 깔아놓는 덫 : 걸리면 체력이 1닳는다. 잠시 스턴에 걸린다 </summary>
    public void Trap(ControllerBase instigator)
    { }

    private void OnTriggerEnter(Collider other)
    {
        if (!isAttack) return;
        AttackModule target = other.GetComponent<AttackModule>();
        //
        if (target == null) return;
        isAttack = false;
        AttackTarget(target.gameObject, damage);
    }

}
