using System;
using UnityEngine;

public class AnimationModule : CharacterModule
{
    //클래스간의 결합
    //is - a 관계 : 상속관계
    //has - a 관계 : 소유관계 MovementModule movementModule; 
    [SerializeField] Animator anim;
    [SerializeField] bool isRotationByMovement;
    AttackModule myattack;
    public sealed override System.Type RegistrationType => typeof(AnimationModule);


    public override void OnRegistration(CharacterBase newOwner)
    {
        base.OnRegistration(newOwner);
        myattack = gameObject.GetComponentInParent<AttackModule>();
        newOwner.OnLookAt -= AnimationByLookRotation;
        newOwner.OnLookAt += AnimationByLookRotation;
        newOwner.OnMovement -= AnimationByMovement;
        newOwner.OnMovement += AnimationByMovement;
    }

    public override void OnUnregistration(CharacterBase oldOwner)
    {
        base.OnUnregistration(oldOwner);
        oldOwner.OnLookAt -= AnimationByLookRotation;
        oldOwner.OnMovement -= AnimationByMovement;

    }

    public void AnimationByLookRotation(Vector3 lookRotation)
    {
        if (!anim) return;

        //Debug.Log(lookRotation.x);

        anim.SetFloat("MoveX", lookRotation.x * 10);
        anim.SetFloat("MoveY", lookRotation.z * 10);

    }
    public void AnimationByMovement(Vector3 moveDelta)
    {
        if (!anim) return;
        if(isRotationByMovement && moveDelta.sqrMagnitude > 0)
        {
            AnimationByLookRotation(moveDelta);
        }
        anim.SetFloat("MoveSpeed", moveDelta.magnitude / Time.fixedDeltaTime);
    }

    public void AnimationByAttack(bool value)
    {
        if (!anim) return;

        anim.SetTrigger("Attack");
    }

    public void AnimAttackOn()
    {
        if (myattack)
        {
            myattack.isAttack = true;
        }
    }

    //죽거나 기절하면 발동
    //기절하고 일정시간이 지나거나 부활하면 다시 넘어감
    //죽거나 기절을 관여하는것은 HP 부활은 InteractableModule
    //그러면 애니메이션자체는 여기서 발동 조작은 HP 부활시에는 HP에서 애니메이션 조작하는것을 실행하는것으로 사용

    public void AnimationByDying(float value)
    {
        if (!anim) return;

        //anim.SetTrigger("Dying");
        anim.SetFloat("Die", value);
    }

    public void AnimAttackOff()
    {
        if (myattack)
        {
            myattack.isAttack = false;
        }
    }
}
