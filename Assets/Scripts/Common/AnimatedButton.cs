using CubeShooter;
using UnityEngine;
using UnityEditor;
#if UNITY_EDITOR
using UnityEditor.UI;
#endif
using UnityEngine.UI;

namespace CanTemplate.UI
{
    [RequireComponent(typeof(Animator)), RequireComponent(typeof(Image))]
    public class AnimatedButton : Button
    {
        [HideInInspector, Space(5)] public StartingBehaviour startingBehaviour;
        [HideInInspector] public bool notInteractableAfterClicked = true;

        private Animator _animator;

        protected override void Awake()
        {
            base.Awake();

            _animator = GetComponent<Animator>();

            onClick.AddListener(Click);
        }

        protected override void Start()
        {
            base.Start();

            switch (startingBehaviour)
            {
                case StartingBehaviour.StartOpen:
                    OpenOrClose(true, true);
                    break;
                case StartingBehaviour.StartClosed:
                    OpenOrClose(false, true);
                    break;
                case StartingBehaviour.StartClosedThenOpen:
                    OpenOrClose(false, true);
                    OpenOrClose(true);
                    break;
            }
        }

        public void OpenOrClose(bool open, bool instant = false, bool returnIfSame = true)
        {
            if (((open && animator.GetCurrentAnimatorStateInfo(0).IsName("Open")) || (!open && animator.GetCurrentAnimatorStateInfo(0).IsName("Close"))) && returnIfSame)
                return;

            if (instant)
            {
                _animator.Play(open ? "Open" : "Close", 0, 1);

                return;
            }

            _animator.SetTrigger(open ? AnimatorParams.OpenId : AnimatorParams.CloseId);
        }

        private void Click()
        {
            if (!interactable) return;

            if (notInteractableAfterClicked) interactable = false;

            animator.SetTrigger(AnimatorParams.ClickId);
        }

        public enum StartingBehaviour
        {
            StartOpen,
            StartClosed,
            StartClosedThenOpen
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(AnimatedButton), true), CanEditMultipleObjects]
    public class AnimatedButtonEditor : ButtonEditor
    {
        public override void OnInspectorGUI()
        {
            var script = target as AnimatedButton;

            serializedObject.Update();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("startingBehaviour"));
            script.notInteractableAfterClicked = EditorGUILayout.Toggle("Not Interactable After Clicked", script.notInteractableAfterClicked);

            GUILayout.Space(10);

            serializedObject.ApplyModifiedProperties();
            if (GUI.changed) EditorUtility.SetDirty(script);

            base.OnInspectorGUI();
        }
    }
#endif
}