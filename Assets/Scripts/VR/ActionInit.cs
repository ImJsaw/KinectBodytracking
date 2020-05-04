using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class ActionInit : MonoBehaviour
{

    public SteamVR_Action_Boolean m_InitAction;
    private SteamVR_Behaviour_Pose m_Pose = null;


    // Start is called before the first frame update
    void Start()
    {
        m_Pose = GetComponent<SteamVR_Behaviour_Pose>();
    }

    // Update is called once per frame
    void Update()
    {
        //Pointer
        if (m_InitAction.GetStateDown(m_Pose.inputSource))
            InitModel();
       
    }

    //What you want to do
    private void InitModel()
    {

    }

}
