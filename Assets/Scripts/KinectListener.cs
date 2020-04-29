using System;
using UnityEngine;
using Microsoft.Azure.Kinect.Sensor;
using Microsoft.Azure.Kinect.Sensor.BodyTracking;

[CLSCompliant(false)]
public class KinectListener : MonoBehaviour {

    Device device;
    Skeleton skeleton = new Skeleton();
    BodyTracker tracker;
    //make sure initial complete
    private bool initial = false;

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

        if (MainMgr.isCamValid)
            initialCamera();
        //if is a client without cam, send register to server 
        if (MainMgr.isClient)
            sendRegister();
    }

    void initialCamera() {
        device = Device.Open(0);
        var config = new DeviceConfiguration {
            ColorResolution = ColorResolution.r720p,
            ColorFormat = ImageFormat.ColorBGRA32,
            DepthMode = DepthMode.NFOV_Unbinned
        };
        device.StartCameras(config);
        var calibration = device.GetCalibration(config.DepthMode, config.ColorResolution);
        tracker = BodyTracker.Create(calibration);

        initial = true;
    }

    void Update() {
        //Debug.Log("update");
        updatePosition();
        if (!initial) {
            Debug.Log("init not complete yet");
            return;
        }
        updateSkeleton();
    }

    private void OnDisable() {
        if (tracker != null) {
            tracker.Dispose();
        }
        if (device != null) {
            device.Dispose();
        }
    }

    void updateSkeleton() {
        Debug.Log("Enter update skeleton");
        using (Capture capture = device.GetCapture()) {
            tracker.EnqueueCapture(capture);
        }
        using (var frame = tracker.PopResult()) {
            Debug.LogFormat("{0} bodies found.", frame.NumBodies);
            if (frame.NumBodies > 0) {
                skeleton = frame.GetSkeleton(0);
                MainMgr.inst.skeletons[0] = skeleton;
                MainMgr.inst.isFirstDataGet[0] = true;
            }
        }

        //VR
        if (MainMgr.isVRValid) {
            MainMgr.inst.leftCtr[0] = new SerializableTransform(leftController.position, leftController.rotation);
            MainMgr.inst.rightCtr[0] = new SerializableTransform(rightController.position, rightController.rotation);
            MainMgr.inst.leftTkr[0] = new SerializableTransform(leftTracker.position, leftTracker.rotation);
            MainMgr.inst.rightTkr[0] = new SerializableTransform(rightTracker.position, rightTracker.rotation);
        }
        sendModel();
    }

    private void updatePosition() {
        Debug.Log("[CamPosTracker] update index" + 0 + " cam pos" + curCam.transform.position);
        MainMgr.inst.mapPos[0] = curCam.transform.position;
    }

    void sendModel() {
        playerPose msg = new playerPose();
        msg.UID = MainMgr.inst.myUID();
        msg.skeleton = MainMgr.inst.skeletons[0];
        msg.posX = MainMgr.inst.mapPos[0].x;
        msg.posY = MainMgr.inst.mapPos[0].y;
        msg.posZ = MainMgr.inst.mapPos[0].z;
        if (MainMgr.isVRValid) {
            msg.leftHandTransform = MainMgr.inst.leftCtr[0];
            msg.rightHandTransform = MainMgr.inst.rightCtr[0];
            msg.leftFeetTransform = MainMgr.inst.leftTkr[0];
            msg.rightFeetTransform = MainMgr.inst.rightTkr[0];

        }
        //send from net
        byte[] modelDataBytes = Utility.Trans2byte(msg);
        NetMgr.sendMsg(packageType.camModel, modelDataBytes);
    }

    void sendRegister() {
        register msg = new register();
        msg.UID = MainMgr.inst.myUID();
        msg.posX = MainMgr.inst.mapPos[0].x;
        msg.posY = MainMgr.inst.mapPos[0].y;
        msg.posZ = MainMgr.inst.mapPos[0].z;
        //send from net
        byte[] registerDataByte = Utility.Trans2byte(msg);
        NetMgr.sendMsg(packageType.register, registerDataByte);
    }

}