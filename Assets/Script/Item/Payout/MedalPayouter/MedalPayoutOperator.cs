using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using UnityUtility;

namespace MedalPusher.Item.Payout
{
    /// <summary>
    /// メダル払出し要求する
    /// </summary>
    public interface IMedalPayoutOperation
    {
        /// <summary>
        /// メダル払出を要求する
        /// </summary>
        /// <param name="medals">払出しメダル数</param>
        void PayoutRequest(int medals, MedalPayoutMethod method = MedalPayoutMethod.Normal);
    }
    /// <summary>
    /// 払出し待機中のメダル数の購読を提供する
    /// </summary>
    public interface IObservableMedalPayoutStock
    {
        /// <summary>
        /// 払出し待機中のメダル数
        /// </summary>
        IObservable<int> PayoutStock { get; }
    }

    /// <summary>
    /// 各種MedalPayouterに対して払い出し指示を出す
    /// </summary>
    public class MedalPayoutOperator : MonoBehaviour, IMedalPayoutOperation, IObservableMedalPayoutStock
    {
        [SerializeField, Required]
        private NormalMedalPayouter normalPayouter;
        [SerializeField, Required]
        private ShowerMedalPayouter showerPayouter;

        /// <summary>
        /// 払い出し方法と、それに対応するPayouter
        /// </summary>
        private IReadOnlyDictionary<MedalPayoutMethod, IMedalPayouter> medalPayouterTable;

        void Awake()
        {
            //各種Payouterをテーブルに格納
            medalPayouterTable = new Dictionary<MedalPayoutMethod, IMedalPayouter>()
            {
                {MedalPayoutMethod.Normal, normalPayouter },
                {MedalPayoutMethod.Shower, showerPayouter }
            };

            //PayoutStockは、すべてのMedalPayouterのPayoutStockの最後の値を合計したものを発行する
            var ret = Observable.Return<int>(0);
            foreach (var payouter in medalPayouterTable.Values)
            {
                ret = ret.CombineLatest(payouter.PayoutStock, (i, j) => i + j);
            }
            this.PayoutStock = ret.Share();
        }

        public IObservable<int> PayoutStock { get; private set; }

        public void PayoutRequest(int medals, MedalPayoutMethod method = MedalPayoutMethod.Normal)
        {
            //該当のMedalPayouterに払い出し指示を出す
            medalPayouterTable[method].AddPayoutStock(medals);
        }
    }

    /// <summary>
    /// メダルの払出し方法
    /// </summary>
    public enum MedalPayoutMethod
    {
        /// <summary>
        /// 通常払出し
        /// </summary>
        Normal,
        /// <summary>
        /// メダルシャワー
        /// </summary>
        Shower
    }
}