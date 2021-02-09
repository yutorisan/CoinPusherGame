using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace MedalPusher.GameSystem.Facade
{
    public class DebugFacade : MonoBehaviour
    {
        [SerializeField]
        private bool m_isEnabled;
        [SerializeField]
        private int m_instantiateMedals;
        [SerializeField]
        private int m_fillMedals;

        [Inject]
        private IInitialInstantiateMedalCountSetterFacade m_instantiateCntSetter;
        [Inject]
        private IInitialFillMedalCountSetterFacade m_initialFillCntSetter;


        // Start is called before the first frame update
        void Start()
        {
            if (!m_isEnabled)
            {
                m_initialFillCntSetter.Already();
                m_instantiateCntSetter.Already();
                return;
            }

            m_instantiateCntSetter.SetInitialInstantiateMedalCount(m_instantiateMedals);
            m_instantiateCntSetter.Already();
            m_initialFillCntSetter.SetInitialFillMedalCount(m_fillMedals);
            m_initialFillCntSetter.Already();
        }
    }

    public interface IInitialInstantiateMedalCountSetterFacade : IFacadeTarget
    {
        /// <summary>
        /// (Facade専用)ゲーム開始時にInstantiateされるメダル数を設定する
        /// </summary>
        /// <param name="count"></param>
        void SetInitialInstantiateMedalCount(int count);
    }
    public interface IInitialFillMedalCountSetterFacade : IFacadeTarget
    {
        /// <summary>
        /// (Facade専用)ゲーム開始時にフィールドに降らせるメダル量を設定する
        /// </summary>
        /// <param name="count"></param>
        void SetInitialFillMedalCount(int count);
    }
}