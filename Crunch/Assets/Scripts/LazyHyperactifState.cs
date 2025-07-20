using System;
using UnityEngine;

[CreateAssetMenu(fileName = "LazyHyperactifState", menuName = "ScriptableObjects/NPC/LazyHyperactifState")]
public class LazyHyperactifState : LazyState
{
    [SerializeField] private float _radiusSphere = 2.5f;
    [SerializeField] private LayerMask _interactableLayer = 1 << 6;
    [SerializeField] private float _dividerDestressOthers = 30f;

    private Collider[] colliders = new Collider[10];

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
            npc.Agent.enabled = true;
            npc.Agent.speed = StateWalkSpeed;
            npc.Agent.SetDestination(npc.CurrentStation.transform.position + npc.CurrentStation.transform.forward / 2f +
                                     npc.CurrentStation.transform.right / 2f);
        }
    }

    public override void OnUpdateState(NPC npc)
    {
        base.OnUpdateState(npc);
        int detectedHits = Physics.OverlapSphereNonAlloc(npc.transform.position, _radiusSphere, colliders, _interactableLayer);
        if (detectedHits > 0)
        {
            for (int i = 0; i < detectedHits; i++)
            {
                if (colliders[i].TryGetComponent(out NPC npcOther))
                {
                    npcOther.WorkStress -= Time.deltaTime / _dividerDestressOthers;
                }
            }
        }
    }
}
