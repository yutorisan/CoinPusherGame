using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LotteryPrize3DView : MonoBehaviour
{
    private Transform maincamera;

    // Start is called before the first frame update
    void Start()
    {
        maincamera = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.LookAt(maincamera);
    }
}
