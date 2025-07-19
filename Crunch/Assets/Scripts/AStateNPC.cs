
using UnityEngine;

public abstract class AStateNPC : ScriptableObject
{
   public abstract void OnEnterState(NPC npc);
   public abstract void OnUpdateState(NPC npc);
   public abstract void OnLeaveState(NPC npc);
}
