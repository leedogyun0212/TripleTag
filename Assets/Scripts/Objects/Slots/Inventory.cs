using System.Collections.Generic;
using UnityEngine;


public class Inventory : MonoBehaviour
{
    //몇 칸인지?
    //칸 제한을 걸기 위해서 필요한 두 가지의 숫자
    //가로 개수 세로개수
    //Columns, Rows
    //  열      행
    public int columns;
    public int rows;

    //아이템 슬롯을 columns 와 rows 개수만큼 준비해야해요
    //2차원 행렬을 준비!
    //대상을 여러개 저장, 개수가 바뀌지 않고, 순환하는데에 빨라야 해요!
    //배열(Array)
    //      [1,2]
    ItemSlot[,] slots;

    public void Initialize()
    {
        slots = new ItemSlot[rows, columns];
        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                slots[row, column] = new ItemSlot();
            }
        }
    }

    public void WearPlus(int amount)
    {
        ItemContainer Wear = DataManager.LoadDataFile<ItemContainer>("Outwear_02");
        AddItem(Wear,amount);
    }
    public void WearMinus(int amount)
    {
        ItemContainer Wear = DataManager.LoadDataFile<ItemContainer>("Outwear_02");
        RemoveItem(Wear,amount);
    }

    public void Sort(System.Comparison<ItemContainer> Method)
    {

    }

    public void AutoQuickInsert(Inventory other)
    {

    }

    public void AutoQuickInsert(Inventory[] other)
    {

    }

    public bool InsertAll(Inventory other)
    {
        return default;
    }

    public bool InsertAll(Inventory other, ItemContainer target)
    {
        return default;
    }

    public void LockSlot(int wantRow, int wantColumn)
    {

    }

    public void UnlockSlot(int wantRow, int wantColumn)
    {

    }

    public int CountItem(ItemContainer wantItem, out List<ItemSlot> returnSlots)
    {
        returnSlots = default;
        return default;
    }

    //반복기 => 
    public IEnumerable<ItemSlot> GetAllslot()
    {
        //ItemSlot[] result = new ItemSlot[slots.Length];

        int height = slots.GetLength(0);
        int width = slots.GetLength(1);
        for (int row = 0; row < height; row++)
        {
            for (int column = 0; column < width; column++)
            {
                if (slots[row, column] is null) continue;

                yield return slots[row, column];
            }
        }
    }

    public IEnumerable<ItemSlot> GetAllslot(System.Predicate<ItemSlot> pred)
    {
        //ItemSlot[] result = new ItemSlot[slots.Length];

        int height = slots.GetLength(0);
        int width = slots.GetLength(1);
        for (int row = 0; row < height; row++)
        {
            for (int column = 0; column < width; column++)
            {
                if (slots[row, column] is null) continue;

                yield return slots[row, column];
            }
        }
    }

    public IEnumerable<ItemSlot> GetAllslotReverse()
    {
        //ItemSlot[] result = new ItemSlot[slots.Length];

        int height = slots.GetLength(0);
        int width = slots.GetLength(1);
        for (int row = height - 1; row >= 0; row--)
        {
            for (int column = width - 1; column >= 0; column--)
            {
                if (slots[row, column] is null) continue;

                yield return slots[row, column];
            }
        }
    }

    public IEnumerable<ItemSlot> GetAllslotReverse(System.Predicate<ItemSlot> pred)
    {
        //ItemSlot[] result = new ItemSlot[slots.Length];

        int height = slots.GetLength(0);
        int width = slots.GetLength(1);
        for (int row = height - 1; row >= 0; row--)
        {
            for (int column = width - 1; column >= 0; column--)
            {
                if (slots[row, column] is null) continue;

                yield return slots[row, column];
            }
        }
    }

    public ItemSlot FindItem(ItemContainer target)
    {
        return default;
    }

    public ItemSlot FindItem(ItemType wantType)
    {
        return default;
    }

    public ItemSlot FindItem(int wantRow, int wantColumn)
    {
        if (wantRow    < 0 || wantColumn < 0) return null;
        if (wantRow    >= slots.GetLength(0)) return null;
        if (wantColumn >= slots.GetLength(1)) return null;
        return slots[wantRow, wantColumn];
    }

    public ItemSlot FindItem(string containWord)
    {
        return default;
    }

    public IEnumerable<ItemSlot> FindFirstEmptySlot()
    {
        foreach (ItemSlot currentSlot in GetAllslot())
        {
            if (currentSlot.GetIsEmpty()) yield return currentSlot;
        }
    }

    public IEnumerable<ItemSlot> FindLastEmptySlot()
    {
        foreach (ItemSlot currentSlot in GetAllslotReverse())
        {
            if (currentSlot.GetIsEmpty()) yield return currentSlot;
        }
    }

    public IEnumerable<ItemSlot> FindFirstItem(ItemContainer target)
    {
        foreach (ItemSlot currentSlot in GetAllslot())
        {
            if (currentSlot.GetItem() == target) yield return currentSlot;
        }
    }

    public IEnumerable<ItemSlot> FindLastItem(ItemContainer target)
    {
        foreach (ItemSlot currentSlot in GetAllslotReverse())
        {
            if (currentSlot.GetItem() == target) yield return currentSlot;
        }
    }

    public int AddItem(ItemContainer wantItem, int amount = 1)
    {
        amount = AddItemOnExistSlots(wantItem, amount);

        if(amount <= 0) return 0;

        return AddItemOnEmptySlots(wantItem, amount);
    }

    public int AddItemOnExistSlots(ItemContainer wantItem, int amount)
    {
        foreach (ItemSlot currentSlot in FindFirstItem(wantItem))
        {
            if (amount <= 0) return 0;
            amount = currentSlot.AddItem(wantItem, amount);
            currentSlot.NoticeChanged();
        }

        return amount;
    }

    public int AddItemOnEmptySlots(ItemContainer wantItem, int amount)
    {
        foreach (ItemSlot currentSlot in FindFirstEmptySlot())
        {
            if (amount <= 0) return 0;
            amount = currentSlot.AddItem(wantItem, amount);
            currentSlot.NoticeChanged();
        }

        return amount;
    }

    public int AddItemToLocation (ItemContainer wantItem, int amount, int row, int column)
    {
        return default;
    }
    
    public ItemSlot[,] Clear()
    {
        ItemSlot[,] origin = slots;
        Initialize();
        return origin;
    }

    public int RemoveItem(System.Predicate<ItemContainer> condition)
    {
        return default;
    }

    public int RemoveItem(ItemContainer wantItem)
    {
        int result = 0;
        foreach (ItemSlot currentSlot in FindLastItem(wantItem))
        {
            result = currentSlot.RemoveItem(wantItem);
            currentSlot.NoticeChanged();
        }
        return result;
    }

    public int RemoveItem(ItemContainer wantItem, int amount)
    {
        amount = RemoveItemOnExistSlots(wantItem, amount);

        if (amount <= 0) return 0;

        return amount;
    }

    public int RemoveItemOnExistSlots(ItemContainer wantItem, int amount)
    {
        foreach (ItemSlot currentSlot in FindLastItem(wantItem))
        {
            if (amount <= 0) return 0;
            amount = currentSlot.RemoveItem(wantItem, amount);
            currentSlot.NoticeChanged();
        }
        return amount;
    }

    public int RemoveItemFormLocation( int row, int column)
    {
        ItemSlot targetSlot = FindItem(row, column);
        int removed = 0;
        if ( targetSlot is not null)
        {
            removed = targetSlot.RemoveItem(targetSlot.GetItem());
            targetSlot.NoticeChanged();
        }

        return removed;
    }

    public int RemoveItemFormLocation(int row, int column, int amount)
    {
        ItemSlot targetSlot = FindItem(row, column);
        if (targetSlot is not null)
        {
            amount = targetSlot.RemoveItem(targetSlot.GetItem(), amount);
            targetSlot.NoticeChanged();
        }
        return amount;
    }

    public void MoveItem(int startRow, int startColumn, Inventory targetInventory, int targetRow, int targetColumn , int amount = -1)
    {

    }

    public bool UseItem(ItemContainer target)
    {
        return default;
    }

    public bool UseItem(int startRow, int startColumn)
    {
        return default;
    }
}
