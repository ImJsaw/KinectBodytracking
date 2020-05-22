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


    public void LateUpdate() {
        //Utility.limRot(hand, min, max);
        Utility.dropOutRot(hand, min, max);

        Debug.Log(hand.localRotation.eulerAngles.ToString());
    }
}
