using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LotteryBowlRotater : MonoBehaviour
{
    [SerializeField]
    private float m_rotateSpeed = 1f;

    private Rigidbody _rigidbody;
    private float _rotateAngle = 0; 

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        //_rigidbody.centerOfMass = new Vector3(0, 0, 1000);
        
    }

    // Update is called once per frame
    void Update()
    {
        _rigidbody.MoveRotation(Quaternion.Euler(0, _rotateAngle += m_rotateSpeed, 0));
        //_rigidbody.rotation = Quaternion.Euler(0, ++_rotateAngle, 0);
    }
}
