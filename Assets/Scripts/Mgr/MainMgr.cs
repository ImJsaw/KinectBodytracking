using Microsoft.Azure.Kinect.Sensor.BodyTracking;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum SceneID : int {
    None = -1,
    Start,
    Connect,
    Server,
    Client,
    Calibration,
    General
};

[CLSCompliant(false)]
public class MainMgr : MonoBehaviour {
    
    public static MainMgr inst = null;

    public ModelController modelPrefab = null;
    //components
    [HideInInspector]
    public TcpClient client = null;
    [HideInInspector]
    public TcpServer server = null;
    [HideInInspector]
    public ClientListener clientListener = null;
    [HideInInspector]
    public ServerListener serverListener = null;
    [HideInInspector]
    public bool getListenerComplete = false;
    //game settings
    public static bool isClient = false;
    public static bool isVRValid = false;
    public static bool isCamValid = false;
    //read only settings
    private static readonly Vector3 INIT_CAM_POS = new Vector3(0, 0, -10);

    //panel queue
    public Queue<string> panelWaitingList = new Queue<string>();

    //models data
    //public List<Quaternion[]> modelRot = new List<Quaternion[]>();
    //public List<Vector3> modelPos = new List<Vector3>();
    [HideInInspector]
    public List<Vector3> mapPos = new List<Vector3>();
    [HideInInspector]
    public List<Skeleton> skeletons = new List<Skeleton>();
    [HideInInspector]
    public List<bool> isFirstDataGet = new List<bool>();
    [HideInInspector]
    public int modelSum = 0;

    //default instantiate
    private Vector3 initPos = new Vector3(-12, -2.5f, -6.16f);
    private Dictionary<string, int> playerUIDDict = new Dictionary<string, int>();

    [HideInInspector]
    private string _myUID = "";


    SceneID curScene = SceneID.None;

    void Awake() {
        if (inst == null) {
            inst = this;
            DontDestroyOnLoad(this);
        } else if (this != inst) {
            Destroy(gameObject);
        }
    }

    void Start() {
        client = GameObject.Find("TCP_Client").GetComponent<TcpClient>();
        server = GameObject.Find("TCP_Server").GetComponent<TcpServer>();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    //called when new user enter
    public void addNewModel(string UID) {
        //log  UID/index in dictionary
        playerUIDDict.Add(UID, modelSum);
        //init data
        mapPos.Add(INIT_CAM_POS);
        skeletons.Add(new Skeleton());
        isFirstDataGet.Add(false);
        //instantiate model & set index
        ModelController modelInstant = Instantiate(modelPrefab);
        modelInstant.modelIndex = modelSum;
        Debug.Log("[model instantiate] generate " + modelSum + " th model");
        modelSum++;
    }

    public void gotoServerSelect(string myAccount) {
        _myUID = myAccount;
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
            case 4:
                return SceneID.Calibration;
            case 5:
                return SceneID.General;

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
            case SceneID.General:
                addNewModel(_myUID);
                break;
            default:
                Debug.Log("[Scene] goto "+id+" scene");
                break;
        }
    }

    public int getIndexfromUID(string UID) {
        if (!playerUIDDict.ContainsKey(UID))
            addNewModel(UID);
        return playerUIDDict[UID];
    }

    public string myUID() {
        return _myUID;
    }

}
