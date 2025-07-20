    using UnityEngine;

[CreateAssetMenu(fileName = "StressedHyperactifState", menuName = "ScriptableObjects/NPC/StressedHyperactifState")]
public class StressedHyperactifState : StressedState
{
    public override void OnUpdateState(NPC npc)
    {
        base.OnUpdateState(npc);
        if(!npc.finishFrenzy)
        {
            GameManager.Instance.ProduceMoney(npc.FrenzyWorkEfficiencyRate);
            if (npc.animator != null)
            {
                npc.animator.SetBool(npc._isWorkingParamName, true);
            }
        }
        if (npc.OldTimer > 0 && npc.TimeCounter <= 0)
        {
            npc.Agent.enabled = false;
            npc.Agent.speed = StateWalkSpeed;
            npc.Agent.SetDestination(new Vector3(Random.Range(-15f, 15f), 0f, Random.Range(-15f, 15f)));
            npc.finishFrenzy = true;
        }
        if (npc.Agent.velocity.magnitude < 0.1f && npc.finishFrenzy)
        {
            npc.Agent.SetDestination(new Vector3(Random.Range(-15f, 15f), 0f, Random.Range(-15f, 15f)));
        }
    }
}
