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
                npc.Agent.isStopped = false;
                npc.Agent.speed = StateWalkSpeed;
                npc.Agent.SetDestination(npc.CurrentStation.transform.position + npc.CurrentStation.transform.forward);
            }
        }
    }

    public override void OnUpdateState(NPC npc)
    {
        if (npc.IsWorking)
        {
            GameManager.Instance.ProduceMoney(npc.WorkEfficiencyRate);
            return;
        }
        if (!npc.IsWorking && !npc.CurrentStation.freeStation)
        {
            npc.CurrentStation = FindClosestStation(npc.transform.position);
            if (npc.CurrentStation)
            {
                npc.Agent.speed = StateWalkSpeed;
                npc.Agent.SetDestination(npc.CurrentStation.transform.position + npc.CurrentStation.transform.forward);
            }
        }
        if (Vector3.Distance(npc.transform.position, npc.Agent.destination) < npc.DistanceToDestination)
        {
            npc.CurrentStation.freeStation = false;
            npc.CurrentStation.currentNPC = npc;
            npc.IsWorking = true;
        }
    }

    public override void OnLeaveState(NPC npc)
    {
    }

    public override bool ShouldLeaveState(NPC npc)
    {
        return npc.WorkStress >= npc.OverworkedMax || npc.WorkStress <= npc.UnderworkedMin;
    }
}
