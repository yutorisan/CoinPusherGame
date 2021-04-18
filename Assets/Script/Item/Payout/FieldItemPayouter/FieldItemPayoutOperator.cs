using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MedalPusher.Item.Payout
{
    /// <summary>
    /// アイテムの払い出しを要求できる
    /// </summary>
    public interface IFieldItemPayoutOperation
    {
        /// <summary>
        /// アイテムの払い出しを要求します
        /// </summary>
        /// <typeparam name="TFieldItem">要求するアイテムの種類</typeparam>
        void PayoutRequest<TFieldItem>() where TFieldItem : IFieldItem;
    }
    /// <summary>
    /// 払い出し中のアイテム一覧を購読できる
    /// </summary>
    public interface IObservableFieldItemPayouter
    {
        //未実装
    }
    /// <summary>
    /// FieldItemPayouterにアイテムの払い出しを支持する
    /// </summary>
    public class FieldItemPayoutOperator : SerializedMonoBehaviour, IObservableFieldItemPayouter, IFieldItemPayoutOperation
    {
        [SerializeField]
        private List<FieldItem> itemList;
        [SerializeField]
        private IFieldItemPayouter fieldItemPayouter;

        public void PayoutRequest<TFieldItem>() where TFieldItem : IFieldItem
        {
            //設定されたアイテムリストの中から、払い出すアイテムのPrefabを見つけ出す
            FieldItem item = itemList.Find(i => i is TFieldItem);
            //そのPrefabを払い出す
            fieldItemPayouter.Payout(item);
        }
    }
}