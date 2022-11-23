using CanTemplate.Camera;
using CanTemplate.Extensions;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MoneyPopup : MonoBehaviour
{
    // SerializeField is for earnHashId's AnimatorParam attribute
    [SerializeField] private Animator animator;

    [SerializeField, AnimatorParam("_animator")]
    private int earnHashId;

    [SerializeField, Space(5)] private float directionToCamMultiplier = 1;

    private TextMeshProUGUI _textMeshPro;

    private void Awake()
    {
        _textMeshPro = GetComponentInChildren<TextMeshProUGUI>();

        _textMeshPro.color = _textMeshPro.color.WithA(0);

        var image = GetComponentInChildren<Image>();
        image.color = image.color.WithA(0);
    }

    private void LateUpdate()
    {
        transform.localRotation = Quaternion.LookRotation(transform.parent.InverseTransformDirection(transform.position.GetDir(CinemachineManager.MainCamTransform.position)));

        transform.localPosition = transform.parent.InverseTransformDirection(transform.position.GetDir(CinemachineManager.MainCamTransform.position)) * directionToCamMultiplier;
    }

    public void Play(int amount)
    {
        _textMeshPro.text = $"+{amount}";
        animator.Play(earnHashId);
    }
}