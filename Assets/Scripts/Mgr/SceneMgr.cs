﻿using System;
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

    public void gotoCalibration() {
        //save vr setting
        MainMgr.inst.hasVR[0] = MainMgr.isVRValid;
        mainMgr.changeScene(SceneID.Calibration);
    }
    public void gotoGeneral() {
        mainMgr.changeScene(SceneID.General);
        //save vr setting
        MainMgr.inst.hasVR[0] = MainMgr.isVRValid;
    }

    public void gotoNewClient() {
        mainMgr.setServerIP(ipAddress.text);
        mainMgr.changeScene(SceneID.General);
    }

    ////////////////////////////

    // change scene

}
