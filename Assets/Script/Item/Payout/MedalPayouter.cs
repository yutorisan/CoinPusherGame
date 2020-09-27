using System;
using System.Runtime.CompilerServices;
using UniRx;
using UnityEngine;
using UnityUtility;

namespace MedalPusher.Item.Payout
{
    /// <summary>
    /// メダルを払い出す
    /// </summary>
    public class MedalPayouter : MonoBehaviour, IMedalPayoutOperator, IObservableMedalPayouter
    {
        /// <summary>
        /// メダルのモデル
        /// </summary>
        private UniReadOnly<UnityEngine.Object> _medalObject = new UniReadOnly<UnityEngine.Object>();
        /// <summary>
        /// 払出し中のメダル数
        /// </summary>
        private ReactiveProperty<int> m_payoutMedalStocks = new ReactiveProperty<int>(0);
        /// <summary>
        /// 払出しポイント
        /// </summary>
        [SerializeField]
        private Transform m_payoutPoint;
        /// <summary>
        /// 払出しの間隔
        /// </summary>
        private TimeSpan m_payoutInterval = TimeSpan.FromMilliseconds(100);
        private PayoutStatus m_status = PayoutStatus.Idol;

        private void Start()
        {
            _medalObject.Initialize(Resources.Load("Medal1"));

            m_payoutMedalStocks.Pairwise()
                               //Idol中のみ
                               .Where(_ => m_status == PayoutStatus.Idol)
                               //前より増えている
                               .Where(pair => pair.Current > pair.Previous)
                               //払出しを開始するのでステータスを変える
                               .Do(_ => m_status = PayoutStatus.Payouting)
                               //一定間隔で発行するIObservableを生成
                               .SelectMany(_ => Observable.Interval(m_payoutInterval)
                                                          //ストックが0になるまで
                                                          .TakeUntil(m_payoutMedalStocks.Where(n => n == 0))
                                                          //0になったらステータスをIdolにする
                                                          .DoOnCompleted(() => m_status = PayoutStatus.Idol))
                               //メダルを投入
                               .Subscribe(_ =>
                               {
                                   Instantiate(_medalObject.Value, m_payoutPoint.position, Quaternion.identity);
                                   --m_payoutMedalStocks.Value;
                               });
        }


        public IObservable<int> PayoutStockMedals => m_payoutMedalStocks.AsObservable();

        public void Payout(int medals)
        {
            m_payoutMedalStocks.Value += medals;
        }

        private enum PayoutStatus
        {
            Idol,
            Payouting
        }
    }
}