using UnityEngine;

public class HitPointModule : CharacterModule
{
    float _hp;
    public float HP => _hp;
    float _maxhp;
    public float MaxHP => _maxhp;

    public sealed override System.Type RegistrationType => typeof(HitPointModule);
    
    //public float IncreaseHP(float value);
    public float DecreaseHP(float value)
    {
        if (Owner.CharType is CharacterType.Chaser) return 0;
        return _hp--;
    }
    public float SetHP(float value)
    {
        _hp = _maxhp = value;
        return _hp;
    }

    //public float Damage();
    public float Heal()
    {
        _hp = _maxhp;
        return _hp;
    }

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
