using System;
using System.Collections.Generic;
using System.Linq;
using CanTemplate.Utilities;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;

public class HumanoidRagdollHandler : MonoBehaviour
{
    public bool getBonesByName = true;

    [ShowIf("getBonesByName"), Required("Pelvis cannot be empty")]
    public Transform pelvis;

    [ShowIf("getBonesByName")] public string bonesContainsName;

    [HideIf("getBonesByName")] public List<Transform> bones = new();

    private Animator _animator;

    public Tuple<List<Rigidbody>, List<Collider>, List<CharacterJoint>> RagdollInfos { get; } = new(new List<Rigidbody>(), new List<Collider>(), new List<CharacterJoint>());
    public Vector3 CenterOfMass => RigidbodyUtilities.GetRbsCenterOfMass(RagdollInfos.Item1.ToArray());

    public Rigidbody HipsRigidbody => RagdollInfos.Item1.First();

    private bool _ragdollActiveness;

    public bool RagdollActiveness
    {
        get => _ragdollActiveness;
        private set
        {
            _ragdollActiveness = value;

            foreach (var boneRigidbody in RagdollInfos.Item1)
            {
                boneRigidbody.collisionDetectionMode = value ? CollisionDetectionMode.Discrete : CollisionDetectionMode.ContinuousSpeculative;

                boneRigidbody.isKinematic = !value;
                boneRigidbody.useGravity = value;
            }

            foreach (var boneCollider in RagdollInfos.Item2)
            {
                boneCollider.isTrigger = !value;
            }

            _animator.enabled = !value;
        }
    }

    private void Awake()
    {
        _animator = GetComponent<Animator>();

        SetRagdollVariables();
    }

    private void SetRagdollVariables()
    {
        if (getBonesByName) bones = pelvis.GetComponentsInChildren<Transform>().Where(x => x.name.Contains(bonesContainsName)).ToList();

        foreach (var bone in bones)
        {
            AddRagdollComponentToList(bone, RagdollInfos.Item1);
            AddRagdollComponentToList(bone, RagdollInfos.Item2);
            AddRagdollComponentToList(bone, RagdollInfos.Item3);
        }

        void AddRagdollComponentToList<T>(Transform obj, List<T> targetList) where T : Component
        {
            var foundComponent = obj.GetComponent<T>();
            if (foundComponent != null) targetList.Add(foundComponent);
        }

        RagdollActiveness = false;

        if (!getBonesByName) return;

        var allBones = SelectTransforms(RagdollInfos.Item1).Concat(SelectTransforms(RagdollInfos.Item2)).Concat(SelectTransforms(RagdollInfos.Item3));
        bones.RemoveAll(x => !allBones.Contains(x));

        IEnumerable<Transform> SelectTransforms<T>(IEnumerable<T> list) where T : Component => list.Select(x => x.transform);
    }

    public void OpenCloseRagdoll(bool status) => RagdollActiveness = status;

    public Rigidbody GetClosestRigidBody(Vector3 pos) => RagdollInfos.Item1.OrderBy(x => Vector3.Distance(pos, x.position)).First();

    public void ImproveStability()
    {
        foreach (var chJoint in RagdollInfos.Item3)
        {
            chJoint.enableProjection = true;
        }
    }
}