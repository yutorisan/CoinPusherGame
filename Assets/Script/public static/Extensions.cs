using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 拡張メソッド群
/// </summary>
public static class Extensions
{
	public static double ToRadian(this double degree) => degree * Mathf.PI / 180;
	public static double ToDegree(this double radian) => radian * 180 / Mathf.PI;

    public static Transform GetChildTransforms(this MonoBehaviour mono) => mono.gameObject.transform;
    public static Transform GetChildTransform(this MonoBehaviour mono, string name) => GetChildTransforms(mono).Find(name);
    public static GameObject GetChildGameObject(this MonoBehaviour mono, string name) => GetChildTransform(mono, name).gameObject;
}
