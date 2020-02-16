using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneMgr : MonoBehaviour {

    public InputField ipAddress = null;

    MainMgr mainMgr = null;

    public
    // Use this for initialization
    void Start() {
        mainMgr = GameObject.Find("MainMgr").GetComponent<MainMgr>();
        //default
        ipAddress.text = "140.118.127.113";
    }

    // Update is called once per frame
    void Update() {

    }

    /// for button event /////
    public void gotoServerScene() {
        mainMgr.changeScene(SceneID.Server);
    }

    public void gotoClientScene() {
        mainMgr.changeScene(SceneID.Client);
        //set server ip
        mainMgr.setServerIP(ipAddress.text);
    }
    ////////////////////////////
    
    // change scene

}
