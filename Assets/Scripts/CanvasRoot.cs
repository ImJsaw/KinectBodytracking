using System;
using UnityEngine;

[CLSCompliant(false)]
public class CanvasRoot : MonoBehaviour {
    void Awake() {
        UIMgr.inst.canvasRoot = gameObject;
    }
}