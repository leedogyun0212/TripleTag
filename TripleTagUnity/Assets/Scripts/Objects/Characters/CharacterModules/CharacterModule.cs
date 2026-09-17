using UnityEngine;

public class CharacterModule : MonoBehaviour
{
    //MovementModule
    //나의 대분류를 저장하는 방법
    public virtual System.Type RegistrationType => typeof(CharacterModule);

    CharacterBase _owner;
    public CharacterBase Owner => _owner;

    public virtual void OnRegistration(CharacterBase newOwner) { _owner = newOwner; }
    public virtual void OnUnregistration(CharacterBase oldOwner) { _owner = null; }
}
