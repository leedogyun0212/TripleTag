using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_ItemSlotInfo : UIBase
{
    [SerializeField] Image iconImage;
    [SerializeField] TextMeshProUGUI amountText;

    [SerializeField] Sprite noneIcon;

    protected ItemSlot _connectedSlot;

    public ItemSlot ConnectedSlot => _connectedSlot;

    public void ConnectSlot(ItemSlot targetSlot)
    {
        DisconnectSlot();
        if (targetSlot is null) return;
        _connectedSlot = targetSlot;
        _connectedSlot.OnItemSlotChanged -= VisualUpdate;
        _connectedSlot.OnItemSlotChanged += VisualUpdate;
        VisualUpdate(_connectedSlot);
    }

    public void DisconnectSlot()
    {
        if (_connectedSlot is null) return;
        _connectedSlot.OnItemSlotChanged -= VisualUpdate;
        _connectedSlot = null;
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
