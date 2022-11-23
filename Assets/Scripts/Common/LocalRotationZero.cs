using CanTemplate.Extensions;
using UnityEngine;

public class LocalRotationZero : MonoBehaviour
{
    private Transform _parent;

    private void Awake()
    {
        _parent = transform.parent;
    }

    private void LateUpdate()
    {
        transform.localRotation = _parent.InverseTransformRotation(Quaternion.identity);
    }
}
