using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace MedalPusher.Slot
{
    /// <summary>
    /// 各リールの動きを制御することができる
    /// </summary>
    public interface IReelController
    {

    }
    /// <summary>
    /// 各リールの動きを制御する
    /// </summary>
    public class ReelController : MonoBehaviour, IReelController
    {
        [Inject]
        private IReelOperation _reelOperation;

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