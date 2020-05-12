using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class Follow : MonoBehaviour {

    public Transform pelvisTracker = null;

    private Quaternion initRotate = Quaternion.identity;

    public SteamVR_Action_Boolean m_InitAction;
    private SteamVR_Behaviour_Pose m_Pose = null;

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {

        if (m_InitAction.GetStateDown(m_Pose.inputSource)) {
            Debug.Log("trigger");
            initRotate = pelvisTracker.rotation;
        }
        transform.position = pelvisTracker.transform.position;
        transform.rotation = pelvisTracker.transform.rotation * Quaternion.Inverse(initRotate);
    }
}
