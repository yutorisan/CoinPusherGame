using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class DOTweenInitialzer : MonoBehaviour
{
    private void Awake()
    {
        //DOTweenのSequenceやTweenを生成したときに自動生成されないように設定
        DOTween.Init();
        DOTween.defaultAutoPlay = AutoPlay.None;
    }
}
