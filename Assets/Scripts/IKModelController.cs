using UnityEngine;
using System;
using Valve.VR;

[CLSCompliant(false)]
public class IKModelController : MonoBehaviour {
    //model index
    int _modelIndex;
    [HideInInspector]
    public int modelIndex {
        get {
            if (_modelIndex == -1)
                Debug.LogError("model number not set");
            return _modelIndex;
        }
        set {
            _modelIndex = value;
            if (_modelIndex < 0)
                Debug.LogError("model index < 0 !");
            if (_modelIndex == -1)
                Debug.LogError("model number not set");
            scaleByHand(MainMgr.inst.handDist[modelIndex]);
        }
    }

    public Transform pelvisPosition = null;
    //vr tracker
    private SerializableTransform hmt;
    private SerializableTransform rightCtr = null;
    private SerializableTransform leftCtr = null;
    private SerializableTransform rightTkr = null;
    private SerializableTransform leftTkr = null;
    private SerializableTransform pelvisTkr = null;
    //apply tracker pos to target
    public Transform rightHandTarget = null;
    public Transform leftHandTarget = null;
    public Transform rightHandGoal = null;
    public Transform leftHandGoal = null;
    public Transform rightLegTarget = null;
    public Transform leftLegTarget = null;
    public Transform headTarget = null;

    //tracker init rotation
    private Quaternion leftArmInitRot = Quaternion.identity;
    private Quaternion rightArmInitRot = Quaternion.identity;
    private Quaternion leftLegInitRot = Quaternion.identity;
    private Quaternion rightLegInitRot = Quaternion.identity;
    private Quaternion pelvisInitRot = Quaternion.identity;
    //target init
    private Quaternion leftArmTargetRot = Quaternion.identity;
    private Quaternion rightArmTargetRot = Quaternion.identity;
    private Quaternion leftLegTargetRot = Quaternion.identity;
    private Quaternion rightLegTargetRot = Quaternion.identity;

    //controller len
    float controllerLen = 0.1f;
    // initial pos


    void Start() {
        logTargetInitRotation();
    }

    void Update() {
        hmt = MainMgr.inst.headPos[modelIndex];
        leftCtr = MainMgr.inst.leftCtr[modelIndex];
        rightCtr = MainMgr.inst.rightCtr[modelIndex];
        leftTkr = MainMgr.inst.leftTkr[modelIndex];
        rightTkr = MainMgr.inst.rightTkr[modelIndex];
        pelvisTkr = MainMgr.inst.pelvisTkr[modelIndex];

        leftArmInitRot = MainMgr.inst.leftInitCtr[modelIndex].rot;
        rightArmInitRot = MainMgr.inst.rightInitCtr[modelIndex].rot;
        leftLegInitRot = MainMgr.inst.leftInitTkr[modelIndex].rot;
        rightLegInitRot = MainMgr.inst.rightInitTkr[modelIndex].rot;
        pelvisInitRot = MainMgr.inst.pelvisInitTkr[modelIndex].rot;

        updateModelTransform();
        //pelvis
        headTarget.position = hmt.pos ;
        headTarget.rotation = hmt.rot;
        headTarget.localPosition = headTarget.localPosition + Vector3.Scale(headTarget.forward, new Vector3(-0.2f, -0.2f, -0.2f));
        //arm
        leftHandTarget.position = leftCtr.pos ;
        rightHandTarget.position = rightCtr.pos ;
        leftHandTarget.rotation = leftCtr.rot * Quaternion.Inverse(leftArmInitRot) * leftArmTargetRot;
        rightHandTarget.rotation = rightCtr.rot * Quaternion.Inverse(rightArmInitRot) * rightArmTargetRot;
        //assist point from kinect
        if (leftHandGoal != null && MainMgr.inst.leftArmGoal[modelIndex].v3() != new Vector3(0, 0, 0))
            leftHandGoal.position = MainMgr.inst.leftArmGoal[modelIndex].v3();
        if (rightHandGoal != null && MainMgr.inst.rightArmGoal[modelIndex].v3() != new Vector3(0, 0, 0))
            rightHandGoal.position = MainMgr.inst.rightArmGoal[modelIndex].v3();
        //leg
        leftLegTarget.position = leftTkr.pos ;
        rightLegTarget.position = rightTkr.pos ;
        leftLegTarget.rotation = leftTkr.rot * Quaternion.Inverse(leftLegInitRot) * leftLegTargetRot;
        rightLegTarget.rotation = rightTkr.rot * Quaternion.Inverse(rightLegInitRot) * rightLegTargetRot;

    }

    private void logTargetInitRotation() {
        leftArmTargetRot = leftHandTarget.rotation;
        rightArmTargetRot = rightHandTarget.rotation;
        leftLegTargetRot = leftLegTarget.rotation;
        rightLegTargetRot = rightLegTarget.rotation;
    }

    private void updateModelTransform() {
        //make model horizon move with cam
        pelvisPosition.position = pelvisTkr.pos ;
        pelvisPosition.rotation = pelvisTkr.rot * Quaternion.Inverse(pelvisInitRot);
        pelvisPosition.localPosition += MainMgr.inst.pelvisInitTkr[modelIndex].pos;
        //offset to avoid cam in face problem
        //modelPosition.localPosition = modelPosition.localPosition + Vector3.Scale(modelPosition.forward, new Vector3(-0.1f, -0.1f, -0.1f));
    }

    //scale model to fit
    private void scaleByHand(float handDistance) {
        if (handDistance == -1)
            return;

        float modelHandDis = Vector3.Distance(rightHandTarget.position, leftHandTarget.position);
        float scale = (handDistance - controllerLen) / (modelHandDis - controllerLen);
        Debug.Log("scale model " + scale + " time to fit");
        transform.localScale = new Vector3(scale, scale, scale);
    }
}


