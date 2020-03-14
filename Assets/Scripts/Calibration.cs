using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.Azure.Kinect.Sensor;
using Microsoft.Azure.Kinect.Sensor.BodyTracking;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[CLSCompliant(false)]
public class Calibration : ListenerBase {
    Device device;
    BodyTracker tracker;
    public new Renderer renderer;
    // threshold of pose calibration
    private float angleThershold = 20;
    private bool isStop = false;

    private void OnEnable() {
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
    }

    private void OnDisable() {
        if (tracker != null) {
            tracker.Dispose();
        }
        if (device != null) {
            device.Dispose();
        }
    }

    void Start() {
        startCalibration();
    }

    void Update() {
        //SCREEN
        using (Capture capture = device.GetCapture()) {
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
            if (frame.NumBodies > 0) {
                //send skeleton
                skeleton = frame.GetSkeleton(0);
                updateModel();
            }
        }
        //split for force update
        updateModel();
        getModelRotation();
    }

    private void getModelRotation() {
        for (int i = 0; i < jointNum; i++) {
            bodyRotations[i] = getModelTransform(i).rotation;
        }
    }

    private void startCalibration() {
        checkTPose();
        checkHandRising();
    }

    private void checkHandRising() {


    }

    private void checkTPose() {
        bool tPoseCorrect = false;
        while (!tPoseCorrect) {
            bool correct = true;
            for (int i = 0; i < jointNum; i++) {
                correct = true;
                for (int j = 0; j < 3; j++) {
                    if (Math.Abs(bodyRotations[i].eulerAngles[j]) > angleThershold)
                        correct = false;
                }
                
                if (!correct) {
                    Debug.Log("[Calibration]" + getModelName(i) + "  incorrect");
                    UIMgr.inst.generatePanel("NetErrorPanel");
                    break;
                }
            }
            tPoseCorrect = correct;
        }
    }

}
