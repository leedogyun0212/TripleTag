using UnityEngine;

public class UI_ScreenChanger : UIBase
{
    [SerializeField] Animator anim;

    public void ChangeStart()
    {
        anim.SetTrigger("Out");
    }

    public void ChangeEnd()
    {
        anim.SetTrigger("In");
    }
}
