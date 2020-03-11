using UnityEngine;
using Microsoft.Azure.Kinect.Sensor.BodyTracking;
using Joint = Microsoft.Azure.Kinect.Sensor.BodyTracking.Joint;

public class ListenerBase : MonoBehaviour {

    protected Skeleton skeleton;
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
        public Transform position;
    }
    private readonly int jointNum = 21;
    Quaternion[] bodyRotations = new Quaternion[21];
    Vector3 bodyPosition = new Vector3();
    //Transform[] initialModel = new Transform[21];

    private static readonly Quaternion[] modelOffset = {
        //body
        Quaternion.Euler(0f, 90f, 90f),
        Quaternion.Euler(0f, 90f, 90f),
        Quaternion.Euler(0f, 90f, 90f),
        Quaternion.Euler(0f, 90f, 90f),

        //left arm
        Quaternion.Euler(180f, 0f, 180f),
        Quaternion.Euler(180f, 0f, 180f),
        Quaternion.Euler(180f, 0f, 180f),
        Quaternion.Euler(-90f, 0f, 180f),

        //right arm //(0,180,0)?
        Quaternion.Euler(0f, 0f, 180f),
        Quaternion.Euler(0f, 0f, 180f),
        Quaternion.Euler(0f, 0f, 180f),
        Quaternion.Euler(-90f, 0f, 180f),

        //left leg
        Quaternion.Euler(0f, 90f, 90f),
        Quaternion.Euler(0f, 90f, 90f),
        Quaternion.Euler(0f, 90f, 90f),
        Quaternion.Euler(90f, 0f, 0f),

        //right leg  //0,90,-90(?)
        Quaternion.Euler(180f, -90f, 90f),
        Quaternion.Euler(180f, -90f, 90f),
        Quaternion.Euler(180f, -90f, 90f),
        Quaternion.Euler(90f, 0f, 180f),

        //head
        Quaternion.Euler(0f, 90f, -90f),

    };

    private static readonly Quaternion[] axisTrans ={
        //body
        Quaternion.LookRotation(Vector3.left, Vector3.back),
        Quaternion.LookRotation(Vector3.left, Vector3.back),
        Quaternion.LookRotation(Vector3.left, Vector3.back),
        Quaternion.LookRotation(Vector3.left, Vector3.back),
        //left arm
        Quaternion.LookRotation(Vector3.left, Vector3.back),
        Quaternion.LookRotation(Vector3.left, Vector3.back),
        Quaternion.LookRotation(Vector3.left, Vector3.back),
        Quaternion.LookRotation(Vector3.left, Vector3.back),
        //right arm
        Quaternion.LookRotation(Vector3.left, Vector3.back),
        Quaternion.LookRotation(Vector3.left, Vector3.back),
        Quaternion.LookRotation(Vector3.left, Vector3.back),
        Quaternion.LookRotation(Vector3.left, Vector3.back),
        //left leg
        Quaternion.LookRotation(Vector3.left, Vector3.back),
        Quaternion.LookRotation(Vector3.left, Vector3.back),
        Quaternion.LookRotation(Vector3.left, Vector3.back),
        Quaternion.LookRotation(Vector3.left, Vector3.back),
        //right leg
        Quaternion.LookRotation(Vector3.left, Vector3.back),
        Quaternion.LookRotation(Vector3.left, Vector3.back),
        Quaternion.LookRotation(Vector3.left, Vector3.back),
        Quaternion.LookRotation(Vector3.left, Vector3.back),
        //head
        Quaternion.LookRotation(Vector3.left, Vector3.back),
     };
    
    protected static TcpClient client = null;
    protected static TcpServer server = null;

    // Use this for initialization
    void Awake() {
        //get client & server from persist node
        client = MainMgr.inst.client;
        server = MainMgr.inst.server;
    }

    void applyModel() {
        //mid body
        chan.Pelvis.rotation = bodyRotations[0];
        chan.SpinNaval.rotation = bodyRotations[1];
        chan.SpinChest.rotation = bodyRotations[2];
        chan.Neck.rotation = bodyRotations[3];
        //left arm
        chan.ClavicleLeft.rotation = bodyRotations[4];
        chan.ShoulderLeft.rotation = bodyRotations[5];
        chan.ElbowLeft.rotation = bodyRotations[6];
        chan.WristLeft.rotation = bodyRotations[7];
        //right arm
        chan.ClavicleRight.rotation = bodyRotations[8];
        chan.ShoulderRight.rotation = bodyRotations[9];
        chan.ElbowRight.rotation = bodyRotations[10];
        chan.WristRight.rotation = bodyRotations[11];
        //left leg
        chan.HipLeft.rotation = bodyRotations[12];
        chan.KneeLeft.rotation = bodyRotations[13];
        chan.AnkleLeft.rotation = bodyRotations[14];
        chan.FootLeft.rotation = bodyRotations[15];
        //right leg
        chan.HipRight.rotation = bodyRotations[16];
        chan.KneeRight.rotation = bodyRotations[17];
        chan.AnkleRight.rotation = bodyRotations[18];
        chan.FootRight.rotation = bodyRotations[19];
        //head
        chan.Head.rotation = bodyRotations[20];

        //position
        chan.position.position = bodyPosition;

    }

    protected void updateModelFromSkeleton() {
        //get model position/orientation from skeleton, save it
        for (int i = 0; i < jointNum; i++) {
            var rot1 = skeleton.Joints[i].Orientation;
            var rot2 = new Quaternion(rot1[1], rot1[2], rot1[3], rot1[0]);
            bodyRotations[i] = rot2 * modelOffset[i] * axisTrans[i];
        }
        var pos = skeleton.Joints[0].Position;
        Vector3 move = new Vector3(pos[0], -pos[1], pos[2]) * 0.001f;
        Vector3 initPos = new Vector3(2, -3, 2); //決定起始點
        bodyPosition = move + initPos;

        //apply
        applyModel();
    }

    Skeleton deserializeJoints(string s) {
        Debug.Log(s);
        //avoid get two data same time
        s = s.Split('@')[0];
        //get joint array
        string[] jointStr = s.Split('^');
        Skeleton skeleton = new Skeleton();
        skeleton.Joints = new Joint[jointStr.Length];
        Debug.Log("*****JOINT" + jointStr.Length);
        Joint[] joints = skeleton.Joints;
        for (int i = 0; i < 26; i++) {
            joints[i] = new Joint();
            //get single joint
            string[] oriAndPos = jointStr[i].Split('$');
            Debug.Log("*****rot" + oriAndPos[0] + "//" + i);
            //orientation
            string[] orientationStr = oriAndPos[0].Split('#');
            float[] orientations = new float[orientationStr.Length];
            for (int j = 0; j < orientations.Length; j++) {
                Debug.Log(orientationStr[j]);
                orientations[j] = float.Parse(orientationStr[j]);
            }
            joints[i].Orientation = orientations;
            //position
            Debug.Log("*****Pos" + oriAndPos[1] + "//" + i);
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

    string serializeJoints(Joint[] joints) {
        string s = "";
        for (int i = 0; i < 26; i++) {
            for (int j = 0; j < joints[i].Orientation.Length; j++) {
                s += joints[i].Orientation[j] + "#";
            }
            s = s.Remove(s.Length - 1);
            s += "$";
            for (int j = 0; j < joints[i].Position.Length; j++) {
                s += joints[i].Position[j] + "|";
            }
            s = s.Remove(s.Length - 1);
            s += "^";
        }
        s = s.Remove(s.Length - 1);
        return s + "@";
    }

}


