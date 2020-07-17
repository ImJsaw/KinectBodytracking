//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class Modelhand : MonoBehaviour
//{
//    private HandController m_hand = null;
//    public ModelController m_model = null;
//    public bool is_Left = true;

//    // Start is called before the first frame update
//    void Start()
//    {

//        if (is_Left)
//            m_hand = GameObject.Find("Controller (left)").GetComponent<HandController>();
//        else
//            m_hand = GameObject.Find("Controller (right)").GetComponent<HandController>();

//        m_hand.modelHand = this;
//        m_hand.m_Joint = GetComponent<FixedJoint>();

//        Debug.Log("mhand :", m_hand); Debug.Log("start");
//    }

//    // Update is called once per frame
//    void Update()
//    {
        
//    }

//    private void OnTriggerEnter(Collider other)
//    {
//        //print("isTrigger!");
//        //if (!other.gameObject.CompareTag("interactable"))
//        //    return;

//        ////if (m_model.modelIndex != 0)  //確定是不是自己
//        ////    return;
        
//        //m_hand.m_ContactInteractables.Add(other.gameObject.GetComponent<Interactable>());

//    }

//    private void OnTriggerExit(Collider other)
//    {

//        //if (!other.gameObject.CompareTag("interactable"))
//        //    return;

//        ////if (m_model.modelIndex != 0)  //確定是不是自己
//        ////    return;

//        //m_hand.m_ContactInteractables.Remove(other.gameObject.GetComponent<Interactable>());
//    }
//}
