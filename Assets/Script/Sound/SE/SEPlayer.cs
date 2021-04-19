using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MedalPusher.Sound
{
    /// <summary>
    /// SEを再生できる
    /// </summary>
    public interface ISEPlayer
    {
        /// <summary>
        /// SEを再生する
        /// </summary>
        void Play();
    }
    public class SEPlayer : MonoBehaviour, ISEPlayer
    {
        private AudioSource audioSource;

        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
        }

        public void Play() => audioSource.Play();
    }
}
