using System;
using UnityEngine;

namespace MedalPusher.Item
{

    public interface IFieldItem : IFieldObject
    {
        //未実装
    }

    public class FieldItem : MonoBehaviour, IFieldItem
    {
        private IFieldItemEvent m_event;

        //未実装
    }
}