using System;
using UnityEngine;

[CreateAssetMenu(fileName = "LazyHyperactifState", menuName = "ScriptableObjects/LazyHyperactifState")]
public class LazyHyperactifState : LazyState
{
    Station FindClosestOccupiedStation(Vector3 pos)
    {
        Station[] allStations = FindObjectsByType<Station>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        float distance = Single.MaxValue;
        int index = -1;
        for (int i = 0; i < allStations.Length; i++)
        {
            float curDist = Vector3.Distance(allStations[i].transform.position, pos);
            if (curDist < distance && !allStations[i].freeStation)
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
        base.OnEnterState(npc);
        npc.CurrentStation = FindClosestOccupiedStation(npc.transform.position);
        if (npc.CurrentStation)
        {
            npc.Agent.speed = StateWalkSpeed;
            npc.Agent.SetDestination(npc.CurrentStation.transform.position + npc.CurrentStation.transform.forward +
                                     npc.CurrentStation.transform.right);
        }
    }

    public override void OnUpdateState(NPC npc)
    {
        base.OnUpdateState(npc);
       //  = Physics.SphereCastNonAlloc(transform.position, interactRadius, transform.forward, _screamedDetectedHit, interactRange, interactableLayer);

    }
}
