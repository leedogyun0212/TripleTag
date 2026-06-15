using UnityEngine;

[CreateAssetMenu(fileName = "Item_Equipment", menuName = "Item/Equipment")]
public class Item_Equipment : ItemContainer
{
    public virtual void OnEquip(CharacterBase target)
    {

    }

    public virtual void OnEquip(CharacterBase target, BoneChange clothes)
    {
        //장착을 했을때 캐릭터의 SkinnedModel의 Transform을 이용해 옷을 자식으로 추가 
        //포지션을 (0,0,0)으로 설정
        //BoneChange를 통해 뼈를 맞춘다.

        //그러면 장착 할 때 장착하려는 것이 만들어 졌으면 꺼져 있는 것 켜고
        //없으면 새로 만든다. 

        GameObject currentEquip = ObjectManager.CreateObject(displayName, target.Body);
        currentEquip.transform.position = Vector3.zero;
        clothes.EquipClothing(currentEquip);
    }

    public virtual void OnUnequip(CharacterBase target)
    {
        // 비활성화? 뼈 매칭한것을 해제한 후 비활성화?
        // 그리고 장착할때 기존 아이템의 장착해제


    }
}
