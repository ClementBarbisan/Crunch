using UnityEngine;

[CreateAssetMenu(fileName = "StressedState", menuName = "ScriptableObjects/StressedState")]
public class StressedState : AStateNPC
{
    public override void OnEnterState(NPC npc)
    {
        npc.DEBUG_ChangeColor(Color.red);
    }

    public override void OnUpdateState(NPC npc)
    {
    }

    public override void OnLeaveState(NPC npc)
    {
    }

    public override bool ShouldLeaveState(NPC npc)
    {
        return npc.WorkStress <= npc.OverworkedMin;
    }
}
