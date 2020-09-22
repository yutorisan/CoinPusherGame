using System;
using UnityEngine;

namespace MedalPusher.Item
{
    public interface IReadOnlyMedal : IFieldObject
    {

    }
    public interface IMedal : IReadOnlyMedal
    {

    }
    public class Medal : MonoBehaviour, IMedal
    {

    }
}