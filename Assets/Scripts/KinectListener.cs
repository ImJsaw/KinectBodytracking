using System;
using UnityEngine;
using Microsoft.Azure.Kinect.Sensor;
using Microsoft.Azure.Kinect.Sensor.BodyTracking;
using Joint = Microsoft.Azure.Kinect.Sensor.BodyTracking.Joint;
using UnityEngine.UI;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[CLSCompliant(false)]
public class KinectListener : MonoBehaviour {

    Device device;
    Skeleton skeleton = new Skeleton();
    BodyTracker tracker;
    GameObject[] debugObjects;
    //make sure initial complete
    private bool initial = false;
    public new Renderer renderer;
    //chatRoom
    string username = "server";
    bool updatechat = false;

    public Text connectNum;

    int myIndex;

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

        myIndex = getIndex();

        initial = true;
    }
    
    void Update() {
        Debug.Log("enter update");
        
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
            }
        }
        updateModel();
        sendModel();
    }

    private int getIndex() {
        //get index from server

        //暫時server0 client1 測試
        return 0;
    }

    void sendModel() {
        CamModel msg = new CamModel();
        msg.index = myIndex;
        msg.rot = MainMgr.inst.modelRot[myIndex];
        msg.pos = MainMgr.inst.modelPos[myIndex];
        //send from net
        byte[] modelDataBytes = Utility.Trans2byte(msg);
        NetMgr.sendMsg(packageType.camModel, modelDataBytes, false);
    }

    void updateModel() {
        
        //       0
        Joint joint1 = skeleton.Joints[0];
        var pos = joint1.Position;
        var rot1 = joint1.Orientation;
        var rot2 = new Quaternion(rot1[1], rot1[2], rot1[3], rot1[0]);
        Quaternion r = (Quaternion.Inverse(Quaternion.Euler(0, -90, -90)) * rot2);
        Quaternion q = new Quaternion(r.z, -r.x, -r.y, r.w);
        MainMgr.inst.modelRot[myIndex][0] = q;

        ////         1          
        joint1 = this.skeleton.Joints[1];
        rot1 = joint1.Orientation;
        rot2 = new Quaternion(rot1[1], rot1[2], rot1[3], rot1[0]);
        r = (Quaternion.Inverse(Quaternion.Euler(0, -90, -90)) * rot2);
        q = new Quaternion(r.z, -r.x, -r.y, r.w);
        MainMgr.inst.modelRot[myIndex][1] = q;

        //        2           
        joint1 = this.skeleton.Joints[2];
        rot1 = joint1.Orientation;
        rot2 = new Quaternion(rot1[1], rot1[2], rot1[3], rot1[0]);
        r = (Quaternion.Inverse(Quaternion.Euler(0, -90, -90)) * rot2);
        q = new Quaternion(r.z, -r.x, -r.y, r.w);
        MainMgr.inst.modelRot[myIndex][2] = q;
        //        3           
        joint1 = this.skeleton.Joints[3];
        rot1 = joint1.Orientation;
        rot2 = new Quaternion(rot1[1], rot1[2], rot1[3], rot1[0]);
        r = (Quaternion.Inverse(Quaternion.Euler(0, -90, -90)) * rot2);
        q = new Quaternion(r.z, -r.x, -r.y, r.w);
        MainMgr.inst.modelRot[myIndex][3] = q;

        ////4
        joint1 = this.skeleton.Joints[4];
        rot1 = joint1.Orientation;
        rot2 = new Quaternion(rot1[1], rot1[2], rot1[3], rot1[0]);
        r = (Quaternion.Euler(180, 0, 180) * rot2);
        //q = new Quaternion(r.x, r.y, r.z, r.w);
        q = r;
        MainMgr.inst.modelRot[myIndex][4] = q;


        //         5         
        joint1 = this.skeleton.Joints[5];
        rot1 = joint1.Orientation;
        rot2 = new Quaternion(rot1[1], rot1[2], rot1[3], rot1[0]);
        r = (Quaternion.Euler(180, 0, 180) * rot2);
        //q = new Quaternion(r.z, r.y, r.x, r.w);
        q = r;
        MainMgr.inst.modelRot[myIndex][5] = q;


        ////         6         
        joint1 = this.skeleton.Joints[6];
        rot1 = joint1.Orientation;
        rot2 = new Quaternion(rot1[1], rot1[2], rot1[3], rot1[0]);
        r = (Quaternion.Inverse(Quaternion.Euler(180, 0, 180)) * rot2);
        q = new Quaternion(r.y, r.x, r.z, r.w);
        q = r;
        MainMgr.inst.modelRot[myIndex][6] = q;



        ////         7         
        //joint1 = this.skeleton.Joints[7];
        //rot1 = joint1.Orientation;
        //r = new Quaternion(rot1[1], rot1[2], rot1[3], rot1[0]);
        //chan.WristLeft.rotation = r;

        //8
        joint1 = this.skeleton.Joints[8];
        rot1 = joint1.Orientation;
        rot2 = new Quaternion(rot1[1], rot1[2], rot1[3], rot1[0]);
        r = (Quaternion.Inverse(Quaternion.Euler(0, 0, 180)) * rot2);
        q = new Quaternion(r.x, -r.y, -r.z, r.w);
        MainMgr.inst.modelRot[myIndex][8] = q;



        //        9           
        joint1 = this.skeleton.Joints[9];
        rot1 = joint1.Orientation;
        rot2 = new Quaternion(rot1[1], rot1[2], rot1[3], rot1[0]);
        r = (Quaternion.Inverse(Quaternion.Euler(0, 0, 180)) * rot2);
        q = new Quaternion(r.x, -r.y, -r.z, r.w);
        MainMgr.inst.modelRot[myIndex][9] = q;


        //        10           
        joint1 = this.skeleton.Joints[10];
        rot1 = joint1.Orientation;
        rot2 = new Quaternion(rot1[1], rot1[2], rot1[3], rot1[0]);
        r = (Quaternion.Inverse(Quaternion.Euler(0, 0, 180)) * rot2);
        q = new Quaternion(r.x, -r.y, -r.z, r.w);
        MainMgr.inst.modelRot[myIndex][10] = q;



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
        MainMgr.inst.modelRot[myIndex][12] = q;

        //        13           
        joint1 = this.skeleton.Joints[13];
        rot1 = joint1.Orientation;
        rot2 = new Quaternion(rot1[1], rot1[2], rot1[3], rot1[0]);
        r = (Quaternion.Inverse(Quaternion.Euler(0, -90, -90)) * rot2);
        q = new Quaternion(r.z, -r.x, -r.y, r.w);
        MainMgr.inst.modelRot[myIndex][13] = q;

        //        14           
        joint1 = this.skeleton.Joints[14];
        rot1 = joint1.Orientation;
        rot2 = new Quaternion(rot1[1], rot1[2], rot1[3], rot1[0]);
        r = (Quaternion.Inverse(Quaternion.Euler(0, -90, -90)) * rot2);
        q = new Quaternion(r.z, -r.x, -r.y, r.w);
        MainMgr.inst.modelRot[myIndex][14] = q;

        //        15           
        joint1 = this.skeleton.Joints[15];
        rot1 = joint1.Orientation;
        rot2 = new Quaternion(rot1[1], rot1[2], rot1[3], rot1[0]);
        r = (Quaternion.Inverse(Quaternion.Euler(0, -90, -90)) * rot2);
        q = new Quaternion(r.z, -r.x, -r.y, r.w);
        MainMgr.inst.modelRot[myIndex][15] = q;

        //        16           
        joint1 = this.skeleton.Joints[16];
        rot1 = joint1.Orientation;
        rot2 = new Quaternion(rot1[1], rot1[2], rot1[3], rot1[0]);
        r = (Quaternion.Inverse(Quaternion.Euler(180, 90, -90)) * rot2);
        q = new Quaternion(r.z, r.x, r.y, r.w);
        MainMgr.inst.modelRot[myIndex][16] = q;


        //        17           
        joint1 = this.skeleton.Joints[17];
        rot1 = joint1.Orientation;
        rot2 = new Quaternion(rot1[1], rot1[2], rot1[3], rot1[0]);
        r = (Quaternion.Inverse(Quaternion.Euler(180, 90, -90)) * rot2);
        q = new Quaternion(r.z, r.x, r.y, r.w);
        MainMgr.inst.modelRot[myIndex][17] = q;


        //        18           
        joint1 = this.skeleton.Joints[18];
        rot1 = joint1.Orientation;
        rot2 = new Quaternion(rot1[1], rot1[2], rot1[3], rot1[0]);
        r = (Quaternion.Inverse(Quaternion.Euler(180, 90, -90)) * rot2);
        q = new Quaternion(r.z, r.x, r.y, r.w);
        MainMgr.inst.modelRot[myIndex][18] = q;


        ////        19           
        //joint1 = this.skeleton.Joints[19];
        //rot1 = joint1.Orientation;
        //r = new Quaternion(rot1[1], rot1[2], rot1[3], rot1[0]);
        //chan.FootRight.rotation = Quaternion.Euler(-r.eulerAngles.x + 180, r.eulerAngles.y, -r.eulerAngles.z);

        //        20           
        joint1 = this.skeleton.Joints[20];
        rot1 = joint1.Orientation;
        rot2 = new Quaternion(rot1[1], rot1[2], rot1[3], rot1[0]);
        r = (Quaternion.Inverse(Quaternion.Euler(0, -90, -90)) * rot2);
        q = new Quaternion(r.z, -r.x, -r.y, r.w);
        MainMgr.inst.modelRot[myIndex][20] = q;

        //model position
        var v = new Vector3(pos[0], -pos[1], pos[2]) * 0.002f;
        var restore = new Vector3(-2, 3, -2); //決定起始點
        MainMgr.inst.modelPos[myIndex] = v - restore;
        

    }
}