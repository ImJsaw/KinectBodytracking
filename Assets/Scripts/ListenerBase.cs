﻿using System;
using UnityEngine;

[CLSCompliant(false)]
public class ListenerBase : MonoBehaviour {
    
    public GameObject VrPrefab = null;
    protected GameObject VRroot = null;

    private GameObject curCam = null;
    protected Transform leftCtr = null;
    protected Transform rightCtr = null;
    protected Transform leftTkr = null;
    protected Transform rightTkr = null;

    public void Start() {
        curCam = GameObject.FindWithTag("camera");
        getVRComp();
    }

    protected void getVRComp() {
        if (MainMgr.isVRValid) {
            //generate VR camera if vr valid
            leftCtr = VRroot.GetComponentInChildren<Transform>().Find("Controller (left)");
            rightCtr = VRroot.GetComponentInChildren<Transform>().Find("Controller (right)");
            leftTkr = VRroot.GetComponentInChildren<Transform>().Find("Tracker (left)");
            rightTkr = VRroot.GetComponentInChildren<Transform>().Find("Tracker (right)");
        }
    }

    public void Update() {
        //Debug.Log("update");
        if (!MainMgr.isVRValid) {
            Debug.LogError("NO VR !");
            return;
        }
        updatePosition();
    }

    protected void updatePosition() {
        Debug.Log("[CamPosTracker] update index" + 0 + " cam pos" + curCam.transform.position);
        MainMgr.inst.headPos[0] = new SerializableTransform(curCam.transform.position, curCam.transform.rotation);
        MainMgr.inst.leftCtr[0] = new SerializableTransform(leftCtr.position, leftCtr.rotation);
        MainMgr.inst.rightCtr[0] = new SerializableTransform(rightCtr.position, rightCtr.rotation);
        MainMgr.inst.leftTkr[0] = new SerializableTransform(leftTkr.position, leftTkr.rotation);
        MainMgr.inst.rightTkr[0] = new SerializableTransform(rightTkr.position, rightTkr.rotation);
    }

}