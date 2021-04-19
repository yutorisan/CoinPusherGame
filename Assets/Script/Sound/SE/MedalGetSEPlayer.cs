using System.Collections;
using System.Collections.Generic;
using MedalPusher.Item.Checker;
using Sirenix.OdinInspector;
using UnityEngine;
using UniRx;

namespace MedalPusher.Sound
{
    public class MedalGetSEPlayer : MonoBehaviour, ISEPlayer
    {
        /// <summary>
        /// SEを再生するタイミングとなる<see cref="MedalChecker"/>
        /// </summary>
        [SerializeField]
        private IObservableMedalChecker medalChecker;

        private AudioSource audioSource;

        void Start()
        {
            audioSource = GetComponent<AudioSource>();
        }

        public void Play()
        {
            throw new System.NotImplementedException();
        }
    }
}
