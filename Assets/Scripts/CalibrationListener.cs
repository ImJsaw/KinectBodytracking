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
    public Transform model = null;
    public Transform modelLeftHand = null;
    public Transform modelRightHand = null;


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
        //Debug.Log("offset" + Vector3.Scale(pelvisOffset.localPosition, pelvisTkr.localScale).ToString());
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
        int leftCtrIndex = -1;
        int rightCtrIndex = -1;
        float minRightCtrDis = float.MaxValue;
        float minLeftCtrDis = float.MaxValue;
        for (int i = 0; i < trackers.Length; i++) {
            //skip index already check not open
            if (!isTkrValid[i])
                continue;
            //right ctr index
            float curDis = (trackers[i].transform.position - rightCtr.transform.position).sqrMagnitude;
            if (curDis < minRightCtrDis) {
                minRightCtrDis = curDis;
                rightCtrIndex = i;
            }
            //left ctr index
            curDis = (trackers[i].transform.position - leftCtr.transform.position).sqrMagnitude;
            if (curDis < minLeftCtrDis) {
                minLeftCtrDis = curDis;
                leftCtrIndex = i;
            }
        }
        isTkrValid[leftCtrIndex] = false;
        isTkrValid[rightCtrIndex] = false;
        Debug.Log("** remove controller **" + (leftCtrIndex+1) + ", " + (rightCtrIndex+1));
        logCurValidIndex();
        //remove lighthouse
        for (int lightHouseCount = 0; lightHouseCount < 2; lightHouseCount++) {
            int lightHouseIndex = -1;
            float maxVal = 0;
            for (int i = 0; i < trackers.Length; i++) {
                //skip index already check not open
                if (!isTkrValid[i])
                    continue;
                // the 2 most far from (0,0,0)
                float curLighthouseDis = trackers[i].transform.position.sqrMagnitude;
                if (maxVal < curLighthouseDis) {
                    maxVal = curLighthouseDis;
                    lightHouseIndex = i;
                }
            }
            isTkrValid[lightHouseIndex] = false;
            Debug.Log("** remove lighthouse " + (lightHouseIndex+1));
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
            msg += ((i+1) + " ");
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
            float minRightGoal = float.MaxValue;
            float minLeftGoal = float.MaxValue;
            float minRightFoot = float.MaxValue;
            float minLeftFoot = float.MaxValue;

            for (int i = 0; i < trackers.Length; i++) {
                //skip index already check not open
                if (!isTkrValid[i])
                    continue;
                //GetRightGoal index
                float curRightGoal = (trackers[i].transform.position - rightCtr.transform.position).sqrMagnitude;
                if (curRightGoal < minRightGoal) {
                    minRightGoal = curRightGoal;
                    rightGoalIndex = i;
                }
                //GetLeftGoal index
                float curLeftGoal = (trackers[i].transform.position - leftCtr.transform.position).sqrMagnitude;
                if (curLeftGoal < minLeftGoal) {
                    minLeftGoal = curLeftGoal;
                    leftGoalIndex = i;
                }
                //GetRightFoot index
                float curRightFoot = (trackers[i].transform.position - target[0].transform.position).sqrMagnitude;
                if (curRightFoot < minRightFoot) {
                    minRightFoot = curRightFoot;
                    rightFootIndex = i;
                }
                //GetLeftFoot index
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
            scaleModel();
            curState++;
        }
    }



    void setIndex() {
        //set index to tracker component 
        leftFootTkr.GetComponent<SteamVR_TrackedObject>().SetDeviceIndex(MainMgr.leftFootTkrIndex+1);
        rightFootTkr.GetComponent<SteamVR_TrackedObject>().SetDeviceIndex(MainMgr.rightFootTkrIndex+1);
        pelvisTkr.GetComponent<SteamVR_TrackedObject>().SetDeviceIndex(MainMgr.pelvisTkrIndex+1);
        //leftGoalTkr.GetComponent<SteamVR_TrackedObject>().SetDeviceIndex(MainMgr.leftGoalTkrIndex+1);
        //rightGoalTkr.GetComponent<SteamVR_TrackedObject>().SetDeviceIndex(MainMgr.rightGoalTkrIndex+1);
        //disable tracker balls
        for (int i = 0; i < trackers.Length; i++) {
            trackers[i].gameObject.SetActive(false);
        }
    }

    void checkInit() {
        if (m_InitAction.GetStateDown(m_Pose.inputSource)) {
            
            Debug.Log("trigger");
            MainMgr.inst.leftInitCtr[0].rot = leftCtr.rotation;
            MainMgr.inst.rightInitCtr[0].rot = rightCtr.rotation;
            MainMgr.inst.leftInitTkr[0].rot = leftFootTkr.rotation;
            MainMgr.inst.rightInitTkr[0].rot = rightFootTkr.rotation;
            //MainMgr.inst.pelvisInitTkr[0].pos = Vector3.Scale(pelvisOffset.localPosition, pelvisTkr.localScale);
            MainMgr.inst.pelvisInitTkr[0].rot = pelvisTkr.rotation;
            MainMgr.inst.handDist[0] = Vector3.Distance(leftCtr.position, rightCtr.position);
            curState++;
        }
    }

    void gotoGeneral() {
        Debug.Log("complete calibration");
        MainMgr.inst.changeScene(SceneID.General);
    }

    void scaleModel() {
        const float controllerLen = 0.1f;
        float scale = (Vector3.Distance(leftCtr.position, rightCtr.position) - controllerLen) / Vector3.Distance(modelLeftHand.position, modelRightHand.position);
        model.localScale = Vector3.Scale(model.localScale, new Vector3(scale, scale, scale));
    }


}