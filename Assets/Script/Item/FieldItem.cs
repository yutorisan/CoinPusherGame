using System;
using UnityEngine;

namespace MedalPusher.Item
{

    public interface IFieldItem : IFieldObject
    {

    }

    public class FieldItem : MonoBehaviour, IFieldItem
    {
        private IFieldItemEvent m_event;
    }
}