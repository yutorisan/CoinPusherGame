using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtility;
using UniRx;
using System;

namespace MedalPusher.Item.Payout
{
    /// <summary>
    /// フィールド上部からメダルを大量にシャワーのように払い出す
    /// </summary>
    public class ShowerMedalPayouter : MedalPayouter
    {
        /// <summary>
        /// 払い出しの間隔
        /// </summary>
        private TimeSpan payoutInterval = TimeSpan.FromMilliseconds(10);

        void Start()
        {
            //オブジェクトの大きさを保持する
            var size = GetComponent<Collider>().bounds.size;

            //払い出し要求を受けたらストックが無くなるまで払い出す
            PowerOnTiming.SelectMany(_ => Observable.Interval(payoutInterval)
                                                    //ストックが0になるまで
                                                    .TakeUntil(PayoutStock.Where(n => n == 0))
                                                    //0になったらステータスをIdolにする
                                                    .DoOnCompleted(() => status = PayoutStatus.Idol))
                         //このオブジェクトの内部のxz座標のランダムの位置にメダルを配置する
                         .Select(_ => new Vector3(UnityEngine.Random.Range(transform.position.x - size.x / 2,
                                                               transform.position.x + size.x / 2),
                                                  transform.position.y,
                                                  UnityEngine.Random.Range(transform.position.z - size.z / 2,
                                                               transform.position.z + size.z / 2)))
                         //メダルを投入
                         .Subscribe(pos => PayoutToField(pos, Quaternion.Euler(UnityEngine.Random.Range(-180f, 180f),
                                                                               UnityEngine.Random.Range(-180f, 180f),
                                                                               UnityEngine.Random.Range(-180f, 180f))));
        }

    }
}