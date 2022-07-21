using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CanTemplate.Extensions;
using CanTemplate.Utilities;
using DG.DeInspektor.Attributes;
using UnityEngine;

public class CollisionMsgSender : MonoBehaviour
{
    [SerializeField] private bool iHaveTarget;

    [DeConditional("iHaveTarget", true, ConditionalBehaviour.Hide), SerializeField]
    private Transform target;

    public Action<Collider> onTriggerEnter;
    public Action<Collision> onCollisionEnter;
    public Action<Collider> onTriggerExit;

    private void Update()
    {
        if (target) transform.localPosition = transform.localPosition.WithX(transform.parent.InverseTransformPoint(target.parent.TransformPoint(target.localPosition)).x);
    }

    private void OnTriggerEnter(Collider other)
    {
        onTriggerEnter?.Invoke(other);
    }

    public void OnTriggerExit(Collider other)
    {
        onTriggerExit?.Invoke(other);
    }

    public void OnCollisionEnter(Collision collision)
    {
        onCollisionEnter?.Invoke(collision);
    }
}