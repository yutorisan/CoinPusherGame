using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using MedalPusher.Slot;
using UnityUtility;

namespace MedalPusher.Medal
{
	public class MedalInputer : MonoBehaviour, IMedalChecker
	{
		private UnityEngine.Object m_MedalResource;
		private Subject<Unit> m_medalInputedSubject = new Subject<Unit>();

		public IObservable<Unit> ObservableMedalChecked => m_medalInputedSubject.AsObservable();

        private void Start() {
			//メダルの3Dモデル
			m_MedalResource = Resources.Load("Medal");

			//スペースキー押下を購読してメダル投下
			Observable.EveryUpdate()
					  .Where(_ => UnityEngine.Input.GetKeyDown(KeyCode.Space))
					  .Subscribe(_ => GenerateMedal());
			Observable.EveryUpdate()
					  .Where(_ => UnityEngine.Input.GetKeyDown(KeyCode.Return))
					  .Subscribe(_ => GenerateMedal(10, 5));


			////最初に1000枚投入
			//GenerateMedal(1000, 5);
		}

		public void GenerateMedal()
		{
			Instantiate(m_MedalResource, new Vector3(-0.125f, 2f, UnityEngine.Random.Range(-0.2f, 0.2f)), Quaternion.identity);
			m_medalInputedSubject.OnNext(Unit.Default);
		}
		public void GenerateMedal(int count, int interval)
		{
			Observable.Interval(TimeSpan.FromMilliseconds(interval))
					  .Take(count)
					  .Subscribe(_ => GenerateMedal())
					  .AddTo(this);
		}
	}

}

