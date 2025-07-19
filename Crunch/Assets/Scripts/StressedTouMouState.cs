using System;
using UnityEngine;

[CreateAssetMenu(fileName = "StressedTouMouState", menuName = "ScriptableObjects/StressedTouMouState")]
public class StressedTouMouState : StressedState
{
    
    Exit FindClosestExit(Vector3 pos)
    {
        Exit[] exits = FindObjectsByType<Exit>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        float distance = Single.MaxValue;
        int index = -1;
        for (int i = 0; i < exits.Length; i++)
        {
            float curDist = Vector3.Distance(exits[i].transform.position, pos);
            if (curDist < distance)
            {
                distance = curDist;
                index = i;
            }
        }
        if (index >= 0)
            return (exits[index]);
        return (null);
    }
    
    public override void OnUpdateState(NPC npc)
    {
        base.OnUpdateState(npc);
        if (!npc.finishFrenzy)
        {
            GameManager.Instance.ProduceMoney(npc.FrenzyWorkEfficiencyRate);
        }
        if (npc.OldTimer >= 0 && npc.TimeCounter < 0)
        {
            Exit door = FindClosestExit(npc.transform.position);
            if (door)
            {
                npc.Agent.speed = StateWalkSpeed;
                npc.Agent.SetDestination(door.transform.position);
            }
            npc.finishFrenzy = true;
        }
        if (Vector3.Distance(npc.transform.position, npc.Agent.destination) < npc.DistanceToDestination && npc.finishFrenzy)
        {
            npc.gameObject.SetActive(false);
        }
    }
}
