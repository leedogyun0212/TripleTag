using Fusion;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : SimulationBehaviour, IPlayerJoined, IPlayerLeft
{

    [SerializeField] GameObject playerPrefab;
    [SerializeField] Vector3[] spawnRandom;
    private Dictionary<PlayerRef, NetworkObject> _spawnedCharacters = new Dictionary<PlayerRef, NetworkObject>();

    //public void PlayerJoined(PlayerRef player)
    //{
    //    //if (player != Runner.LocalPlayer)
    //    //{
    //    //    return;
    //    //}

    //    int spawnRandomNum = Random.Range(0, spawnRandom.Length);

    //    //Instantiate(playerPrefab, , Quaternion.identity);
    //    Runner.Spawn(playerPrefab, spawnRandom[spawnRandomNum], Quaternion.identity);
    //}

    

    public void PlayerJoined(PlayerRef player)
    {
        if (Runner.IsServer)
        {
            int spawnRandomNum = Random.Range(0, spawnRandom.Length-1);

            // 캐릭터 생성 시 player를 넘겨 Input Authority 부여
            NetworkObject networkPlayerObject = Runner.Spawn(playerPrefab, spawnRandom[spawnRandomNum], Quaternion.identity, player);

            // 딕셔너리에 저장하여 나중에 누가 나갔는지 식별 가능하게 함
            _spawnedCharacters.Add(player, networkPlayerObject);
        }
    }

    public void PlayerLeft(PlayerRef player)
    {
        // 서버에서만 실행
        if (Runner.IsServer)
        {
            // 딕셔너리에서 나간 플레이어의 객체를 찾음
            if (_spawnedCharacters.TryGetValue(player, out NetworkObject networkObject))
            {
                Debug.Log($"플레이어 {player.PlayerId}가 나갔습니다. 객체를 제거합니다.");

                // 네트워크 상에서 객체 제거
                Runner.Despawn(networkObject);

                // 리스트에서 삭제
                _spawnedCharacters.Remove(player);
            }
        }
    }
}
