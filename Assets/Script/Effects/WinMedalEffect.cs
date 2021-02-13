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
using MedalPusher.Item;

namespace MedalPusher.Effects
{
    public class WinMedalEffect : SerializedMonoBehaviour, IObservableMedalChecker
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
        /// メダルエフェクトの高さの振れ幅係数
        /// </summary>
        [SerializeField, MinMaxSlider(0.5f, 1.5f)]
        private Vector2 m_effectHeightCoefficient = new Vector2(0.8f, 1.2f);
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
        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private RectTransform m_medalInventoryUITarget;

        private MedalImagePool m_pool;
        private Camera m_mainCamera;
        private Subject<IMedal> m_effectDoneSubject = new Subject<IMedal>();

        public System.IObservable<IMedal> Checked => m_effectDoneSubject.AsObservable();

        // Start is called before the first frame update
        void Start()
        {
            m_pool = new MedalImagePool(this);
            m_pool.PreloadAsync(20, 1).Subscribe();
            m_mainCamera = Camera.main;

            m_winMedalChecker.Checked
                             //落ちたメダルの画面上のx座標を取得
                             .Select(medal => (RectTransformUtility.WorldToScreenPoint(m_mainCamera, medal.position).x, medal))
                             //プールからImageオブジェクトを取得して出現座標を設定
                             .Select(pair => (img: m_pool.RentAndSetPositionX(pair.x), pair.medal))
                             //Tweenに乗せる
                             .Select(pair => DOTween.Sequence()
                                                    //画面下から上がってくるTween
                                                    .Append(pair.img.rectTransform.DOMoveY(m_effectMaxHeight * Random.Range(m_effectHeightCoefficient.x, m_effectHeightCoefficient.y), m_riseDuration).SetEase(Ease.OutCubic))
                                                    //上がった後にくるっと回るTween
                                                    .Append(pair.img.rectTransform.DORotate(new Vector3(0, 360, 0), m_rotateDuration, RotateMode.FastBeyond360))
                                                    //透明にして、
                                                    .Append(pair.img.DOColor(Color.clear, m_disappearanceDuration).SetEase(Ease.InQuad))
                                                    //小さくしながら、
                                                    .Join(pair.img.rectTransform.DOScale(0.3f, m_disappearanceDuration).SetEase(Ease.InQuad))
                                                    //メダルインベントリのUIに吸い込まれながらフェードアウト
                                                    .Join(pair.img.rectTransform.DOMove(m_medalInventoryUITarget.position, m_disappearanceDuration))
                                                    //終わったらオブジェクトをプールに返す
                                                    .OnComplete(() =>
                                                    {
                                                        m_effectDoneSubject.OnNext(pair.medal); //エフェクト完了通知を送信
                                                        pair.img.color = Color.white; //透明にした色をもとに戻す
                                                        pair.img.rectTransform.localScale = Vector3.one;
                                                        m_pool.Return(pair.img);
                                                    }))
                             //SequenceとSEを再生する
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
                img.rectTransform.position = new Vector2(x, 0);
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