using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MedalPusher.Item;
using UnityEngine;
using UnityUtility;

namespace MedalPusher.Item.Pool
{
    public interface IMedalPool
    {
        IMedal PickUp(MedalValue valueType, Vector3 position, Quaternion rotation);
    }

    public class MedalPool : MonoBehaviour, IMedalPool
    {
        //[SerializeField]
        //private readonly Medal m_medal1Prefab;
        //[SerializeField]
        //private readonly int m_medal1InitializeCount;

        //[SerializeField]
        //private readonly Medal m_medal3Prefab;
        //[SerializeField]
        //private readonly int m_medal3InitializeCount;

        //[SerializeField]
        //private readonly Medal m_medal10Prefab;
        //[SerializeField]
        //private readonly int m_medal10InitializeCount;

        //private UniReadOnly<List<IMedal>> m_medal1Pool = new UniReadOnly<List<IMedal>>();
        //private UniReadOnly<List<IMedal>> m_medal3Pool = new UniReadOnly<List<IMedal>>();
        //private UniReadOnly<List<IMedal>> m_medal10Pool = new UniReadOnly<List<IMedal>>();

        private Dictionary<MedalValue, IList<Medal>> m_medalPool = new Dictionary<MedalValue, IList<Medal>>();

        [SerializeField]
        private List<TypePrefabInitCapaSet> m_poolList;


        private void Start()
        {
            //SerializeFieldで設定された数だけメダルオブジェクトを生成する
            foreach (var set in m_poolList)
            {
                m_medalPool.Add(set.ValueType, Enumerable.Range(1, set.Capacity).Select(_ =>
                {
                    var medal = Instantiate(set.Prefab);
                    medal.gameObject.SetActive(false);
                    return medal;
                }).ToList());
            }

            //m_medal1Pool.Initialize(Enumerable.Range(1, m_medal1InitializeCount)
            //                                  .Select(_ => (IMedal)Instantiate(m_medal1Prefab))
            //                                  .ToList());
            //m_medal3Pool.Initialize(Enumerable.Range(1, m_medal3InitializeCount)
            //                                  .Select(_ => (IMedal)Instantiate(m_medal3Prefab))
            //                                  .ToList());
            //m_medal10Pool.Initialize(Enumerable.Range(1, m_medal10InitializeCount)
            //                                  .Select(_ => (IMedal)Instantiate(m_medal10Prefab))
            //                                  .ToList());

        }


        public IMedal PickUp(MedalValue valueType, Vector3 position, Quaternion rotation)
        {
            //activeでないメダルを見つける
            var idolMedal = m_medalPool[valueType].FirstOrDefault(m => m.gameObject.activeSelf == false);
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
            m_medalPool[valueType].Add(generated);
            return generated;
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
}
