using System;
using UnityEngine;

public class Station : MonoBehaviour
{
    [SerializeField] private float _workStressValueOnThrow = 0.6f;
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
            npc.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
            npc.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            npc.transform.position = transform.position + transform.forward;
            npc.transform.forward = -transform.forward;
            freeStation = false;
            currentNPC = npc;
            npc.CurrentStation = this;
            npc.IsWorking = true;
            npc.WorkStress = _workStressValueOnThrow;
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.collider.TryGetComponent(out NPC npc))
        {
            if (npc == currentNPC)
            {
                currentNPC = null;
                npc.CurrentStation = null;
                freeStation = true;
            }
        }
    }
}
