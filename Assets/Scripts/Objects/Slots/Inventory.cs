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
        return default;
    }

    public ItemSlot FindItem(string containWord)
    {
        return default;
    }

    public ItemSlot FindFirstEmptySlot()
    {
        return default;
    }

    public ItemSlot FindLastEmptySlot()
    {
        return default;
    }

    public ItemSlot FindFirstItem(ItemContainer target)
    {
        return default;
    }

    public ItemSlot FindLastItem(ItemContainer target)
    {
        return default;
    }

    public int AddItem(ItemContainer wantItem, int amount = 1)
    {
        return default;
    }

    public int AddItemOnExistSlots (ItemContainer wantItem, int amount)
    {
        return default;
    }

    public int AddItemOnEmptySlots (ItemContainer wantItem, int amount)
    {
        return default;
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
        return default;
    }

    public int RemoveItem(ItemContainer wantItem, int amount)
    {
        return default;
    }

    public int RemoveItemOnExistSlots(ItemContainer wantItem, int amount)
    {
        return default;
    }

    public int RemoveItemFormLocation( int row, int column)
    {
        return default;
    }

    public int RemoveItemFormLocation(int row, int column, int amount)
    {
        return default;
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
