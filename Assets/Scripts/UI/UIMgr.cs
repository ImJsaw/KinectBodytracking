using System;
using System.Collections.Generic;
using UnityEngine;

[CLSCompliant(false)]
public class UIMgr {
    //single inst
    private static UIMgr _inst = null;
    public static UIMgr inst {
        get {
            if (_inst == null)
                _inst = new UIMgr();
            return _inst;
        }
    }

    public GameObject canvasRoot;
    //save opening panel
    public Dictionary<string, GameObject> panelList = new Dictionary<string, GameObject>();
    //panel prefab route
    private readonly string UI_GAMEPANEL_ROOT = "Prefabs/Panels/";

    //stop runnung when UI shows
    public bool isStop = false;

    private bool isCanvasRootNull() {
        if (canvasRoot == null) {
            Debug.LogError("forget add CanvasRoot.cs on canvas?");
            return true;
        }
        return false;
    }

    private bool isPanelLive(string name) {
        return panelList.ContainsKey(name);
    }

    public GameObject generatePanel(string name) {
        if (isCanvasRootNull())
            return null;

        if (isPanelLive(name)) {
            Debug.LogErrorFormat("[{0}] is Showing, if you want to show, please close first!!", name);
            return null;
        }

        GameObject panelPrefab = Utility.resLoad<GameObject>(UI_GAMEPANEL_ROOT + name);
        if (panelPrefab == null)
            return null;
        
        GameObject panel = Utility.instantiateGameObject(canvasRoot, panelPrefab);
        panel.name = name;
        //save opened panel
        panelList.Add(name, panel);
        isStop = true;
        return panel;
    }

    public void setPanelActive(string name, bool isOn) {
        if (isPanelLive(name)) {
            if (panelList[name] != null)
                panelList[name].SetActive(isOn);
        } else {
            Debug.LogErrorFormat("[TogglePanel] [{0}] not found.", name);
        }
    }

    public void destroyPanel(string name) {
        if (isPanelLive(name)) {
            if (panelList[name] != null)
                UnityEngine.Object.Destroy(panelList[name]);
            panelList.Remove(name);
        } else {
            Debug.LogErrorFormat("[ClosePanel] [{0}] not found.", name);
        }
    }

    public void closeAllPanel() {
        foreach (KeyValuePair<string, GameObject> item in panelList) {
            if (item.Value != null)
                item.Value.SetActive(false);
        }
        isStop = false;
    }

    public void destroyAllPanel() {
        foreach (KeyValuePair<string, GameObject> item in panelList) {
            if (item.Value != null)
                UnityEngine.Object.Destroy(item.Value);
        }
        panelList.Clear();
    }

    public Vector2 canvasSize() {
        if (isCanvasRootNull())
            return Vector2.one * -1;
        RectTransform trans = canvasRoot.transform as RectTransform;
        return trans.sizeDelta;
    }
}
