using System;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "StressedVenerState", menuName = "ScriptableObjects/NPC/StressedVenerState")]
public class StressedVenerState : StressedState
{
    [SerializeField] private float timeStun = 2.5f;
    private bool _hasSpawnedVfx;

    public override void OnEnterState(NPC npc)
    {
        base.OnEnterState(npc);
        _hasSpawnedVfx = false;
    }

    public override void OnUpdateState(NPC npc)
    {
        base.OnUpdateState(npc);
        if (!npc.finishFrenzy && npc.CurrentStation != null && npc.IsWorking)
        {
            GameManager.Instance.ProduceMoney(npc.FrenzyWorkEfficiencyRate);
            if (npc.animator != null)
            {
                npc.animator.SetBool(npc._isWorkingParamName, true);
            }
            return;
        }


        if (!_hasSpawnedVfx)
        {
            if (npc.OverworkedVFXs.Length > 0)
            {
                npc.OverworkedVFXs[Random.Range(0, npc.OverworkedVFXs.Length)].Play();
            }
            _hasSpawnedVfx = true;
        }

        if (npc.animator != null)
        {
            npc.animator.SetBool(npc._isWorkingParamName, false);
        }

        npc.Agent.SetDestination(Camera.main.transform.position);
        if (Physics.Raycast(npc.transform.position, npc.transform.forward, out RaycastHit hit, 1f, 1 << 7))
        {
            hit.collider.GetComponent<PlayerController>().OnStun(timeStun);
            npc.WorkStress = 0.5f;
        }
    }

    public override void OnLeaveState(NPC npc)
    {
        base.OnLeaveState(npc);
        if (npc.CurrentStation != null && npc.IsWorking)
        {
            if (npc.animator != null)
            {
                npc.animator.SetBool(npc._isWorkingParamName, false);
            }
        }
    }
}
