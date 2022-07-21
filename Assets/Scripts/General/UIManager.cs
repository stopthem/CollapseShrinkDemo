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
    public static UIManager instance;

    [HideInInspector] public List<RectTransform> childs = new();
    [SerializeField] private TextMeshProUGUI[] levelTexts;

    [Header("Fade"), Space(5), SerializeField]
    private Image fadeImage;

    [SerializeField] private float fadeDuration;
    [SerializeField] private EaseSelection fadeEase;

    [Header("Menu"), Space(5), SerializeField]
    private GameObject menuPanel;

    [Header("Game Panel"), Space(5), SerializeField]
    private GameObject gamePanel;

    [Header("Fail Panel"), Space(5), SerializeField]
    private GameObject failPanel;

    [Header("Success Panel"), Space(5), SerializeField]
    private GameObject successPanel;

    [SerializeField] private ParticleSystem confetti;

    private void Awake()
    {
        instance = this;

        childs = GetComponentsInChildren<RectTransform>(true).ToList();
        childs.Remove(GetComponent<RectTransform>());
    }

    private void Start()
    {
        fadeImage.color = fadeImage.color.WithA(1);
        HideAllPanel();
        DoFade(false).OnComplete(() => ShowPanel(menuPanel));

        foreach (var levelText in levelTexts)
        {
            levelText.text = "Level " + GameManager.CurrentLevelCount;
        }

        GameManager.instance.onGameFailed.AddListener(Fail);
        GameManager.instance.onGameSuccess.AddListener(Success);
    }

    public void StartGameButton()
    {
        GameManager.instance.StartGame();
        ShowPanel(gamePanel);
    }

    public void RestartLevelButton() => DoFade(true).OnComplete(GameManager.instance.RestartLevel);

    public void NextLevelButton() => DoFade(true).OnComplete(GameManager.instance.NextLevel);

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

    private Tween DoFade(bool fadeIn)
    {
        float toA = fadeIn ? 1 : 0;
        fadeImage.color = fadeImage.color.WithA(fadeIn ? 0 : 1);
        return fadeImage.DOColor(fadeImage.color.WithA(toA), fadeDuration).SetEase(fadeEase);
    }
}