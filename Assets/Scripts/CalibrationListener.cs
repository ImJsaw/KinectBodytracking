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

    public GameObject[] target;
    public SteamVR_TrackedObject[] trackers;
    public SteamVR_Action_Boolean m_InitAction;
    public Transform pelvis;
    public Transform pelvisOffset;
    private SteamVR_Behaviour_Pose m_Pose = null;
    private calibrationState curState = calibrationState.checkIndex;

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
        for (int i = 0; i < trackers.Length; i++) {
            isTkrValid[i] = trackers[i].isValid;
        }
        Debug.Log("**clean not open index**");
        logCurValidIndex();
        //remove controller
        for (int i = 0; i < trackers.Length; i++) {
            //skip index already check not open
            if (!isTkrValid[i])
                continue;

            if (trackers[i].transform.position == leftCtr.position)
                isTkrValid[i] = false;
            if (trackers[i].transform.position == rightCtr.position)
                isTkrValid[i] = false;

        }

        Debug.Log("** remove controller **");
        logCurValidIndex();

        //remove lighthouse
        //TODO:
        for (int lightHouseCount = 0; lightHouseCount < 2; lightHouseCount++) {

            int lightHouseIndex = -1;
            for (int i = 0; i < trackers.Length; i++) {
                //skip index already check not open
                if (!isTkrValid[i])
                    continue;

                // the 2 most far from (0,0,0)
                float maxVal = 0;
                float curLighthouseDis = trackers[i].transform.position.sqrMagnitude;
                if (maxVal < curLighthouseDis) {
                    maxVal = curLighthouseDis;
                    lightHouseIndex = i;
                }

            }
            isTkrValid[lightHouseIndex] = false;
        }

        Debug.Log("** remove lighthouse **");
        logCurValidIndex();
    }

    void logCurValidIndex() {
        string msg = "";
        for (int i = 0; i < trackers.Length; i++) {
            //skip index already check not open
            if (!isTkrValid[i])
                continue;

            msg += (i + " ");

        }
        Debug.Log(msg + " is valid");
    }

    void checkParts() {
        //check goal,pelvis,foot tracker index

        int rightGoalIndex = -1;
        int leftGoalIndex = -1;
        int rightFootIndex = -1;
        int leftFootIndex = -1;
        int pelvisIndex = -1;


        if (m_InitAction.GetStateDown(m_Pose.inputSource)) {
            for (int i = 0; i < trackers.Length; i++) {
                //skip index already check not open
                if (!isTkrValid[i])
                    continue;
                //GetRightGoal index
                float minRightGoal = float.MaxValue;
                float curRightGoal = (trackers[i].transform.position - rightCtr.transform.position).sqrMagnitude;
                if (curRightGoal < minRightGoal) {
                    minRightGoal = curRightGoal;
                    rightGoalIndex = i;
                }

                //GetLeftGoal index
                float minLeftGoal = float.MaxValue;
                float curLeftGoal = (trackers[i].transform.position - leftCtr.transform.position).sqrMagnitude;
                if (curLeftGoal < minLeftGoal) {
                    minLeftGoal = curLeftGoal;
                    leftGoalIndex = i;
                }

                //GetRightFoot index
                float minRightFoot = float.MaxValue;
                float curRightFoot = (trackers[i].transform.position - target[0].transform.position).sqrMagnitude;
                if (curRightFoot < minRightFoot) {
                    minRightFoot = curRightFoot;
                    rightFootIndex = i;
                }

                //GetLeftFoot index
                float minLeftFoot = float.MaxValue;
                float curLeftFoot = (trackers[i].transform.position - target[1].transform.position).sqrMagnitude;
                if (curLeftFoot < minLeftFoot) {
                    minLeftFoot = curLeftFoot;
                    leftFootIndex = i;
                }

            }

            //get pelvis
            for (int i = 0; i < trackers.Length; i++) {
                //skip index already check not open
                if (!isTkrValid[i])
                    continue;

                //the only one which did not get index
                if (i != rightGoalIndex && i != leftGoalIndex && i != rightFootIndex && i != leftFootIndex) {
                    pelvisIndex = i;
                }

            }

            MainMgr.leftFootTkrIndex = leftFootIndex;
            MainMgr.rightFootTkrIndex = rightFootIndex;
            MainMgr.leftGoalTkrIndex = leftGoalIndex;
            MainMgr.rightGoalTkrIndex = rightGoalIndex;
            MainMgr.pelvisTkrIndex = pelvisIndex;

            curState++;
        }
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