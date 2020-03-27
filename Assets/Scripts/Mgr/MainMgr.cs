using Microsoft.Azure.Kinect.Sensor.BodyTracking;
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
    Calibration,
    General
};

[CLSCompliant(false)]
public class MainMgr : MonoBehaviour {
    
    public static MainMgr inst = null;

    public ModelController model = null;
    public TcpClient client = null;
    public TcpServer server = null;
    public ClientListener clientListener = null;
    public ServerListener serverListener = null;
    public bool getListenerComplete = false;
    
    //panel queue
    public Queue<string> panelWaitingList = new Queue<string>();

    //models data
    //public List<Quaternion[]> modelRot = new List<Quaternion[]>();
    //public List<Vector3> modelPos = new List<Vector3>();
    public List<Vector3> mapPos = new List<Vector3>();
    public List<Skeleton> skeletons = new List<Skeleton>();
    public List<bool> isFirstDataGet = new List<bool>();
    public int modelSum = 0;

    //default instantiate
    private Vector3 initPos = new Vector3(-12, -2.5f, -6.16f);


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
    public void addNewModel() {
        //modelRot.Add(new Quaternion[21]);
        //modelPos.Add(initPos);
        mapPos.Add(new Vector3());
        skeletons.Add(new Skeleton());
        isFirstDataGet.Add(false);
        //TODO:
        //instantiate model to scene
        addModel();

        modelSum++;
    }

    void addModel() {
        ModelController modelInstant = Instantiate(model);
        modelInstant.modelIndex = modelSum;
        Debug.Log("[model instantiate] generate " + modelSum + " th model");
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
                addNewModel();
                break;
            default:
                Debug.Log("[Scene] goto "+id+" scene");
                break;
        }
    }

}
