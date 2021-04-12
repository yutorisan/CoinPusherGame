using System.Collections;
using System.Collections.Generic;
using MedalPusher.Item.Pool;
using UnityEngine;
using Zenject;
using UniRx;
using UnityEngine.UI;
using Sirenix.OdinInspector;

namespace MedalPusher.Debug
{
    /// <summary>
    /// デバッグ用
    /// MedalPoolの情報を表示させる
    /// </summary>
    public class MedalPoolDebugInfoView : MonoBehaviour
    {
        /// <summary>
        /// メダルプールの情報の購読元
        /// </summary>
        [Inject]
        private IObservableMedalPoolInfo poolInfo;
        /// <summary>
        /// 表示するTextのPrefab
        /// </summary>
        [SerializeField, Required]
        private Text sourceText;

        void Start()
        {
            //MedalPoolで管理されるメダル種の分だけTextのPrefabをInstantiateして、そこに情報を表示させる
            foreach (var kvp in poolInfo.ObservableCountInfo)
            {
                //Instantiate数を表示する
                var text = Instantiate(sourceText);
                text.transform.SetParent(transform);
                kvp.Value.InstantiatedMedals.SubscribeToText(text, i => $"{kvp.Key}:Instantiated:{i}");
                //OnField数を表示する
                var text2 = Instantiate(sourceText);
                text2.transform.SetParent(transform);
                kvp.Value.OnFieldMedals.SubscribeToText(text2, i => $"{kvp.Key}:OnField:{i}");
            }
        }

    }
}