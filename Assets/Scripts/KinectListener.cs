using System;
using UnityEngine;
using Microsoft.Azure.Kinect.Sensor;
using Microsoft.Azure.Kinect.Sensor.BodyTracking;
using Joint = Microsoft.Azure.Kinect.Sensor.BodyTracking.Joint;
using UnityEngine.UI;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[CLSCompliant(false)]
public class KinectListener : MonoBehaviour {

    Device device;
    Skeleton skeleton = new Skeleton();
    BodyTracker tracker;
    //make sure initial complete
    private bool initial = false;
    
    public bool hasCam = false;

    int myIndex;

    void Start() {
        if (!hasCam)
            return;
        // KINECT INITIALIZE
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
        Debug.Log("enter update");
        
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
        if (!initial) {
            Debug.Log("init not complete yet");
            return;
        }
        Debug.Log("Enter updateBody");
        using (Capture capture = device.GetCapture()) {
            tracker.EnqueueCapture(capture);
        }
        using (var frame = tracker.PopResult()) {
            Debug.LogFormat("{0} bodies found.", frame.NumBodies);
            if (frame.NumBodies > 0) {
                skeleton = frame.GetSkeleton(0);
                MainMgr.inst.skeletons[myIndex] = skeleton;
            }
        }
        sendModel();
    }

    private int getIndex() {
        //get index from server

        //暫時server0 client1 測試
        return 0;
    }

    void sendModel() {
        CamModel msg = new CamModel();
        msg.index = myIndex;
        //msg.rot = MainMgr.inst.modelRot[myIndex];
        //msg.pos = MainMgr.inst.modelPos[myIndex];
        msg.skeleton = MainMgr.inst.skeletons[myIndex];
        //send from net
        byte[] modelDataBytes = Utility.Trans2byte(msg);
        NetMgr.sendMsg(packageType.camModel, modelDataBytes, false);
    }
    
}