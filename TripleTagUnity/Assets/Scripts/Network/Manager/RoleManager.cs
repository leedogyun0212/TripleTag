using Fusion;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoleManager : NetworkBehaviour
{
    public void AssignRoles(List<NetworkManager> teamA, List<NetworkManager> teamB)
    {
        if (!Runner.IsServer)
            return;

        AssignTeamRole(teamA);
        AssignTeamRole(teamB);
    }
    //
    private void AssignTeamRole(List<NetworkManager> team)
    {
        if (team.Count == 0)
            return;

        int chaserIndex = Random.Range(0, team.Count);

        for (int i = 0; i < team.Count; i++)
        {
            if (i == chaserIndex)
            {
                team[i].CharType = CharacterType.Chaser;

                Debug.Log(
                $"[Role] Player {team[i].Object.InputAuthority} → Chaser");
            }
            else
            {
                team[i].CharType = CharacterType.Runner;

                Debug.Log(
                $"[Role] Player {team[i].Object.InputAuthority} → Runner");
            }
        }
    }
}