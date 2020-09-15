using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Zenject;

public class DITestComponent2 : MonoBehaviour
{
    [Inject]
    private IHelloWorlder helloWorlder;

    // Start is called before the first frame update
    void Start()
    {
        Observable.Interval(TimeSpan.FromSeconds(1.5))
                  .Subscribe(_ => helloWorlder.HelloWorld("Component2"));    
    }

}
