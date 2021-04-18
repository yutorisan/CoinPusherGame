using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MedalPusher.Item.Payout
{
    public class FieldItemPayouter : MonoBehaviour, IFieldItemPayouter
    {
        public void Payout(FieldItem item)
        {
            Instantiate(item, transform);
        }
    }
}
