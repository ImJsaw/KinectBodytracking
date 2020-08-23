using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CLSCompliant(false)]
public class General : MonoBehaviour {

    public ObsController obsController;
    public IKModelController[] modelPrefab;
    public IKModelController[] modelPrefabSelf;
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
            IKModelController ikModelPrefab = new IKModelController();
            if (MainMgr.inst.is_custom)
            {
                GameObject customModel = MainMgr.inst.customModelList[0];
                GameObject newer = Instantiate(customModel, new Vector3(0, 0, -6), Quaternion.identity);
                newer.AddComponent<IKModelController>();
                newer.AddComponent<RootMotion.FinalIK.IKauto>();
                ikModelPrefab = newer.GetComponent<IKModelController>();
            }
            else
            {
                ikModelPrefab = Instantiate(modelPrefabSelf[MainMgr.inst.modelType[index]], new Vector3(0, 0, -6), Quaternion.identity);
            }
            ikModelPrefab.modelIndex = index;
        } else {
            IKModelController ikModelPrefab = Instantiate(modelPrefab[MainMgr.inst.modelType[index]], new Vector3(0, 0, -6), Quaternion.identity);
            ikModelPrefab.modelIndex = index;
        }
        Debug.Log("[model instantiate] generate " + index + " th model");
    }
}