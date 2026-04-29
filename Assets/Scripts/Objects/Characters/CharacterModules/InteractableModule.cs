using UnityEngine;

public class InteractableModule : CharacterModule
{
    public bool isPickUp = false;

    public sealed override System.Type RegistrationType => typeof(InteractableModule);

    /// <summary> 줍기 </summary>
    public void PickUp()
    {

    }

    /// <summary> 부활 </summary>
    public void Respawn()
    {

    }

    /// <summary> 시야 </summary>
    public void Vision()
    {

    }
}
