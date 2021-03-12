﻿using System.Collections;
using System.Collections.Generic;
using MedalPusher.Slot;
using UnityEngine;
using Zenject;
using UniRx;
using Sirenix.OdinInspector;
using UnityUtility.Enums;

namespace MedalPusher.Production.Light
{

    public class SlotLightController : MonoBehaviour
    {
        [Inject]
        private IObservableSlotProdctionStatus m_slotStatus;

        [SerializeField, Required, TitleGroup("LightComponent")]
        private UnityEngine.Light m_leftLight;
        [SerializeField, Required, TitleGroup("LightComponent")]
        private UnityEngine.Light m_rightLight;
        [SerializeField, Required, TitleGroup("LookAt")]
        private Transform m_leftCircleCenter;
        [SerializeField, Required, TitleGroup("LookAt")]
        private Transform m_rightCircleCenter;

        // Start is called before the first frame update
        void Start()
        {
            var observableStatus = m_slotStatus.ProductionStatus.Share();
            new SlotLightColorChanger(m_leftLight, observableStatus);
            new SlotLightColorChanger(m_rightLight, observableStatus);
            new SlotLightIntensityChanger(m_leftLight, observableStatus);
            new SlotLightIntensityChanger(m_rightLight, observableStatus);
            new SlotLightLookAtChanger(m_leftLight, observableStatus, m_leftCircleCenter.position, .1f, LeftRight.Left);
            new SlotLightLookAtChanger(m_rightLight, observableStatus, m_rightCircleCenter.position, .1f, LeftRight.Right);
        }
    }
}