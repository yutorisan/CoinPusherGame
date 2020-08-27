using System.Collections;
using System.Collections.Generic;
using MedalPusher.Medal;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MedalInputer))]
public class MedalInputerEditor : Editor
{
    int clones = 1;
    private MedalInputer medalInputer;

    private void OnEnable()
    {
        medalInputer = target as MedalInputer;
    }



    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var medals = EditorGUILayout.IntField("投入枚数", clones);
        clones = medals;
        if (GUILayout.Button("投入"))
        {
            medalInputer.GenerateMedal(medals, 5);
        }
    }
}
