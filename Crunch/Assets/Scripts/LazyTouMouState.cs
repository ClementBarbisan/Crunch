using UnityEngine;

[CreateAssetMenu(fileName = "LazyTouMouState", menuName = "ScriptableObjects/NPC/LazyTouMouState")]
public class LazyTouMouState : LazyState
{
    public override void OnEnterState(NPC npc)
    {
        base.OnEnterState(npc);
        for (int i = 0; i < npc.UnderworkedVFXs.Length; i++)
        {
            npc.UnderworkedVFXs[i].Play();
        }
        //npc._audioSourceSleeping.Play();
    }

    public override void OnLeaveState(NPC npc)
    {
        base.OnLeaveState(npc);
        for (int i = 0; i < npc.UnderworkedVFXs.Length; i++)
        {
            npc.UnderworkedVFXs[i].Stop();
        }
        //npc._audioSourceSleeping.Stop();
    }
}
