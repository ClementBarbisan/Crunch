using System;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "StressedTouMouState", menuName = "ScriptableObjects/NPC/StressedTouMouState")]
public class StressedTouMouState : StressedState
{
    private bool _hasSpawnedVfx;
    Exit FindClosestExit(Vector3 pos)
    {
        Exit[] exits = FindObjectsByType<Exit>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        float distance = Single.MaxValue;
        int index = -1;
        for (int i = 0; i < exits.Length; i++)
        {
            float curDist = Vector3.Distance(exits[i].transform.position, pos);
            if (curDist < distance)
            {
                distance = curDist;
                index = i;
            }
        }
        if (index >= 0)
            return (exits[index]);
        return (null);
    }
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
        }

        if (npc.OldTimer >= 0 && npc.TimeCounter < 0)
        {
            Exit door = FindClosestExit(npc.transform.position);
            if (door)
            {
                npc.Agent.enabled = true;
                npc.Agent.speed = StateWalkSpeed;
                npc.Agent.SetDestination(door.transform.position + new Vector3(Random.Range(-2, 2), 0, Random.Range(-2, 2)));
            }
            npc.finishFrenzy = true;

            if (npc.animator != null)
            {
                npc.animator.SetBool(npc._isWorkingParamName, false);
            }
            if(!_hasSpawnedVfx)
            {
                if (npc.OverworkedVFXs.Length > 0)
                {
                    npc.OverworkedVFXs[Random.Range(0, npc.OverworkedVFXs.Length)].Play();
                }
                _hasSpawnedVfx = true;
            }

        }
        /*if (Vector3.Distance(npc.transform.position, npc.Agent.destination) < npc.DistanceToDestination && npc.finishFrenzy)
        {
            npc.gameObject.SetActive(false);
        }*/
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
