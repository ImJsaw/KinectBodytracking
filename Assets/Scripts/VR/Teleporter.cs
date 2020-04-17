using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class Teleporter : MonoBehaviour
{

    private GameObject m_Pointer;
    public SteamVR_Action_Boolean m_TeleportAction;

    private SteamVR_Behaviour_Pose m_Pose = null;
    private bool m_HasPosition = false;
    private bool m_IsTeleporting = false;
    private bool m_IsGround = false;
    private float m_FadeTime = 0.5f;


    // Start is called before the first frame update
    void Start()
    {
        m_Pose = GetComponent<SteamVR_Behaviour_Pose>();
        m_Pointer = GameObject.FindWithTag("target");
    }

    // Update is called once per frame
    void Update()
    {
        //Pointer
        if (m_TeleportAction.GetStateDown(m_Pose.inputSource))
            m_HasPosition = UpdatePointer();
            m_Pointer.SetActive(m_HasPosition);

        //Teleport
        if (m_TeleportAction.GetStateUp(m_Pose.inputSource))
            TryTeleport();
    }

    private void TryTeleport()
    {
        //Check for valid position ,and if already teleporting
        if (!m_HasPosition || m_IsTeleporting)
            return;

        //Get Camera rig , and head position
        Transform cameraRig = SteamVR_Render.Top().origin;
        Vector3 headPosition = SteamVR_Render.Top().head.position;

        //Figure out translation
        Vector3 grounPosition = new Vector3(headPosition.x, cameraRig.position.y, headPosition.z);
        Vector3 translateVector = m_Pointer.transform.position - grounPosition;

        //Move
        StartCoroutine(MoveRig(cameraRig, translateVector));


    }

    private IEnumerator MoveRig(Transform cameraRig, Vector3 translation)
    {
        //Flag
        m_IsTeleporting = true;

        //Fade to Black
        SteamVR_Fade.Start(Color.black, m_FadeTime, true);

        //apply translation
        yield return new WaitForSeconds(m_FadeTime);
        cameraRig.position += translation;

        //Fade to clear
        SteamVR_Fade.Start(Color.clear, m_FadeTime, true);

        //De-Flag
        m_IsTeleporting = false;
    }

    private bool UpdatePointer()
    {
        // Ray from controller 
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        //if it's a hit
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.tag == "ground")
            {
                m_Pointer.transform.position = hit.point;
                return true;
            }
        }
        //if it's not a hit
        return false;
    }
}
