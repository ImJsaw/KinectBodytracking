using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class Follow : MonoBehaviour {

    public Transform pelvisTracker = null;
    public GameObject ctr = null;
    public Transform hip = null;

    private Quaternion initRotate = Quaternion.identity;

    private Vector3 hipInit;

    public SteamVR_Action_Boolean m_InitAction;
    private SteamVR_Behaviour_Pose m_Pose = null;

    // Start is called before the first frame update
    void Start() {
        m_Pose = ctr.GetComponent<SteamVR_Behaviour_Pose>();
        hipInit = transform.position;
    }

    // Update is called once per frame
    void Update() {

        if (m_InitAction.GetStateDown(m_Pose.inputSource)) {
            Debug.Log("trigger");
            initRotate = pelvisTracker.rotation;
        }
        hip.transform.position = pelvisTracker.transform.position;
        hip.transform.rotation = pelvisTracker.transform.rotation * Quaternion.Inverse(initRotate);
    }
}
