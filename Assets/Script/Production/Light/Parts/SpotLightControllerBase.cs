using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityUtility;
using MedalPusher.Slot;
using System;
using DG.Tweening;

namespace MedalPusher.Production.Light
{
    public abstract class SpotLightControllerBase
    {
        private readonly IObservable<SlotProductionStatus> m_observableStatus;
        protected readonly UnityEngine.Light m_light;

        public SpotLightControllerBase(UnityEngine.Light light, IObservable<SlotProductionStatus> observableStatus)
        {
            m_light = light;
            m_observableStatus = observableStatus;
        }

        protected abstract IReadOnlyDictionary<SlotProductionStatus, Sequence> SequenceTable { get; }
        protected abstract IReadOnlyDictionary<SlotProductionStatus, Action> ActionTable { get; }

        protected IObservable<Action> ObservableAction =>
            m_observableStatus.DistinctUntilChanged()
                              .Select(status => ActionTable[status]);
    }
}