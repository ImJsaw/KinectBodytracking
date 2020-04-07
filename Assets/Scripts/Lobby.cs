using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[CLSCompliant(false)]
public class Lobby : MonoBehaviour {
    public InputField account = null;

    public void login() {
        MainMgr.inst.gotoServerSelect(account.text);
    }
}
