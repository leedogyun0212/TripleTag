using Fusion;
using UnityEngine;

public class PlayerSpawner : SimulationBehaviour, IPlayerJoined
{

    [SerializeField] GameObject playerPrefab;
    [SerializeField] Vector3[] spawnRandom;

    public void PlayerJoined(PlayerRef player)
    {
        //if (player != Runner.LocalPlayer)
        //{
        //    return;
        //}

        int spawnRandomNum = Random.Range(0, spawnRandom.Length);

        //Instantiate(playerPrefab, spawnRandom[spawnRandomNum], Quaternion.identity);
        Runner.Spawn(playerPrefab, spawnRandom[spawnRandomNum], Quaternion.identity);
    }

}
