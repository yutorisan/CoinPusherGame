using System.Collections;
using System.Collections.Generic;
using MedalPusher.Item.Payout;
using MedalPusher.Medal;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MedalPayouter))]
public class MedalInputerEditor : Editor
{
    int clones = 1;
    private MedalPayouter payouter;

    private void OnEnable()
    {
        payouter = target as MedalPayouter;
    }



    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var medals = EditorGUILayout.IntField("投入枚数", clones);
        clones = medals;
        if (GUILayout.Button("投入"))
        {
            payouter.Payout(medals);
        }
    }
}
