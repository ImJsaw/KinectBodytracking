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
    [HideInInspector]
    public Transform pelvisPosition = null;
    public float length = 0.5f; // set the lenth of controller to goal 
    [Range(0f, 1f)]
    public float GoalWeight = 0.5f;
    public string prefix = "mixamorig:";
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
    [HideInInspector]
    public Transform rightHandTarget = null;
    [HideInInspector]
    public Transform leftHandTarget = null;
    [HideInInspector]
    public Transform rightHandGoal = null;
    [HideInInspector]
    public Transform leftHandGoal = null;
    [HideInInspector]
    public Transform rightLegTarget = null;
    [HideInInspector]
    public Transform leftLegTarget = null;
    [HideInInspector]
    public Transform headTarget = null;
    //給lock接雙手的rotation   
    [HideInInspector]
    public GameObject rightHand = null;
    [HideInInspector]
    public GameObject leftHand = null;
    //讓IK_auto 確認有沒有接到ModelController
    [HideInInspector]
    public Boolean is_catch = false;
    //確認是不是客製化model
    public Boolean is_CustomModel = false;
    

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

    public Vector3 multiplier = new Vector3(1.5f, 1.5f, 1.5f);
    //controller len
    float controllerLen = 0.1f;
    // initial pos
    private float modelHandDis;

    //Tkr to floor offset
    private float rightLegOffset;
    private float leftLegOffset;
    //model to floor offset
    private float pelvisOffset;
    //check init foot tkr 
    private Boolean is_first = true;
    void Start() {

        pelvisPosition = transform.Find(prefix + "Hips");


        setTargetGroup();
        //logTargetInitRotation();
        modelHandDis = Vector3.Distance(rightHandTarget.position, leftHandTarget.position);
        scaleByHand(MainMgr.inst.handDist[modelIndex]);
    }

    void Update() {
        //always update scale to make client scale correct

        hmt = MainMgr.inst.headPos[modelIndex];
        leftCtr = MainMgr.inst.leftCtr[modelIndex];
        rightCtr = MainMgr.inst.rightCtr[modelIndex];
        leftTkr = MainMgr.inst.leftTkr[modelIndex];
        rightTkr = MainMgr.inst.rightTkr[modelIndex];
        pelvisTkr = MainMgr.inst.pelvisTkr[modelIndex];

        if(is_first)
        {
            leftLegOffset = leftTkr.pos.y ;
            rightLegOffset = rightTkr.pos.y ;
            is_first = false;
        }

        leftArmInitRot = MainMgr.inst.leftInitCtr[modelIndex].rot;
        rightArmInitRot = MainMgr.inst.rightInitCtr[modelIndex].rot;
        leftLegInitRot = MainMgr.inst.leftInitTkr[modelIndex].rot;
        rightLegInitRot = MainMgr.inst.rightInitTkr[modelIndex].rot;
        pelvisInitRot = MainMgr.inst.pelvisInitTkr[modelIndex].rot;

        updateModelTransform();
        //pelvis
        headTarget.position = hmt.pos;
        headTarget.rotation = hmt.rot;
        headTarget.localPosition = headTarget.localPosition;//+ Vector3.Scale(headTarget.forward, new Vector3(-0.25f, -0.25f, -0.25f));
                                                            //arm
        leftHandTarget.position = leftCtr.pos;
        rightHandTarget.position = rightCtr.pos;
        leftHandTarget.rotation = leftCtr.rot * Quaternion.Inverse(leftArmInitRot) * leftArmTargetRot;
        rightHandTarget.rotation = rightCtr.rot * Quaternion.Inverse(rightArmInitRot) * rightArmTargetRot;

        //setGoalplace();

        ///////////////////////////////////////////
        //assist point from kinect
        if (leftHandGoalNode != null && MainMgr.inst.leftArmGoal[modelIndex].v3() != new Vector3(0, 0, 0))
            leftHandGoalNode.transform.position = MainMgr.inst.leftArmGoal[modelIndex].v3();
        if (rightHandGoalNode != null && MainMgr.inst.rightArmGoal[modelIndex].v3() != new Vector3(0, 0, 0))
            rightHandGoalNode.transform.position = MainMgr.inst.rightArmGoal[modelIndex].v3();
        //leg
        leftLegTarget.position = leftTkr.pos - new Vector3(0, leftLegOffset, 0); //腳踝到腳底板的offset
        rightLegTarget.position = rightTkr.pos - new Vector3(0, rightLegOffset, 0); //腳踝到腳底板的offset
        leftLegTarget.rotation = leftTkr.rot * Quaternion.Inverse(leftLegInitRot) * leftLegTargetRot;
        rightLegTarget.rotation = rightTkr.rot * Quaternion.Inverse(rightLegInitRot) * rightLegTargetRot;


 
    }

    private void logTargetInitRotation() {
        leftArmTargetRot = leftHandTarget.rotation;
        rightArmTargetRot = rightHandTarget.rotation;
        leftLegTargetRot = leftLegTarget.rotation;
        rightLegTargetRot = rightLegTarget.rotation;
    }

    private void updateModelTransform() {
        Debug.Log("i want to know is Target Head i catch?", pelvisPosition);
        //make model horizon move with cam
        pelvisPosition.position = pelvisTkr.pos;
        pelvisPosition.rotation = pelvisTkr.rot * Quaternion.Inverse(pelvisInitRot);
        //Debug.Log("init " + pelvisInitRot.eulerAngles.ToString() + "cur " + pelvisTkr.rot.eulerAngles.ToString() + "final " + pelvisPosition.rotation.eulerAngles.ToString());
        Vector3 offset = Vector3.Scale(MainMgr.inst.pelvisInitTkr[modelIndex].pos, multiplier);
        Debug.Log("set pelvis pos " + pelvisTkr.pos.ToString() + ", after assign " + pelvisPosition.position.ToString() + " offset : " + offset.ToString());
        pelvisPosition.localPosition += offset;
        Debug.Log("after offset" + pelvisPosition.position.ToString());
        //this.transform.localPosition = new Vector3(this.transform.localPosition.x, this.transform.localPosition.y - rightLegOffset, this.transform.localPosition.z);
    }

    //scale model to fit
    private void scaleByHand(float handDistance) {
        if (handDistance == -1)
            return;


        float scale = (handDistance - controllerLen) / (modelHandDis - controllerLen);
        Debug.Log("scale model " + scale + " time to fit, model hand dis" + modelHandDis);
        transform.localScale = transform.localScale * scale;
    }

    private void setTargetGroup() {
        //SetRightTarget
        string rightHandPath = prefix + "Hips/" + prefix + "Spine/" + prefix + "Spine1/" + prefix + "Spine2/" + prefix + "RightShoulder/" + prefix + "RightArm/" + prefix + "RightForeArm/" + prefix + "RightHand";
        rightHandTargetNode = new GameObject("rightHandTarget");
        rightHand = GameObject.Find(rightHandPath);
        rightHandTargetNode.transform.SetParent(this.transform.Find(rightHandPath));
        rightHandTargetNode.transform.localPosition = new Vector3(0, 0, 0);
        rightHandTargetNode.transform.SetParent(this.transform, true);
        //SetLeftTarget
        string leftHandPath = prefix + "Hips/" + prefix + "Spine/" + prefix + "Spine1/" + prefix + "Spine2/" + prefix + "LeftShoulder/" + prefix + "LeftArm/" + prefix + "LeftForeArm/" + prefix + "LeftHand";
        leftHandTargetNode = new GameObject("leftHandTarget");
        leftHand = GameObject.Find(leftHandPath);
        leftHandTargetNode.transform.SetParent(this.transform.Find(leftHandPath));
        leftHandTargetNode.transform.localPosition = new Vector3(0, 0, 0);
        leftHandTargetNode.transform.SetParent(this.transform, true);

        GameObject rightArm = GameObject.Find(prefix + "Hips/" + prefix + "Spine/" + prefix + "Spine1/" + prefix + "Spine2/" + prefix + "RightShoulder/" + prefix + "RightArm");
        rightArm.transform.Rotate(0, 90, 0);

        GameObject leftArm = GameObject.Find(prefix + "Hips/" + prefix + "Spine/" + prefix + "Spine1/" + prefix + "Spine2/" + prefix + "LeftShoulder/" + prefix + "LeftArm");
        leftArm.transform.Rotate(0, -90, 0);

        //other Targets  
        rightHandGoalNode = new GameObject("rightHandGoalNode");
        leftHandGoalNode = new GameObject("leftHandGoalNode");
        rightLegTargetNode = new GameObject("rightLegTargetNode");
        leftLegTargetNode = new GameObject("leftLegTargetNode");
        headTargetNode = new GameObject("headTargetNode");



        leftHandGoalNode.transform.SetParent(pelvisPosition);
        rightHandGoalNode.transform.SetParent(pelvisPosition);
        leftHandGoalNode.transform.localPosition = new Vector3(0, 0, 0) - new Vector3(0, 0, 1f);
        rightHandGoalNode.transform.localPosition = new Vector3(0, 0, 0) - new Vector3(0, 0, 1f);
        //保留原本model設定
        if (is_CustomModel) {
            //SetRightTarget
            rightHandTargetNode.transform.localRotation *= Quaternion.Euler(90, 90, 0);

            //SetLeftTarget
            leftHandTargetNode.transform.localRotation *= Quaternion.Euler(-90, 90, 0);



            //rightArm offset for custom model

            rightArm.transform.Rotate(0, 90, 0);
            rightArm.transform.GetChild(0).Rotate(0, 90, 0);



            //other offset setting
            headTargetNode.transform.position += new Vector3(0, 2, 0);
            rightLegTargetNode.transform.localRotation = Quaternion.Euler(0, 180, 0);
            leftLegTargetNode.transform.localRotation = Quaternion.Euler(0, 180, 0);


            leftHandGoalNode.transform.localPosition = new Vector3(0, 0, 0) - new Vector3(0, 0, 0.001f);
            rightHandGoalNode.transform.localPosition = new Vector3(0, 0, 0) - new Vector3(0, 0, 0.001f);


        }

        //set target node to target
        rightHandTarget = rightHandTargetNode.transform;
        leftHandTarget = leftHandTargetNode.transform;
        rightHandGoal = rightHandGoalNode.transform;
        leftHandGoal = leftHandGoalNode.transform;
        rightLegTarget = rightLegTargetNode.transform;
        leftLegTarget = leftLegTargetNode.transform;
        headTarget = headTargetNode.transform;


        is_catch = true;
        logTargetInitRotation();

        //set model's hight
       // pelvisOffset = this.transform.position.y + 5f;
        


    }

    private void setGoalplace() {
        Vector3 rightHandbackVector = rightHandTarget.right * -1;
        Vector3 leftHandbackVector = leftHandTarget.right;
        rightHandGoal.transform.position = (rightHandbackVector * length + rightHandTarget.transform.position) * GoalWeight + (pelvisPosition.transform.position + pelvisPosition.transform.rotation * new Vector3(0.5f, 0, 0)) * (1 - GoalWeight);
        leftHandGoal.transform.position = (leftHandbackVector * length + leftHandTarget.transform.position) * GoalWeight + (pelvisPosition.transform.position + pelvisPosition.transform.rotation * new Vector3(-0.5f, 0, 0)) * (1 - GoalWeight);


        //Debug.DrawRay(rightHandTarget.transform.position, rightHandbackVector, Color.red);
    }

}


