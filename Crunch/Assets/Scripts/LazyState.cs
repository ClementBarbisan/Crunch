using UnityEngine;

[CreateAssetMenu(fileName = "LazyState", menuName = "ScriptableObjects/LazyState")]
public class LazyState : AStateNPC
{
    public override void OnEnterState(NPC npc)
    {
        npc.DEBUG_ChangeColor(Color.blue);
        npc.CurrentStation.freeStation = true;
        npc.CurrentStation.currentNPC = null;
        npc.CurrentStation = null;
        npc.IsWorking = false;
        npc.Agent.SetDestination(new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5f, 5f)));
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
