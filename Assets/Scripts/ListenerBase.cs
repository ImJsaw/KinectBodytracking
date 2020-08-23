using System;
using UnityEngine;

[CLSCompliant(false)]
public class ListenerBase : MonoBehaviour {
    
    public GameObject VrPrefab = null;
    protected GameObject VRroot = null;

    protected GameObject curCam = null;
    protected Transform leftCtr = null;
    protected Transform rightCtr = null;
    protected Transform leftFootTkr = null;
    protected Transform rightFootTkr = null;
    protected Transform pelvisTkr = null;
    protected Transform leftGoalTkr = null;
    protected Transform rightGoalTkr = null;

    public void Start() {
        curCam = GameObject.FindWithTag("MainCamera");
        getVRComp();
    }

    protected void getVRComp() {
        if (MainMgr.isVRValid) {
            curCam = GameObject.FindWithTag("camera");
            //generate VR camera if vr valid
            leftCtr = VRroot.GetComponentInChildren<Transform>().Find("Controller (left)");
            rightCtr = VRroot.GetComponentInChildren<Transform>().Find("Controller (right)");
            leftFootTkr = VRroot.GetComponentInChildren<Transform>().Find("Tracker (left)");
            rightFootTkr = VRroot.GetComponentInChildren<Transform>().Find("Tracker (right)");
            pelvisTkr = VRroot.GetComponentInChildren<Transform>().Find("Tracker (pelvis)");
            leftGoalTkr = VRroot.GetComponentInChildren<Transform>().Find("Tracker (leftGoal)");
            rightGoalTkr = VRroot.GetComponentInChildren<Transform>().Find("Tracker (rightGoal)");
        }
    }

    public void Update() {
        MainMgr.inst.headPos[0] = new SerializableTransform(curCam.transform.position, curCam.transform.rotation);
        GC.Collect();
        if (MainMgr.isVRValid) {
            updatePosition();
            //return;
        } else {
            Debug.Log("NO VR !");
        }
    }

    protected void updatePosition() {
        //Debug.Log("[CamPosTracker] update index" + 0 + " cam pos" + curCam.transform.position);
        MainMgr.inst.leftCtr[0] = new SerializableTransform(leftCtr.position, leftCtr.rotation);
        MainMgr.inst.rightCtr[0] = new SerializableTransform(rightCtr.position, rightCtr.rotation);
        MainMgr.inst.leftTkr[0] = new SerializableTransform(leftFootTkr.position, leftFootTkr.rotation);
        MainMgr.inst.rightTkr[0] = new SerializableTransform(rightFootTkr.position, rightFootTkr.rotation);
        MainMgr.inst.pelvisTkr[0] = new SerializableTransform(pelvisTkr.position, pelvisTkr.rotation);
        MainMgr.inst.rightArmGoal[0] = new SerializablePos(rightGoalTkr.position);
        MainMgr.inst.leftArmGoal[0] = new SerializablePos(leftGoalTkr.position);
    }

}