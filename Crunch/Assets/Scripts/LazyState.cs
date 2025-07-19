using UnityEngine;

[CreateAssetMenu(fileName = "LazyState", menuName = "ScriptableObjects/LazyState")]
public class LazyState : AStateNPC
{
    public override void OnEnterState(NPC npc)
    {
        npc.DEBUG_ChangeColor(Color.blue);
        if (npc.CurrentStation)
        {
            npc.CurrentStation.freeStation = true;
            npc.CurrentStation.currentNPC = null;
            npc.CurrentStation = null;
        }
        npc.IsWorking = false;
        npc.Agent.SetDestination(new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5f, 5f)));

        if (npc.UnderworkedVFXs.Length > 0)
        {
            npc.UnderworkedVFXs[Random.Range(0, npc.UnderworkedVFXs.Length)].Play();
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
        return npc.WorkStress >= npc.UnderworkedMax;
    }
}
