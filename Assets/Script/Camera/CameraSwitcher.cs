using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityUtility;
using UnityEngine.Events;
using DG.Tweening;
using Zenject;

public class CameraSwitcher : MonoBehaviour
{
    [SerializeField]
    private Dropdown m_cameraCombobox;

    // Start is called before the first frame update
    void Start()
    {
        //メインカメラを取得
        Camera mainCamera = Camera.main;
        //メインカメラのTransformのクローンを生成
        GameObject cameraCloneGO = new GameObject("MainCamera");
        cameraCloneGO.transform.position = mainCamera.transform.position;
        cameraCloneGO.transform.rotation = mainCamera.transform.rotation;
        cameraCloneGO.transform.localScale = mainCamera.transform.localScale;
        //メインカメラと、子オブジェクトのTransformを列挙
        List<Transform> cameraPoss = new List<Transform>() { cameraCloneGO.transform };
        cameraPoss.AddRange(this.transform.Cast<Transform>());
        //DropDownの項目に追加
        m_cameraCombobox.options = cameraPoss.Select(tr => new Dropdown.OptionData(tr.name)).ToList();

        //DropDownのOnChangedイベントを設定
        var ddEvent = new Dropdown.DropdownEvent();
        ddEvent.AddListener(index => mainCamera.transform.DOMove(cameraPoss[index].position, 0.5f)
                                                         .SetEase(Ease.OutCirc));
        m_cameraCombobox.onValueChanged = ddEvent;

    }
}
