using UnityEngine;

public enum ItemType
{
    None,
    Equipment,Consumable, material, Miscellaneous, Quest, Important,
    Length
}

[CreateAssetMenu(fileName = "ItemContainer", menuName = "Item/ItemBase")]
public class ItemContainer : InfoContainer
{
    public ItemType type;
    public int maxStack;
}
