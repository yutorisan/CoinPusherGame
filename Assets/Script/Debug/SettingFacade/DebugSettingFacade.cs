using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace MedalPusher.Debug
{
    /// <summary>
    /// デバッグ用
    /// ゲーム内のよく変更する設定を一括設定できるFacadeオブジェクト
    /// </summary>
    public class DebugSettingFacade : MonoBehaviour
    {
        /// <summary>
        /// Facadeによる一括設定変更の有効/無効
        /// </summary>
        [SerializeField]
        private bool isEnabled;

        /// <summary>
        /// Instantiateするメダル数
        /// </summary>
        [SerializeField]
        private int instantiateMedals;
        /// <summary>
        /// ゲーム開始時にフィールドに自動充填するメダル数
        /// </summary>
        [SerializeField]
        private int fillMedals;

        [Inject]
        private IInitialInstantiateMedalCountSetterFacade m_instantiateCntSetter;
        [Inject]
        private IInitialFillMedalCountSetterFacade m_initialFillCntSetter;


        // Start is called before the first frame update
        void Start()
        {
            //Facadeが無効の場合は何もせずに準備完了状態にする
            if (!isEnabled)
            {
                m_initialFillCntSetter.Already();
                m_instantiateCntSetter.Already();
                return;
            }

            //Facadeが有効の場合は、Inspectorにより設定された各種設定を各オブジェクトに発行する
            //各オブジェクトへの発行が完了したら準備完了通知を送る
            m_instantiateCntSetter.SetInitialInstantiateMedalCount(instantiateMedals);
            m_instantiateCntSetter.Already();
            m_initialFillCntSetter.SetInitialFillMedalCount(fillMedals);
            m_initialFillCntSetter.Already();
        }
    }

    /// <summary>
    /// Facade用
    /// Instantiateするメダル数を設定できる
    /// </summary>
    public interface IInitialInstantiateMedalCountSetterFacade : IDebugSettingFacadeTarget
    {
        /// <summary>
        /// (Facade専用)ゲーム開始時にInstantiateされるメダル数を設定する
        /// </summary>
        /// <param name="count"></param>
        void SetInitialInstantiateMedalCount(int count);
    }
    /// <summary>
    /// Facade用
    /// ゲーム開始時に充填するメダル数を設定できる
    /// </summary>
    public interface IInitialFillMedalCountSetterFacade : IDebugSettingFacadeTarget
    {
        /// <summary>
        /// (Facade専用)ゲーム開始時にフィールドに降らせるメダル量を設定する
        /// </summary>
        /// <param name="count"></param>
        void SetInitialFillMedalCount(int count);
    }
}