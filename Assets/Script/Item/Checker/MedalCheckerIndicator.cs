using System.Collections;
using System.Collections.Generic;
using MedalPusher.Item.Checker;
using UnityEngine;
using UniRx;
using DG.Tweening;
using Sirenix.OdinInspector;
using System;

/// <summary>
/// MedalCheckerにメダルがはいったら発光させる
/// </summary>
public class MedalCheckerIndicator : MonoBehaviour
{
    private void Start()
    {
        Material material = this.GetComponent<Renderer>().material;

        this.GetComponentInChildren<IObservableMedalChecker>()
            .Count
            .Select(count => count > 0 ? Color.red : Color.gray)
            .Subscribe(color => material.color = color);
    }


    //private Light _light;

    ///// <summary>
    ///// 発光の最大値
    ///// </summary>
    //[SerializeField]
    //private float m_maxIntensity;
    ///// <summary>
    ///// アニメーション継続時間
    ///// </summary>
    //[SerializeField]
    //private float m_animationInterval;

    //// Start is called before the first frame update
    //void Start()
    //{
    //    //子オブジェクトのライトを取得
    //    _light = GetComponentInChildren<Light>();

    //    //メダルがはいったらライトを発光させる
    //    this.GetComponentInChildren<IObservableMedalChecker>()
    //        .Checked
    //        .Subscribe(_ => Luminous());

    //    //Observable.Interval(TimeSpan.FromSeconds(0.5))
    //    //    .Subscribe(_ => _light.intensity += 0.1f);
    //}

    //private void Luminous()
    //{
    //    DOTween.To(() => _light.intensity,
    //               val => _light.intensity = val,
    //               m_maxIntensity,
    //               m_animationInterval)
    //           .SetLoops(2, LoopType.Yoyo)
    //           .Play();
    //}
}
