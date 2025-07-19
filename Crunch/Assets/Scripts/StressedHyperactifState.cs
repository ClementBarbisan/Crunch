using UnityEngine;

[CreateAssetMenu(fileName = "StressedHyperactifState", menuName = "ScriptableObjects/StressedHyperactifState")]
public class StressedHyperactifState : StressedState
{
    public override void OnUpdateState(NPC npc)
    {
        base.OnUpdateState(npc);
        if (npc.Agent.velocity.magnitude < 0.1f)
        {
            npc.Agent.isStopped = false;
            npc.Agent.speed = 5f;
            npc.Agent.SetDestination(new Vector3(Random.Range(-15f, 15f), 0f, Random.Range(-15f, 15f)));
        }
    }
}
