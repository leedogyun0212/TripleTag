using UnityEngine;

public class HitPointModule : CharacterModule
{
    float _hp;
    public float HP => _hp;
    float _maxhp;
    public float MaxHP => _maxhp;

    public sealed override System.Type RegistrationType => typeof(HitPointModule);

    //public float IncreaseHP(float value);
    /// <summary> 맞았을때 내려가는 체력  </summary>
    public float DecreaseHP(float value)
    {
        if (Owner.CharType is CharacterType.Chaser) return 0;
        if(value > 4) value = 4;
        return _hp-= value;
    }

    /// <summary> 시작시 체력 세팅  </summary>
    public float SetHP(float value)
    {
        _hp = _maxhp = value;
        return _hp;
    }

    //public float Damage();
    /// <summary>  체력 회복(죽지만 않으면 일정 시간이 지난후 자동)  </summary>
    public float Heal()
    {
        _hp = _maxhp;
        return _hp;
    }

    /// <summary> 술래가 때린게 아니면 기절을 한다. 술래면 그대로 사망한다  </summary>
    public bool OutCheck(ControllerBase instigator)
    {
        if (instigator.Character.CharType is not CharacterType.Runner)
        {
            Owner.onStun = true;
        }
        
        Owner.onStun = false;

        return Owner.onStun;
    }

}
