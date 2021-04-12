using System;
using System.Linq;
using MedalPusher.Item.Payout;
using UnityEditor;
using UnityEngine;
using UnityUtility;

[CustomEditor(typeof(MedalPayouterFacade))]
public class MedalPayouterStorageEditor : Editor
{
    private MedalPayouterFacade _payouter;
    private int clones = 50;
    private int index = 0;

    private void OnEnable()
    {
        _payouter = target as MedalPayouterFacade;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        //投入するメダル数
        int inputMedals;
        //投入方式
        MedalPayoutMethod method;

        //EditorGUILayout.DropdownButton()
        using(new EditorGUILayout.HorizontalScope())
        {
            MedalPayoutMethod[] methods = UnityUtility.Enum.All<MedalPayoutMethod>().ToArray();

            index = EditorGUILayout.Popup(index, methods.Select(m => m.ToString()).ToArray());
            method = methods[index];

            inputMedals = EditorGUILayout.IntField("投入枚数", clones);
            clones = inputMedals;
        }
        if (GUILayout.Button("投入"))
        {
            _payouter.PayoutRequest(inputMedals, method);
        }
    }
}
