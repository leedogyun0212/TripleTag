using UnityEngine;

/// <summary> 캐릭터의 현재 역할   </summary>
public enum CharacterType
{
    None,
    Runner, Chaser,
    Length
}

/// <summary> 캐릭터의 움직임 상태  </summary>
public enum MoveType
{
    None,
    walk, Run, Jump,
    Length
}