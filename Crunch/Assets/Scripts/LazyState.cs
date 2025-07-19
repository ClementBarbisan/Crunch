using UnityEngine;

[CreateAssetMenu(fileName = "LazyState", menuName = "ScriptableObjects/LazyState")]
public class LazyState : AStateNPC
{
    public override void OnEnterState(NPC npc)
    {
        npc.DEBUG_ChangeColor(Color.blue);

    }

    public override void OnUpdateState(NPC npc)
    {
    }

    public override void OnLeaveState(NPC npc)
    {
        npc.Agent.isStopped = true;
    }

    public override bool ShouldLeaveState(NPC npc)
    {
        return npc.WorkStress >= npc.UnderworkedMax;
    }
}
