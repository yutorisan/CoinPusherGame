using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using UniRx;
using System;

public class DITestComponent : MonoBehaviour
{
    [Inject]
    private IHelloWorlder helloWorlder;

    // Start is called before the first frame update
    void Start()
    {
        Observable.Interval(TimeSpan.FromSeconds(1))
                  .Subscribe(_ => helloWorlder.HelloWorld("Component1"));
    }
}

interface IHelloWorlder
{
    void HelloWorld(string name);
}
class HelloWorlder : IHelloWorlder
{
    public HelloWorlder(string param)
    {
        Debug.Log("コンストラクタ！引数：" + param);
    }
    public void HelloWorld(string name)
    {
        Debug.Log($"{name} : HelloWorld!");
    }
}