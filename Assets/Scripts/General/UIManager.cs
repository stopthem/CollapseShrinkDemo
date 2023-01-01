using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using CanTemplate.GameManaging;
using CanTemplate.Money;
using DG.Tweening;
using TMPro;
using NaughtyAttributes;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace CanTemplate.UI
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [HideInInspector] public List<RectTransform> childs = new();
        [SerializeField] private TextMeshProUGUI[] levelTexts;

        private ScreenTransition _screenTransition;

        [SerializeField] private PanelInfo[] panelInfos;
        [Space(5)] public UnityEvent<GameStatus> onPanelShowed;

        [InfoBox("Money Area Can Be Null")] public MoneyArea moneyArea;

        [SerializeField, Space(5)] private ParticleSystem confetti;

        [Space(5)] public Transform reactionTextParent;

        private void Awake()
        {
            Instance = this;

            childs = GetComponentsInChildren<RectTransform>(true).ToList();
            childs.Remove(GetComponent<RectTransform>());

            _screenTransition = GetComponentInChildren<ScreenTransition>();
        }

        private void Start()
        {
            HandleMoneyArea(false);

            HideAllPanel();
            _screenTransition.transform.SetAsLastSibling();
            _screenTransition.OpenClose(false).OnComplete(() => ShowPanel(GameStatus.Menu));

            foreach (var levelText in levelTexts)
            {
                levelText.text = "Level " + GameManager.CurrentLevelCount;
            }
        }

        public void StartGameButton()
        {
            ShowPanel(GameStatus.Play);
            GameManager.Instance.StartGame();
        }

        public void RestartLevelButton() => _screenTransition.OpenClose(true).OnComplete(GameManager.Instance.RestartLevel);

        public void NextLevelButton() => _screenTransition.OpenClose(true).OnComplete(GameManager.Instance.NextLevel);

        public void Success()
        {
            HandleMoneyArea(true);

            confetti.Play();

            ShowPanel(GameStatus.Success);
        }

        public void Fail()
        {
            ShowPanel(GameStatus.Fail);
        }

        private void ShowPanel(GameStatus gameStatus)
        {
            var foundPanel = panelInfos.First(panelInfo => panelInfo.gameStatus == gameStatus);

            HideAllPanel();

            foundPanel.panelObj.gameObject.SetActive(true);
            onPanelShowed.Invoke(gameStatus);
        }

        private void HideAllPanel()
        {
            foreach (var panelInfo in panelInfos)
            {
                panelInfo.panelObj.SetActive(false);
            }
        }

        private void HandleMoneyArea(bool status)
        {
            if (moneyArea)
                moneyArea.gameObject.SetActive(status);
        }

        [Serializable]
        private class PanelInfo
        {
            public GameStatus gameStatus;
            public GameObject panelObj;
        }
    }
}