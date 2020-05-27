using System;
using UnityEngine;
using Valve.VR;

[CLSCompliant(false)]
public class CalibrationListener : ListenerBase {
    private enum calibrationState {
        initRot = 0,
        finish
    } 
    public SteamVR_Action_Boolean m_InitAction;
    public Transform pelvis;
    public Transform pelvisOffset;
    private SteamVR_Behaviour_Pose m_Pose = null;
    private calibrationState curState = calibrationState.initRot;

    public Transform VRRef = null;

    new void Start() {
        VRroot = VRRef.gameObject;
        base.Start();
        m_Pose = rightCtr.GetComponent<SteamVR_Behaviour_Pose>();
    }

    new void Update() {
        base.Update();
        switch (curState) {
            case calibrationState.initRot:
                checkInit();
                break;
            case calibrationState.finish:
                gotoGeneral();
                break;
            default:
                Debug.LogError("unknown state");
                break;
        }
        pelvisOffset.position = pelvis.position;
        Debug.Log("offset" + Vector3.Scale(pelvisOffset.localPosition, pelvisTkr.localScale).ToString());
    }

    void checkInit() {
        if (m_InitAction.GetStateDown(m_Pose.inputSource)) {
            Debug.Log("trigger");
            MainMgr.inst.leftInitCtr[0].rot = leftCtr.rotation;
            MainMgr.inst.rightInitCtr[0].rot = rightCtr.rotation;
            MainMgr.inst.leftInitTkr[0].rot = leftTkr.rotation;
            MainMgr.inst.rightInitTkr[0].rot = rightTkr.rotation;
            MainMgr.inst.pelvisInitTkr[0].pos = Vector3.Scale(pelvisOffset.localPosition, pelvisTkr.localScale);
            MainMgr.inst.pelvisInitTkr[0].rot = pelvisTkr.rotation;
            MainMgr.inst.handDist[0] = Vector3.Distance(leftCtr.position, rightCtr.position);
            curState++;
        }
    }

    void gotoGeneral() {
        Debug.Log("complete calibration");
        MainMgr.inst.changeScene(SceneID.General);
    }


}