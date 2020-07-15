using UnityEngine;
using System;

[CLSCompliant(false)]
public class ObsController : MonoBehaviour {

    //model index
    int _modelIndex;

    [HideInInspector]
    public int modelIndex {
        get {
            if (_modelIndex == -1)
                Debug.LogError("model number not set");
            return _modelIndex;
        }
        set {
            _modelIndex = value;
            if (_modelIndex < 0)
                Debug.LogError("model index < 0 !");
            if (_modelIndex == -1)
                Debug.LogError("model number not set");
        }
    }

    public void Update() {
        transform.position = MainMgr.inst.headPos[modelIndex].pos;
        //Debug.Log(MainMgr.inst.headPos[modelIndex].pos);
        //Debug.Log("model index: " + modelIndex);
    }
}


