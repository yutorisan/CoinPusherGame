using System.Collections;
using System.Collections.Generic;
using MedalPusher.Item.Pool;
using UnityEngine;
using Zenject;
using UniRx;
using UnityEngine.UI;

namespace MedalPusher.Debug
{

    public class MedalPoolDebug : MonoBehaviour
    {
        [Inject]
        private IObservableMedalPoolInfo _poolInfo;

        [SerializeField]
        private Text m_sourceText;

        // Start is called before the first frame update
        void Start()
        {
            
            foreach (var kvp in _poolInfo.ObservableCountInfo)
            {
                var text = Instantiate(m_sourceText);
                text.transform.SetParent(transform);
                kvp.Value.InstantiatedMedals.SubscribeToText(text, i => $"{kvp.Key}:Instantiated:{i}");
                var text2 = Instantiate(m_sourceText);
                text2.transform.SetParent(transform);
                kvp.Value.OnFieldMedals.SubscribeToText(text2, i => $"{kvp.Key}:OnField:{i}");
            }
        }

    }
}