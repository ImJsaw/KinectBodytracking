using System;
using UnityEngine;

[CLSCompliant(false)]
public class UserListener : ListenerBase {

    new void Start() {
        if (MainMgr.isVRValid)
            VRroot = Instantiate(VrPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        base.Start();
        //tell other my stat
        sendRegister();
    }

    new void Update() {
        base.Update();
        sendModel();
    }

    void sendModel() {
        playerPose msg = new playerPose();
        msg.UID = MainMgr.inst.myUID();
        msg.headTransform = MainMgr.inst.headPos[0];
        if (MainMgr.isVRValid) {
            msg.leftHandTransform = MainMgr.inst.leftCtr[0];
            msg.rightHandTransform = MainMgr.inst.rightCtr[0];
            msg.leftLegTransform = MainMgr.inst.leftTkr[0];
            msg.rightLegTransform = MainMgr.inst.rightTkr[0];
        }
        //send from net
        byte[] modelDataBytes = Utility.Trans2byte(msg);
        NetMgr.sendMsg(packageType.camModel, modelDataBytes);
    }

    void sendRegister() {
        register msg = new register();
        msg.UID = MainMgr.inst.myUID();
        msg.headInitTransform = MainMgr.inst.headPos[0];
        msg.leftHandInitTransform = MainMgr.inst.leftInitCtr[0];
        msg.rightHandInitTransform = MainMgr.inst.rightInitCtr[0];
        msg.leftLegInitTransform = MainMgr.inst.leftInitTkr[0];
        msg.rightLegInitTransform = MainMgr.inst.rightInitTkr[0];
        //send from net
        byte[] registerDataByte = Utility.Trans2byte(msg);
        NetMgr.sendMsg(packageType.register, registerDataByte);
    }

}