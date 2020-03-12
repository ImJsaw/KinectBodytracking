using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneID : int {
    None = -1,
    Start,
    Connect,
    Server,
    Client,
};

[CLSCompliant(false)]
public class MainMgr : MonoBehaviour {

    private static MainMgr _inst = null;
    public static MainMgr inst {
        get {
            if (_inst == null) {
                _inst = new MainMgr();
            }
            return _inst;
        }
    }

    public TcpClient client = null;
    public TcpServer server = null;
    public ClientListener clientListener = null;
    public ServerListener serverListener = null;
    public bool getListenerComplete = false;
    
    SceneID curScene = SceneID.None;

    void Awake() {
        DontDestroyOnLoad(this);
    }

    void Start() {
        client = GameObject.Find("TCP_Client").GetComponent<TcpClient>();
        server = GameObject.Find("TCP_Server").GetComponent<TcpServer>();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void gotoServerSelect() {
        changeScene(SceneID.Connect);
    }

    public void changeScene(SceneID sceneID) {
        SceneManager.LoadScene((int)sceneID);
        curScene = sceneID;
    }

    public void setServerIP(string ip) {
        try {
            client.InitSocket(ip);
        }
        catch (SocketException e) {
            Debug.Log(e);
        }
    }

    SceneID getCurScenID(Scene scene) {
        int sceneID = scene.buildIndex;
        switch (sceneID) {
            case 0:
                return SceneID.Start;
            case 1:
                return SceneID.Connect;
            case 2:
                return SceneID.Server;
            case 3:
                return SceneID.Client;
        }
        return SceneID.None;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        SceneID id = getCurScenID(scene);
        switch (id) {
            case SceneID.Client:
                clientListener = GameObject.Find("Listener").GetComponent<ClientListener>();
                if (clientListener == null)
                    Debug.Log("null remote");
                else {
                    getListenerComplete = true;
                    Debug.Log("complete find listener");
                }
                break;
            case SceneID.Server:
                serverListener = GameObject.Find("Listener").GetComponent<ServerListener>();
                if (serverListener == null)
                    Debug.Log("null remote");
                else {
                    getListenerComplete = true;
                    Debug.Log("complete find listener");
                }
                break;
        }
    }

}
