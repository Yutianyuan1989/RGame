using Engine.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCColliderTrigger : MonoBehaviour
{

    public int NPCID;

    void OnCollisionEnter2D(Collision2D collidedObject)
    {
        Debug.Log("OnTriggerEnter2D");
        EventEngine.Instance().DispatchEvent((int)GameEventID.ON_NPC_INTERACTIVE, NPCID);
    }

    void OnCollisionExit2D(Collision2D collidedObject) 
    {
        Debug.Log("OnTriggerExit2D");
        EventEngine.Instance().DispatchEvent((int)GameEventID.ON_NPC_INTERDEACTIVE, NPCID);
    }
}
