using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class DITestInjectedComponent : MonoBehaviour
{
    [Inject]
    private IDITestInjectComponent injectComponent;

    // Start is called before the first frame update
    void Start()
    {
        injectComponent.Invoke();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
