using System;
using UnityEngine;
using Valve.VR;

[CLSCompliant(false)]
public class UserListener : ListenerBase {

    new void Start() {
        if (MainMgr.isVRValid) {
            VRroot = Instantiate(VrPrefab, new Vector3(0, 0, -6), Quaternion.identity);
        }
        base.Start();
        if (MainMgr.isVRValid) {

            leftFootTkr.GetComponent<SteamVR_TrackedObject>().SetDeviceIndex(MainMgr.leftFootTkrIndex + 1);
            rightFootTkr.GetComponent<SteamVR_TrackedObject>().SetDeviceIndex(MainMgr.rightFootTkrIndex + 1);
            pelvisTkr.GetComponent<SteamVR_TrackedObject>().SetDeviceIndex(MainMgr.pelvisTkrIndex + 1);
            leftGoalTkr.GetComponent<SteamVR_TrackedObject>().SetDeviceIndex(MainMgr.leftGoalTkrIndex + 1);
            rightGoalTkr.GetComponent<SteamVR_TrackedObject>().SetDeviceIndex(MainMgr.rightGoalTkrIndex + 1);
        }
        //tell other my stat
        sendRegister();
    }

    new void Update() {
        base.Update();
        //update goal from tracker
        if (!MainMgr.isVRValid) {
            //move func
            Vector3 movement = new Vector3(0, 0, 0);
            if (Input.GetKeyDown(KeyCode.W)) {
                movement += new Vector3(0, 0, 1);
            }
            if (Input.GetKeyDown(KeyCode.S)) {
                movement += new Vector3(0, 0, -1);
            }
            if (Input.GetKeyDown(KeyCode.A)) {
                movement += new Vector3(-1, 0, 0);
            }
            if (Input.GetKeyDown(KeyCode.D)) {
                movement += new Vector3(1, 0, 0);
            }
            if (Input.GetKeyDown(KeyCode.Space)) {
                movement += new Vector3(0, 1, 0);
            }
            if (Input.GetKeyDown(KeyCode.LeftShift)) {
                movement += new Vector3(0, -1, 0);
            }
            if (Input.GetKeyDown(KeyCode.Q))
            {
                curCam.transform.Rotate(0, 90, 0);
            }
            curCam.transform.position += movement;
        } else {
            if (leftGoalTkr == null)
                MainMgr.inst.leftArmGoal[0] = new SerializablePos(new Vector3(0, 0, 0));
            else
                MainMgr.inst.leftArmGoal[0] = new SerializablePos(leftGoalTkr.position);
            if (rightGoalTkr == null)
                MainMgr.inst.rightArmGoal[0] = new SerializablePos(new Vector3(0, 0, 0));
            else
                MainMgr.inst.rightArmGoal[0] = new SerializablePos(rightGoalTkr.position);
        }
        sendModel();
    }

    void sendModel() {
        Debug.Log("send my model");
        playerPose msg = new playerPose();
        msg.UID = MainMgr.inst.myUID();
        msg.modelType = MainMgr.inst.modelType[0];
        msg.headTransform = MainMgr.inst.headPos[0];
        //if (MainMgr.isVRValid) {
            msg.leftHandTransform = MainMgr.inst.leftCtr[0];
            msg.rightHandTransform = MainMgr.inst.rightCtr[0];
            msg.leftLegTransform = MainMgr.inst.leftTkr[0];
            msg.rightLegTransform = MainMgr.inst.rightTkr[0];
            msg.pelvisTransform = MainMgr.inst.pelvisTkr[0];
            msg.leftArmGoal = MainMgr.inst.leftArmGoal[0];
            msg.rightArmGoal = MainMgr.inst.rightArmGoal[0];
        //}
        //send from net
        byte[] modelDataBytes = Utility.Trans2byte(msg);
        NetMgr.sendMsg(packageType.playerPose, modelDataBytes);
    }

    public static void sendRegister() {
        Debug.Log("sendREG");
        register msg = new register();
        msg.UID = MainMgr.inst.myUID();
        msg.headInitTransform = MainMgr.inst.headPos[0];
        msg.hasVR = MainMgr.isVRValid;
        //if (MainMgr.isVRValid) {
            msg.leftHandInitTransform = MainMgr.inst.leftInitCtr[0];
            msg.rightHandInitTransform = MainMgr.inst.rightInitCtr[0];
            msg.leftLegInitTransform = MainMgr.inst.leftInitTkr[0];
            msg.rightLegInitTransform = MainMgr.inst.rightInitTkr[0];
            msg.pelvisInitTransform = MainMgr.inst.pelvisInitTkr[0];
            //hand dist for scale
            msg.handDist = MainMgr.inst.handDist[0];
            //model type
        //}
        msg.modelType = MainMgr.inst.modelType[0];

        //send from net
        byte[] registerDataByte = Utility.Trans2byte(msg);
        NetMgr.sendMsg(packageType.register, registerDataByte);
    }

}