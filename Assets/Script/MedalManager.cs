using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using MedalPusher.Slot;

namespace MedalPusher
{
	public interface IObservableMedalManager
	{
		/// <summary>
		/// 獲得したメダル枚数を購読します。
		/// </summary>
		IObservable<int> ObservableGetMedalCount { get; }
		/// <summary>
		/// 獲得できず回収されたメダル枚数を購読します。
		/// </summary>
		IObservable<int> ObservableFailedMedalCount { get; }
		/// <summary>
		/// 投入したメダル枚数を購読します。
		/// </summary>
		IObservable<int> ObservablePutInMedalCount { get; }
	}
	public class MedalManager : MonoBehaviour, IObservableMedalManager
	{
		/// <summary>
		/// 投入したメダル枚数
		/// </summary>
		private ReactiveProperty<int> mPutInMedalCount = new ReactiveProperty<int>();
		/// <summary>
		/// 獲得したメダル枚数
		/// </summary>
		private ReactiveProperty<int> mGetMedalCount = new ReactiveProperty<int>();
		/// <summary>
		/// 獲得できず回収されたメダル枚数
		/// </summary>
		private ReactiveProperty<int> mFailedMedalCount = new ReactiveProperty<int>();

		private ReactiveCollection<IMedal> mMedals = new ReactiveCollection<IMedal>();
		private Medal mMedalResource;

		private void Start() {
			//メダルの3Dモデル
			mMedalResource = (Resources.Load("Medal") as GameObject).GetComponent<Medal>();
			ISlotController slotController = GameObject.Find("SlotController").GetComponent<ISlotController>();

			//メダルが追加されたら、そのメダルの行方を購読
			mMedals.ObserveAdd().Subscribe(addMedal =>
			{
				++mPutInMedalCount.Value;
				addMedal.Value.ObservableCheckerPassed.Subscribe(_ => ++mGetMedalCount.Value).AddTo(this);
				addMedal.Value.ObservableFailedCheckerPassed.Subscribe(_ => ++mFailedMedalCount.Value).AddTo(this);
				addMedal.Value.ObservableSlotCheckerPassed.Subscribe(_ => slotController.AddStock()).AddTo(this);
			});

			//スペースキー押下を購読してメダル投下
			Observable.EveryUpdate()
					  .Where(_ => Input.GetKeyDown(KeyCode.Space))
					  .Subscribe(_ => GenerateMedal());
			Observable.EveryUpdate()
					  .Where(_ => Input.GetKeyDown(KeyCode.Return))
					  .Subscribe(_ => GenerateMedal(100, 10));


			//最初に1000枚投入
			GenerateMedal(1000, 5);
		}

		private void Update() {
		
		}

		private void GenerateMedal()
		{
			IMedal medal = Instantiate(mMedalResource, new Vector3(-0.125f, 2f, UnityEngine.Random.Range(-0.4f, 0.4f)), Quaternion.identity);
			mMedals.Add(medal);
		}
		private void GenerateMedal(int count, int interval)
		{
			Observable.Interval(TimeSpan.FromMilliseconds(interval))
					  .Take(count)
					  .Subscribe(_ => GenerateMedal())
					  .AddTo(this);
		}

		public IObservable<int> ObservableGetMedalCount => mGetMedalCount;

		public IObservable<int> ObservableFailedMedalCount => mFailedMedalCount;

		public IObservable<int> ObservablePutInMedalCount => mPutInMedalCount;
	}

}

