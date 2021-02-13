using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MedalPusher.Item.Checker;
using UnityEngine;
using UniRx;
using Sirenix.OdinInspector;

namespace MedalPusher.Effects
{
    public class WinMedalViewEffect : SerializedMonoBehaviour
    {
        [SerializeField]
        private IObservableMedalChecker m_medalGot;
        [SerializeField]
        private float m_duration;

        private RectTransform m_rectTransform;
        private Tweener tweener;

        // Start is called before the first frame update
        void Start()
        {
            m_rectTransform = transform as RectTransform;
            tweener = this.m_rectTransform.DOPunchScale(new Vector3(.1f, .1f, 0f), m_duration, 2)
                          .SetEase(Ease.OutQuint)
                          .SetRelative()
                          .SetRecyclable()
                          .SetAutoKill(false);

            m_medalGot.Checked.Subscribe(_ =>
            {
                if (tweener.IsComplete()) tweener.Rewind();
                if (tweener.IsPlaying()) tweener.Restart();
                else tweener.Play();
            });
        }
    }
}