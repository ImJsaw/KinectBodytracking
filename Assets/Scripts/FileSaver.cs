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
    GameObject customModel =  Utility.loadModelWithTex(ObjName.text);
    customModel.transform.localScale = new Vector3(100, 100, 100);
    MainMgr.inst.customModelList.Add(customModel);
    MainMgr.inst.is_custom = true;
    }

    public void SaveModel2()
    {
        GameObject customModel = Utility.loadModelWithTex("SOP");
        
        GameObject newer = Instantiate(customModel);
        newer.AddComponent<IKModelController>();
        newer.AddComponent<RootMotion.FinalIK.IKauto>();
        IKModelController IK = newer.GetComponent<IKModelController>();
    }
}
