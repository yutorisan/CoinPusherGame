using System.Collections;
using System.Collections.Generic;
using MedalPusher.Item.Checker;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UniRx;

namespace MedalPusher.Case
{
    public class FalledMedalView : SerializedMonoBehaviour
    {
        [SerializeField]
        private IObservableMedalChecker m_winMedalChecker;
        [SerializeField]
        private IObservableMedalChecker m_loseMedalChecker;
        [SerializeField]
        private TextMeshProUGUI m_winMedalView;
        [SerializeField]
        private TextMeshProUGUI m_loseMedalView;

        // Start is called before the first frame update
        void Start()
        {
            m_winMedalChecker.Checked.Count().Subscribe(n => m_winMedalView.text = n.ToString());
            m_loseMedalChecker.Checked.Count().Subscribe(n => m_loseMedalView.text = n.ToString());
        }
    }
}