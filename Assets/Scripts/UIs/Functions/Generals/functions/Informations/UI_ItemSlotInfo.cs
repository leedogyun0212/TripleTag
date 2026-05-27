using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_ItemSlotInfo : UIBase
{
    [SerializeField] Image iconImage;
    [SerializeField] TextMeshProUGUI amountText;

    [SerializeField] Sprite noneIcon;

    ItemSlot connectedSlot;

    public void ConnectSlot(ItemSlot targetSlot)
    {
        if (targetSlot is null) return;
        connectedSlot = targetSlot;
        VisualUpdate(connectedSlot);
    }

    protected virtual void VisualUpdate(ItemSlot targetSlot)
    {
        if (targetSlot is null) return;
        ItemContainer targetItem = targetSlot.GetItem();
        if (iconImage)
        {
            if (targetItem)
            {
                iconImage.sprite = targetItem.icon;
                iconImage.enabled = true;
            }
            else
            {
                iconImage.enabled = false;
            }
        }
        if (amountText)
        {
            int targetStack = targetSlot.GetStack();
            if(!targetItem || targetItem.maxStack <= 1 || targetStack <= 0)
            {
                amountText.SetText("");
            }
            else
            {
                //bool isMax = targetStack.GetIsMax();
                amountText.SetText($"{targetStack}");
            }
        }
    }
}
