using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace MedalPusher.Slot.Internal.Stock
{
    internal interface IIndicator
    {
        void Indicate(Sprite sprite);
    }
    public class Indicator : MonoBehaviour, IIndicator
    {
        private SpriteRenderer spriteRenderer;

        // Start is called before the first frame update
        void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void Indicate(Sprite sprite)
        {
            spriteRenderer.sprite = sprite;
        }
    }
}