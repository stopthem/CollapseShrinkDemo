using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Linq;

public class UIManager : Singleton<UIManager>
{
    private List<RectTransform> childs = new List<RectTransform>();

    private float confettiIteration;

    [SerializeField] private GameObject menuPanel;

    [Header("GamePanel")]
    [SerializeField] private GameObject gamePanel;

    [Header("FailPanel")]
    [SerializeField] private GameObject failPanel;

    [Header("SuccessPanel")]
    [SerializeField] private GameObject successPanel;
    [SerializeField] private ParticleSystem confetti;

    private void Awake()
    {
        childs = GetComponentsInChildren<RectTransform>(true).ToList();
    }

    private void Start()
    {
        // ShowPanel(menuPanel);
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

    public static void OpenClose(string name, bool status, bool updateChildsList = false)
    {
        if (updateChildsList) Instance.childs = Instance.GetComponentsInChildren<RectTransform>(true).ToList();

        RectTransform obj = Instance.childs.FirstOrDefault(x => x.name == name);
        
        if (!obj)
        {
            Debug.Log("ui manager coulnd't find = " + "<color=green>" + name + "</color>");
            return;
        }

        obj.gameObject.SetActive(status);
    }

    public static bool IsPointerOverUIObject(Transform rootTransform = null)
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        if (rootTransform)
        {
            var childList = rootTransform.GetComponentsInChildren<RectTransform>().ToList();
            results.RemoveAll(x => !childList.Contains(x.gameObject.transform));
        }
        return results.Count > 0;
    }
}