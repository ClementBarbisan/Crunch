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
        npc.DEBUG_ChangeColor(Color.gray);

        if (!npc.CurrentStation)
        {
            npc.CurrentStation = FindClosestStation(npc.transform.position);
            if (npc.CurrentStation)
            {
                Debug.Log("Choose station");
                npc.Agent.isStopped = false;
                npc.Agent.speed = 3.5f;
                npc.Agent.SetDestination(npc.CurrentStation.transform.position + npc.CurrentStation.transform.forward);
            }
        }
    }

    public override void OnUpdateState(NPC npc)
    {
        if (!npc.IsWorking && !npc.CurrentStation.freeStation)
        {
            npc.CurrentStation = FindClosestStation(npc.transform.position);
            if (npc.CurrentStation)
            {
                Debug.Log("Choose station");
                npc.Agent.isStopped = false;
                npc.Agent.speed = 3.5f;
                npc.Agent.SetDestination(npc.CurrentStation.transform.position + npc.CurrentStation.transform.forward);
            }
        }
        Debug.Log("Test");
        if (Vector3.Distance(npc.transform.position, npc.Agent.destination) < 0.5f)
        {
            npc.CurrentStation.freeStation = false;
            npc.IsWorking = true;
        }
    }

    public override void OnLeaveState(NPC npc)
    {
        npc.Agent.isStopped = true;
    }

    public override bool ShouldLeaveState(NPC npc)
    {
        return npc.WorkStress >= npc.OverworkedMin || npc.WorkStress <= npc.UnderworkedMin;
    }
}
