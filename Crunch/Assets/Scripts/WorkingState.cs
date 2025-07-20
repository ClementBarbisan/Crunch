using System;
using UnityEngine;

[CreateAssetMenu(fileName = "WorkingState", menuName = "ScriptableObjects/NPC/WorkingState")]
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
        npc.SwitchFaceRenderer(0);

        if (!npc.CurrentStation)
        {
            npc.CurrentStation = FindClosestStation(npc.transform.position);
            if (npc.CurrentStation)
            {
                npc.Agent.enabled = true;
                npc.Agent.speed = StateWalkSpeed;
                npc.Agent.SetDestination(npc.CurrentStation.transform.position + npc.CurrentStation.transform.forward / 2f);
            }
        }
    }

    public override void OnUpdateState(NPC npc)
    {
        if (npc.IsWorking)
        {
            GameManager.Instance.ProduceMoney(npc.WorkEfficiencyRate);
            if (npc.animator != null && npc.CurrentStation != null && npc.IsWorking)
            {
                npc.animator.SetBool(npc._isWorkingParamName, true);
            }
            return;
        }
        if ((!npc.CurrentStation || !npc.CurrentStation.freeStation && npc.CurrentStation.currentNPC != npc))
        {
            npc.CurrentStation = FindClosestStation(npc.transform.position);
            if (npc.CurrentStation)
            {
                npc.Agent.enabled = true;
                npc.Agent.speed = StateWalkSpeed;
                npc.Agent.SetDestination(npc.CurrentStation.transform.position + npc.CurrentStation.transform.forward);
            }
            else
            {
                npc.Agent.enabled = false;
            }
        }
        if (npc.CurrentStation && Vector3.Distance(npc.transform.position, npc.CurrentStation.transform.position) < npc.DistanceToDestination)
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
