using System;
using UnityEngine;
using UnityEngine.UI;

public class UI_InventoryWindow : OpenableUIBase
{
    [SerializeField] Inventory targetInventory;
    [SerializeField] LayoutGroup layout;
    [SerializeField] string itemSlotPrefabName;

    public override void Registration(UIManager manager)
    {
        base.Registration(manager);
        targetInventory?.Initialize();
        ConnectInventory(targetInventory);
    }

    public override void Unregistration(UIManager manager)
    {
        base.Unregistration(manager);
        DisconnectInventory();
    }

    private void ConnectInventory(Inventory newInventory)
    {
        if (!newInventory) return;
        targetInventory = newInventory;

        //레이아웃 그룹이 그리드 레이아웃 그룹이라면
        if (layout is GridLayoutGroup asGridLayout)
        {
            asGridLayout.constraintCount = targetInventory.columns;
        }

        

        foreach (ItemSlot currentSlot in newInventory.GetAllslot())
        {
            if (currentSlot is null) continue;
            GameObject instance = ObjectManager.CreateObject(itemSlotPrefabName, layout.transform);
            if (!instance) continue;
            if (instance.TryGetComponent(out UI_ItemSlotInfo createdSlot)) //슬롯이 아닌데?
            {
                createdSlot.ConnectSlot(currentSlot);
            }
        }
    }

    private void DisconnectInventory()
    {
        if (!layout) return;
        while(layout.transform.childCount > 0)
        {
            Transform targetChild = layout.transform.GetChild(0);
            targetChild.SetParent(null);
            ObjectManager.DestroyObject(targetChild.gameObject);
        }
    }
}
