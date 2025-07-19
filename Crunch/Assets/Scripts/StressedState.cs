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
}
