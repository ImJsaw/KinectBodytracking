﻿using UnityEngine;
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
        }
    }
    public Transform modelPosition = null;
    //vr tracker
    private SerializableTransform hmt;
    private SerializableTransform rightCtr = null;
    private SerializableTransform leftCtr = null;
    private SerializableTransform rightTkr = null;
    private SerializableTransform leftTkr = null;
    //apply tracker pos to target
    public Transform rightHandTarget = null;
    public Transform leftHandTarget = null;
    public Transform rightLegTarget = null;
    public Transform leftLegTarget = null;
    //init rotation
    private Quaternion leftArmInitRot = Quaternion.identity;
    private Quaternion rightArmInitRot = Quaternion.identity;
    private Quaternion leftLegInitRot = Quaternion.identity;
    private Quaternion rightLegInitRot = Quaternion.identity;
    private Quaternion pelvisInitRot = Quaternion.identity;

    void Start() {
        pelvisInitRot = transform.rotation;
        updateInitRotation();
    }

    void Update() {
        hmt = MainMgr.inst.headPos[modelIndex];
        leftCtr = MainMgr.inst.leftCtr[modelIndex];
        rightCtr = MainMgr.inst.rightCtr[modelIndex];
        leftTkr = MainMgr.inst.leftTkr[modelIndex];
        rightTkr = MainMgr.inst.rightTkr[modelIndex];

        updateInitRotation();

        updateModelTransform();
        //arm
        leftHandTarget.position = leftCtr.pos;
        rightHandTarget.position = rightCtr.pos;
        leftHandTarget.rotation = leftCtr.rot * Quaternion.Inverse(leftArmInitRot) * Quaternion.Inverse(pelvisInitRot);
        rightHandTarget.rotation = rightCtr.rot * Quaternion.Inverse(rightArmInitRot) * Quaternion.Inverse(pelvisInitRot);
        //leg
        leftLegTarget.position = leftTkr.pos;
        rightLegTarget.position = rightTkr.pos;
        leftLegTarget.rotation = leftTkr.rot * Quaternion.Inverse(leftLegInitRot);
        rightLegTarget.rotation = rightTkr.rot * Quaternion.Inverse(rightLegInitRot);

    }

    private void updateInitRotation() {
        leftArmInitRot = leftCtr.rot;
        rightArmInitRot = rightCtr.rot;
        leftLegInitRot = leftTkr.rot;
        rightLegInitRot = rightTkr.rot;

    }

    private void updateModelTransform() {
        //make model horizon move with cam
        modelPosition.position = new Vector3(hmt.pos.x, modelPosition.position.y, hmt.pos.z);
        modelPosition.rotation = Quaternion.Euler(0, hmt.rot.y, 0);
    }
}


