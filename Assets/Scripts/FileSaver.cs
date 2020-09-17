using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FileSaver : MonoBehaviour
{

    public InputField ObjName;

    // Start is called before the first frame update
    void Start()
    {
        ObjName.text = "SOP";
        //SaveModel2();
    }

    void Awake()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SaveModel()
    {
        Utility.saveModel(ObjName.text);
    }

    public void SaveModel2()
    {
        GameObject customModel = Utility.loadModelWithTex("Fat");
        //customModel.transform.localScale = new Vector3(100, 100, 100);
        GameObject newer = Instantiate(customModel);
        newer.AddComponent<IKModelController>();
        newer.AddComponent<RootMotion.FinalIK.IKauto>();
        //newer.AddComponent<Lock>();
        IKModelController IK = newer.GetComponent<IKModelController>();
    }

}
