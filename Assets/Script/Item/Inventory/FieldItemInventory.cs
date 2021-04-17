using System.Collections;
using System.Collections.Generic;
using MedalPusher.Item.Checker;
using UnityEngine;
using UniRx;
using Sirenix.OdinInspector;

namespace MedalPusher.Item.Inventory
{
    /// <summary>
    /// 所持しているアイテムを管理する
    /// </summary>
    public class FieldItemInventory : SerializedMonoBehaviour
    {
        [SerializeField]
        IObservableFieldItemChecker itemChecker;
        // Start is called before the first frame update
        void Start()
        {
            //現段階では、アイテムを獲得したら即座に使う
            itemChecker.Checked.Subscribe(item => item.Use());
        }
    }
}
