using CanTemplate.Extensions;
using DG.Tweening;
using NaughtyAttributes;
using TMPro;
using UnityEngine;

namespace CanTemplate.Money
{
    public class MoneyArea : MonoBehaviour
    {
        [Header("Tween Options"), Space(5), SerializeField]
        private TextMeshProUGUI moneyAmountText;

        [SerializeField, InfoBox("target becomes to scale offset")]
        private FTweenOptions scaleTweenOptions = new()
        {
            duration = .5f,
            easeSelection = new EaseSelection(Ease.InOutQuad),
            target = .1f
        };

        [SerializeField] private Color incrementColor, decrementColor;

        private Color _moneyAmountTextStartColor;

        private Sequence _moneyAmountTextSeq;

        private void Start()
        {
            moneyAmountText.text = MoneyManager.MoneyAmount.ToString();

            _moneyAmountTextStartColor = moneyAmountText.color;
        }

        public void DoAmountText()
        {
            int.TryParse(moneyAmountText.text, out var start);

            _moneyAmountTextSeq.Kill();

            moneyAmountText.transform.localScale = Vector3.one;
            moneyAmountText.color = _moneyAmountTextStartColor;

            moneyAmountText.text = MoneyManager.MoneyAmount.ToString();

            _moneyAmountTextSeq = DOTween.Sequence()
                .Append(moneyAmountText.transform.DOScale(start < MoneyManager.MoneyAmount ? 1 + scaleTweenOptions.target : 1 - scaleTweenOptions.target, scaleTweenOptions.duration / 2).SetEase(scaleTweenOptions.easeSelection)
                    .SetLoops(2, LoopType.Yoyo))
                .Join(moneyAmountText.DOColor(start < MoneyManager.MoneyAmount ? incrementColor : decrementColor, scaleTweenOptions.duration / 2).SetEase(scaleTweenOptions.easeSelection).SetLoops(2, LoopType.Yoyo));
            // .Join(DOTween.To(x => moneyAmountText.text = ((int)x).ToString(), start, MoneyAmount, tmpTweenDuration).SetEase(tmpEase));
        }
    }
}