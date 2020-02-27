using System;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using Microsoft.Azure.Kinect.Sensor;
using Microsoft.Azure.Kinect.Sensor.BodyTracking;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class ServerListener : MonoBehaviour {

    Device device;
    BodyTracker tracker;
    Skeleton skeleton;
    GameObject[] debugObjects;
    public Renderer renderer;
    
    MainMgr mainMgr = null;
    public Text count = null;
    
    private TcpServer server = null;
    private int curPackage = 0;
    static byte[] nullByte = new byte[1024];
    static string nullByteStr = Encoding.ASCII.GetString(nullByte);



    //make sure initial complete
    private bool initial = false;

    // Use this for initialization
    void Awake() {
        //get client & server from persist node
        mainMgr = GameObject.Find("MainMgr").GetComponent<MainMgr>();
        server = mainMgr.server;
        if (server == null)
            Debug.LogWarning("null server");
    }

    private void OnEnable() {
        
        // KINECT INITIALIZE
        this.device = Device.Open(0);
        Debug.Log("open device");
        var config = new DeviceConfiguration {
            ColorResolution = ColorResolution.r720p,
            ColorFormat = ImageFormat.ColorBGRA32,
            DepthMode = DepthMode.NFOV_Unbinned
        };
        device.StartCameras(config);
        Debug.Log("start camera");

        var calibration = device.GetCalibration(config.DepthMode, config.ColorResolution);
        this.tracker = BodyTracker.Create(calibration);
        Debug.Log("create trcker complete");
        //tell update init complete
        initial = true;
        
        //cubes?
        debugObjects = new GameObject[(int)JointId.Count];
        for (var i = 0; i < (int)JointId.Count; i++) {
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = Enum.GetName(typeof(JointId), i);
            cube.transform.localScale = Vector3.one * 0.4f;
            debugObjects[i] = cube;
        }
        Debug.Log("init cube");

    }

    // Update is called once per frame
    void Update() {
        Debug.Log("enter update");
        updateBody();
    }

    private void OnDisable() {
        if (tracker != null) {
            tracker.Dispose();
        }
        if (device != null) {
            device.Dispose();
        }
    }

    void updateBody() {
        if (!initial) {
            Debug.Log("init not complete yet");
            return;
        }
        Debug.Log("Enter updateBody");
        using (Capture capture = device.GetCapture()) {
            tracker.EnqueueCapture(capture);
        }
        using (var frame = tracker.PopResult()) {
            Debug.Log("using");
            Debug.LogFormat("{0} bodies found.", frame.NumBodies);
            if (frame.NumBodies > 0) {
                var bodyId = frame.GetBodyId(0);
                Debug.LogFormat("bodyId={0}", bodyId);
                //send skeleton
                this.skeleton = frame.GetSkeleton(0);

                //send from net
                byte[] userDataBytes;
                MemoryStream ms = new MemoryStream();
                BinaryFormatter bf1 = new BinaryFormatter();
                bf1.Serialize(ms, frame.GetSkeleton(0));
                userDataBytes = ms.ToArray();

           




                //send to client
                sendData(userDataBytes);

                for (var i = 0; i < 6; i++) {
                    var joint = this.skeleton.Joints[i];
                    var pos = joint.Position;
                    var rot = joint.Orientation;
                    var v = new Vector3(pos[0], -pos[1], pos[2]) * 0.004f;
                    var r = new Quaternion(rot[1], rot[2], rot[3], rot[0]);
                    var obj = debugObjects[i];
                    obj.transform.SetPositionAndRotation(v, r);
                }

            }
        }
    }

    void sendData(byte[] data) {
        //TODO:
        server.SocketSend(data);
    }

}
