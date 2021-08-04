using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UIManager : Singleton<UIManager>
{
    private float confettiIteration;

    [SerializeField] private GameObject menuPanel;

    [Header("GamePanel")]
    [SerializeField] private GameObject gamePanel;

    [Header("FailPanel")]
    [SerializeField] private GameObject failPanel;

    [Header("SuccessPanel")]
    [SerializeField] private GameObject successPanel;
    [SerializeField] private ParticleSystem confetti;

    private void Start()
    {
        ShowPanel(menuPanel);
    }

    public void StartGameButton()
    {
        GameManager.Instance.StartGame();
        ShowPanel(gamePanel);
    }

    public void RestartLevelButton()
    {
        GameManager.Instance.RestartLevel();
    }

    public void NextLevelButton()
    {
        GameManager.Instance.NextLevel();
    }

    public void Success()
    {
        if (confettiIteration == 0)
        {
            confettiIteration++;
            confetti.Play();
        }

        ShowPanel(successPanel);
    }

    public void Fail()
    {
        ShowPanel(failPanel);
    }

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
}
