using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtility;
using UniRx;

namespace MedalPusher.Item.Payout
{
    public class ShowerMedalPayouter : MedalPayouter
    {
        // Start is called before the first frame update
        void Start()
        {
            var size = GetComponent<Collider>().bounds.size;

            PowerOnTiming.SelectMany(_ => Observable.Interval(System.TimeSpan.FromMilliseconds(10))
                                                    //ストックが0になるまで
                                                    .TakeUntil(PayoutStock.Where(n => n == 0))
                                                    //0になったらステータスをIdolにする
                                                    .DoOnCompleted(() => m_status = PayoutStatus.Idol))
                         .Select(_ => new Vector3(Random.Range(transform.position.x - size.x / 2,
                                                               transform.position.x + size.x / 2),
                                                  transform.position.y,
                                                  Random.Range(transform.position.z - size.z / 2,
                                                               transform.position.z + size.z / 2)))
                         //メダルを投入
                         .Subscribe(pos => PayoutToField(pos));
        }

    }
}