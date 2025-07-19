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
    
    public override void OnEnterState(NPC npc)
    {
        base.OnEnterState(npc);
        Exit door = FindClosestExit(npc.transform.position);
        if (door)
        {
            npc.Agent.isStopped = false;
            npc.Agent.speed = 1f;
            npc.Agent.SetDestination(door.transform.position);
        }
    }

    public override void OnUpdateState(NPC npc)
    {
        base.OnUpdateState(npc);
        if (Vector3.Distance(npc.transform.position, npc.Agent.destination) < 0.5f)
        {
            npc.gameObject.SetActive(false);
        }
    }
}
