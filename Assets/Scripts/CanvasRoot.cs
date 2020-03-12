using System;
using UnityEngine;

[CLSCompliant(false)]
public class UIRootHandler : MonoBehaviour {
    void Awake() {
        UIMgr.inst.canvasRoot = gameObject;
    }
}