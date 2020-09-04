using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[CLSCompliant(false)]
public class Lock : MonoBehaviour {
    public Transform hand;
    public Vector3 min;
    public Vector3 max;

    private IKModelController m_model;
    private Quaternion lastValiableRotate = Quaternion.identity;

    public void Start()
    {
        m_model = this.GetComponent<IKModelController>();
    }

    public void LateUpdate() {
        //Utility.limRot(hand, min, max);
        /*if (Utility.testRotValid(hand, min, max)) {
            lastValiableRotate = hand.localRotation;
            //Debug.Log("valid" + lastValiableRotate.eulerAngles);
        } else {
            //Debug.Log("not valid" + hand.localRotation.eulerAngles);
            hand.localRotation = lastValiableRotate;
        }*/
        m_model.leftHand.transform.localRotation = Quaternion.identity;
        m_model.rightHand.transform.localRotation = Quaternion.identity;


    }
}
