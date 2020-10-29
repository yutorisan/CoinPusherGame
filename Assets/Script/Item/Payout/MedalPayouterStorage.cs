using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityUtility;
using Zenject;

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

    public class MedalPayouterStorage : MonoBehaviour, IMedalPayoutOperation, IObservableMedalPayoutStock
    {
        [SerializeField]
        private NormalMedalPayouter m_normal;
        [SerializeField]
        private ShowerMedalPayouter m_shower;

        private UniReadOnly<IReadOnlyDictionary<MedalPayoutMethod, IMedalPayouter>> _medalPayouterTable = new UniReadOnly<IReadOnlyDictionary<MedalPayoutMethod, IMedalPayouter>>();

        void Start()
        {
            _medalPayouterTable.Initialize(new Dictionary<MedalPayoutMethod, IMedalPayouter>()
            {
                {MedalPayoutMethod.Normal, m_normal },
                {MedalPayoutMethod.Shower, m_shower }
            });
        }

        public IObservable<int> PayoutStock
        {
            get
            {
                //すべてのPayouterの最後の値を合計したものを返す
                var ret = Observable.Return<int>(0);
                foreach (var payouter in _medalPayouterTable.Value.Values)
                {
                    ret = ret.CombineLatest(payouter.PayoutStock, (i, j) => i + j);
                }
                return ret;
            }
        }

        public void PayoutRequest(int medals, MedalPayoutMethod method = MedalPayoutMethod.Normal)
        {
            _medalPayouterTable.Value[method].AddPayoutStock(medals);
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