using System;
using System.Collections;
using System.Collections.Generic;
using MedalPusher.Input;
using MedalPusher.Item;
using MedalPusher.Item.Checker;
using MedalPusher.Item.Payout;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using Zenject;

namespace MedalPusher.Item.Inventory
{
    /// <summary>
    /// インベントリ内のメダル数を購読可能
    /// </summary>
    public interface IObservableMedalInventory
    {
        /// <summary>
        /// インベントリ内のメダル枚数を購読します。
        /// </summary>
        IObservable<int> ObservableMedalInventoryCount { get; }
    }

    /// <summary>
    /// 所持しているメダルを管理する
    /// </summary>
    public class MedalInventory : SerializedMonoBehaviour, IObservableMedalInventory
    {
        /// <summary>
        /// メダルの獲得通知を受け取る
        /// </summary>
        [SerializeField, Required]
        private IObservableMedalChecker winMedalChecker;
        /// <summary>
        /// メダル投入時に払出し司令を行う
        /// </summary>
        [Inject]
        private IMedalPayoutOperation medalPayoutOperator;
        /// <summary>
        /// ゲームコマンド投入司令を受け取る
        /// </summary>
        [Inject]
        private IGameCommandProvider gameCommandProvider;

        /// <summary>
        /// インベントリ内のメダル枚数
        /// </summary>
        [SerializeField]
        private IntReactiveProperty medalCount = new IntReactiveProperty();

        /// <summary>
        /// インスペクタのメダルからフィールドに支払う
        /// </summary>
        private void PayoutFromInventory()
        {
            //メダルが1枚以上あったら支払う
            if (medalCount.Value > 0)
            {
                medalPayoutOperator.PayoutRequest(1);
                --medalCount.Value;
            }
        }

        private void Start()
        {
            //獲得メダルを購読してインベントリに追加
            winMedalChecker.Checked
                           .Subscribe(medal => medalCount.Value += medal.Value);
            //メダル投入コマンドを受け取ったらメダルを投入
            gameCommandProvider.ObservableGameCommand
                               .Where(cmd => cmd == GameCommand.InputInspectorMedal)
                               .Subscribe(_ => PayoutFromInventory());
            //デバッグ用500枚投入コマンドで500枚シャワーで投入
            gameCommandProvider.ObservableGameCommand
                               .Where(cmd => cmd == GameCommand.debug_Input500Medals)
                               .Subscribe(_ => medalPayoutOperator.PayoutRequest(500, MedalPayoutMethod.Shower));

        }

        public IObservable<int> ObservableMedalInventoryCount => medalCount.AsObservable();
    }
}