using System;
using UnityEngine;

[CreateAssetMenu(fileName = "WorkingState", menuName = "ScriptableObjects/WorkingState")]
public class WorkingState : AStateNPC
{
    Station FindClosestStation(Vector3 pos)
    {
        Station[] allStations = FindObjectsByType<Station>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        float distance = Single.MaxValue;
        int index = -1;
        for (int i = 0; i < allStations.Length; i++)
        {
            float curDist = Vector3.Distance(allStations[i].transform.position, pos);
            if (curDist < distance && allStations[i].freeStation)
            {
                distance = curDist;
                index = i;
            }
        }
        if (index >= 0)
            return (allStations[index]);
        return (null);
    }
    
    public override void OnEnterState(NPC npc)
    {
        if (!npc.currentStation)
        {
            npc.currentStation = FindClosestStation(npc.transform.position);
            if (npc.currentStation)
            {
                npc.agent.destination = npc.currentStation.transform.position;
                npc.currentStation.freeStation = false;
            }
        }
    }

    public override void OnUpdateState(NPC npc)
    {
       
    }

    public override void OnLeaveState(NPC npc)
    {
        if (npc.currentStation)
        {
            npc.currentStation.freeStation = true;
            npc.currentStation = null;
        }
    }

    public override bool ShouldLeaveState(NPC npc)
    {
        throw new System.NotImplementedException();
    }
}
