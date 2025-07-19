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
                npc.Agent.destination = npc.CurrentStation.transform.position;
                npc.CurrentStation.freeStation = false;
            }
        }
    }

    public override void OnUpdateState(NPC npc)
    {
       
    }

    public override void OnLeaveState(NPC npc)
    {
    }

    public override bool ShouldLeaveState(NPC npc)
    {
        return npc.WorkStress >= npc.OverworkedMin || npc.WorkStress <= npc.UnderworkedMin;
    }
}
