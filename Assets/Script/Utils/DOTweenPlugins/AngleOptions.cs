using System;
using DG.Tweening.Plugins.Options;
using UnityEngine;

/// <summary>
/// プラグインのオプション本体
/// </summary>
public struct AngleOptions : IPlugOptions
{
    public void Reset()
    {
        Direction = AngleTweenDirection.Both;
        AngularVelocityUnit = AngularVelocityUnit.Unspecified;
    }

    public AngleTweenDirection Direction { get; set; }
    public AngularVelocityUnit AngularVelocityUnit { get; set; }
}

/// <summary>
/// Tweenする回転方向
/// </summary>
public enum AngleTweenDirection
{
    /// <summary>
    /// 両方
    /// </summary>
    Both,
    /// <summary>
    /// 正転のみ
    /// </summary>
    Forward,
    /// <summary>
    /// 逆転のみ
    /// </summary>
    Backward
}

/// <summary>
/// 角速度の単位
/// </summary>
public enum AngularVelocityUnit
{
    /// <summary>
    /// 未指定
    /// </summary>
    Unspecified,
    /// <summary>
    /// °/s
    /// </summary>
    DegreePerSecond,
    /// <summary>
    /// rad/s
    /// </summary>
    RadianPerSecond
}