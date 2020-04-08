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

    void Start() {
        //only open one cam at a time
        screenCam.transform.position = new Vector3(0, 0, -10);
        if (MainMgr.isVRValid) {
            //generate VR camera if vr valid
            VRroot = Instantiate(VrPrefab);
            curCam = VRroot.GetComponentInChildren<Transform>().Find("Camera").gameObject;
        }
        else
            curCam = screenCam;

        if (MainMgr.isCamValid)
            initialCamera();
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
        Debug.Log("update");
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
        //send from net
        byte[] modelDataBytes = Utility.Trans2byte(msg);
        NetMgr.sendMsg(packageType.camModel, modelDataBytes, false);
    }
    
}