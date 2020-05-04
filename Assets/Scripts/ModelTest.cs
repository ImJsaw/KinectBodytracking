using UnityEngine;
using System;
using Microsoft.Azure.Kinect.Sensor.BodyTracking;
using Joint = Microsoft.Azure.Kinect.Sensor.BodyTracking.Joint;

[CLSCompliant(false)]
public class ModelTest : MonoBehaviour {

    //test mode
    public bool demoMode = true;
    public Transform modelPosition = null;
    public Transform helmetPosition = null;
    //vr tracker
    public Transform leftCtr = null;
    public Transform rightCtr = null;
    public Transform leftTkr = null;
    public Transform rightTkr = null;
    //apply tracker pos to target
    public Transform leftHandTarget = null;
    public Transform rightHandTarget = null;
    public Transform leftLegTarget = null;
    public Transform rightLegTarget = null;


    private Vector3 hmtPos;

    private void updateModelTransform() {

        //make model horizon move with cam
        modelPosition.position = new Vector3(hmtPos.x, modelPosition.position.y, hmtPos.z);
    }

    void Update() {
        if(helmetPosition != null) {
            //position
            hmtPos = helmetPosition.position;
        }

        leftHandTarget.position = leftCtr.position;
        rightHandTarget.position = rightCtr.position;
        leftLegTarget.position = leftTkr.position;
        rightLegTarget.position = rightTkr.position;

        updateModelTransform();
       
    }
    
}


