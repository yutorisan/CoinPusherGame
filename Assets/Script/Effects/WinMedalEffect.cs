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
    /// <summary>
    /// メダルを獲得したときのエフェクト
    /// </summary>
    /// <remarks>
    /// <see cref="IObservableMedalChecker"/>が実装されているのは、エフェクト完了時にメダル獲得とするため
    /// </remarks>
    public class WinMedalEffect : SerializedMonoBehaviour, IObservableMedalChecker
    {
        /// <summary>
        /// エフェクト本体の画像
        /// </summary>
        [SerializeField, Required]
        private Image medalImage;
        /// <summary>
        /// エフェクト再生時のSE
        /// </summary>
        [SerializeField, Required]
        private AudioSource audioSource;
        /// <summary>
        /// エフェクト再生トリガーとなるMedalChecker
        /// </summary>
        [SerializeField, Required]
        private IObservableMedalChecker winMedalChecker;
        /// <summary>
        /// メダルエフェクトが上がる高さ
        /// </summary>
        [SerializeField]
        private float effectMaxHeight;
        /// <summary>
        /// メダルエフェクトの高さの振れ幅係数
        /// </summary>
        [SerializeField, MinMaxSlider(0.5f, 1.5f)]
        private Vector2 effectHeightCoefficient = new Vector2(0.8f, 1.2f);
        /// <summary>
        /// 上昇エフェクトのduration
        /// </summary>
        [SerializeField]
        private float riseDuration;
        /// <summary>
        /// 上昇後の回転エフェクトのduration
        /// </summary>
        [SerializeField]
        private float rotateDuration;
        /// <summary>
        /// 消失エフェクトのduration
        /// </summary>
        [SerializeField]
        private float disappearanceDuration;
        /// <summary>
        /// エフェクトの消失場所のTransform
        /// </summary>
        [SerializeField, Required]
        private RectTransform effectDestination;

        /// <summary>
        /// エフェクトの再生完了通知
        /// </summary>
        private readonly Subject<IMedal> effectDoneSubject = new Subject<IMedal>();

        public System.IObservable<IMedal> Checked => effectDoneSubject.AsObservable();

        // Start is called before the first frame update
        void Start()
        {
            MedalImagePool pool = new MedalImagePool(this);
            UnityEngine.Camera mainCamera = UnityEngine.Camera.main;

            //事前に20個のメダルエフェクトオブジェクトを生成しておく
            pool.PreloadAsync(20, 1).Subscribe();

            //エフェクト開始トリガーがかかったらエフェクトを再生する
            winMedalChecker.Checked
                           //落ちたメダルの画面上のx座標を取得
                           .Select(medal => (RectTransformUtility.WorldToScreenPoint(mainCamera, medal.position).x, medal))
                           //プールからImageオブジェクトを取得して出現座標を設定
                           .Select(pair => (img: pool.RentAndSetPositionX(pair.x), pair.medal))
                           //Tweenに乗せる
                           .Select(pair => DOTween.Sequence()
                                                  //画面下から上がってくるTween
                                                  .Append(pair.img.rectTransform.DOMoveY(effectMaxHeight * Random.Range(effectHeightCoefficient.x, effectHeightCoefficient.y), riseDuration).SetEase(Ease.OutCubic))
                                                  //上がった後にくるっと回るTween
                                                  .Append(pair.img.rectTransform.DORotate(new Vector3(0, 360, 0), rotateDuration, RotateMode.FastBeyond360))
                                                  //透明にして、
                                                  .Append(pair.img.DOColor(Color.clear, disappearanceDuration).SetEase(Ease.InQuad))
                                                  //小さくしながら、
                                                  .Join(pair.img.rectTransform.DOScale(0.3f, disappearanceDuration).SetEase(Ease.InQuad))
                                                  //メダルインベントリのUIに吸い込まれながらフェードアウト
                                                  .Join(pair.img.rectTransform.DOMove(effectDestination.position, disappearanceDuration))
                                                  //終わったらオブジェクトをプールに返す
                                                  .OnComplete(() =>
                                                  {
                                                      effectDoneSubject.OnNext(pair.medal); //エフェクト完了通知を送信
                                                      pair.img.color = Color.white; //透明にした色をもとに戻す
                                                      pair.img.rectTransform.localScale = Vector3.one;
                                                      pool.Return(pair.img);
                                                  }))
                           //SequenceとSEを再生する
                           .Subscribe(sq =>
                           {
                               sq.Play();
                               audioSource.Play();
                           });
        }

        /// <summary>
        /// メダルエフェクトのオブジェクトプール
        /// </summary>
        private class MedalImagePool : ObjectPool<Image>
        {
            private readonly WinMedalEffect m_parent;

            public MedalImagePool(WinMedalEffect parent)
            {
                this.m_parent = parent;
            }

            /// <summary>
            /// オブジェクトを取得して指定のx座標にセットする
            /// </summary>
            /// <param name="x">設定するx座標</param>
            /// <returns>取得したメダルエフェクトオブジェクト</returns>
            public Image RentAndSetPositionX(float x)
            {
                var img = Rent();
                img.rectTransform.position = new Vector2(x, 0);
                return img;
            }

            protected override Image CreateInstance()
            {
                //生成時にcanvasに乗せる
                var img = Instantiate(m_parent.medalImage);
                img.transform.SetParent(m_parent.transform);
                return img;
            }
        }
    }
}