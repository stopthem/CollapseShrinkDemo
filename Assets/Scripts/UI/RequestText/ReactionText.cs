using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using CanTemplate.Extensions;
using CanTemplate.Pooling;
using CanTemplate.Utilities;
using NaughtyAttributes;

namespace CanTemplate.ReactionText
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class ReactionText : MonoBehaviour
    {
        public TextMeshProUGUI TMPText { get; private set; }
        [SerializeField, Range(0, 1)] private float underlayDarkerColorPercent = .75f, outlineDarkerPercent = .5f;
        [SerializeField] private Ease inEase = Ease.OutBack;

        [SerializeField, InfoBox("'target' is closing scale multiplier")]
        private FTweenOptions closingScaleTweenOptions = new()
        {
            duration = .25f,
            easeSelection = new EaseSelection(Ease.OutQuart),
            target = 1.25f
        };

        private RectTransform _rectTransform;

        private Sequence _seq;

        private HorizontalLayoutGroup _horizontalLayoutGroup;

        private Poolable _poolable;

        public ReactionTextInfo MyReqTextInfo { get; private set; }

        private void Awake()
        {
            TMPText = GetComponent<TextMeshProUGUI>();
            TMPText.fontSize = 0;
            _rectTransform = GetComponent<RectTransform>();
        }

        private void Start()
        {
            _poolable = GetComponent<Poolable>();
        }

        public Sequence PlayText(ReactionTextInfo reactionTextInfo)
        {
            MyReqTextInfo = reactionTextInfo;

            _rectTransform.anchoredPosition = _rectTransform.anchoredPosition.WithY(reactionTextInfo.playY);

            TMPText.text = reactionTextInfo.text;

            var actualColor = reactionTextInfo.color == Color.black.WithA(0)
                ? ((RandomColoredReactionTextInfo)reactionTextInfo).randomColors?.GetRandomElement().WithA(((RandomColoredReactionTextInfo)reactionTextInfo).colorA)
                : reactionTextInfo.color;

            TMPText.color = actualColor!.Value;
            TMPText.fontSize = reactionTextInfo.fontSize;

            if (reactionTextInfo.outlineOptions is not null)
            {
                SetOutline(reactionTextInfo.outlineOptions);
            }
            else TMPText.fontMaterial.EnableKeyword("OUTLINE_OFF");

            if (reactionTextInfo.underlayOptions is not null)
            {
                SetUnderlay(reactionTextInfo.underlayOptions);
            }
            else TMPText.fontMaterial.EnableKeyword("UNDERLAY_OFF");

            SetStyle(reactionTextInfo.fontStyle);

            transform.localScale = Vector3.zero;
            return _seq = DOTween.Sequence()
                .Append(transform.DOScale(Vector3.one, reactionTextInfo.tweenDuration / 2).SetEase(inEase))
                .AppendInterval(reactionTextInfo.stayDuration)
                .Append(CloseSeq(reactionTextInfo.tweenDuration / 2))
                .SetUpdate(true);
        }

        /// <summary>
        /// Sets underlay for the given ReactionText.
        /// </summary>
        /// <param name="underlayColor">If null, color will be this text's color multiplied with this scripts outlineDarkerPercent
        /// <para>BE AWARE: underlay color uses HDR. so if tmp.text's color is not picked as hdr, underlay color will be not accurate.</para></param>
        /// <returns></returns>
        public ReactionText SetUnderlay(Color? underlayColor = null)
        {
            TMPText.fontMaterial.EnableKeyword("UNDERLAY_OFF");
            SetMatProperty("_UnderlayColor", underlayColor ?? TMPText.color * underlayDarkerColorPercent, keywordToEnable:
                ShaderUtilities.Keyword_Underlay);
            return this;
        }

        public ReactionText SetUnderlay(UnderlayOptions underlayOptions)
        {
            TMPText.fontMaterial.EnableKeyword("UNDERLAY_OFF");

            SetMatProperty("_UnderlayOffsetX", underlayOptions.offsetX);
            SetMatProperty("_UnderlayOffsetY", underlayOptions.offsetY);
            SetMatProperty("_UnderlayDilate", underlayOptions.dilate);
            SetMatProperty("_UnderlaySoftness", underlayOptions.softness);
            SetMatProperty("_UnderlayColor", underlayOptions.underlayColor, keywordToEnable: ShaderUtilities.Keyword_Underlay);
            return this;
        }

        /// <summary>
        /// Sets outline for the given ReactionText.
        /// </summary>
        /// <param name="thickness"></param>
        /// <param name="outlineColor">If null, color will be this text's color multiplied with this scripts outlineDarkerPercent
        /// <para>BE AWARE: outline color uses HDR. so if tmp.text's or given color is not picked as hdr, outline color will be not accurate.</para></param>
        /// <returns></returns>
        public ReactionText SetOutline(float thickness = .35f, Color? outlineColor = null)
        {
            TMPText.fontMaterial.EnableKeyword("OUTLINE_OFF");

            SetMatProperty("_OutlineColor", outlineColor ?? TMPText.color * outlineDarkerPercent, keywordToEnable: ShaderUtilities.Keyword_Outline);
            SetMatProperty("_OutlineWidth", thickness);
            return this;
        }

        /// <summary>
        /// Sets outline for the given ReactionText.
        /// </summary>
        /// <para>BE AWARE: outline color uses HDR. so if tmp.text's or given color is not picked as hdr, outline color will be not accurate.</para></param>
        /// <returns></returns>
        public ReactionText SetOutline(OutlineOptions outlineOptions)
        {
            TMPText.fontMaterial.EnableKeyword("OUTLINE_OFF");

            SetMatProperty("_OutlineColor", outlineOptions.color, keywordToEnable: ShaderUtilities.Keyword_Outline);
            SetMatProperty("_OutlineWidth", outlineOptions.width);
            return this;
        }

        public ReactionText SetStyle(FontStyles fontStyle)
        {
            TMPText.fontStyle = fontStyle;
            return this;
        }

        private void SetMatProperty(string propertyName, Color color, bool waitForAFrame = true, string keywordToEnable = "")
        {
            void SetColor()
            {
                TMPText.fontMaterial.SetColor(propertyName, color);
                EnableKeywordNUpdate(keywordToEnable);

                TMPText.UpdateAll();
            }

            if (waitForAFrame)
                WaitUtilities.WaitForAFrame(SetColor);
            else SetColor();
        }

        private void SetMatProperty(string propertyName, float value, bool waitForAFrame = true, string keywordToEnable = "")
        {
            void SetFloat()
            {
                TMPText.fontMaterial.SetFloat(propertyName, value);
                EnableKeywordNUpdate(keywordToEnable);

                TMPText.UpdateAll();
            }

            if (waitForAFrame)
                WaitUtilities.WaitForAFrame(SetFloat);
            else SetFloat();
        }

        private void EnableKeywordNUpdate(string keyword)
        {
            TMPText.fontMaterial.EnableKeyword(keyword);
            TMPText.UpdateAll();
        }

        public void Close(TweenCallback callback = null, bool instant = false)
        {
            if (_seq.IsActive() && _seq.IsPlaying())
                _seq.Kill();

            if (instant)
            {
                transform.localScale = Vector3.zero;
                if (!_poolable) _poolable = GetComponent<Poolable>();
                _poolable.ReturnToPool();
            }
            else
                CloseSeq(closingScaleTweenOptions.duration).OnComplete(callback);
        }

        private Sequence CloseSeq(float duration)
        {
            return DOTween.Sequence()
                .Append(transform.DOScale(closingScaleTweenOptions.target, duration).SetEase(closingScaleTweenOptions.easeSelection))
                .Join(TMPText.DOColor(TMPText.color.WithA(0), duration).SetEase(closingScaleTweenOptions.easeSelection))
                .AppendCallback(() => _poolable.ReturnToPool())
                .SetUpdate(true);
        }
    }

    [Serializable]
    public class UnderlayOptions
    {
        [ColorUsage(true, true)] public Color underlayColor = Color.black;
        [Range(-1, 1)] public float offsetX, offsetY = -.75f;
        [Range(-1, 1)] public float dilate = .25f;
        [Range(0, 1)] public float softness;
    }

    [Serializable]
    public class OutlineOptions
    {
        [Range(0, 1)] public float width = .35f;
        public Color color;
    }
}