using System.Collections;
using System.Collections.Generic;
using MedalPusher.Item.Checker;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;
using UniRx;

namespace MedalPusher.Lottery
{
    public class LotteryStockController : SerializedMonoBehaviour
    {
        [Inject]
        private IBallBornOperator _bornOperator;

        [SerializeField]
        private IObservableMedalChecker m_medalChecker;

        // Start is called before the first frame update
        void Start()
        {
            m_medalChecker.Checked.Subscribe(_ => _bornOperator.Born());
        }
    }
}