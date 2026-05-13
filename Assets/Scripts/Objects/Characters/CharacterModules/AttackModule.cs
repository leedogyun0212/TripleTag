using System;
using UnityEngine;
using UnityEngine.Rendering;

public class AttackModule : CharacterModule
{
    float damage;
    HitPointModule hpModule;
    
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

    //                                가격할 상대?
    public float Attack(float wantDamage)
    {
        if (Owner == null || hpModule == null) return 0;

        return hpModule.DecreaseHP(wantDamage);
    }

    /// <summary> 술래의 공격 : 맞으면 죽는다 </summary>
    // instigator가 술래일때 나를 떄리면 발동한다 instigator가 나를 때리고 나는 데미지를 받는다
    public void ChaserAttack(ControllerBase instigator)
    {
    }
    ///<summary>생존자의 공격 : 술래에게는 통하지 않고 같은 생존자에게만 통한다</summary>
    public void RunnerAttack(ControllerBase instigator)
    {
        
    }

    //공격

    //AttackModule에서 공격을 하면 HitPointModule에서 HP가 다는 방식

    //술래 : 생존자들을 처치하는 용도, 한대 맞으면 바로 죽는다. 술래끼리는 통하지 않아야 한다.
    //데미지를 입히는 스크립트, 술래 생존자에 따라 데미지 강도 변경, 
    //생존자 : 다른 생존자를 공격해 체력이 0이되면 잠시 기절 기절과 기절이 끝난후 잠시 동안은 생존자의 공격으로 부터 무적

    ///<summary> 깔아놓는 덫 : 걸리면 체력이 1닳는다. 잠시 스턴에 걸린다 </summary>
    public void Trap(ControllerBase instigator)
    { }
}
