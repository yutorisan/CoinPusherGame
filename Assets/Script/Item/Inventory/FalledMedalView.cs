using System.Collections;
using System.Collections.Generic;
using MedalPusher.Item.Checker;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UniRx;

namespace MedalPusher.Case
{
    /// <summary>
    /// フィールドから落下したメダルをカウントして表示する
    /// </summary>
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

        void Start()
        {
            m_winMedalChecker.Checked.Count().Subscribe(n => m_winMedalView.text = n.ToString());
            m_loseMedalChecker.Checked.Count().Subscribe(n => m_loseMedalView.text = n.ToString());
        }
    }
}