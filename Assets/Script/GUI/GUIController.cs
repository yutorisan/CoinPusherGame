using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using MedalPusher.Medal;

namespace MedalPusher.GUI
{
	public class GUIController : MonoBehaviour
	{
		[SerializeField]
		private MedalManager m_MedalMagaer;
		[SerializeField]
		private Text m_PutInText, m_GetText, m_FailedText;

		private IObservableMedalManager m_RefMedalManager;
		// Start is called before the first frame update
		void Start()
		{
			m_RefMedalManager = m_MedalMagaer;
			m_RefMedalManager.ObservablePutInMedalCount
							 .Subscribe(c => m_PutInText.text = "投入枚数：" + c + "枚");
			m_RefMedalManager.ObservableGetMedalCount
							 .Subscribe(c => m_GetText.text = "獲得枚数：" + c + "枚");
			m_RefMedalManager.ObservableFailedMedalCount
							 .Subscribe(c => m_FailedText.text = "回収枚数：" + c + "枚");
		}

		// Update is called once per frame
		void Update()
		{
        
		}
	}
}


