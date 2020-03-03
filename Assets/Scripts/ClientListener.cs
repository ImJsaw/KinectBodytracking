using System;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using Microsoft.Azure.Kinect.Sensor;
using Microsoft.Azure.Kinect.Sensor.BodyTracking;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class ClientListener : MonoBehaviour {

    Device device;
    BodyTracker tracker;
    Skeleton skeleton;
    GameObject[] debugObjects;
    public Renderer renderer;
    public JointChan chan;
    [System.Serializable]
    public struct JointChan {
        public Transform Pelvis;// id = 0
        public Transform SpinNaval;// id = 1
        public Transform SpinChest;// id = 2
        public Transform Neck;// id = 3
        public Transform ClavicleLeft;// id = 4
        public Transform ShoulderLeft;// id = 5
        public Transform ElbowLeft;// id = 6
        public Transform WristLeft;// id = 7
        public Transform ClavicleRight;// id = 8
        public Transform ShoulderRight;// id = 9
        public Transform ElbowRight;// id = 10
        public Transform WristRight;// id = 11
        public Transform HipLeft;// id = 12
        public Transform KneeLeft;// id = 13
        public Transform AnkleLeft;// id = 14
        public Transform FootLeft;// id = 15
        public Transform HipRight;// id = 16
        public Transform KneeRight;// id = 17
        public Transform AnkleRight;// id = 18
        public Transform FootRight;// id = 19
        public Transform Head;// id = 20
    }

    MainMgr mainMgr = null;
    public Text count = null;
    
    private TcpClient client = null;
    private int curPackage = 0;
    static byte[] nullByte = new byte[1024];
    static string nullByteStr = Encoding.ASCII.GetString(nullByte);

    // Use this for initialization
    void Awake() {
        //get client & server from persist node
        mainMgr = GameObject.Find("MainMgr").GetComponent<MainMgr>();
        client = mainMgr.client;
        if (client == null)
            Debug.LogWarning("null server");
    }

    private void OnEnable() {
        //cube init
        debugObjects = new GameObject[(int)JointId.Count];
        for (var i = 0; i < (int)JointId.Count; i++) {
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = Enum.GetName(typeof(JointId), i);
            cube.transform.localScale = Vector3.one * 0.4f;
            debugObjects[i] = cube;
        }
    }

    // Update is called once per frame
    void Update() {
        
    }

    public void updateBody(byte[] bodyData) { //get data from net

        MemoryStream ms = new MemoryStream(bodyData);
        BinaryFormatter bf = new BinaryFormatter();
        ms.Position = 0;
        object rawObj = bf.Deserialize(ms);
        this.skeleton = (Skeleton)rawObj;

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
