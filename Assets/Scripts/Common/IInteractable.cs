using UnityEngine;

public interface IInteractable
{
    void OnTriggerEnterInteract(Collider colliderObj);
    void OnTriggerExitInteract(Collider colliderObj);
}