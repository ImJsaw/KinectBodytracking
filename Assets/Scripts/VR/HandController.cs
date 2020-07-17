//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using Valve.VR;

//public class HandController : MonoBehaviour
//{

//    public SteamVR_Action_Boolean m_GrabAction = null;

//    private SteamVR_Behaviour_Pose m_Pose = null;
//    [HideInInspector]
//    public FixedJoint m_Joint = null;

//    [HideInInspector]
//    public Modelhand modelHand = null;

//    private Interactable m_CurrentInteractable = null;
//    [HideInInspector]
//    public List<Interactable> m_ContactInteractables = new List<Interactable>();

//    private void Awake()
//    {
//        m_Pose = GetComponent<SteamVR_Behaviour_Pose>();
//        m_Joint = modelHand.GetComponent<FixedJoint>();
//        print("m_Joint  :" + m_Joint);
//    }
//    // Start is called before the first frame update
//    void Start()
//    {
        
//    }

//    // Update is called once per frame
//    void Update()
//    {
//        //Down 
//        if(m_GrabAction.GetStateDown(m_Pose.inputSource))
//        {
//            print(m_Pose.inputSource + " trigger down");
//            Pickup();
//        }

//        //Up 
//        if (m_GrabAction.GetStateUp(m_Pose.inputSource))
//        {
//            print(m_Pose.inputSource + " trigger up");
//            Drop();
//        }
//    }

//    private void OnTriggerEnter(Collider other)
//    {
//        if (!other.gameObject.CompareTag("interactable"))
//            return;

//        m_ContactInteractables.Add(other.gameObject.GetComponent<Interactable>());

//        print("m_ContactInteractables :" + m_ContactInteractables);
//    }

//    private void OnTriggerExit(Collider other)
//    {
//        if (!other.gameObject.CompareTag("interactable"))
//            return;

//        m_ContactInteractables.Remove(other.gameObject.GetComponent<Interactable>());
//    }

//    public void Pickup()
//    {
//        //Get nearest 
//        m_CurrentInteractable = GetNearestInteractable();
//        print("connectbody :" + m_CurrentInteractable);
//        //null check
//        if (!m_CurrentInteractable)
//            return;

//        //Already held
//        if (m_CurrentInteractable.m_ActiveHand)
//            m_CurrentInteractable.m_ActiveHand.Drop();

//        //Position
//        m_CurrentInteractable.transform.position = modelHand.transform.position;

//        //Attach
//        Rigidbody targetBody = m_CurrentInteractable.GetComponent<Rigidbody>();
//        m_Joint.connectedBody = targetBody;

       
//        //Set Active Hand
//        m_CurrentInteractable.m_ActiveHand = this;
//    }

//    public void Drop()
//    {
//        // null check
//        if (!m_CurrentInteractable)
//            return;

//        //Apply velocity
//        Rigidbody targetBody = m_CurrentInteractable.GetComponent<Rigidbody>();
//        targetBody.velocity = m_Pose.GetVelocity();
//        targetBody.angularVelocity = m_Pose.GetAngularVelocity();

//        //Detach 
//        m_Joint.connectedBody = null;

//        //Clear
//        m_CurrentInteractable.m_ActiveHand = null;
//        m_CurrentInteractable = null;


//    }

//    private Interactable GetNearestInteractable()
//    {
//        Interactable nearest = null;
//        float minDistant = float.MaxValue;
//        float distance = 0.0f;

//        foreach(Interactable interactable in m_ContactInteractables)
//        {
//            distance = (interactable.transform.position - modelHand.transform.position).sqrMagnitude;

//            if(distance < minDistant)
//            {
//                minDistant = distance;
//                nearest = interactable;
//            }
//        }
//        return nearest;
//    }
//}

