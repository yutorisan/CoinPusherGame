using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MedalPusher.Item;
using UniRx;
using UnityEngine;
using UnityUtility;
using UniRx.Diagnostics;
using UnityUtility.Linq;
using MedalPusher.Debug;

namespace MedalPusher.Item.Pool
{
    /// <summary>
    /// MedalPoolからMedalインスタンスを取得できる
    /// </summary>
    public interface IMedalPoolPickUper
    {
        /// <summary>
        /// MedalPoolからメダルオブジェクトをアクティブ化して配置する
        /// rotationはQuaternion.identity
        /// </summary>
        /// <param name="valueType"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        IMedal PickUp(MedalValue valueType, Vector3 position);
        /// <summary>
        /// MedalPoolからメダルオブジェクトをアクティブ化して配置する
        /// </summary>
        /// <param name="valueType"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <returns></returns>
        IMedal PickUp(MedalValue valueType, Vector3 position, Quaternion rotation);
    }
    /// <summary>
    /// MedalPoolの情報を取得する
    /// </summary>
    public interface IObservableMedalPoolInfo
    {
        IReadOnlyDictionary<MedalValue, MedalPool.ObservableMedalPoolInfo> ObservableCountInfo { get; }
    }

    /// <summary>
    /// 各種Medalのオブジェクトプール
    /// </summary>
    public class MedalPool : DebugSettingFacadeTargetMonoBehaviour, IMedalPoolPickUper, IObservableMedalPoolInfo, IInitialInstantiateMedalCountSetterFacade
    {
        /// <summary>
        /// メダルプール本体
        /// </summary>
        private readonly Dictionary<MedalValue, IReactiveCollection<IPoolObjectKeeper>> medalPool = new Dictionary<MedalValue, IReactiveCollection<IPoolObjectKeeper>>();
        /// <summary>
        /// メダルのカウント情報を管理するモジュール
        /// </summary>
        private readonly MedalPoolCounter couter = new MedalPoolCounter();

        /// <summary>
        /// メダルの初期化情報
        /// </summary>
        [SerializeField]
        private List<MedalInitSettings> poolList;

        /// <summary>
        /// メダルプールのカウント情報を取得する
        /// </summary>
        public IReadOnlyDictionary<MedalValue, ObservableMedalPoolInfo> ObservableCountInfo => couter.GetObservableInfo();

        private async void Start()
        {
            //Facadeによる設定対象なので、Facadeによる設定完了まで待機する
            await this.FacadeAlreadyOKAsync;

            //SerializeFieldで設定された数だけメダルオブジェクトを生成する
            foreach (var set in poolList)
            {
                //メダルのインスタンスを生成
                var medals = Enumerable.Range(1, set.Capacity)
                                       .Select(_ => Instantiate(set.Prefab))
                                       .ToArray(); //遅延評価回避
                
                //メダルプールに入れる
                medalPool.Add(set.ValueType, new ReactiveCollection<IPoolObjectKeeper>(medals));//ReactiveCollectionの初期値を発行したいよぉ
                //メダルのアクティブ数のカウントを委託する
                couter.OutsourceCounting(medals);
            }
        }

        public IMedal PickUp(MedalValue valueType, Vector3 position) => PickUp(valueType, position, Quaternion.identity);
        public IMedal PickUp(MedalValue valueType, Vector3 position, Quaternion rotation)
        {
            //activeでないメダルを見つける
            var idolMedal = medalPool[valueType].FirstOrDefault(m => m.Status.Value == PoolObjectUseStatus.Idol);
            //見つかったらそれを返す
            if (idolMedal != null)
            {
                idolMedal.Keep(position, rotation);
                return (IMedal)idolMedal; //MedalであることはInstantiateから保証されているのでダウンキャストでも問題ない
            }
            else
            {
                //見つからなかった（全て使用中）なら、新しく生成する
                var medal = Instantiate(poolList.First(set => set.ValueType == valueType).Prefab, position, rotation);
                //メダルプールに入れる
                medalPool[valueType].Add(medal);
                //アクティブ数のカウントを委託
                couter.OutsourceCounting(medal);

                return medal;
            }
        }

        public void SetInitialInstantiateMedalCount(int count)
        {
            //MedalValue1の初期生成数を上書きする
            this.poolList.Find(n => n.ValueType == MedalValue.Value1).Capacity = count;
        }

        #region InnerClass
        /// <summary>
        /// メダル種別ごとの設定
        /// </summary>
        [Serializable]
        private class MedalInitSettings
        {
            [SerializeField]
            private MedalValue valueType;
            [SerializeField]
            private Medal prefab;
            [SerializeField]
            private int initCapacity;

            /// <summary>
            /// メダルの種類
            /// </summary>
            public MedalValue ValueType => valueType;
            /// <summary>
            /// メダルオブジェクトのPrefab
            /// </summary>
            public Medal Prefab => prefab;
            /// <summary>
            /// Instantiateする数
            /// </summary>
            public int Capacity { get => initCapacity; set => initCapacity = value; }
        }

        /// <summary>
        /// メダルプールで管理されている各種メダルについて
        /// アクティブ数（フィールド上に存在する数）、インスタンス生成数を管理する
        /// </summary>
        private class MedalPoolCounter
        {
            /// <summary>
            /// 各メダルのアクティブメダル数
            /// </summary>
            private readonly IReadOnlyDictionary<MedalValue, ReactiveProperty<int>> activeMedalCountTable;
            /// <summary>
            /// 各メダルのインスタンス生成総数
            /// </summary>
            private readonly IReadOnlyDictionary<MedalValue, ReactiveProperty<int>> instantiatedMedalCountTable;


            public MedalPoolCounter()
            {
                //Dictionaryを初期化
                var medalTypes = UnityUtility.Enum.All<MedalValue>();
                activeMedalCountTable = medalTypes.ToDictionary(medalval => medalval, _ => new ReactiveProperty<int>());
                instantiatedMedalCountTable = medalTypes.ToDictionary(medalval => medalval, _ => new ReactiveProperty<int>());
            }

            /// <summary>
            /// メダルのアクティブ状態のチェックを委託する
            /// </summary>
            /// <param name="medal"></param>
            public void OutsourceCounting(Medal medal)
            {
                //メダルのアクティブ状態の変化を購読して、全体のアクティブメダル数をカウントする
                //各々のメダルがアクティブになれば+1, 非アクティブになれば-1する
                medal.Status
                     .SkipLatestValueOnSubscribe()
                     .Select(status => status == PoolObjectUseStatus.Used ? 1 : -1)
                     .Subscribe(diff => activeMedalCountTable[medal.ValueType].Value += diff);
                //インスタンス生成数を更新
                ++instantiatedMedalCountTable[medal.ValueType].Value;
            }
            public void OutsourceCounting(IEnumerable<Medal> medals)
            {
                foreach (var medal in medals) OutsourceCounting(medal);
            }

            /// <summary>
            /// カウント情報の購読を取得する
            /// </summary>
            /// <returns></returns>
            public IReadOnlyDictionary<MedalValue, ObservableMedalPoolInfo> GetObservableInfo() =>
                //アクティブ数とインスタンス数のReactivePropertyをIObservableとしてObservableMedalPoolInfoにまとめる
                instantiatedMedalCountTable.DictionaryZip(activeMedalCountTable, (instantiateKVP, activeKVP) =>
                                                           new ObservableMedalPoolInfo(instantiateKVP, activeKVP))
                                           .ToDictionary();
        }

        /// <summary>
        /// MedalPool内の情報の購読権をまとめたオブジェクト
        /// </summary>
        public struct ObservableMedalPoolInfo
        {
            public ObservableMedalPoolInfo(IObservable<int> instantiated, IObservable<int> onField)
            {
                InstantiatedMedals = instantiated;
                OnFieldMedals = onField;
            }
            /// <summary>
            /// Instantiateしたメダル数
            /// </summary>
            public IObservable<int> InstantiatedMedals { get; }
            /// <summary>
            /// フィールド上に存在するメダル数
            /// </summary>
            public IObservable<int> OnFieldMedals { get; }
        }
        #endregion
    }

}
