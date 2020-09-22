using System;
using UnityEngine;

namespace MedalPusher.Item
{
    public interface IReadOnlyFieldItem : IFieldObject
    {

    }

    public interface IFieldItem : IReadOnlyFieldItem
    {

    }

    public class FieldItem : MonoBehaviour, IFieldItem
    {
        private IFieldItemEvent m_event;
    }
}