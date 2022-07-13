using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using DG.Tweening;
using TMPro;
using CanTemplate.Extensions;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    private List<RectTransform> childs = new List<RectTransform>();

    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration;
    [SerializeField] private EaseSelection fadeEase;
    [Header("Menu")] [SerializeField] private GameObject menuPanel;

    [Header("GamePanel")] [SerializeField] private GameObject gamePanel;

    [Header("FailPanel")] [SerializeField] private GameObject failPanel;

    [Header("SuccessPanel")] [SerializeField]
    private GameObject successPanel;

    [SerializeField] private ParticleSystem confetti;

    private void Awake()
    {
        Instance = this;

        childs = GetComponentsInChildren<RectTransform>(true).ToList();
        childs.Remove(GetComponent<RectTransform>());
    }

    private void Start()
    {
        fadeImage.color = fadeImage.color.WithA(1);
        HideAllPanel();
        DoFade(false).OnComplete(() => ShowPanel(menuPanel));

        GameManager.instance.onGameFailed.AddListener(Fail);
        GameManager.instance.onGameSuccess.AddListener(Success);
    }

    public void StartGameButton()
    {
        GameManager.instance.StartGame();
        ShowPanel(gamePanel);
    }

    public void RestartLevelButton() => GameManager.instance.RestartLevel();

    public void NextLevelButton()
    {
        GameManager.instance.NextLevel();
    }

    private void Success()
    {
        confetti.Play();

        ShowPanel(successPanel);
    }

    private void Fail() => ShowPanel(failPanel);

    private void ShowPanel(GameObject panel)
    {
        HideAllPanel();
        panel.SetActive(true);
    }

    private void HideAllPanel()
    {
        menuPanel.SetActive(false);
        gamePanel.SetActive(false);
        failPanel.SetActive(false);
        successPanel.SetActive(false);
    }

    public static void OpenClose(string name, bool status, bool updateChildsList = false)
    {
        if (updateChildsList) Instance.childs = Instance.GetComponentsInChildren<RectTransform>(true).ToList();

        RectTransform obj = Instance.childs.FirstOrDefault(x => x.name == name);

        if (!obj)
        {
            Debug.Log("ui manager couldn't find = " + "<color=green>" + name + "</color>");
            return;
        }

        obj.gameObject.SetActive(status);
    }

    private Tween DoFade(bool fadeIn)
    {
        float toA = fadeIn ? 1 : 0;
        fadeImage.color = fadeImage.color.WithA(fadeIn ? 0 : 1);
        return fadeImage.DOColor(fadeImage.color.WithA(toA), fadeDuration).SetEase(fadeEase);
    }
}