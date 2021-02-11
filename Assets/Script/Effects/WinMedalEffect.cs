using System.Collections;
using System.Collections.Generic;
using MedalPusher.Item.Checker;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Diagnostics;
using UniRx.Toolkit;
using UnityUtility;
using DG.Tweening;

namespace MedalPusher.Effects
{
    public class WinMedalEffect : SerializedMonoBehaviour
    {
        [SerializeField]
        private Image m_medalImage;
        [SerializeField]
        private AudioSource m_audioSource;
        [SerializeField]
        private IObservableMedalChecker m_winMedalChecker;
        /// <summary>
        /// メダルエフェクトが上がる高さ
        /// </summary>
        [SerializeField]
        private float m_effectMaxHeight;
        /// <summary>
        /// 上昇エフェクトのduration
        /// </summary>
        [SerializeField]
        private float m_riseDuration;
        /// <summary>
        /// 上昇後の回転エフェクトのduration
        /// </summary>
        [SerializeField]
        private float m_rotateDuration;
        /// <summary>
        /// 消失エフェクトのduration
        /// </summary>
        [SerializeField]
        private float m_disappearanceDuration;

        private MedalImagePool m_pool;
        private Camera m_mainCamera;

        // Start is called before the first frame update
        void Start()
        {
            m_pool = new MedalImagePool(this);
            m_pool.PreloadAsync(20, 1).Subscribe();
            m_mainCamera = Camera.main;

            m_winMedalChecker.Checked
                             //落ちたメダルの画面上のx座標を取得
                             .Select(medal => RectTransformUtility.WorldToScreenPoint(m_mainCamera, medal.position).x)
                             //プールからImageオブジェクトを取得して出現座標を設定
                             .Select(x => m_pool.RentAndSetPositionX(x))
                             //Tweenに乗せる
                             .Select(img => DOTween.Sequence()
                                                   //画面下から上がってくるTween
                                                   .Append(img.rectTransform.DOAnchorPosY(m_effectMaxHeight, m_riseDuration))
                                                   //上がった後にくるっと回るTween
                                                   .Append(img.rectTransform.DORotate(new Vector3(0, 360, 0), m_rotateDuration, RotateMode.FastBeyond360))
                                                   //透明になってフェードアウト
                                                   .Append(img.DOColor(Color.clear, m_disappearanceDuration))
                                                   //終わったらオブジェクトをプールに返す
                                                   .OnComplete(() =>
                                                   {
                                                       img.color = Color.white; //透明にした色をもとに戻す
                                                       m_pool.Return(img);
                                                   }))
                             .Subscribe(sq =>
                             {
                                 sq.Play();
                                 m_audioSource.Play();
                             });
        }

        private class MedalImagePool : ObjectPool<Image>
        {
            private readonly WinMedalEffect m_parent;

            public MedalImagePool(WinMedalEffect parent)
            {
                this.m_parent = parent;
            }

            public Image RentAndSetPositionX(float x)
            {
                var img = Rent();
                img.transform.position = new Vector3(x, 0, 0);
                return img;
            }

            protected override Image CreateInstance()
            {
                //生成時にcanvasに乗せる
                var img = Instantiate(m_parent.m_medalImage);
                img.transform.SetParent(m_parent.transform);
                return img;
            }
        }
    }
}