using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace MedalPusher.Slot
{
	public interface ISlot
	{
		void Roll();
		IObservable<SlotStatus> ObservableSlotStatus { get; }
	}

	public class Slot : MonoBehaviour, ISlot
	{
		[SerializeField]
		private TextMesh m_Slot3DText;

		private ReactiveProperty<SlotStatus> m_Status = new ReactiveProperty<SlotStatus>();

		private ReactiveProperty<string> m_s1 = new ReactiveProperty<string>();
		private ReactiveProperty<string> m_s2 = new ReactiveProperty<string>();
		private ReactiveProperty<string> m_s3 = new ReactiveProperty<string>();

		private static string[] rollItems = new string[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0" };
		private static IObservable<int> ObservableRollIndex = Observable.Interval(TimeSpan.FromMilliseconds(100))
																		.Select(_ => UnityEngine.Random.Range(0, rollItems.Length));

		public void Roll()
		{
			ObservableRollIndex.Take(50).Subscribe(i => m_s1.Value = rollItems[i]);
			ObservableRollIndex.Take(50).Subscribe(i => m_s2.Value = rollItems[i]);
			ObservableRollIndex.Take(50).Subscribe(i => m_s3.Value = rollItems[i]);
		}

		public IObservable<SlotStatus> ObservableSlotStatus => m_Status;

		// Start is called before the first frame update
		void Start()
		{
            m_s1.Subscribe(_ => UpdateSlotText());
            m_s2.Subscribe(_ => UpdateSlotText());
			m_s3.Subscribe(_ => UpdateSlotText());
		}

		private void UpdateSlotText()
		{
			m_Slot3DText.text = m_s1.Value + m_s2.Value + m_s3.Value;
		}

	}

	public enum SlotStatus
	{
		Idol,
		Rolling
	}


}

