﻿using System.Collections;
using System.Collections.Generic;
using MedalPusher.Item.Payout;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NormalMedalPayouter))]
public class MedalInputerEditor : Editor
{
    int clones = 1;
    private NormalMedalPayouter payouter;

    private void OnEnable()
    {
        payouter = target as NormalMedalPayouter;
    }



    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var medals = EditorGUILayout.IntField("投入枚数", clones);
        clones = medals;
        if (GUILayout.Button("投入"))
        {
            payouter.AddPayoutStock(medals);
        }
    }
}
