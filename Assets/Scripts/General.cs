using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CLSCompliant(false)]
public class General : MonoBehaviour {

    public IKModelController[] modelPrefabVR;
    public ModelController modelPrefab = null;
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
        IKModelController modelPrefab = Instantiate(modelPrefabVR[MainMgr.inst.modelType[index]]);
        modelPrefab.modelIndex = index;

        Debug.Log("[model instantiate] generate " + index + " th model");
    }
}