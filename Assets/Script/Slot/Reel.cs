﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MedalPusher.Slot
{
    /// <summary>
    /// リールに対して動きの指示を与える
    /// </summary>
    public interface IReelOperation
    {

    }
    /// <summary>
    /// リールの出目が決定したことを通知可能
    /// </summary>
    public interface IObservableReelDecided
    {

    }
    /// <summary>
    /// スロットのリール
    /// </summary>
    public class Reel : MonoBehaviour, IObservableReelDecided, IReelOperation
    {
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}