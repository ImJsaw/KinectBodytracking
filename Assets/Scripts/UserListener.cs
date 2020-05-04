using System;
using UnityEngine;

[CLSCompliant(false)]
public class UserListener : MonoBehaviour {

    public GameObject screenCam = null;
    public GameObject VrPrefab = null;
    private GameObject VRroot = null;

    private GameObject curCam = null;
    private Transform leftController = null;
    private Transform rightController = null;
    private Transform leftTracker = null;
    private Transform rightTracker = null;

    void Start() {
        //only open one cam at a time
        screenCam.transform.position = MainMgr.INIT_CAM_POS;
        if (MainMgr.isVRValid) {
            //generate VR camera if vr valid
            VRroot = Instantiate(VrPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            curCam = GameObject.FindWithTag("camera");
            leftController = VRroot.GetComponentInChildren<Transform>().Find("Controller (left)");
            rightController = VRroot.GetComponentInChildren<Transform>().Find("Controller (right)");
            leftTracker = VRroot.GetComponentInChildren<Transform>().Find("Tracker (left)");
            rightTracker = VRroot.GetComponentInChildren<Transform>().Find("Tracker (right)");
        } else
            curCam = screenCam;

        //tell other my stat
        sendRegister();
    }

    void Update() {
        //Debug.Log("update");
        if (!MainMgr.isVRValid) {
            Debug.LogError("NO VR !");
            return;
        }
        updatePosition();
        sendModel();
    }

    private void updatePosition() {
        Debug.Log("[CamPosTracker] update index" + 0 + " cam pos" + curCam.transform.position);
        MainMgr.inst.headPos[0] = new SerializableTransform(curCam.transform.position, curCam.transform.rotation);
        MainMgr.inst.leftCtr[0] = new SerializableTransform(leftController.position, leftController.rotation);
        MainMgr.inst.rightCtr[0] = new SerializableTransform(rightController.position, rightController.rotation);
        MainMgr.inst.leftTkr[0] = new SerializableTransform(leftTracker.position, leftTracker.rotation);
        MainMgr.inst.rightTkr[0] = new SerializableTransform(rightTracker.position, rightTracker.rotation);
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