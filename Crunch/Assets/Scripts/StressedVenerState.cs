using System;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "StressedVenerState", menuName = "ScriptableObjects/StressedVenerState")]
public class StressedVenerState : StressedState
{
    [SerializeField] private float timeStun = 2.5f;
    public override void OnUpdateState(NPC npc)
    {
        base.OnUpdateState(npc);
        if (!npc.finishFrenzy)
        {
            GameManager.Instance.ProduceMoney(npc.FrenzyWorkEfficiencyRate);
            return;
        }
        npc.Agent.SetDestination(Camera.main.transform.position);
        if (Physics.Raycast(npc.transform.position, npc.transform.forward, out RaycastHit hit, 1f, 1 << 7))
        {
            hit.collider.GetComponent<PlayerController>().OnStun(timeStun);
            npc.WorkStress = 0.5f;
        }
    }
}
