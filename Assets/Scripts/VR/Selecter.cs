using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using System;

public class Selecter : MonoBehaviour
{

    public GameObject m_Pointer;
    public GameObject[] modelList;
    public SteamVR_Action_Boolean m_TeleportAction;

    private SteamVR_Behaviour_Pose m_Pose = null;
    private bool m_HasPosition = false;
    private bool m_IsGround = false;

    private int modelindex;
   


    // Start is called before the first frame update
    void Start()
    {
        m_Pose = GetComponent<SteamVR_Behaviour_Pose>();
        
    }

    // Update is called once per frame
    void Update()
    {

        m_HasPosition = UpdatePointer();
        m_Pointer.SetActive(m_HasPosition);

        //Pointer
        if (m_TeleportAction.GetStateDown(m_Pose.inputSource))
        {
            
        }


        //Teleport
        if (m_TeleportAction.GetStateUp(m_Pose.inputSource))
        {
            Debug.Log("clickUp");
            SelectModle();
        }
            
    }

    private void SelectModle()
    {
        //Check for valid position ,and if already teleporting
        if (!m_HasPosition)
            return;

        MainMgr.inst.setModelType(0, modelindex);
    }


    private bool UpdatePointer()
    {
        // Ray from controller 
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        //if it's a hit
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.tag == "model")
            {
                 m_Pointer.transform.position = hit.point;     
                 modelindex = Array.IndexOf(modelList, hit.transform.gameObject);
                Debug.Log("find!!");
                return true;
            }
        }
        //if it's not a hit
        return false;
    }
}
