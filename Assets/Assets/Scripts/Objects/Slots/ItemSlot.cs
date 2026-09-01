using System;
using Unity.VisualScripting;
using UnityEngine;

public delegate void ItemSlotChangeEvent(ItemSlot changedSlot);

public class ItemSlot
{
    //이 칸에  들어 있는 아이템의 정보
    [SerializeField] ItemContainer item;
    //이 칸 만의 정보
    [SerializeField] int currentStack;

    public event ItemSlotChangeEvent OnItemSlotChanged;

    public void NoticeChanged() => OnItemSlotChanged?.Invoke(this);

    public virtual bool Containable(ItemContainer wantItem)
    {
        if (wantItem is null) return false;
        
        if (item is not null && item != wantItem) return false;

        if(GetIsMax()) return false;

        return true;
    }

    public ItemContainer GetItem() => item;

    public int GetStackable(ItemContainer wantItem) => Containable(wantItem) ? wantItem.maxStack - currentStack : 0;
    public int GetStackable() => GetStackable(item);
    public int GetStack()          => currentStack;
    public bool GetIsMax()         => item ? currentStack >= item.maxStack : false;
    public bool GetIsEmpty()       => item is null || currentStack <= 0;

    public int Clear()
    {
        item = null; //아이템 비움
        int removed = currentStack; //비우기 전에 저장
        currentStack = 0; //스택 초기화
        return removed; // 얼마나 비웠는지 반환
    }

    public int AddItem(ItemContainer wantItem, int amount)
    {
        if (amount <= 0) return 0;
        if (!Containable(wantItem)) return amount;

        item = wantItem;
        int stackable = Mathf.Min(item.maxStack - currentStack, amount);
        currentStack += stackable;

        return amount - stackable; // 추가하려는 값 - 추가한 값
    }

    public int RemoveItem(ItemContainer wantItem)
    {
        Debug.Log($"RemoveItem: {wantItem.name} /// {currentStack}");
        if (!wantItem) return 0;

        if(GetIsEmpty()) return 0;

        if(item != wantItem) return 0;

        return Clear();
    }

    public int RemoveItem(ItemContainer wantItem, int amount)
    {
        if (amount <= 0) return 0;

        if (!wantItem) return 0;

        if (GetIsEmpty()) return amount;

        if (item != wantItem) return amount;

        if (amount >= currentStack)return amount - Clear();

        currentStack -= amount;

        return 0;
    }

    public void ExchangeItem(ItemSlot wantSlot)
    {
        if (wantSlot is null) return;

        ItemContainer wasItem = item;
        int wasStack = currentStack;

        item = wantSlot.item;
        currentStack = wantSlot.currentStack;
        wantSlot.item = wasItem;
        wantSlot.currentStack = wasStack;
    }

    public int GiveItem(ItemSlot wantSlot) => GiveItem(wantSlot, currentStack);
    public int GiveSingleItem(ItemSlot wantSlot) => GiveItem(wantSlot, 1);

    public int GiveItem(ItemSlot wantSlot, int amount)
    {
        if(wantSlot is null) return amount;
        if(!item) return amount;
        if (currentStack <= 0 || amount <= 0) return amount;
        ItemContainer targetItem = item;

        amount = Mathf.Min(amount, wantSlot.GetStackable(targetItem));

        amount -= RemoveItem(targetItem, amount);

        amount = wantSlot.AddItem(targetItem, amount);

        return amount;
    }

    public void LeftClick(ItemSlot wantSlot)
    {
        if (wantSlot is null) return;
        if (InputManager.IsShift)
        {
            if(wantSlot.GetIsEmpty())
            {
                if (GetIsEmpty()) return; 
                else if (wantSlot.Containable(item))
                {
                    GiveItem(wantSlot, Mathf.CeilToInt(currentStack * 0.5f));
                }
            }
            else if (Containable(wantSlot.item))
            {
                wantSlot.GiveItem(this, Mathf.CeilToInt(wantSlot.currentStack * 0.5f));
            }
        } 
        else
        {
            if (wantSlot.Containable(item))
            {
                GiveItem(wantSlot);
            }
            else
            {
                ExchangeItem(wantSlot);
            }
        }
        NoticeChanged();
        wantSlot.NoticeChanged();
    }

    public void RightClick(ItemSlot wantSlot)
    {
        if (wantSlot is null) return;

        if (InputManager.IsShift || wantSlot.GetIsEmpty())
        {
            if (wantSlot.Containable(item)) GiveSingleItem(wantSlot);
            else return;
        }
        else if (wantSlot.Containable(item))
        {
             wantSlot.GiveSingleItem(this);
        }
        else return;
        NoticeChanged();
        wantSlot.NoticeChanged();
    }
}

