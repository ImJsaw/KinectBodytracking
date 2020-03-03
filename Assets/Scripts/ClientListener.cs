using System;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using Microsoft.Azure.Kinect.Sensor;
using Microsoft.Azure.Kinect.Sensor.BodyTracking;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Joint = Microsoft.Azure.Kinect.Sensor.BodyTracking.Joint;

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
    bool startUpdate = false;

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
        if(startUpdate)
            updateModel();
    }

    public void updateBody(byte[] bodyData) { //get data from net

        //MemoryStream ms = new MemoryStream(bodyData);
        //BinaryFormatter bf = new BinaryFormatter();
        //ms.Position = 0;
        //this.skeleton.Joints = deserializeJoints((string)bf.Deserialize(ms));
        this.skeleton = deserializeJoints(Encoding.ASCII.GetString(bodyData));
        startUpdate = true;
        //Debug.Log("count" + (int)JointId.Count);
        //for (var i = 0; i < (int)JointId.Count; i++) {
        //    Debug.Log(i);
        //    var joint = this.skeleton.Joints[i];
        //    var pos = joint.Position;
        //    Debug.Log("pos" + joint.Position);
        //    var rot = joint.Orientation;
        //    Debug.Log("rot" + joint.Orientation);
        //    var v = new Vector3(pos[0], -pos[1], pos[2]) * 0.004f;
        //    var r = new Quaternion(rot[1], rot[2], rot[3], rot[0]);
        //    var obj = debugObjects[i];
        //    //obj.transform.SetPositionAndRotation(v, r);
        //}

        //updateModel();
    }

    void updateModel() {
        Debug.Log("LLL****************************"+this.skeleton.Joints.Length.ToString());
        //       0            
        var joint1 = this.skeleton.Joints[0];
        var rot1 = joint1.Orientation;
        var rot2 = new Quaternion(rot1[1], rot1[2], rot1[3], rot1[0]);
        Quaternion r = (Quaternion.Inverse(Quaternion.Euler(0, -90, -90)) * rot2);
        Quaternion q = new Quaternion(r.z, -r.x, -r.y, r.w);
        chan.Pelvis.rotation = q;

        ////         1          
        joint1 = this.skeleton.Joints[1];
        rot1 = joint1.Orientation;
        rot2 = new Quaternion(rot1[1], rot1[2], rot1[3], rot1[0]);
        r = (Quaternion.Inverse(Quaternion.Euler(0, -90, -90)) * rot2);
        q = new Quaternion(r.z, -r.x, -r.y, r.w);
        chan.SpinNaval.rotation = q;

        //        2           
        joint1 = this.skeleton.Joints[2];
        rot1 = joint1.Orientation;
        rot2 = new Quaternion(rot1[1], rot1[2], rot1[3], rot1[0]);
        r = (Quaternion.Inverse(Quaternion.Euler(0, -90, -90)) * rot2);
        q = new Quaternion(r.z, -r.x, -r.y, r.w);
        chan.SpinChest.rotation = q;
        //        3           
        joint1 = this.skeleton.Joints[3];
        rot1 = joint1.Orientation;
        rot2 = new Quaternion(rot1[1], rot1[2], rot1[3], rot1[0]);
        r = (Quaternion.Inverse(Quaternion.Euler(0, -90, -90)) * rot2);
        q = new Quaternion(r.z, -r.x, -r.y, r.w);
        chan.Neck.rotation = q;

        //        4           
        joint1 = this.skeleton.Joints[4];
        rot1 = joint1.Orientation;
        rot2 = new Quaternion(rot1[1], rot1[2], rot1[3], rot1[0]);
        r = (Quaternion.Inverse(Quaternion.Euler(180, 0, 0)) * rot2);
        q = new Quaternion(r.y, -r.x, -r.z, r.w);
        chan.ClavicleLeft.rotation = q;


        //         5         
        joint1 = this.skeleton.Joints[5];
        rot1 = joint1.Orientation;
        rot2 = new Quaternion(rot1[1], rot1[2], rot1[3], rot1[0]);
        r = (Quaternion.Inverse(Quaternion.Euler(180, 0, 0)) * rot2);
        q = new Quaternion(r.y, -r.x, -r.z, r.w);
        chan.ShoulderLeft.rotation = q;


        //         6         
        joint1 = this.skeleton.Joints[6];
        rot1 = joint1.Orientation;
        rot2 = new Quaternion(rot1[1], rot1[2], rot1[3], rot1[0]);
        r = (Quaternion.Inverse(Quaternion.Euler(180, 0, 0)) * rot2);
        q = new Quaternion(r.y, -r.x, -r.z, r.w);
        chan.ElbowLeft.rotation = q;



        ////         7         
        //joint1 = this.skeleton.Joints[7];
        //rot1 = joint1.Orientation;
        //r = new Quaternion(rot1[1], rot1[2], rot1[3], rot1[0]);
        //chan.WristLeft.rotation = r;

        //8
        joint1 = this.skeleton.Joints[8];
        rot1 = joint1.Orientation;
        rot2 = new Quaternion(rot1[1], rot1[2], rot1[3], rot1[0]);
        r = (Quaternion.Inverse(Quaternion.Euler(0, 180, 0)) * rot2);
        q = new Quaternion(r.x, r.y, r.z, r.w);
        chan.ClavicleRight.rotation = q;



        //        9           
        joint1 = this.skeleton.Joints[9];
        rot1 = joint1.Orientation;
        rot2 = new Quaternion(rot1[1], rot1[2], rot1[3], rot1[0]);
        r = (Quaternion.Inverse(Quaternion.Euler(0, 180, 0)) * rot2);
        q = new Quaternion(r.x, r.y, r.z, r.w);
        chan.ShoulderRight.rotation = q;


        //        10           
        joint1 = this.skeleton.Joints[10];
        rot1 = joint1.Orientation;
        rot2 = new Quaternion(rot1[1], rot1[2], rot1[3], rot1[0]);
        r = (Quaternion.Inverse(Quaternion.Euler(0, 180, 0)) * rot2);
        q = new Quaternion(r.x, r.y, r.z, r.w);
        chan.ElbowRight.rotation = q;



        ////        11 
        //joint1 = this.skeleton.Joints[11];
        //rot1 = joint1.Orientation;
        //r = new Quaternion(rot1[1], rot1[2], rot1[3], rot1[0]);
        //chan.WristRight.rotation = Quaternion.Euler(r.eulerAngles.x, r.eulerAngles.y, r.eulerAngles.z);

        //        12           
        joint1 = this.skeleton.Joints[12];
        rot1 = joint1.Orientation;
        rot2 = new Quaternion(rot1[1], rot1[2], rot1[3], rot1[0]);
        r = (Quaternion.Inverse(Quaternion.Euler(0, -90, -90)) * rot2);
        q = new Quaternion(r.z, -r.x, -r.y, r.w);
        chan.HipLeft.rotation = q;

        //        13           
        joint1 = this.skeleton.Joints[13];
        rot1 = joint1.Orientation;
        rot2 = new Quaternion(rot1[1], rot1[2], rot1[3], rot1[0]);
        r = (Quaternion.Inverse(Quaternion.Euler(0, -90, -90)) * rot2);
        q = new Quaternion(r.z, -r.x, -r.y, r.w);
        chan.KneeLeft.rotation = q;

        //        14           
        joint1 = this.skeleton.Joints[14];
        rot1 = joint1.Orientation;
        rot2 = new Quaternion(rot1[1], rot1[2], rot1[3], rot1[0]);
        r = (Quaternion.Inverse(Quaternion.Euler(0, -90, -90)) * rot2);
        q = new Quaternion(r.z, -r.x, -r.y, r.w);
        chan.AnkleLeft.rotation = q;

        //////        15           
        ////joint1 = this.skeleton.Joints[15];
        ////pos1 = joint1.Position;
        ////rot1 = joint1.Orientation;
        ////v = new Vector3(pos1[0], -pos1[1], pos1[2]) * 0.004f;
        ////r = new Quaternion(rot1[1], rot1[2], rot1[3], rot1[0]);
        ////chan.FootLeft.rotation = r;

        //        16           
        joint1 = this.skeleton.Joints[16];
        rot1 = joint1.Orientation;
        rot2 = new Quaternion(rot1[1], rot1[2], rot1[3], rot1[0]);
        r = (Quaternion.Inverse(Quaternion.Euler(180, 90, -90)) * rot2);
        q = new Quaternion(r.z, r.x, r.y, r.w);
        chan.HipRight.rotation = q;


        //        17           
        joint1 = this.skeleton.Joints[17];
        rot1 = joint1.Orientation;
        rot2 = new Quaternion(rot1[1], rot1[2], rot1[3], rot1[0]);
        r = (Quaternion.Inverse(Quaternion.Euler(180, 90, -90)) * rot2);
        q = new Quaternion(r.z, r.x, r.y, r.w);
        chan.KneeRight.rotation = q;


        //        18           
        joint1 = this.skeleton.Joints[18];
        rot1 = joint1.Orientation;
        rot2 = new Quaternion(rot1[1], rot1[2], rot1[3], rot1[0]);
        r = (Quaternion.Inverse(Quaternion.Euler(180, 90, -90)) * rot2);
        q = new Quaternion(r.z, r.x, r.y, r.w);
        chan.AnkleRight.rotation = q;


        //////        19           
        ////joint1 = this.skeleton.Joints[19];
        ////rot1 = joint1.Orientation;
        ////r = new Quaternion(rot1[1], rot1[2], rot1[3], rot1[0]);
        ////chan.FootRight.rotation = Quaternion.Euler(-r.eulerAngles.x +180, r.eulerAngles.y, -r.eulerAngles.z );

        //        20           
        joint1 = this.skeleton.Joints[20];
        rot1 = joint1.Orientation;
        rot2 = new Quaternion(rot1[1], rot1[2], rot1[3], rot1[0]);
        r = (Quaternion.Inverse(Quaternion.Euler(0, -90, -90)) * rot2);
        q = new Quaternion(r.z, -r.x, -r.y, r.w);
        chan.Head.rotation = q;
    }


    Skeleton deserializeJoints(string s) {
        Debug.Log(s);
        string[] jointStr = s.Split('_');
        Skeleton skeleton = new Skeleton();
        skeleton.Joints = new Joint[jointStr.Length];
        Joint[] joints = skeleton.Joints;
        for (int i = 0; i < jointStr.Length; i++) {
            joints[i] = new Joint();
            string[] oriAndPos = jointStr[i].Split('$');
            Debug.Log("*****rot"+oriAndPos[0]);
            string[] orientationStr = oriAndPos[0].Split('#');
            float[] orientations = new float[orientationStr.Length];
            for (int j = 0; j < orientations.Length; j++) {
                Debug.Log(orientationStr[j]);
                orientations[j] = float.Parse(orientationStr[j]);
            }
            joints[i].Orientation = orientations;
            Debug.Log("*****Pos" + oriAndPos[1]);
            string[] positionStr = oriAndPos[1].Split('|');
            float[] positions = new float[positionStr.Length];
            for (int j = 0; j < positions.Length; j++) {
                Debug.Log(positionStr[j]);
                positions[j] = float.Parse(positionStr[j]);
            }
            joints[i].Position = positions;
        }
        return skeleton;
    }

}
