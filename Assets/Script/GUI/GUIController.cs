using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using MedalPusher.Medal;
using UnityUtility;
using System;

namespace MedalPusher.GUI
{
	public class GUIController : MonoBehaviour
	{
		[SerializeField]
		private SerializableIMedalCounter m_medalCounter;
		[SerializeField]
		private Text m_inputMedalText, m_winMedalText, m_failedMedalText;

		// Start is called before the first frame update
		void Start()
		{
			m_medalCounter.Interface.ObservableInputMedalCount.SubscribeToText(m_inputMedalText, c => $"投入枚数：{c}枚");
			m_medalCounter.Interface.ObservableWonMedalCount.SubscribeToText(m_winMedalText, c => $"獲得枚数：{c}枚");
			m_medalCounter.Interface.ObservableFailedMedalCount.SubscribeToText(m_failedMedalText, c => $"回収枚数：{c}枚");
		}

		[Serializable]
		private class SerializableIMedalCounter : SerializeInterface<IObservableMedalCounter> { }
	}
}


