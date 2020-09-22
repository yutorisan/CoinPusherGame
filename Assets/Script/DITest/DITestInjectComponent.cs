using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDITestInjectComponent
{
    void Invoke();
}

public class DITestInjectComponent : MonoBehaviour, IDITestInjectComponent
{
    public void Invoke()
    {
        Debug.Log("Invoked!!");
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
