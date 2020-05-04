using UnityEngine;
using System;
using Valve.VR;

[CLSCompliant(false)]
public class ModelTest : MonoBehaviour {

    //test mode
    public bool demoMode = true;
    public Transform modelPosition = null;
    public Transform helmetPosition = null;
    //vr tracker
    public Transform leftCtr = null;
    public Transform rightCtr = null;
    public Transform leftTkr = null;
    public Transform rightTkr = null;
    //apply tracker pos to target
    public Transform leftHandTarget = null;
    public Transform rightHandTarget = null;
    public Transform leftLegTarget = null;
    public Transform rightLegTarget = null;
    //init rotation
    private Quaternion leftArmInitRot = Quaternion.identity;
    private Quaternion rightArmInitRot = Quaternion.identity;
    private Quaternion leftLegInitRot = Quaternion.identity;
    private Quaternion rightLegInitRot = Quaternion.identity;
    private Quaternion pelvisInitRot = Quaternion.identity;

    public SteamVR_Action_Boolean m_InitAction;
    private SteamVR_Behaviour_Pose m_Pose = null;

    private Vector3 hmtPos;

    private void updateModelTransform() {
        //make model horizon move with cam
        modelPosition.position = new Vector3(hmtPos.x, modelPosition.position.y, hmtPos.z);
    }

    void Start() {
        m_Pose = rightCtr.GetComponent<SteamVR_Behaviour_Pose>();
        pelvisInitRot = transform.rotation;
        if (demoMode)
            updateInitRotation();
    }

    void Update() {
        if (m_Pose != null) {

            if (m_InitAction.GetStateDown(m_Pose.inputSource)) {
                Debug.Log("trigger");
                updateInitRotation();
            }
        }

        if (helmetPosition != null) {
            //position
            hmtPos = helmetPosition.position;
        }

        leftHandTarget.position = leftCtr.position;
        rightHandTarget.position = rightCtr.position;

        leftHandTarget.rotation = leftCtr.rotation * Quaternion.Inverse(leftArmInitRot) * Quaternion.Inverse(pelvisInitRot);
        rightHandTarget.rotation = rightCtr.rotation * Quaternion.Inverse(rightArmInitRot) * Quaternion.Inverse(pelvisInitRot);

        leftLegTarget.position = leftTkr.position;
        rightLegTarget.position = rightTkr.position;
        leftLegTarget.rotation = leftTkr.rotation * Quaternion.Inverse(leftLegInitRot);
        rightLegTarget.rotation = rightTkr.rotation * Quaternion.Inverse(rightLegInitRot);


        updateModelTransform();

    }

    private void updateInitRotation() {
        leftArmInitRot = leftCtr.rotation;
        rightArmInitRot = rightCtr.rotation;
        leftLegInitRot = leftTkr.rotation;
        rightLegInitRot = rightTkr.rotation;

    }

}


