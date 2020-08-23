using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace MedalPusher.Medal
{
    //public interface IMedal
    //{
    //	IObservable<Unit> ObservableCheckerPassed { get; }
    //	IObservable<Unit> ObservableFailedCheckerPassed { get; }
    //	IObservable<Unit> ObservableSlotCheckerPassed { get; }
    //}

    public class Medal : MonoBehaviour
    {
        //	// Start is called before the first frame update
        //	void Start()
        //	{

        //	}

        //	// Update is called once per frame
        //	void Update()
        //	{

        //	}

        //	private void OnTriggerEnter(Collider other)
        //	{
        //		switch(other.gameObject.tag)
        //		{
        //			case "GetMedalChecker":
        //				m_CheckerPassedStream.OnNext(Unit.Default);
        //				Destroy(this.gameObject);
        //				break;
        //			case "FailedMedalChecker":
        //				m_FailedCheckerPassedStream.OnNext(Unit.Default);
        //				Destroy(this.gameObject);
        //				break;
        //			case "SlotChecker":
        //				m_SlotCheckerPassedStream.OnNext(Unit.Default);
        //				break;
        //			default:
        //				break;
        //		}
        //	}

        //	public IObservable<Unit> ObservableCheckerPassed => m_CheckerPassedStream;
        //	public IObservable<Unit> ObservableFailedCheckerPassed => m_FailedCheckerPassedStream;
        //	public IObservable<Unit> ObservableSlotCheckerPassed => m_SlotCheckerPassedStream;

        //	private Subject<Unit> m_CheckerPassedStream = new Subject<Unit>();
        //	private Subject<Unit> m_FailedCheckerPassedStream = new Subject<Unit>();
        //	private Subject<Unit> m_SlotCheckerPassedStream = new Subject<Unit>();

    }

}

