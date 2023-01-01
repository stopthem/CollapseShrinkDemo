using CanTemplate.UI;
using UnityEngine;

namespace CanTemplate.Money
{
    public class MoneyManager : MonoBehaviour
    {
        public static MoneyManager Instance { get; private set; }

        public static int MoneyAmount
        {
            get => PlayerPrefs.GetInt("money_amount", 0);
            private set => PlayerPrefs.SetInt("money_amount", Mathf.Clamp(value, 0, int.MaxValue));
        }

        private void Awake()
        {
            Instance = this;
        }

        public bool HasEnough(int amount) => MoneyAmount >= amount;

        public void MoneyChanged(int changeAmount, bool doAmountText = true)
        {
            MoneyAmount += changeAmount;
            if (doAmountText) UIManager.Instance.moneyArea.DoAmountText();
        }
    }
}