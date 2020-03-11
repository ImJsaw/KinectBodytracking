using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneID : int {
    None = -1,
    Start,
    Connect,
    Server,
    Client,
};

public class MainMgr : MonoBehaviour {

    public static MainMgr inst = null;

    public TcpClient client = null;
    public TcpServer server = null;
    public ClientListener clientListener = null;
    public ServerListener serverListener = null;
    public bool getListenerComplete = false;

    private int test = 0;
    SceneID curScene = SceneID.None;
    
    void Awake() {
        if (inst == null) {
            Debug.Log("init");
            inst = this;
            DontDestroyOnLoad(this);
        }
        else if (this != inst) {
            Debug.Log("del duplicate");
            Destroy(gameObject);
        }
    }

    void Start() {
        printCur();
        client = GameObject.Find("TCP_Client").GetComponent<TcpClient>();
        server = GameObject.Find("TCP_Server").GetComponent<TcpServer>();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    void Update() {
    }

    //public void onMsgRcv(byte[] data) {
    //    switch (curScene) {
    //        case SceneID.Client:
    //            //if(remoteListener == null)
    //            //    remoteListener = GameObject.Find("Listener").GetComponent<ClientListener>();
    //            if(!getListenerComplete || clientListener == null) {
    //                Debug.Log("null remote");
    //                break;
    //            }
    //            clientListener.updateBody(data);
    //            Debug.Log("/////MGR LEN" + data.Length);
    //            break;
    //        default:
    //            Debug.Log("[msg rcv]Scene error!"); ;
    //            break;
    //    }
    //}

    public void printCur() {
        Debug.Log("Cur : " + test);
        test++;
    }

    public void gotoServerSelect() {
        changeScene(SceneID.Connect);
    }

    public void changeScene(SceneID sceneID) {
        SceneManager.LoadScene((int)sceneID);
        curScene = sceneID;
    }

    public void setServerIP(string ip) {
        client.InitSocket(ip);
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
                if (clientListener == null) Debug.Log("null remote");
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
