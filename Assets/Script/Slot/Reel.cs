using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityUtility;

namespace MedalPusher.Slot
{
    /// <summary>
    /// リールに対して動きの指示を与える
    /// </summary>
    public interface IReelOperation
    {
        /// <summary>
        /// リールの各役のGameObject
        /// </summary>
        IReadOnlyList<IRoleOperation> Roles { get; }
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
        private IReadOnlyList<IRoleOperation> _reelsCash;
        public IReadOnlyList<IRoleOperation> Roles
        {
            get
            {
                //子オブジェクト"Roles"の子オブジェクトのRoleをすべて取得
                //一度取得したらキャッシュしておく
                return _reelsCash ??
                    (_reelsCash = this.transform
                                      .Find("Roles")
                                      .transform
                                      .Cast<Transform>()
                                      .Select(transform => transform.GetComponent<IRoleOperation>())
                                      .ToList());
            }
        }
    }
}