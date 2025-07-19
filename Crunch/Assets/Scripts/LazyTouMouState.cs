using UnityEngine;

[CreateAssetMenu(fileName = "LazyTouMouState", menuName = "ScriptableObjects/LazyTouMouState")]
public class LazyTouMouState : LazyState
{
    public override void OnEnterState(NPC npc)
    {
        base.OnEnterState(npc);
        if (npc.CurrentStation)
        {
            npc.IsWorking = false;
        }
    }
}
