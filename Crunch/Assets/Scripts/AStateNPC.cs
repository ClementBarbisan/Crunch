
using UnityEngine;

public enum EStateCategory
{
    Working,
    Underworked,
    Overworked,
}
public abstract class AStateNPC : ScriptableObject
{
    public float StateWalkSpeed = 3.5f;
    public EStateCategory StateCategory;
   public abstract void OnEnterState(NPC npc);
   public abstract void OnUpdateState(NPC npc);
   public abstract void OnLeaveState(NPC npc);
   public abstract bool ShouldLeaveState(NPC npc);
}
