using System;
using UnityEngine;
using Microsoft.Azure.Kinect.Sensor;
using Microsoft.Azure.Kinect.Sensor.BodyTracking;
using Joint = Microsoft.Azure.Kinect.Sensor.BodyTracking.Joint;
using UnityEngine.UI;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Valve.VR;

[CLSCompliant(false)]
public class KinectListener : MonoBehaviour {

    Device device;
    Skeleton skeleton = new Skeleton();
    BodyTracker tracker;
    //make sure initial complete
    private bool initial = false;
    
    public GameObject screenCam = null;
    public GameObject VRroot = null;
    public GameObject VRCam = null;

    private GameObject curCam = null;

    int myIndex;

    void Start() {
        //only open one cam at a time
        screenCam.transform.position = new Vector3(0, 0, -10);
        VRroot.SetActive(MainMgr.isVRValid);
        screenCam.SetActive(!MainMgr.isVRValid);
        if (MainMgr.isVRValid)
            curCam = VRroot;
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

        myIndex = getIndex();

        initial = true;
    }
    
    void Update() {
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
                MainMgr.inst.skeletons[myIndex] = skeleton;
                MainMgr.inst.isFirstDataGet[myIndex] = true;
            }
        }
        sendModel();
    }

    private void updatePosition() {
        Debug.Log("[CamPosTracker] update index" + myIndex + " cam pos" + curCam.transform.position);
        MainMgr.inst.mapPos[myIndex] = curCam.transform.position;
    }

    private int getIndex() {
        //get index from server

        //暫時server0 client1 測試
        return 0;
    }

    void sendModel() {
        CamModel msg = new CamModel();
        msg.index = myIndex;
        msg.skeleton = MainMgr.inst.skeletons[myIndex];
        //send from net
        byte[] modelDataBytes = Utility.Trans2byte(msg);
        NetMgr.sendMsg(packageType.camModel, modelDataBytes, false);
    }
    
}