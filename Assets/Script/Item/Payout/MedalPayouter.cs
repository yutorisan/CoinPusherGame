using UnityEngine;
using System.Collections;
using MedalPusher.Item.Pool;
using UniRx;
using Zenject;
using System;

namespace MedalPusher.Item.Payout
{
    /// <summary>
    /// MedalPayouterへのアクセス
    /// </summary>
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

    /// <summary>
    /// メダルの払い出しを担当するベースクラス
    /// </summary>
    public abstract class MedalPayouter : MonoBehaviour, IMedalPayouter
    {
        /// <summary>
        /// 払出し中のメダル数
        /// </summary>
        private readonly ReactiveProperty<int> payoutMedalStocks = new ReactiveProperty<int>(0);
        /// <summary>
        /// メダルプールからメダルを取得する
        /// </summary>
        [Inject]
        private IMedalPoolPickUper medalPickuper;
        /// <summary>
        /// 払出しステータス
        /// </summary>
        protected PayoutStatus status = PayoutStatus.Idol;

        /// <summary>
        /// フィールドにメダルを出現させる
        /// </summary>
        protected void PayoutToField(Vector3 position, Quaternion rotation)
        {
            medalPickuper.PickUp(MedalValue.Value1, position, rotation);
            --payoutMedalStocks.Value;
        }
        /// <summary>
        /// フィールドにメダルを出現させる
        /// </summary>
        protected void PayoutToField(Vector3 position) => PayoutToField(position, Quaternion.identity);
        /// <summary>
        /// メダルの払出しを開始させるタイミング
        /// </summary>
        protected IObservable<Unit> PowerOnTiming =>
            PayoutStock.Pairwise()
                       //Idol中のみ
                       .Where(_ => status == PayoutStatus.Idol)
                       //前より増えている
                       .Where(pair => pair.Current > pair.Previous)
                       .AsUnitObservable()
                       //払出しを開始するのでステータスを変える
                       .Do(_ => status = PayoutStatus.Payouting);

        public void AddPayoutStock(int medals)
        {
            payoutMedalStocks.Value += medals;
        }

        public IObservable<int> PayoutStock => payoutMedalStocks.AsObservable();

        protected enum PayoutStatus
        {
            Idol,
            Payouting
        }
    }
}