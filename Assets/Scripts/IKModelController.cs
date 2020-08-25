using UnityEngine;
using System;
using Valve.VR;


[CLSCompliant(false)]
public class IKModelController : MonoBehaviour
{
    //model index
    int _modelIndex;
    [HideInInspector]
    public int modelIndex
    {
        get
        {
            if (_modelIndex == -1)
                Debug.LogError("model number not set");
            return _modelIndex;
        }
        set
        {
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

        [HideInInspector]
        public Boolean is_catch = false;

        public Boolean is_CustomModel = true;
        public Quaternion leftLegTargetRotOffset = Quaternion.Euler(0, 180, 0);
        public Quaternion righteftLegTargetRotOffset = Quaternion.Euler(0, 180, 0);

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
        float controllerLen = 0.0f;
        // initial pos

        private float modelHandDis;


        void Start()
        {
        setTargetGroup();
        //logTargetInitRotation();
        modelHandDis = Vector3.Distance(rightHandTarget.position, leftHandTarget.position);
        scaleByHand(MainMgr.inst.handDist[modelIndex]);
    }

        void Update()
        {
            //always update scale to make client scale correct
            
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
            headTarget.position = hmt.pos;
            headTarget.rotation = hmt.rot;
            headTarget.localPosition = headTarget.localPosition + Vector3.Scale(headTarget.forward, new Vector3(-0.25f, -0.25f, -0.25f));
            //arm
            leftHandTarget.position = leftCtr.pos;
            rightHandTarget.position = rightCtr.pos;
            leftHandTarget.rotation = leftCtr.rot * Quaternion.Inverse(leftArmInitRot) * leftArmTargetRot;
            rightHandTarget.rotation = rightCtr.rot * Quaternion.Inverse(rightArmInitRot) * rightArmTargetRot;
            //assist point from kinect
            //if (leftHandGoal != null && MainMgr.inst.leftArmGoal[modelIndex].v3() != new Vector3(0, 0, 0))
            leftHandGoal.position = MainMgr.inst.leftArmGoal[modelIndex].v3();
            //if (rightHandGoal != null && MainMgr.inst.rightArmGoal[modelIndex].v3() != new Vector3(0, 0, 0))
            rightHandGoal.position = MainMgr.inst.rightArmGoal[modelIndex].v3();
            //leg
            leftLegTarget.position = leftTkr.pos - new Vector3(0, 0.1f, 0); //腳踝到腳底板的offset
            rightLegTarget.position = rightTkr.pos - new Vector3(0, 0.1f, 0); //腳踝到腳底板的offset
            leftLegTarget.rotation = leftTkr.rot * Quaternion.Inverse(leftLegInitRot) * leftLegTargetRot;
            rightLegTarget.rotation = rightTkr.rot * Quaternion.Inverse(rightLegInitRot) * rightLegTargetRot;


            Debug.Log(rightHandTargetNode.transform.position);

        }

        private void logTargetInitRotation()
        {
            leftArmTargetRot = leftHandTarget.rotation;
            rightArmTargetRot = rightHandTarget.rotation;
            leftLegTargetRot = leftLegTarget.rotation;
            rightLegTargetRot = rightLegTarget.rotation ;
        }

        private void updateModelTransform()
        {
            pelvisPosition = transform.Find("mixamorig:Hips");
            Debug.Log("i want to know is Target Head i catch?", pelvisPosition);
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
        private void scaleByHand(float handDistance)
        {
            if (handDistance == -1)
                return;


            float scale = (handDistance - controllerLen) / (modelHandDis - controllerLen);
            Debug.Log("scale model " + scale + " time to fit, model hand dis" + modelHandDis);
            transform.localScale = transform.localScale * scale*2;
        }

        private void setTargetGroup()
        {
            //保留原本model設定
            if (is_CustomModel)
            {
                //SetRightTarget
                string rightHandPath = "mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:RightShoulder/mixamorig:RightArm/mixamorig:RightForeArm/mixamorig:RightHand";
                rightHandTargetNode = new GameObject("rightHandTarget");
                rightHandTargetNode.transform.SetParent(this.transform.Find(rightHandPath));
                rightHandTargetNode.transform.localPosition = new Vector3(0, 0.01f, 0); //避免node Target 座標重合
                rightHandTargetNode.transform.SetParent(this.transform, true);
                //SetLeftTarget
                string leftHandPath = "mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:LeftShoulder/mixamorig:LeftArm/mixamorig:LeftForeArm/mixamorig:LeftHand";
                leftHandTargetNode = new GameObject("leftHandTarget");
                leftHandTargetNode.transform.SetParent(this.transform.Find(leftHandPath));
                leftHandTargetNode.transform.localPosition = new Vector3(0, 0.01f, 0); //避免node Target 座標重合
                leftHandTargetNode.transform.SetParent(this.transform, true);

            GameObject leftshoulder = GameObject.Find("mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:LeftShoulder/mixamorig:LeftArm");
            leftshoulder.transform.Rotate(0, 180, 0);
            leftshoulder.transform.GetChild(0).Rotate(0, 180, 0);
            //other Targets

            rightHandGoalNode = new GameObject("rightHandGoalNode");
                leftHandGoalNode = new GameObject("leftHandGoalNode");
                rightLegTargetNode = new GameObject("rightLegTargetNode");
                leftLegTargetNode = new GameObject("leftLegTargetNode");
                headTargetNode = new GameObject("headTargetNode");

                //other offset setting
                headTargetNode.transform.position += new Vector3(0, 2, 0);
                rightLegTargetNode.transform.localRotation = Quaternion.Euler(0, 180, 0);
                leftLegTargetNode.transform.localRotation = Quaternion.Euler(0, 180, 0);

                //set target node to target
                rightHandTarget = rightHandTargetNode.transform;
                leftHandTarget = leftHandTargetNode.transform;
                rightHandGoal = rightHandGoalNode.transform;
                leftHandGoal = leftHandGoalNode.transform;
                rightLegTarget = rightLegTargetNode.transform;
                leftLegTarget = leftLegTargetNode.transform;
                headTarget = headTargetNode.transform;


            is_catch = true;
            }

            logTargetInitRotation();
    }

    }


