using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CLSCompliant(false)]
public class General : MonoBehaviour {

    public ObsController obsController;
    public GameObject[] modelPrefab;
    public GameObject[] modelPrefabSelf;
    public string[] prefix;
    //public ModelController modelPrefab = null;
    private Dictionary<string, int> localPlayerUIDDict = new Dictionary<string, int>();

    // Update is called once per frame
    void Update() {
        checkNewPlayer();
    }

    private void checkNewPlayer() {
        foreach (KeyValuePair<string, int> kvp in MainMgr.inst.getUIDDect()) {
            Debug.Log("key : " + kvp.Key + ", " + kvp.Value);
            if (!localPlayerUIDDict.ContainsKey(kvp.Key))
                addNewPlayer(kvp.Key, kvp.Value);
        }
    }

    private void addNewPlayer(string UID, int index) {
        //log  UID/index in local dictionary
        localPlayerUIDDict.Add(UID, index);

        //instantiate model & set index
        //generate chosed type
        if (MainMgr.inst.modelType[index] == -1) {
            //observer mode
            ObsController obsModelPrefab = Instantiate(obsController);
            obsModelPrefab.modelIndex = index;
        } else if (UID == MainMgr.inst.myUID() ) {
            //if self, generate no head model
            GameObject model;
            string prefixStr = "mixamorig:";
            if (MainMgr.inst.is_custom)
            {
                GameObject customModel = MainMgr.inst.customModelList[0];
                model = Instantiate(customModel, new Vector3(0, 0, -6), Quaternion.identity);
                model.transform.localScale = new Vector3(100, 100, 100);
            }
            else
            {
                prefixStr = prefix[MainMgr.inst.modelType[index]];
                model = Instantiate(modelPrefabSelf[MainMgr.inst.modelType[index]], new Vector3(0, 0, -6), Quaternion.identity);
            }
            model.AddComponent<IKModelController>();
            model.AddComponent<RootMotion.FinalIK.IKauto>();
            model.GetComponent<RootMotion.FinalIK.IKauto>().setPrefix(prefixStr);   
            //model.AddComponent<Lock>();
            Debug.Log(MainMgr.inst.modelType[index] + "add prefix : " + prefixStr);
            model.GetComponent<IKModelController>().prefix = prefixStr;
            model.GetComponent<IKModelController>().modelIndex = index;
            if (MainMgr.inst.is_custom) {
                model.GetComponent<IKModelController>().is_CustomModel = true;
             }
        } else {
            GameObject model = Instantiate(modelPrefab[MainMgr.inst.modelType[index]], new Vector3(0, 0, -6), Quaternion.identity);
            model.GetComponent<IKModelController>().modelIndex = index;
        }
        Debug.Log("[model instantiate] generate " + index + " th model");
    }
}