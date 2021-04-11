using System.Collections;
using System.Collections.Generic;
using MedalPusher.Item.Checker;
using UnityEngine;
using UniRx;
using DG.Tweening;
using Sirenix.OdinInspector;
using System;

namespace MedalPusher.Item.Checker
{
    /// <summary>
    /// MedalCheckerにメダルがはいったら発光させる
    /// </summary>
    public class MedalCheckerIndicator : MonoBehaviour
    {
        private void Start()
        {
            Material material = this.GetComponent<Renderer>().material;

            this.GetComponentInChildren<IObservableInColliderCountableMedalChecker>()
                .InColliderCount
                .Select(count => count > 0 ? Color.red : Color.gray)
                .Subscribe(color => material.color = color);
        }
    }
}