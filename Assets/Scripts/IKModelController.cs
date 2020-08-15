using UnityEngine;
using System;
using Valve.VR;

[CLSCompliant(false)]
public class IKModelController : MonoBehaviour {
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
            //scaleByHand(MainMgr.inst.handDist[modelIndex]);
        }
    }

    public Transform pelvisPosition = null;

    //===========================new Target=========================
    private GameObject rightHandTargetNode;
    private GameObject leftHandTargetNode;
    private GameObject rightHandGoalNode;
    private GameObject leftHandGoalNode;
    private GameObject rightLegTargetNode;
    private GameObject leftLegTargetNode;
    private GameObject headTargetNode;
    //===============================================================



    //vr tracker
    private SerializableTransform hmt;
    private SerializableTransform rightCtr = null;
    private SerializableTransform leftCtr = null;
    private SerializableTransform rightTkr = null;
    private SerializableTransform leftTkr = null;
    private SerializableTransform pelvisTkr = null;
    //apply tracker pos to target
    public Transform rightHandTarget = null;
    public Transform leftHandTarget = null;
    public Transform rightHandGoal = null;
    public Transform leftHandGoal = null;
    public Transform rightLegTarget = null;
    public Transform leftLegTarget = null;
    public Transform headTarget = null;

  

    //tracker init rotation
    private Quaternion leftArmInitRot = Quaternion.identity;
    private Quaternion rightArmInitRot = Quaternion.identity;
    private Quaternion leftLegInitRot = Quaternion.identity;
    private Quaternion rightLegInitRot = Quaternion.identity;
    private Quaternion pelvisInitRot = Quaternion.identity;
    //target init
    private Quaternion leftArmTargetRot = Quaternion.identity;
    private Quaternion rightArmTargetRot = Quaternion.identity;
    private Quaternion leftLegTargetRot = Quaternion.identity;
    private Quaternion rightLegTargetRot = Quaternion.identity;

    public Vector3 multiplier = new Vector3(1.5f,1.5f,1.5f);
    //controller len
    float controllerLen = 0.1f;
    // initial pos

    private float modelHandDis;


    void Start() {
        setTargetGroup();
        logTargetInitRotation();
        modelHandDis = Vector3.Distance(rightHandTarget.position, leftHandTarget.position);
    }

    void Update() {
        //always update scale to make client scale correct
        scaleByHand(MainMgr.inst.handDist[modelIndex]);
        hmt = MainMgr.inst.headPos[modelIndex];
        leftCtr = MainMgr.inst.leftCtr[modelIndex];
        rightCtr = MainMgr.inst.rightCtr[modelIndex];
        leftTkr = MainMgr.inst.leftTkr[modelIndex];
        rightTkr = MainMgr.inst.rightTkr[modelIndex];
        pelvisTkr = MainMgr.inst.pelvisTkr[modelIndex];

        leftArmInitRot = MainMgr.inst.leftInitCtr[modelIndex].rot;
        rightArmInitRot = MainMgr.inst.rightInitCtr[modelIndex].rot;
        leftLegInitRot = MainMgr.inst.leftInitTkr[modelIndex].rot;
        rightLegInitRot = MainMgr.inst.rightInitTkr[modelIndex].rot;
        pelvisInitRot = MainMgr.inst.pelvisInitTkr[modelIndex].rot;

        updateModelTransform();
        //pelvis
        headTarget.position = hmt.pos ;
        headTarget.rotation = hmt.rot;
        headTarget.localPosition = headTarget.localPosition + Vector3.Scale(headTarget.forward, new Vector3(-0.25f, -0.25f, -0.25f));
        //arm
        leftHandTarget.position = leftCtr.pos ;
        rightHandTarget.position = rightCtr.pos ;
        leftHandTarget.rotation = leftCtr.rot * Quaternion.Inverse(leftArmInitRot) * leftArmTargetRot;
        rightHandTarget.rotation = rightCtr.rot * Quaternion.Inverse(rightArmInitRot) * rightArmTargetRot;
        //assist point from kinect
        if (leftHandGoal != null && MainMgr.inst.leftArmGoal[modelIndex].v3() != new Vector3(0, 0, 0))
            leftHandGoal.position = MainMgr.inst.leftArmGoal[modelIndex].v3();
        if (rightHandGoal != null && MainMgr.inst.rightArmGoal[modelIndex].v3() != new Vector3(0, 0, 0))
            rightHandGoal.position = MainMgr.inst.rightArmGoal[modelIndex].v3();
        //leg
        leftLegTarget.position = leftTkr.pos - new Vector3(0,0.3f,0) ;
        rightLegTarget.position = rightTkr.pos - new Vector3(0, 0.3f, 0);
        leftLegTarget.rotation = leftTkr.rot * Quaternion.Inverse(leftLegInitRot) * leftLegTargetRot;
        rightLegTarget.rotation = rightTkr.rot * Quaternion.Inverse(rightLegInitRot) * rightLegTargetRot;


        Debug.Log(rightHandTargetNode.transform.position);

    }

    private void logTargetInitRotation() {
        leftArmTargetRot = leftHandTarget.rotation;
        rightArmTargetRot = rightHandTarget.rotation;
        leftLegTargetRot = leftLegTarget.rotation * Quaternion.Euler(0, 180, 0);
        rightLegTargetRot = rightLegTarget.rotation * Quaternion.Euler(0, 180, 0);
    }

    private void updateModelTransform() {
        //make model horizon move with cam
        pelvisPosition.position = pelvisTkr.pos;
        pelvisPosition.rotation = pelvisTkr.rot * Quaternion.Inverse(pelvisInitRot);
        //Debug.Log("init " + pelvisInitRot.eulerAngles.ToString() + "cur " + pelvisTkr.rot.eulerAngles.ToString() + "final " + pelvisPosition.rotation.eulerAngles.ToString());
        Vector3 offset = Vector3.Scale(MainMgr.inst.pelvisInitTkr[modelIndex].pos, multiplier);
        Debug.Log("set pelvis pos " + pelvisTkr.pos.ToString() + ", after assign " + pelvisPosition.position.ToString() + " offset : " + offset.ToString());
        pelvisPosition.localPosition += offset;
        Debug.Log("after offset" + pelvisPosition.position.ToString());
    }

    //scale model to fit
    private void scaleByHand(float handDistance) {
        if (handDistance == -1)
            return;

        
        float scale = (handDistance - controllerLen) / (modelHandDis - controllerLen);
        Debug.Log("scale model " + scale + " time to fit, model hand dis" + modelHandDis);
        transform.localScale = new Vector3(scale, scale, scale);
    }

    private void setTargetGroup()
    {
        //保留原本model設定
        if(rightHandTarget == null)
        {
                //SetRightTarget
                string rightHandPath = "mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:RightShoulder/mixamorig:RightArm/mixamorig:RightForeArm/mixamorig:RightHand";
                rightHandTargetNode = new GameObject("rightHandTarget");
                rightHandTargetNode.transform.SetParent(this.transform.Find(rightHandPath));
                rightHandTargetNode.transform.localPosition = new Vector3(0, 0, 0);
                //SetLeftTarget
                string leftHandPath = "mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:LeftShoulder/mixamorig:LeftArm/mixamorig:LeftForeArm/mixamorig:LeftHand";
                leftHandTargetNode = new GameObject("leftHandTarget");
                leftHandTargetNode.transform.SetParent(this.transform.Find(leftHandPath));
                leftHandTargetNode.transform.localPosition = new Vector3(0, 0, 0);

                //other Target

                rightHandGoalNode = new GameObject("rightHandGoalNode");
                leftHandGoalNode = new GameObject("leftHandGoalNode");
                rightLegTargetNode = new GameObject("rightLegTargetNode");
                leftLegTargetNode = new GameObject("leftLegTargetNode");
                headTargetNode = new GameObject("headTargetNode");

                //set target node to target
                rightHandTarget = rightHandTargetNode.transform;
                leftHandTarget = leftHandTargetNode.transform;
                rightHandGoal = rightHandGoalNode.transform;
                leftHandGoal = leftHandGoalNode.transform;
                rightLegTarget = rightLegTargetNode.transform;
                leftLegTarget = leftLegTargetNode.transform;
                headTarget = headTargetNode.transform;
        }

    }
}


