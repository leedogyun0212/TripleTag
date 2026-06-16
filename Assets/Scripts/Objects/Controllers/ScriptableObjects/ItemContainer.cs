using UnityEngine;

public enum ItemType
{
    None,
    Equipment        = 500,
    Consumable       = 400,
    material         = 10,
    Miscellaneous    = 0,
    Quest            = 90,
    Important        = 100,
}

[CreateAssetMenu(fileName = "ItemContainer", menuName = "Item/ItemBase")]
public class ItemContainer : InfoContainer
{
    [Header("Item Base Info")]
    public int id;
    [Space]
    [Header("Item Detail")]
    public ItemType type;
    public int maxStack;

    public virtual int CompareByType(ItemContainer other)
    {
        int result = type - other.type;
        if (result != 0) return result;
        return id - other.id;
    }

    public virtual int CompareByType(ItemContainer other1, ItemContainer other2)
    {
        return default;
    }
}
