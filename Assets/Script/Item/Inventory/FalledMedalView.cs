using MedalPusher.Item.Checker;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UniRx;
using MedalPusher.Utils;

namespace MedalPusher.Case
{
    /// <summary>
    /// フィールドから落下したメダルをカウントして表示する
    /// </summary>
    public class FalledMedalView : SerializedMonoBehaviour
    {
        [SerializeField, Required]
        private IObservableMedalChecker m_winMedalChecker;
        [SerializeField, Required]
        private IObservableMedalChecker m_loseMedalChecker;
        [SerializeField, Required]
        private TextMeshProUGUI m_winMedalView;
        [SerializeField, Required]
        private TextMeshProUGUI m_loseMedalView;

        void Start()
        {
            m_winMedalChecker.Checked.Count().Subscribe(n => m_winMedalView.text = n.ToString());
            m_loseMedalChecker.Checked.Count().Subscribe(n => m_loseMedalView.text = n.ToString());
        }
    }
}