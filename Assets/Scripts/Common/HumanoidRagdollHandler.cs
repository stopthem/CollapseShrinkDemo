using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CanTemplate.Utilities;
using DG.DeInspektor.Attributes;
using UnityEditor;
using UnityEngine;

public class HumanoidRagdollHandler : MonoBehaviour
{
    public bool getBonesByName = true;

    [HideInInspector] public Transform pelvis;

    [HideInInspector] public List<Transform> bones = new();

    [HideInInspector] public string bonesContainsName;

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

            foreach (var item in RagdollInfos.Item1)
            {
                item.collisionDetectionMode = value ? CollisionDetectionMode.Discrete : CollisionDetectionMode.ContinuousSpeculative;

                item.isKinematic = !value;
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
}

#if UNITY_EDITOR
[CustomEditor(typeof(HumanoidRagdollHandler)), CanEditMultipleObjects]
public class HumanoidRagdollHandlerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var humanoidRagdollHandler = target as HumanoidRagdollHandler;

        if (humanoidRagdollHandler!.getBonesByName)
        {
            humanoidRagdollHandler.pelvis = (Transform)EditorGUILayout.ObjectField("Pelvis", humanoidRagdollHandler.pelvis, typeof(Transform), true);
            humanoidRagdollHandler.bonesContainsName = EditorGUILayout.TextField("Bones Contains Name", "mixamorig");
        }
        else EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(humanoidRagdollHandler.bones)), new GUIContent("Bones"));

        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }
}
#endif