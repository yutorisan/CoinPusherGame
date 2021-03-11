using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MedalPusher.Item;
using UniRx;
using UnityEngine;
using UnityUtility;
using UniRx.Diagnostics;
using MedalPusher.GameSystem.Facade;

namespace MedalPusher.Item.Pool
{
    public interface IMedalPool
    {
        /// <summary>
        /// MedalPoolからメダルオブジェクトをアクティブ化して配置する
        /// rotationはQuaternion.identity
        /// </summary>
        /// <param name="valueType"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
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
        IReadOnlyDictionary<MedalValue, ObservableMedalPoolInfo> ObservableCountInfo { get; }
    }

    public class MedalPool : FacadeTargetMonoBehaviour, IMedalPool, IObservableMedalPoolInfo, IInitialInstantiateMedalCountSetterFacade
    {
        /// <summary>
        /// メダルプール本体
        /// </summary>
        private readonly Dictionary<MedalValue, IReactiveCollection<Medal>> _medalPool = new Dictionary<MedalValue, IReactiveCollection<Medal>>();
        /// <summary>
        /// メダルのカウント情報を管理するモジュール
        /// </summary>
        private readonly MedalPoolCounter _couter = new MedalPoolCounter();

        [SerializeField]
        private List<TypePrefabInitCapaSet> m_poolList;

        /// <summary>
        /// メダルプールのカウント情報を取得する
        /// </summary>
        public IReadOnlyDictionary<MedalValue, ObservableMedalPoolInfo> ObservableCountInfo => _couter.GetObservableInfo();

        private async void Start()
        {
            await this.FacadeAlreadyOKAsync;

            //SerializeFieldで設定された数だけメダルオブジェクトを生成する
            foreach (var set in m_poolList)
            {
                //メダルのインスタンスを生成
                var medals = Enumerable.Range(1, set.Capacity)
                                       .Select(_ => Instantiate(set.Prefab))
                                       .ToArray(); //遅延評価回避
                
                //メダルプールに入れる
                _medalPool.Add(set.ValueType, new ReactiveCollection<Medal>(medals));//ReactiveCollectionの初期値を発行したいよぉ
                //メダルのアクティブ数のカウントを委託する
                _couter.OutsourceCounting(medals);
            }
        }

        public IMedal PickUp(MedalValue valueType, Vector3 position) => PickUp(valueType, position, Quaternion.identity);
        public IMedal PickUp(MedalValue valueType, Vector3 position, Quaternion rotation)
        {
            //activeでないメダルを見つける
            var idolMedal = _medalPool[valueType].FirstOrDefault(m => m.gameObject.activeSelf == false);
            //見つかったらそれを返す
            if (idolMedal != null)
            {
                idolMedal.Takeout(position, rotation);
                return idolMedal;
            }
            else
            {
                //見つからなかった（全て使用中）なら、新しく生成する
                var medal = Instantiate(m_poolList.First(set => set.ValueType == valueType).Prefab, position, rotation);
                //メダルプールに入れる
                _medalPool[valueType].Add(medal);
                //アクティブ数のカウントを委託
                _couter.OutsourceCounting(medal);

                return medal;
            }
        }

        public void SetInitialInstantiateMedalCount(int count)
        {
            this.m_poolList.Find(n => n.ValueType == MedalValue.Value1).Capacity = count;
        }

        [Serializable]
        private class TypePrefabInitCapaSet
        {
            [SerializeField]
            private MedalValue valueType;
            [SerializeField]
            private Medal prefab;
            [SerializeField]
            private int initCapacity;

            public MedalValue ValueType => valueType;
            public Medal Prefab => prefab;
            public int Capacity { get => initCapacity; set => initCapacity = value; }
        }

        /// <summary>
        /// メダルプールで管理されているメダルの各種カウント情報を管理する
        /// </summary>
        private class MedalPoolCounter
        {
            /// <summary>
            /// 各メダルのアクティブメダル数
            /// </summary>
            private readonly IReadOnlyDictionary<MedalValue, ReactiveProperty<int>> _activeMedalCountTable;
            /// <summary>
            /// 各メダルのインスタンス生成総数
            /// </summary>
            private readonly IReadOnlyDictionary<MedalValue, ReactiveProperty<int>> _instantiatedMedalCountTable;


            public MedalPoolCounter()
            {
                //Dictionaryを初期化
                var medalTypes = UnityUtility.Enum.All<MedalValue>();
                _activeMedalCountTable = medalTypes.ToDictionary(medalval => medalval, _ => new ReactiveProperty<int>());
                _instantiatedMedalCountTable = medalTypes.ToDictionary(medalval => medalval, _ => new ReactiveProperty<int>());
            }

            /// <summary>
            /// メダルのアクティブ状態のチェックを委託する
            /// </summary>
            /// <param name="medal"></param>
            public void OutsourceCounting(IReadOnlyMedal medal)
            {
                //メダルのアクティブ状態の変化を購読して、全体のアクティブメダル数をカウントする
                //各々のメダルがアクティブになれば+1, 非アクティブになれば-1する
                medal.Status
                     .Skip(1)
                     .Select(status => status.IsUsed())
                     .Select(used => used ? 1 : -1)
                     .Subscribe(diff => _activeMedalCountTable[medal.ValueType].Value += diff);
                //インスタンス生成数を更新
                ++_instantiatedMedalCountTable[medal.ValueType].Value;
            }
            public void OutsourceCounting(IEnumerable<IReadOnlyMedal> medals)
            {
                foreach (var medal in medals) OutsourceCounting(medal);
            }

            /// <summary>
            /// カウント情報の購読を取得する
            /// </summary>
            /// <returns></returns>
            public IReadOnlyDictionary<MedalValue, ObservableMedalPoolInfo> GetObservableInfo() =>
                _instantiatedMedalCountTable.Zip(_activeMedalCountTable, (instantiateKVP, activeKVP) =>
                                                    new { ValueType = instantiateKVP.Key,
                                                          Info = new ObservableMedalPoolInfo(instantiateKVP.Value, activeKVP.Value) })
                                            .ToDictionary(tuple => tuple.ValueType, tuple => tuple.Info);
        }
    }


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
}
