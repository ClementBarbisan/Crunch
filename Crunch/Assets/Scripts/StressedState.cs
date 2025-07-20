using UnityEngine;

[CreateAssetMenu(fileName = "StressedState", menuName = "ScriptableObjects/NPC/StressedState")]
public class StressedState : AStateNPC
{
    public override void OnEnterState(NPC npc)
    {
        npc.DEBUG_ChangeColor(Color.red);
        npc.SwitchFaceRenderer(2);
    }

    public override void OnUpdateState(NPC npc)
    {
    }

    public override void OnLeaveState(NPC npc)
    {
        npc.finishFrenzy = false;
        if (npc.animator != null)
        {
            npc.animator.SetBool(npc._isWorkingParamName, false);
        }
    }

    public override bool ShouldLeaveState(NPC npc)
    {
        return npc.WorkStress <= npc.OverworkedMin;
    }
}
