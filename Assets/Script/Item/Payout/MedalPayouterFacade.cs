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
    /// 各種MedalPayouterを保持して適切に操作するMedalPayouterのFacade
    /// </summary>
    public class MedalPayouterFacade : MonoBehaviour, IMedalPayoutOperation, IObservableMedalPayoutStock
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
        }

        public IObservable<int> PayoutStock
        {
            get
            {
                //すべてのPayouterの最後の値を合計したものを返す
                var ret = Observable.Return<int>(0);
                foreach (var payouter in medalPayouterTable.Values)
                {
                    ret = ret.CombineLatest(payouter.PayoutStock, (i, j) => i + j);
                }
                return ret;
            }
        }

        public void PayoutRequest(int medals, MedalPayoutMethod method = MedalPayoutMethod.Normal)
        {
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