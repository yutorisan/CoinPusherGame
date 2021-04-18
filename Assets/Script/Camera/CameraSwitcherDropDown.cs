using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityUtility;
using DG.Tweening;

namespace MedalPusher.Camera
{
    /// <summary>
    /// カメラ位置を変更する
    /// </summary>
    public class CameraSwitcherDropDown : MonoBehaviour
    {
        /// <summary>
        /// カメラ位置を選択するドロップダウン
        /// </summary>
        [SerializeField]
        private Dropdown m_cameraCombobox;

        // Start is called before the first frame update
        void Start()
        {
            //メインカメラを取得
            UnityEngine.Camera mainCamera = UnityEngine.Camera.main;

            //メインカメラのTransformのクローンを生成して子オブジェクトにまとめる
            Transform mainCameraTransform = new GameObject("MainCamera").transform;
            mainCameraTransform.parent = this.transform;
            mainCameraTransform.position = mainCamera.transform.position;
            mainCameraTransform.rotation = mainCamera.transform.rotation;
            mainCameraTransform.localScale = mainCamera.transform.localScale;

            //子オブジェクトのTransform（CameraSwitch対象のTransform）を列挙
            IReadOnlyList<Transform> cameraPoss = this.transform.Cast<Transform>().ToList();
            //DropDownの項目に追加
            m_cameraCombobox.options = cameraPoss.Select(tr => new Dropdown.OptionData(tr.name)).ToList();

            //DropDownのOnChangedイベントを設定
            var ddEvent = new Dropdown.DropdownEvent();
            //DropDownが変更されたら、そのTransformにカメラを移動させる
            ddEvent.AddListener(index => mainCamera.transform.DOMove(cameraPoss[index].position, 0.5f)
                                                             .SetEase(Ease.OutCirc));
            m_cameraCombobox.onValueChanged = ddEvent;

        }
    }
}