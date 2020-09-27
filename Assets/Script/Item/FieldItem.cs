using System;
using UnityEngine;

namespace MedalPusher.Item
{
    public interface IReadOnlyFieldItem
    {

    }

    public interface IFieldItem : IReadOnlyFieldItem, IFieldObject
    {

    }

    public class FieldItem : MonoBehaviour, IFieldItem
    {
        private IFieldItemEvent m_event;
    }
}