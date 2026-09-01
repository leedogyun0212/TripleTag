using System.Collections.Generic;
using UnityEngine;


public class Inventory : MonoBehaviour
{
    //인벤토리에서 스태틱으로 만들긴 할 건데
    //주의할 점!
    //static은 해당 프로그램이 종료될 때 까지 유지!
    //인게임 플레이가 종료되거나 세이브되거나 다시 시작하거나 등등
    //다채로운 상황에서 얘를 관리해주셔야 함!
    public static ItemSlot cursorSlot = new ItemSlot();

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

    readonly string[] itemList = { "Outwear_01", "Outwear_02" };

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
        int index = Random.Range(0, itemList.Length);
        ItemContainer Wear = DataManager.LoadDataFile<ItemContainer>(itemList[index]);
        AddItem(Wear,amount);
    }
    public void WearMinus(int amount)
    {
        ItemContainer Wear = DataManager.LoadDataFile<ItemContainer>("Outwear_02");
        RemoveItem(Wear,amount);
    }

    //Comparison
    // 마이너스 : 왼쪽이 작다
    //    0    : 같다
    // 플러스   : 오른쪽이 작다
    public void Sort(System.Comparison<ItemSlot> Method)
    {
        MergeAll();

        //배열 자체를 정렬할 수는 없다.
        //비교를 했을때 내용만 바꿔준다!
        //사다리 타기를 통해 버블정렬 당첨
        int totalLength = slots.Length;
        if (slots is null || totalLength <= 1) return;
        int width =slots.GetLength(1);

        int lastFinder = totalLength - 1;

        while (lastFinder > 0)
        {
            int currentFinder = -1;
            for (int i = 0; i < lastFinder; i++)
            {
                ItemSlot left = GetSlot(i, width);
                ItemSlot right = GetSlot(i + 1, width);
                int comparisonResult = Method(left, right);

                if (comparisonResult < 0)
                {
                    currentFinder = i;
                    left.ExchangeItem(right);
                }
            }

            lastFinder = currentFinder;
        }

        foreach (ItemSlot currentSlot in GetAllslot())
        {
            currentSlot?.NoticeChanged();
        }
    }

    int ItemTypeComparison(ItemSlot left, ItemSlot right)
    {
        int result;
        if (ItemExistComparison(left, right, out result)) return result;

        ItemContainer leftItem = left.GetItem();
        ItemContainer rightItem = right.GetItem();

        // - : 왼쪽이 작음
        // 0 : 같음
        // + : 왼쪽이 
        result = leftItem.CompareByType(rightItem);
        if (result != 0) return result;
        result = left.GetStack() - right.GetStack();
        return result;

        //return leftItem.type - rightItem.type;
    }

    int? ItemExistComparison(ItemSlot left, ItemSlot right)
    {
        if (left is null)
        {
            if (right is null) return 0;
            else return -1;
        }
        if (right is null) return 1;
        ItemContainer leftItem = left.GetItem();
        ItemContainer rightItem = right.GetItem();
        if (!leftItem)
        {
            if (!rightItem) return 0;
            else return -1;
        }
        if (!rightItem) return 1;

        return null;
    }    

    bool ItemExistComparison(ItemSlot left, ItemSlot right, out int result)
    {
        int? calculated = ItemExistComparison(left, right); // 원래 함수 실행
        result = calculated ?? 0; // 결과를 저장하는데 결과가 없으면 0
        return calculated.HasValue; // 값이 나왔는지 여부를 반환
    }

    public void SortByType() => Sort(ItemTypeComparison);

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

    public int CountItem(ItemContainer wantItem)
    {
        if (!wantItem) return 0;

        int result = 0;

        foreach (ItemSlot currentSlot in FindFirstItem(wantItem))
        {
            result += currentSlot.GetStack();
        }

        return result;
    }
    public int CountItem(ItemContainer wantItem, out List<ItemSlot> returnSlots)
    {
        returnSlots = new();
        if (!wantItem) return 0;
        
        int result = 0;

        foreach (ItemSlot currentSlot in FindFirstItem(wantItem))
        {
            returnSlots.Add(currentSlot);

            result += currentSlot.GetStack();
        }

        return result;
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

    public ItemSlot GetSlot(int index, int width) => slots[index / width, index % width];
    public ItemSlot GetSlot(int index)
    {
        if (slots is null || index < 0 || slots.Length == 0 || slots.Length <= index) return null;
        int width = slots.GetLength(1);
        return slots[index / width, index % width];
        //1차원 배열
        //return slots[index];
        //2차원 배열이면?
        //
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

    public IEnumerable<ItemContainer> GetAllItem()
    {
        //어떻게 나는 모든 아이템을 다 뽑아줄 수 있을까?
        //이미 merge한 아이템을 체크할 필요 x
        //중복 없이 모든 아이템을 내보내줘야 할텐데
        //내 인벤토리에 있는 모든 "아이템 종류"를 하나씩
        HashSet<ItemContainer> usedItem = new();

        foreach (ItemSlot currentSlot in GetAllslot())
        {
            ItemContainer currentItem = currentSlot.GetItem();
            if(!currentItem) continue;
            if (!usedItem.Add(currentItem)) continue;
            
            yield return currentItem;
        }
    }

    public Dictionary<ItemContainer, List<ItemSlot>> GetAllItemList()
    {
        Dictionary<ItemContainer, List<ItemSlot>> result = new();

        foreach(ItemSlot currentSlot in GetAllslot())
        {
            ItemContainer currentItem = currentSlot.GetItem();
            if (!currentItem) continue;
            if (result.TryGetValue(currentItem, out List<ItemSlot>  currentList))
            {
                currentList.Add(currentSlot);
            }
            else
            {
                result.Add(currentItem, new() { currentSlot });
            }
        }

        return result;
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

    public void MergeAll()
    {
        foreach(ItemContainer currentItem in GetAllItem())
        {
            MergeItem(currentItem);
        }
    }

    public void MergeItem(ItemContainer wantItem)
    {
        if (!wantItem) return;//아이템이 없다
        int maxStack = wantItem.maxStack;
        if (maxStack <= 1) return;//이거.. 못 합치는데?
        //총합 개수!
        int totalCount = CountItem(wantItem, out List<ItemSlot> containSlots);
        if (totalCount <= 1) return; // 아이템 하나 밖에 없는데?
        //들어있는 슬롯이 없거나      
        if (containSlots is null) return;
        int slotCount = containSlots.Count; // 슬롯 개수
        // 총개수가 슬롯에 담을수있는 개수를 넘음   슬롯이 다해서 1개밖에 없거나
        if (totalCount >= slotCount * maxStack || slotCount <= 1) return;

        int finalSlot = slotCount - 1;
        //모든 슬롯을 돌아주면서
        for (int i = 0; i < finalSlot; i++)
        {
            ItemSlot currentSlot = containSlots[i];
            for (int j = finalSlot; j > i; j--)
            {
                if (currentSlot.GetIsMax()) break;//꽉 찬 슬롯은 병합할 필요가 없으니까 패스!
                ItemSlot targetSlot = containSlots[j];
                targetSlot.GiveItem(currentSlot);
                if (targetSlot.GetIsEmpty()) finalSlot--;
            }
        }
    }
    //실실
    //고백
    //선바
    //아메7137810555
    public void ExchangeItem(int startRow, int startColumn, int targetRow, int targetColumn)
    {
        ExchangeItem(startRow, startColumn, this, targetRow, targetColumn);
    }
    public void ExchangeItem(int startRow, int startColumn, ItemSlot targetSlot)
    {
        ItemSlot first = FindItem(startRow, startColumn);
        if (first is null) return;
        if (targetSlot is null) return;

        ItemSlot second = targetSlot;
        if (second is null) return;
        first.ExchangeItem(second);
        first.NoticeChanged();
        second.NoticeChanged();
    }
    public void ExchangeItem(int startRow, int startColumn, Inventory targetInventory, int targetRow, int targetColumn)
    {
        if (!targetInventory) return;
        ExchangeItem(startRow, startColumn, targetInventory.FindItem(targetRow, targetColumn));
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
