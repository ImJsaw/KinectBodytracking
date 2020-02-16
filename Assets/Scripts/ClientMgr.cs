using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ClientMgr : MonoBehaviour {

    MainMgr mainMgr = null;

    void Awake() {
        mainMgr = GameObject.Find("MainMgr").GetComponent<MainMgr>();
    }

    void Start() {
        mainMgr.printCur();
    }
    
    void Update() {

    }

}
