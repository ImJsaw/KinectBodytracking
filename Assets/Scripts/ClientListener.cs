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
