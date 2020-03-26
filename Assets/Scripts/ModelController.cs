using UnityEngine;
using System;

[CLSCompliant(false)]
public class ModelController : MonoBehaviour {

    //model index
    int _modelIndex;
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
    protected Quaternion[] bodyRotations = new Quaternion[21];
    Vector3 bodyPosition = new Vector3();
    Vector3 mapPosition = new Vector3();

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
        modelPosition.position = bodyPosition + mapPosition;

    }

    void Update() {
        if (modelIndex == -1) {
            Debug.Log("[modelController] No valid modelIndex");
            return;
        }
        Debug.Log("[modelController] update "+modelIndex);
        bodyRotations = MainMgr.inst.modelRot[modelIndex];
        bodyPosition = MainMgr.inst.modelPos[modelIndex];
        mapPosition = MainMgr.inst.mapPos[modelIndex];
        applyModel();
    }

}


