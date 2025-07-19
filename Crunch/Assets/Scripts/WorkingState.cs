using UnityEngine;

[CreateAssetMenu(fileName = "WorkingState", menuName = "ScriptableObjects/WorkingState")]
public class WorkingState : AStateNPC
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
        throw new System.NotImplementedException();
    }

    public override void OnUpdateState(NPC npc)
    {
        throw new System.NotImplementedException();
    }

    public override void OnLeaveState(NPC npc)
    {
        throw new System.NotImplementedException();
    }

    public override bool ShouldLeaveState(NPC npc)
    {
        throw new System.NotImplementedException();
    }
}
