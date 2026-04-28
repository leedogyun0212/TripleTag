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

    public void ChaserAttack(ControllerBase instigator)
    { }
    public void RunnerAttack(ControllerBase instigator)
    { }

    public void Trap(ControllerBase instigator)
    { }
}
