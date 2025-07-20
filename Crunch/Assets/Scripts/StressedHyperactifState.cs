    using UnityEngine;

[CreateAssetMenu(fileName = "StressedHyperactifState", menuName = "ScriptableObjects/NPC/StressedHyperactifState")]
public class StressedHyperactifState : StressedState
{
    private bool _hasSpawnedVfx;

    public override void OnEnterState(NPC npc)
    {
        base.OnEnterState(npc);
        _hasSpawnedVfx = false;
    }

    public override void OnUpdateState(NPC npc)
    {
        base.OnUpdateState(npc);
        if(!npc.finishFrenzy && npc.CurrentStation != null && npc.IsWorking)
        {
            GameManager.Instance.ProduceMoney(npc.FrenzyWorkEfficiencyRate);
            if (npc.animator != null )
            {
                npc.animator.SetBool(npc._isWorkingParamName, true);
            }

        }
        if (npc.OldTimer > 0 && npc.TimeCounter <= 0)
        {
            npc.Agent.isStopped = false;
            npc.Agent.speed = StateWalkSpeed;
            npc.Agent.SetDestination(new Vector3(Random.Range(-15f, 15f), 0f, Random.Range(-15f, 15f)));
            npc.finishFrenzy = true;

            if (!_hasSpawnedVfx)
            {
                if (npc.OverworkedVFXs.Length > 0)
                {
                    npc.OverworkedVFXs[Random.Range(0, npc.OverworkedVFXs.Length)].Play();
                }
                if (npc.animator != null)
                {
                    npc.animator.SetBool(npc._isWorkingParamName, false);
                }
                _hasSpawnedVfx = true;
            }
        }
        if (npc.Agent.velocity.magnitude < 0.1f && npc.finishFrenzy)
        {
            npc.Agent.SetDestination(new Vector3(Random.Range(-15f, 15f), 0f, Random.Range(-15f, 15f)));
        }
    }

    public override void OnLeaveState(NPC npc)
    {
        base.OnLeaveState(npc);
        if (npc.CurrentStation != null && npc.IsWorking)
        {
            if (npc.animator != null)
            {
                npc.animator.SetBool(npc._isWorkingParamName, true);
            }
        }
    }
}
