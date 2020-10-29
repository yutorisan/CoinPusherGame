using UnityEngine;
using System.Collections;
using MedalPusher.Item.Payout.Pool;
using UniRx;
using Zenject;
using System;

namespace MedalPusher.Item.Payout
{
    public interface IMedalPayouter
    {
        /// <summary>
        /// 払出しストックに追加する
        /// </summary>
        /// <param name="medals"></param>
        void AddPayoutStock(int medals);
        /// <summary>
        /// 払出し待機中のメダル数
        /// </summary>
        IObservable<int> PayoutStock { get; }
    }
    public abstract class MedalPayouter : MonoBehaviour, IMedalPayouter
    {
        /// <summary>
        /// 払出し中のメダル数
        /// </summary>
        private ReactiveProperty<int> m_payoutMedalStocks = new ReactiveProperty<int>(0);
        [Inject]
        private IMedalPool m_medalPool;
        /// <summary>
        /// 払出しステータス
        /// </summary>
        protected PayoutStatus m_status = PayoutStatus.Idol;
        /// <summary>
        /// フィールドにメダルを出現させる
        /// </summary>
        protected void PayoutToField(Vector3 position)
        {
            m_medalPool.PickUp(MedalValue.Value1, position, Quaternion.identity);
            --m_payoutMedalStocks.Value;
        }
        /// <summary>
        /// メダルの払出しを開始させるタイミング
        /// </summary>
        protected IObservable<Unit> PowerOnTiming =>
            PayoutStock.Pairwise()
                       //Idol中のみ
                       .Where(_ => m_status == PayoutStatus.Idol)
                       //前より増えている
                       .Where(pair => pair.Current > pair.Previous)
                       .AsUnitObservable()
                       //払出しを開始するのでステータスを変える
                       .Do(_ => m_status = PayoutStatus.Payouting);


        public void AddPayoutStock(int medals)
        {
            m_payoutMedalStocks.Value += medals;
        }

        public IObservable<int> PayoutStock => m_payoutMedalStocks.AsObservable();
    }

    public enum PayoutStatus
    {
        Idol,
        Payouting
    }
}