﻿using UnityEngine;
using System.Collections;
using Zenject;
using TMPro;
using UniRx;

public class MedalInventoryView : MonoBehaviour
{
    [Inject]
    private IObservableMedalInventory m_medalInventory;
    [SerializeField]
    private TextMeshProUGUI m_target;

    // Use this for initialization
    void Start()
    {
        m_medalInventory.ObservableMedalInventoryCount
                        .Subscribe(medals => m_target.text = $"Medals:{medals}");
    }
}

