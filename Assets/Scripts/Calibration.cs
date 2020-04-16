using System;
using UnityEngine;
using UnityEngine.UI;

using Microsoft.Azure.Kinect.Sensor;
using Microsoft.Azure.Kinect.Sensor.BodyTracking;

public enum calibrationState {
    None = 0,
    VR,
    TPose,
    HandRising,
    Complete
}

[CLSCompliant(false)]
public class Calibration : ListenerBase {
    Device device;
    BodyTracker tracker;
    public new Renderer renderer;
    public Text time_UI = null;
    public Text joint_UI = null;
    public Text rot_UI = null;

    public GameObject VRCam = null;

    // threshold of pose calibration
    private readonly float angleThershold = 30;
    private bool isTimeUp = false;
    private int timeCount = 3;
    private bool getPosFirst = false;
    //make sure initial complete
    private bool initial = false;
    private calibrationState curState;
    private Vector3[] tPoseData = {
        //body
        new Vector3(0,180,0),
        new Vector3(0,180,0),
        new Vector3(0,180,0),
        new Vector3(0,180,0),
        //L arm
        new Vector3(0,180,0),
        new Vector3(0,180,0),
        new Vector3(0,180,0),
        new Vector3(0,180,0),
        //R arm
        new Vector3(0,180,0),
        new Vector3(0,180,0),
        new Vector3(0,180,0),
        new Vector3(0,180,0),
        //L leg
        new Vector3(0,180,0),
        new Vector3(0,180,0),
        new Vector3(0,180,0),
        new Vector3(0,180,0),
        //R leg
        new Vector3(0,180,0),
        new Vector3(0,180,0),
        new Vector3(0,180,0),
        new Vector3(0,180,0),
        //head
        new Vector3(0,180,0),
    };

    //ignore wrist,foot,all armX
    private bool[][] tPoseCheck = {
        //body
        new bool[3]{ true,true,true},
        new bool[3]{ true,true,true},
        new bool[3]{ true,true,true},
        new bool[3]{ true,true,true},
        //L arm
        new bool[3]{ false,true,true},
        new bool[3]{ false,true,true},
        new bool[3]{ false,true,true},
        new bool[3]{ false, false, false},
        //R arm
        new bool[3]{ false,true,true},
        new bool[3]{ false,true,true},
        new bool[3]{ false,true,true},
        new bool[3]{ false, false, false},
        //L leg
        new bool[3]{ true,true,true},
        new bool[3]{ true,true,true},
        new bool[3]{ false, false, false},
        new bool[3]{ false, false, false},
        //R leg
        new bool[3]{ true,true,true},
        new bool[3]{ true,true,true},
        new bool[3]{ false, false, false},
        new bool[3]{ false, false, false},
        //head
        new bool[3]{ true,true,true},
    };

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
                curState = calibrationState.VR;
                break;
            case calibrationState.VR:
                checkHelmet();
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

    private void checkHelmet() {
        rot_UI.text = "VR rot : " + VRCam.transform.rotation.eulerAngles.ToString();
        Debug.Log("Pelvis : " + chan.Pelvis.rotation.eulerAngles.ToString());
        bool correct = true;

        Vector3 angle = VRCam.transform.rotation.eulerAngles;
        if (Math.Abs(angle.x) > 10 || Math.Abs(angle.y) > 10 || Math.Abs(angle.z) > 10)
            calibrationFail();
        // check complete, goto next state
        if (correct && isTimeUp) {
            //save init rotation
            MainMgr.inst.initRot[0] = chan.Pelvis.rotation;
            InvokeRepeating("countDown", 1, 1);
            isTimeUp = false;
            //goto complete state
            curState = calibrationState.Complete;
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
        joint_UI.text = "";

        for (int i = 0; i < jointNum; i++) {
            correct = true;
            string jointRot = "";
            for (int j = 0; j < 3; j++) {
                //check this way need to match
                if (!tPoseCheck[i][j])
                    continue;
                //jointRot += bodyRotations[i].eulerAngles[j] + ",";
                int angle = (int)bodyRotations[i].eulerAngles[j] - (int)tPoseData[i][j];
                if (angle < -180)
                    angle += 360;
                if (angle > 180)
                    angle -= 360;
                if (Math.Abs(angle) > angleThershold)
                    correct = false;
            }
            //calibration fail
            if (!correct) {
                Debug.Log("[Calibration]" + getModelName(i) + "  incorrect");
                string str = getModelName(i) + "  error!" + jointRot;
                Debug.Log(str);
                joint_UI.text = str;
                rot_UI.text = bodyRotations[i].eulerAngles.ToString();
                calibrationFail();
                Debug.Log("[Calibration]" + getModelName(i) + " , " + bodyRotations[i].eulerAngles.ToString());
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

        time_UI.text = "維持" + timeCount + "秒完成校正!";

        if (timeCount <= 0) {

            time_UI.text = "校正完成!";

            CancelInvoke("countDown");
            isTimeUp = true;

        }
    }

    private void calibrationFail() {
        //time count reset
        timeCount = 5;
    }

    public void calibrationComplete() {
        Debug.Log("complete calibration");
        MainMgr.inst.changeScene(SceneID.General);
    }
}
