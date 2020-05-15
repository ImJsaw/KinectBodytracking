using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SelectModel : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject arror;
    public GameObject[] modelList;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Ray();
    }

    void Ray()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        int modelindex;
        if (Input.GetMouseButtonUp(0) && Physics.Raycast(ray, out hit))
        {
            if(hit.transform.tag == "model")
            {
                arror.transform.position = hit.transform.position + new Vector3(0, 2.0f, 0);

                modelindex = Array.IndexOf(modelList, hit.transform.gameObject);
                int index = MainMgr.inst.getIndexfromUID(MainMgr.inst.myUID());
                MainMgr.inst.setModelType(index, modelindex);
                MainMgr.inst.setModelType(0, 0);
            }
           // Debug.DrawLine(Camera.main.transform.position, hit.transform.position, Color.red, 0.1f, true);
            
        }
    }
}
