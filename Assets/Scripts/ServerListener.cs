using System;
using UnityEngine;
using Microsoft.Azure.Kinect.Sensor;
using Microsoft.Azure.Kinect.Sensor.BodyTracking;
using UnityEngine.UI;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[CLSCompliant(false)]
public class ServerListener : ListenerBase {

    Device device;
    BodyTracker tracker;
    GameObject[] debugObjects;


    //make sure initial complete
    private bool initial = false;
    public new Renderer renderer;
    //chatRoom
    string username = "server";
    bool updatechat = false;
    bool updatecube = false;

    public Text connectNum;

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

        updateConnectNum();

        updateChatRoom();

        if (updatechat)
        {
            Debug.Log("update true");
            UpdateChat();
        }

        if (updatecube)
            updateCube();

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
                //send from net
                byte[] modelDataBytes = Utility.Trans2byte(skeleton);
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

    public void updateChatRoom(byte[] msgData) //接訊息方
    {
        Debug.Log("updateChatRoom");
        content = Utility.byte2Origin<Messege>(msgData);

        NetMgr.sendMsg(packageType.messege, msgData, false);
        updatechat = true;
    }

    public void rcvCube(byte[] msgData) //接訊息方
    {
        Debug.Log("updateCube");
        controlCubeTransform = Utility.byte2Origin<Cube>(msgData);

        updatecube = true;
    }


    private void updateChatRoom() //接訊息方
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (chatInput.text != "")
            {

                Messege content = new Messege();
                content.username = username;
                content.text = chatInput.text;

                string addText = "\n  " + "<color=red>" + username + "</color>: " + chatInput.text;
                chatText.text += addText;
                chatInput.text = "";
                chatInput.ActivateInputField();
                Canvas.ForceUpdateCanvases();
                scrollRect.verticalNormalizedPosition = 1;
                Canvas.ForceUpdateCanvases();


                byte[] modelDataBytes = Utility.Trans2byte(content);
                NetMgr.sendMsg(packageType.messege, modelDataBytes, false);
            }
        }
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

    void updateConnectNum()
    {
        connectNum.text = MainMgr.inst.server.getConnectNum().ToString();
    }

    Color[] GetRenderTexturePixels(RenderTexture tex)
    {
        RenderTexture.active = tex;
        Texture2D tempTex = new Texture2D(tex.width, tex.height);
        tempTex.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0);
        tempTex.Apply();
        return tempTex.GetPixels();
    }
}