using UnityEngine;

public class InteractableModule : CharacterModule
{
    public bool isPickUp = false;

    public sealed override System.Type RegistrationType => typeof(InteractableModule);

    /// <summary> 줍기 </summary>
    //부활 표식 줍기
    public void PickUp()
    {

    }

    /// <summary> 부활 </summary>
    // 주운 부활 표식으로 부활장소로 이동하면 활성화 하는 기능
    public void Respawn()
    {

    }

    /// <summary> 시야 </summary>
    public void Vision()
    {

    }
}
