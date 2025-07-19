using UnityEngine;

[CreateAssetMenu(fileName = "LazyState", menuName = "ScriptableObjects/LazyState")]
public class LazyState : AStateNPC
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
        npc.DEBUG_ChangeColor(Color.blue);

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
