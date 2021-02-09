using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MedalPusher.GameSystem.Facade
{
    public interface IFacadeTarget
    {
        void Already();
    }

    public abstract class FacadeTargetMonoBehaviour : MonoBehaviour, IFacadeTarget
    {
        private readonly UniTaskCompletionSource uniTaskCompletionSource = new UniTaskCompletionSource();

        public void Already()
        {
            uniTaskCompletionSource.TrySetResult();
        }

        protected UniTask FacadeAlreadyOKAsync => uniTaskCompletionSource.Task;
    }
}