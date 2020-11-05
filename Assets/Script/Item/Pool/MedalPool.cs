using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MedalPusher.Item;
using UniRx;
using UnityEngine;
using UnityUtility;

namespace MedalPusher.Item.Payout.Pool
{
    public interface IMedalPool
    {
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
        IReadOnlyDictionary<MedalValue, ObservableMedalPoolInfo> MedalValueObservableInfoTable { get; }
    }

    public class MedalPool : MonoBehaviour, IMedalPool, IObservableMedalPoolInfo
    {
        /// <summary>
        /// メダルプール本体
        /// </summary>
        private Dictionary<MedalValue, IReactiveCollection<Medal>> _medalPool = new Dictionary<MedalValue, IReactiveCollection<Medal>>();
        /// <summary>
        /// メダル種別のアクティベートされた（フィールド上に存在する）メダル数
        /// </summary>
        private Dictionary<MedalValue, IReactiveProperty<int>> _activatedMedalCountTable = new Dictionary<MedalValue, IReactiveProperty<int>>();


        [SerializeField]
        private List<TypePrefabInitCapaSet> m_poolList;

        public IReadOnlyDictionary<MedalValue, ObservableMedalPoolInfo> MedalValueObservableInfoTable =>
            _medalPool.Zip(_activatedMedalCountTable, (pool, activated) => (pool.Key, new ObservableMedalPoolInfo(pool.Value.ObserveCountChanged(), activated.Value)))
                      .ToDictionary(x => x.Key, x => x.Item2);

        private void Awake()
        {
            //SerializeFieldで設定された数だけメダルオブジェクトを生成する
            foreach (var set in m_poolList)
            {
                _medalPool.Add(set.ValueType, new ReactiveCollection<Medal>(Enumerable.Range(1, set.Capacity).Select(_ => //ReactiveCollectionの初期値を発行したいよぉ
                {
                    var medal = Instantiate(set.Prefab);
                    medal.gameObject.SetActive(false);
                    medal.OnReturnToPool += Returned;
                    return medal;
                }).ToList()));

                _activatedMedalCountTable.Add(set.ValueType, new ReactiveProperty<int>(0));
            }
            //new ReactiveCollection<int>()
        }


        public IMedal PickUp(MedalValue valueType, Vector3 position, Quaternion rotation)
        {
            ++_activatedMedalCountTable[valueType].Value;

            //activeでないメダルを見つける
            var idolMedal = _medalPool[valueType].FirstOrDefault(m => m.gameObject.activeSelf == false);
            //見つかったらそれを返す
            if (idolMedal != null)
            {
                idolMedal.gameObject.SetActive(true);
                idolMedal.gameObject.transform.position = position;
                idolMedal.gameObject.transform.rotation = rotation;
                return idolMedal;
            }

            //見つからなかった（全て使用中）なら、新しく生成する
            var generated = Instantiate(m_poolList.First(set => set.ValueType == valueType).Prefab, position, rotation);
            generated.OnReturnToPool += Returned;
            _medalPool[valueType].Add(generated);
            return generated;
        }

        private void Returned(MedalValue medalValue)
        {
            --_activatedMedalCountTable[medalValue].Value;
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
            public int Capacity => initCapacity;
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
