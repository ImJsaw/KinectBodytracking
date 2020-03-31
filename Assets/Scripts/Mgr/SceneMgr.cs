using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[CLSCompliant(false)]
public class SceneMgr : MonoBehaviour {

    public InputField ipAddress = null;

    public Toggle isCamConnect = null;
    public Toggle isVRConnect = null;

    MainMgr mainMgr = null;

    public
    // Use this for initialization
    void Start() {
        mainMgr = MainMgr.inst;
        //default
        ipAddress.text = "140.118.127.113";
    }

    // Update is called once per frame
    void Update() {
        MainMgr.isCamValid = isCamConnect.isOn;
        MainMgr.isVRValid = isVRConnect.isOn;
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

    public void gotoCalibration() {
        mainMgr.changeScene(SceneID.Calibration);
    }
    public void gotoGeneral() {
        mainMgr.changeScene(SceneID.General);
    }

    public void gotoNewClient() {
        mainMgr.setServerIP(ipAddress.text);
        mainMgr.changeScene(SceneID.General);
    }

    ////////////////////////////

    // change scene

}
