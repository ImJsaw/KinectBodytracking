using System;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

[CLSCompliant(false)]
public class CalibrationListener : ListenerBase {
    private enum calibrationState {
        checkIndex = 0,
        initRot,
        finish
    }
    public SteamVR_TrackedObject[] trackers;
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
            case calibrationState.checkIndex:
                cleanIndex();
                checkParts();
                break;
            case calibrationState.initRot:
                setIndex();
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

    private bool[] isTkrValid; 
    //clean index (remove controller, hmt, lighthouse  & unvaliable tracker)
    void cleanIndex() {
        isTkrValid = new bool[trackers.Length];
        //remove unavaliable tracker
        for(int i = 0;i < trackers.Length; i++) {
            isTkrValid[i] = trackers[i].isValid;
        }
        //remove controller
        for (int i = 0; i < trackers.Length; i++) {
            //skip index already check not open
            if (!isTkrValid[i]) continue;

            if (trackers[i].transform.position == leftCtr.position)
                isTkrValid[i] = false;
            if (trackers[i].transform.position == rightCtr.position)
                isTkrValid[i] = false;

        }
        //remove lighthouse


    }

    void checkParts() {
        string msg = "";
        for (int i = 0; i < trackers.Length; i++) {
            //skip index already check not open
            if (!isTkrValid[i])
                continue;

            msg += (i + " ");

        }

        Debug.Log(msg + " is valid");
        //check goal,pelvis,foot tracker index

        //get left goal

        //get right goal

        //get left foot

        //get right foot

        //get pelvis

    }

    void setIndex() {
        //set index to tracker component 
        leftFootTkr.GetComponent<SteamVR_TrackedObject>().SetDeviceIndex(MainMgr.leftFootTkrIndex);
        rightFootTkr.GetComponent<SteamVR_TrackedObject>().SetDeviceIndex(MainMgr.rightFootTkrIndex);
        pelvisTkr.GetComponent<SteamVR_TrackedObject>().SetDeviceIndex(MainMgr.pelvisTkrIndex);
        leftGoalTkr.GetComponent<SteamVR_TrackedObject>().SetDeviceIndex(MainMgr.leftGoalTkrIndex);
        rightGoalTkr.GetComponent<SteamVR_TrackedObject>().SetDeviceIndex(MainMgr.rightGoalTkrIndex);
    }

    void checkInit() {
        if (m_InitAction.GetStateDown(m_Pose.inputSource)) {
            Debug.Log("trigger");
            MainMgr.inst.leftInitCtr[0].rot = leftCtr.rotation;
            MainMgr.inst.rightInitCtr[0].rot = rightCtr.rotation;
            MainMgr.inst.leftInitTkr[0].rot = leftFootTkr.rotation;
            MainMgr.inst.rightInitTkr[0].rot = rightFootTkr.rotation;
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