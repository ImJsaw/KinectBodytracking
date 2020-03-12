using System;
using UnityEngine;
using Microsoft.Azure.Kinect.Sensor;
using Microsoft.Azure.Kinect.Sensor.BodyTracking;

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[CLSCompliant(false)]
public class ServerListener : ListenerBase {

    Device device;
    BodyTracker tracker;
    GameObject[] debugObjects;
    bool updatechat = false;
    //make sure initial complete
    private bool initial = false;

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
        initial = true;
        //cubes
        debugObjects = new GameObject[(int)JointId.Count];
        for (var i = 0; i < (int)JointId.Count; i++) {
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = Enum.GetName(typeof(JointId), i);
            cube.transform.localScale = Vector3.one * 0.4f;
            debugObjects[i] = cube;
        }
    }
    
    void Update() {
        if (server == null)
            Debug.LogWarning("null server");
        Debug.Log("enter update");
        updateSkeleton();

        if (updatechat)
        {
            Debug.Log("update true");
            UpdateChat();
        }

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
                //send from net
                byte[] modelDataBytes = NetMgr.Trans2byte(skeleton);
                NetMgr.sendMsg(packageType.model, modelDataBytes, false);
                //update cube
                for (var i = 0; i < (int)JointId.Count; i++) {
                    var joint = skeleton.Joints[i];
                    var pos = joint.Position;
                    var rot = joint.Orientation;
                    var v = new Vector3(pos[0], -pos[1], pos[2]) * 0.004f;
                    var r = new Quaternion(rot[1], rot[2], rot[3], rot[0]);
                    var obj = debugObjects[i];
                    obj.transform.SetPositionAndRotation(v, r);
                }
            }
        }
        updateModel();
    }

    public void updateChatRoom(byte[] bodyData) //接訊息方
    {
        Debug.Log("updateChatRoom");
        MemoryStream ms = new MemoryStream(bodyData);
        BinaryFormatter bf = new BinaryFormatter();
        ms.Position = 0;

        content = (Messege)bf.Deserialize(ms);

        updatechat = true;
        Debug.Log("updatechat   :"+updatechat);
    }

    void UpdateChat()
    {
        Debug.Log("UpdateChat()");
        string addText = "\n  " + "<color=red>" + content.username + "</color>: " + content.text;
        chatText.text += addText;

        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 1;
        Canvas.ForceUpdateCanvases();

        updatechat = false;
    }
}