using System;
using UnityEngine;

public class CollisionMsgSender : MonoBehaviour
{
    public Action<Collider> onCollisionMsgSenderTriggerEnter, onCollisionMsgSenderTriggerExit;
    public Action<Collision> onCollisionMsgSenderCollisionEnter;

    private void OnTriggerEnter(Collider other)
    {
        onCollisionMsgSenderTriggerEnter?.Invoke(other);
    }

    public void OnTriggerExit(Collider other)
    {
        onCollisionMsgSenderTriggerExit?.Invoke(other);
    }

    public void OnCollisionEnter(Collision collision)
    {
        onCollisionMsgSenderCollisionEnter?.Invoke(collision);
    }
}