using Unity.VisualScripting;
using UnityEngine;

public interface IStateNPC
{
   public void OnEnterState(NPC npc);
   public void OnUpdateState(NPC npc);
   public void OnLeaveState(NPC npc);
   
}
