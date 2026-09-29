using Fusion;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerSpawner : SimulationBehaviour, IPlayerJoined, IPlayerLeft
{

    [SerializeField] GameObject playerPrefab;
    [SerializeField] Vector3[] spawnRandom;
    private Dictionary<PlayerRef, NetworkObject> _spawnedCharacters = new Dictionary<PlayerRef, NetworkObject>();
    TeamType team;

    [SerializeField] Vector3[] teamASpawnPositions;
    [SerializeField] Vector3[] teamBSpawnPositions;

    public void PlayerJoined(PlayerRef player)
    {
        if (!Runner.IsServer)
            return;

        // 팀 배정
        TeamType team = AssignTeam();

        // 팀에 따른 Spawn 위치 결정
        Vector3 spawnPosition = GetSpawnPosition(team);

        // 캐릭터 생성
        NetworkObject networkPlayerObject = Runner.Spawn(playerPrefab, spawnPosition, Quaternion.identity, player);

        // 플레이어의 Team 정보 설정
        NetworkManager playerManager = networkPlayerObject.GetComponent<NetworkManager>();

        if (playerManager != null)
        {
            playerManager.Team = team;

            Debug.Log($"[Team] Player {player.PlayerId} → {team}");
        }
        else
        {
            Debug.LogError("[Team] PlayerPrefab에서 NetworkManager를 찾을 수 없습니다.");
        }

        // 딕셔너리에 저장
        // 
        _spawnedCharacters.Add(player, networkPlayerObject);
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

    private TeamType AssignTeam()
    {
        int teamACount = 0;
        int teamBCount = 0;

        foreach (NetworkObject playerObject in _spawnedCharacters.Values)
        {
            NetworkManager playerManager = playerObject.GetComponent<NetworkManager>();

            if (playerManager == null)
                continue;

            if (playerManager.Team == TeamType.TeamA)
            {
                teamACount++;
            }
            else if (playerManager.Team == TeamType.TeamB)
            {
                teamBCount++;
            }
        }
        //


        if (teamACount <= teamBCount)
        {
            return TeamType.TeamA;
        }

        return TeamType.TeamB;
    }

    private Vector3 GetSpawnPosition(TeamType team)
    {
        Vector3[] spawnPositions;

        if (team == TeamType.TeamA)
        {
            spawnPositions = teamASpawnPositions;
        }
        else
        {
            spawnPositions = teamBSpawnPositions;
        }

        if (spawnPositions == null || spawnPositions.Length == 0)
        {
            Debug.LogError($"[Team] {team} Spawn 위치가 설정되지 않았습니다.");

            return Vector3.zero;
        }

        int randomIndex = Random.Range(0, spawnPositions.Length);

        return spawnPositions[randomIndex];
    }//
}
