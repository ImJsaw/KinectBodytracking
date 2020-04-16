using UnityEngine;
using System;
using Microsoft.Azure.Kinect.Sensor.BodyTracking;
using Joint = Microsoft.Azure.Kinect.Sensor.BodyTracking.Joint;

[CLSCompliant(false)]
public class ModelController : MonoBehaviour {

    //model index
    int _modelIndex;

    [HideInInspector]
    public int modelIndex {
        get {
            if (_modelIndex == -1)
                Debug.LogError("model number not set");
            return _modelIndex;
        }
        set {
            _modelIndex = value;
            if (_modelIndex < 0)
                Debug.LogError("model index < 0 !");
            if (_modelIndex == -1)
                Debug.LogError("model number not set");
        }
    }

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

    public Transform modelPosition = null;
    Skeleton skeleton;
    Vector3 mapPosition = new Vector3();

    void applyModel() {
        //       0
        Joint joint1 = skeleton.Joints[0];
        var pos = joint1.Position;
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
        Quaternion a = Quaternion.LookRotation(Vector3.forward, Vector3.up);

        ////4
        joint1 = this.skeleton.Joints[4];
        rot1 = joint1.Orientation;
        rot2 = new Quaternion(rot1[1], rot1[2], rot1[3], rot1[0]);
        r = (Quaternion.Euler(180, 0, 180) * rot2);
        //q = new Quaternion(r.x, r.y, r.z, r.w);
        q = r;
        chan.ClavicleLeft.rotation = q;


        //         5         
        joint1 = this.skeleton.Joints[5];
        rot1 = joint1.Orientation;
        rot2 = new Quaternion(rot1[1], rot1[2], rot1[3], rot1[0]);
        r = (Quaternion.Euler(180, 0, 180) * rot2);
        //q = new Quaternion(r.z, r.y, r.x, r.w);
        q = r;
        chan.ShoulderLeft.rotation = q;


        ////         6         
        joint1 = this.skeleton.Joints[6];
        rot1 = joint1.Orientation;
        rot2 = new Quaternion(rot1[1], rot1[2], rot1[3], rot1[0]);
        r = (Quaternion.Inverse(Quaternion.Euler(180, 0, 180)) * rot2);
        q = new Quaternion(r.y, r.x, r.z, r.w);
        q = r;
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
        r = (Quaternion.Inverse(Quaternion.Euler(0, 0, 180)) * rot2);
        q = new Quaternion(r.x, -r.y, -r.z, r.w);
        chan.ClavicleRight.rotation = q;



        //        9           
        joint1 = this.skeleton.Joints[9];
        rot1 = joint1.Orientation;
        rot2 = new Quaternion(rot1[1], rot1[2], rot1[3], rot1[0]);
        r = (Quaternion.Inverse(Quaternion.Euler(0, 0, 180)) * rot2);
        q = new Quaternion(r.x, -r.y, -r.z, r.w);
        chan.ShoulderRight.rotation = q;


        //        10           
        joint1 = this.skeleton.Joints[10];
        rot1 = joint1.Orientation;
        rot2 = new Quaternion(rot1[1], rot1[2], rot1[3], rot1[0]);
        r = (Quaternion.Inverse(Quaternion.Euler(0, 0, 180)) * rot2);
        q = new Quaternion(r.x, -r.y, -r.z, r.w);
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

        //        15           
        joint1 = this.skeleton.Joints[15];
        rot1 = joint1.Orientation;
        rot2 = new Quaternion(rot1[1], rot1[2], rot1[3], rot1[0]);
        r = (Quaternion.Inverse(Quaternion.Euler(0, -90, -90)) * rot2);
        q = new Quaternion(r.z, -r.x, -r.y, r.w);
        chan.AnkleLeft.rotation = q;

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
        chan.Head.rotation = q;

        //0
        joint1 = skeleton.Joints[0];
        pos = joint1.Position;
        rot1 = joint1.Orientation;
        rot2 = new Quaternion(rot1[1], rot1[2], rot1[3], rot1[0]);
        r = (Quaternion.Inverse(Quaternion.Euler(0, -90, -90)) * rot2);
        q = new Quaternion(r.z, -r.x, -r.y, r.w);
        chan.Pelvis.localRotation = Quaternion.Inverse(MainMgr.inst.initRot[modelIndex]) * q;

        //model position
        //var v = new Vector3(pos[0], -pos[1], pos[2]) * 0.002f;
        //modelPosition.position = v + mapPosition;
        //Vector3 offset = Quaternion.Inverse(MainMgr.inst.initRot[modelIndex]).eulerAngles;
        //modelPosition.transform.localRotation = Quaternion.Inverse(MainMgr.inst.initRot[modelIndex]);
        Debug.Log("calibrstion" + Quaternion.Inverse(MainMgr.inst.initRot[modelIndex]).eulerAngles.ToString());

    }

    private void updateModelTransform() {
        //rotation

        //camera VR空間 offset
        

        //position
        mapPosition = MainMgr.inst.mapPos[modelIndex];
        Debug.Log("before: " + mapPosition.ToString() + ", " + chan.Head.position.ToString() + ", " + modelPosition.position.ToString());
        //cam pos = original position + vector(head to model anchor)
        modelPosition.position = mapPosition - chan.Head.position + modelPosition.position;
        Debug.Log("after: " + mapPosition.ToString() + ", " + chan.Head.position.ToString() + ", " + modelPosition.position.ToString());
    }

    void Update() {
        if (modelIndex == -1) {
            Debug.Log("[modelController] No valid modelIndex");
            return;
        }
        updateModelTransform();
        if (!MainMgr.inst.isFirstDataGet[modelIndex]) {
            Debug.Log("[modelController] no." + modelIndex + " first data not get yet");
            return;
        }
        Debug.Log("[modelController] update "+modelIndex);
        skeleton = MainMgr.inst.skeletons[modelIndex];
        applyModel();
    }

}


