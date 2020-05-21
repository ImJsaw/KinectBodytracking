using System;
using UnityEngine;

[CLSCompliant(false)]
public class UserListener : ListenerBase {

    protected Transform leftGoal = null;
    protected Transform rightGoal = null;

    new void Start() {
        if (MainMgr.isVRValid) {
            VRroot = Instantiate(VrPrefab, new Vector3(0, 0, -6), Quaternion.identity);
            leftGoal = VRroot.GetComponentInChildren<Transform>().Find("Tracker (leftGoal)");
            rightGoal = VRroot.GetComponentInChildren<Transform>().Find("Tracker (rightGoal)");
        }
        base.Start();
        //tell other my stat
        sendRegister();
    }

    new void Update() {
        base.Update();
        //update goal from tracker
        MainMgr.inst.leftArmGoal[0] = new SerializablePos(leftGoal.position);
        MainMgr.inst.rightArmGoal[0] = new SerializablePos(rightGoal.position);
        if (!MainMgr.isVRValid) {
            //move func
            Vector3 movement = new Vector3(0, 0, 0);
            if (Input.GetKeyDown(KeyCode.W)) {
                movement += new Vector3(0, 0, 1);
            }
            if (Input.GetKeyDown(KeyCode.S)) {
                movement += new Vector3(0, 0, -11);
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

            curCam.transform.position += movement;

        }
        sendModel();
    }

    void sendModel() {
        Debug.Log("send my model");
        playerPose msg = new playerPose();
        msg.UID = MainMgr.inst.myUID();
        msg.headTransform = MainMgr.inst.headPos[0];
        if (MainMgr.isVRValid) {
            msg.leftHandTransform = MainMgr.inst.leftCtr[0];
            msg.rightHandTransform = MainMgr.inst.rightCtr[0];
            msg.leftLegTransform = MainMgr.inst.leftTkr[0];
            msg.rightLegTransform = MainMgr.inst.rightTkr[0];
            msg.pelvisTransform = MainMgr.inst.pelvisTkr[0];
        }
        //send from net
        byte[] modelDataBytes = Utility.Trans2byte(msg);
        NetMgr.sendMsg(packageType.playerPose, modelDataBytes);
    }

    void sendRegister() {
        register msg = new register();
        msg.UID = MainMgr.inst.myUID();
        msg.headInitTransform = MainMgr.inst.headPos[0];
        if (MainMgr.isVRValid) {
            msg.leftHandInitTransform = MainMgr.inst.leftInitCtr[0];
            msg.rightHandInitTransform = MainMgr.inst.rightInitCtr[0];
            msg.leftLegInitTransform = MainMgr.inst.leftInitTkr[0];
            msg.rightLegInitTransform = MainMgr.inst.rightInitTkr[0];
            msg.pelvisInitTransform = MainMgr.inst.pelvisInitTkr[0];
            //hand dist for scale
            msg.handDist = MainMgr.inst.handDist[0];
            //model type
        }
        msg.modelType = MainMgr.inst.modelType[0];

        //send from net
        byte[] registerDataByte = Utility.Trans2byte(msg);
        NetMgr.sendMsg(packageType.register, registerDataByte);
    }

}