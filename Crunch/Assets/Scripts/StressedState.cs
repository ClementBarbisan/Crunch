using UnityEngine;

[CreateAssetMenu(fileName = "StressedState", menuName = "ScriptableObjects/StressedState")]
public class StressedState : AStateNPC
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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
