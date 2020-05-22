using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SelectModel : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject arror;
    public GameObject[] modelList;
    private Animator[] animatorList;
    private int lastIndex = -1;

    void Start()
    {
        animatorList = new Animator[modelList.Length];
        for (int i=0;i<modelList.Length;i++)
        {
            animatorList[i] = modelList[i].GetComponent<Animator>();
        }

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
                modelAnimator(modelindex);
            }
           // Debug.DrawLine(Camera.main.transform.position, hit.transform.position, Color.red, 0.1f, true);
            
        }
    }

    void modelAnimator(int modelindex)
    {
        if(lastIndex != -1)
        {
            Debug.Log(lastIndex);
            animatorList[lastIndex].SetBool("isSelected", false);
        }

  
        animatorList[modelindex].SetBool("isSelected", true);
        lastIndex = modelindex;
    }
}
