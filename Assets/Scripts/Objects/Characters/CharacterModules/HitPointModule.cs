using UnityEngine;

public class HitPointModule : CharacterModule
{
    float _hp;
    public float HP => _hp;
    float _maxhp;
    public float MaxHP => _maxhp;

    public float time = 5.0f;

    float realTime;

    bool isDead = false;

    AnimationModule animModule;

    public sealed override System.Type RegistrationType => typeof(HitPointModule);

    public override void OnRegistration(CharacterBase newOwner)
    {
        base.OnRegistration(newOwner);
        animModule = gameObject.GetComponentInChildren<AnimationModule>();
        SetHP(4.0f);
        GameManager.OnUpdateCharacter -= UpdateHP;
        GameManager.OnUpdateCharacter += UpdateHP;
        
        if (Owner == null) return;
        newOwner.OnDamage -= OnDamageReceived;
        newOwner.OnDamage += OnDamageReceived;
    }

    public override void OnUnregistration(CharacterBase oldOwner)
    {
        base.OnUnregistration(oldOwner);
        GameManager.OnUpdateCharacter -= UpdateHP;
        
        if (oldOwner == null) return;
        oldOwner.OnDamage -= OnDamageReceived;
    }

    /// <summary> 맞았을때 내려가는 체력  </summary>
    public float DecreaseHP(float value)
    {
        if (Owner.CharType is CharacterType.Chaser || _hp <= 0) return 0;
        if(value > 4) value = 4;
        return _hp -= value;
    }

    /// <summary> 시작시 체력 세팅  </summary>
    public float SetHP(float value)
    {
        _hp = _maxhp = value;
        return _hp;
    }

    /// <summary> 업데이트 되는 체력 </summary>
    //일정 시간이 지난후 체력 초기화
    public void UpdateHP(float deltaTime)
    {
        realTime += deltaTime; //도망자일때만 발동?
        if (time < realTime)
        {
            realTime = 0.0f;
            if(_hp < 4) Heal();
        }
    }
    /// <summary>  체력 회복(죽지만 않으면 일정 시간이 지난후 자동)  </summary> 샤미드 
    public float Heal()
    {
        if (_hp < 0 && Owner.PlayerSet == PlayerSet.Stun) return 0;
        _hp = _maxhp;
        return _hp;
    }

    /// <summary> 술래가 때린게 아니면 기절을 한다. 술래면 그대로 사망한다  </summary>
    //스턴과 기절을 나눠서 애니메이션을 실행-> float 0,1,2로나뉘어 죽으면 0스턴은2 그 무엇도 아니면 1을 애니메이션 모듈로 실행
    public PlayerSet OutCheck(ControllerBase instigator)
    {
        if (instigator.Character.CharType is not CharacterType.Runner)
        {
            Owner.PlayerSet = PlayerSet.Stun;
            Owner.dyingSwitch = 1.0f;
            AnimationOn();
        }

        Owner.PlayerSet = PlayerSet.Dead;
        Owner.dyingSwitch = 0.0f;
        AnimationOn();

        return Owner.PlayerSet;
    }
    
    public void AnimationOn()
    {
        if (Owner.dyingSwitch != 0.0f || Owner.dyingSwitch != 1.0f || Owner.dyingSwitch != 2.0f) return;

        isDead = true;
        animModule.AnimationByDying(Owner.dyingSwitch, isDead);
    }

    void OnDamageReceived(GameObject damageCauser, ControllerBase instigator, float damage)
    {
        // 안전 체크
        if (damage <= 0f) return;

        float newHp = DecreaseHP(damage);

        // HP가 0 이하이면 기절/사망 판정 수행
        if (newHp <= 0f)
        {
            bool isStunned = OutCheck(instigator) == PlayerSet.Stun;
            //추가 동작(사망 처리, 이펙트 등)은 여기에 연결 가능
            //
            //예: if (!isStunned) { /* 사망 처리 */ }
        }
    }

}

//1.죽었을때죽는 애니메이션으로 바꿔야한다
//2.부활하거나 다음라운드에서는 다시 부활하니 일반 숨쉬는 모션으로 바꿔야한다

//그러면 무엇으로전환하냐       > 일단 bool을 생각중이지만 좀더 고민중
//어느 타이밍에 바꾸냐            > 기절인지 스턴인지 결정후에 애니메이션으로 전환되야한다  
//애니메이션은 어디서 실행하냐 > 일단 HitPointModule을 생각중 HitPointModule에서 너 언제 죽는 애니메이션 실행하는지 알려줘야 행동이 편하다

//--------

//부활기능을 HitPointModule로 이전...
//