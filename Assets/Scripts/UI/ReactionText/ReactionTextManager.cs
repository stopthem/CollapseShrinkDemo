using System;
using System.Collections.Generic;
using System.Linq;
using CanTemplate.Extensions;
using CanTemplate.Pooling;
using CanTemplate.UI;
using DG.Tweening;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;

namespace CanTemplate.ReactionText
{
    public class ReactionTextManager : Singleton<ReactionTextManager>
    {
        [Header("General"), Space(5)] public List<Color> randomColors;
        private Pooler _reactionTextPooler;

        protected override void Awake()
        {
            base.Awake();

            _reactionTextPooler = GetComponentInChildren<Pooler>();
        }

        private readonly List<ReactionText> _reactionTexts = new();

        public static Sequence CreateText(ReactionTextInfo reactionTextInfo)
        {
            if (reactionTextInfo.closePrevIfSameText)
                CloseSpecificText(reactionTextInfo.text);

            return GetReqText().PlayText(reactionTextInfo);
        }

        public static Sequence CreateText(ReactionTextInfo reactionTextInfo, out ReactionText reactionText)
        {
            if (reactionTextInfo.closePrevIfSameText)
                CloseSpecificText(reactionTextInfo.text);

            reactionText = GetReqText();
            return reactionText.PlayText(reactionTextInfo);
        }

        ///<summary>It will create text with a random color.</summary>
        public static Sequence CreateText(RandomColoredReactionTextInfo randomColoredReactionTextInfo, out ReactionText reactionText)
        {
            if (randomColoredReactionTextInfo.closePrevIfSameText)
                CloseSpecificText(randomColoredReactionTextInfo.text);

            randomColoredReactionTextInfo.randomColors ??= Instance.randomColors;

            reactionText = GetReqText();
            return reactionText.PlayText(randomColoredReactionTextInfo);
        }

        ///<summary>It will create text with a random color.</summary>
        public static Sequence CreateText(RandomColoredReactionTextInfo randomColoredReactionTextInfo)
        {
            if (randomColoredReactionTextInfo.closePrevIfSameText)
                CloseSpecificText(randomColoredReactionTextInfo.text);

            randomColoredReactionTextInfo.randomColors ??= Instance.randomColors;

            return GetReqText().PlayText(randomColoredReactionTextInfo);
        }

        private static ReactionText GetReqText()
        {
            var reactionText = Instance._reactionTextPooler.Get<ReactionText>();
            Instance._reactionTexts.Add(reactionText);

            reactionText.transform.SetParent(UIManager.Instance.reactionTextParent);
            reactionText.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
            reactionText.transform.DOKill();
            return reactionText;
        }

        public static void CloseAllReactionTexts(bool instant = true)
        {
            foreach (var reqText in Instance._reactionTexts.Where(reqText => reqText != null))
            {
                reqText.Close(instant: instant);
            }

            Instance._reactionTexts.Clear();
        }

        public static void CloseSpecificText(string text, bool instant = true) => CloseReactionText(Instance._reactionTexts.FirstOrDefault(x => x.TMPText.text == text), instant);

        public static void CloseSpecificText(ReactionTextInfo reactionTextInfo, bool instant = false) => CloseReactionText(Instance._reactionTexts.FirstOrDefault(x => x.MyReqTextInfo == reactionTextInfo), instant);

        public static void CloseReactionText(ReactionText reactionText, bool instant)
        {
            if (reactionText is null) return;
            reactionText.Close(instant: instant);
            Instance._reactionTexts.Remove(reactionText);
        }
    }

    [Serializable]
    public class ReactionTextInfo
    {
        public string text;

        [Tooltip("Leave it black with alpha 0 to set a random color")]
        public Color color = Color.white.WithA(1);

        public float tweenDuration = .5f;
        public float stayDuration = 4;

        public float fontSize = 80;

        public float playY = 600;

        public bool closePrevIfSameText = true;

        [CanBeNull] public UnderlayOptions underlayOptions = new()
        {
            dilate = .5f,
            offsetX = -.5f,
            offsetY = -.5f,
            softness = 0,
            underlayColor = Color.white.WithA(0)
        };

        [CanBeNull] public OutlineOptions outlineOptions = new()
        {
            color = Color.black.WithA(0),
            width = 0
        };

        public FontStyles fontStyle;
    }

    [Serializable]
    public class RandomColoredReactionTextInfo : ReactionTextInfo
    {
        [CanBeNull] public List<Color> randomColors;
        public float colorA = 1;
    }
}