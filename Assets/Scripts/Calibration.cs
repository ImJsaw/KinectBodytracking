using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.Azure.Kinect.Sensor;
using Microsoft.Azure.Kinect.Sensor.BodyTracking;

public enum calibrationState {
    None = 0,
    TPose,
    HandRising,
    Complete
}

[CLSCompliant(false)]
public class Calibration : ListenerBase {
    Device device;
    BodyTracker tracker;
    public new Renderer renderer;
    public UnityEngine.UI.Text time_UI = null;
    // threshold of pose calibration
    private float angleThershold = 20;
    private bool isTimeUp = false;
    private int timeCount = 3;
    private bool getPosFirst = false;
    //make sure initial complete
    private bool initial = false;
    private calibrationState curState;

    void Start() {
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
        Debug.Log("initial complete");
        curState = calibrationState.None;
        initial = true;
    }

    private void OnDisable() {
        if (tracker != null) {
            tracker.Dispose();
        }
        if (device != null) {
            device.Dispose();
        }
    }

    void Update() {
        //SCREEN
        if (!initial)
            return;

        using (Capture capture = device.GetCapture()) {
            //Debug.Log("screen update");
            tracker.EnqueueCapture(capture);
            var color = capture.Color;
            if (color.WidthPixels > 0) {
                Texture2D tex = new Texture2D(color.WidthPixels, color.HeightPixels, TextureFormat.BGRA32, false);
                tex.LoadRawTextureData(color.GetBufferCopy());
                tex.Apply();
                renderer.material.mainTexture = tex;
            }
        }

        using (var frame = tracker.PopResult()) {
            //Debug.Log("skeleton update");
            if (frame.NumBodies > 0) {
                //send skeleton
                skeleton = frame.GetSkeleton(0);
                updateModel();
            }
        }
        //split for force update
        updateModel();
        getModelRotation();
        calibrationThread();
    }

    private void getModelRotation() {
        for (int i = 0; i < jointNum; i++) {
            //Debug.Log("get i=" + i);
            bodyRotations[i] = getModelTransform(i).rotation;
        }
        getPosFirst = true;
    }

    private void calibrationThread() {
        switch (curState) {
            case calibrationState.None:
                //start count down
                InvokeRepeating("countDown", 1, 1);
                isTimeUp = false;
                curState = calibrationState.TPose;
                break;
            case calibrationState.TPose:
                checkTPose();
                break;
            case calibrationState.HandRising:
                checkHandRising();
                break;
            case calibrationState.Complete:
                calibrationComplete();
                break;
            default:
                Debug.Log("unknown calibration state");
                break;
        }
    }

    private void checkHandRising() {
        Debug.Log("check habd rising");
    }

    private void checkTPose() {
        if (!getPosFirst) {
            Debug.Log("waiting first rotation");
            return;
        }

        Debug.Log("check pose");
        bool correct = true;

        for (int i = 0; i < jointNum; i++) {
            correct = true;
            for (int j = 0; j < 3; j++) {
                if (Math.Abs(bodyRotations[i].eulerAngles[j]) > angleThershold)
                    correct = false;
            }
            //calibration fail
            if (!correct) {
                Debug.Log("[Calibration]" + getModelName(i) + "  incorrect");
                UIMgr.inst.generatePanel("NetErrorPanel");
                calibrationFail();
                break;
            }
        }
        //t pose check complete, goto next state
        if (correct && isTimeUp) {
            InvokeRepeating("countDown", 1, 1);
            isTimeUp = false;
            curState = calibrationState.HandRising;
        }
    }

    private void countDown() {
        Debug.Log("countDown");
        if (!UIMgr.inst.isStop)
            timeCount -= 1;

        time_UI.text = timeCount + "";

        if (timeCount <= 0) {

            time_UI.text = "time\nup";

            CancelInvoke("countDown");
            isTimeUp = true;

        }
    }

    private void calibrationFail() {
        //time count reset
        timeCount = 3;
    }

    private void calibrationComplete() {
        Debug.Log("complete calibration");
    }
}
