using Fusion;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class ControllerBase : MonoBehaviour, IFunctionable
{
    CharacterBase _character;
    public CharacterBase Character => _character;

    public virtual void RegistrationFunctions()
    {
        Possess(GetComponent<CharacterBase>());
    }

    public virtual void UnregistrationFunctions()
    {
        Unpossess();
    }

    public virtual void OnEnable()
    {
        Possess(GetComponent<CharacterBase>());
    }

    public virtual void OnDisable()
    {
        Unpossess(); 
    }

    protected virtual void OnPossess(CharacterBase newCharacter) { }
    public void Possess(CharacterBase target)
    {
        if (!target) return;//대상이 없습니다.
        //       빙의된 컨트롤러             빙의  내가 너에게 가겠다
        ControllerBase result = target.Possessed(this);
        //내가 당첨되었어! => 제대로 빙의가 된 거구나!
        if (result == this)
        {
            _character = target;
            OnPossess(target);
        }
    }

    protected virtual void OnUnpossess(CharacterBase oldCharacter) {}
    public void Unpossess()
    {
        if(Character)
        {
            //이미 주인이 바뀌었다면?
            //제가 원래 살던 집을 팔거예요.
            //집주인이 이미 바뀐 상태
            //이 상태에서 집을 팝니다 => 팔렸다고 가정
            if (Character.Unpossessed(this))
            {
                OnUnpossess(Character);
            }
        }
        _character = null;
    }

    public void CommandMoveToDirection(Vector3 direction)
    {
        if (!GetComponent<NetworkObject>().HasInputAuthority) return;
        if (Character.PlayerSet != PlayerSet.Alive) return;

        PlayerInputHandler.Instance.SetMoveDirection(direction);
    }
    public void CommandMoveToDestination(Vector3 direction, float tolerance)
    {
        if (Character && Character.GetModule<MovementModule>() is IRunnable target)
        {
            if (Character.PlayerSet != PlayerSet.Alive) return;

            target.MoveToDestination(direction, tolerance);
        }
    }
    public void CommandChangeMoveType(MoveType wantType)
    {
        if (Character && Character.GetModule<MovementModule>() is MovementModule target)
        {
            if (Character.PlayerSet != PlayerSet.Alive) return;

            target.ChangeMoveType(wantType); 
        }
    }

    public void CommandStop()
    {
        if (Character && Character.GetModule<MovementModule>() is IRunnable target)
        {

            target.StopMovement();
        }
    }

    public void CommandAttackTry(bool value)
    {
        if (!GetComponent<NetworkObject>().HasInputAuthority) return;
        if (Character.PlayerSet != PlayerSet.Alive) return;

        if (value)
        {
            PlayerInputHandler.Instance.OnAttack();
        }
    }
}
