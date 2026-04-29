using System;
using UnityEngine;

public class AttackModule : CharacterModule
{
    public sealed override System.Type RegistrationType => typeof(AttackModule);

    //                                가격할 상대?
    public void Attack(ControllerBase instigator)
    {
        if (Owner.onStun) return;

        if (Owner.CharType is CharacterType.Chaser)
        { }
        else
        { }
    }

    //술래의 공격 맞으면 죽는다
    public void ChaserAttack(ControllerBase instigator)
    { }
    //생존자의 공격 : 술래에게는 통하지 않고 같은 생존자에게만 통한다
    public void RunnerAttack(ControllerBase instigator)
    { }

    // 깔아놓는 덫 걸리면 체력이 1닳는다. 잠시 스턴에 걸린다
    public void Trap(ControllerBase instigator)
    { }
}
