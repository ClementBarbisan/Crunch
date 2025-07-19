using UnityEngine;

public class Station : MonoBehaviour
{
    public bool freeStation = true;
    public NPC currentNPC;

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.transform.name);
        if (collision.collider.TryGetComponent(out NPC npc))
        {
            if (!freeStation)
            {
                currentNPC.transform.position = npc.transform.position;
                currentNPC.IsWorking = false;
            }
            npc.transform.position = transform.position + transform.forward;
            npc.transform.forward = -transform.forward;
            freeStation = false;
            currentNPC = npc;
            npc.IsWorking = true;
            npc.WorkStress = 0.5f;
        }
    }
}
