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

    private Quaternion lastValiableRotate = Quaternion.identity;

    public void LateUpdate() {
        //Utility.limRot(hand, min, max);
        if (Utility.testRotValid(hand, min, max)) {
            lastValiableRotate = hand.localRotation;
            //Debug.Log("valid" + lastValiableRotate.eulerAngles);
        } else {
            //Debug.Log("not valid" + hand.localRotation.eulerAngles);
            hand.localRotation = lastValiableRotate;
        }

        
    }
}
