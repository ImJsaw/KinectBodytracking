using UnityEngine;
using System.Collections;
//引入庫
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System;

[CLSCompliant(false)]
public class TcpClient : MonoBehaviour {
    string editString = "hello wolrd"; //編輯框文字

    Socket serverSocket; //伺服器端socket
    IPAddress ip; //主機ip
    IPEndPoint ipEnd;
    string recvStr; //接收的字串
    string sendStr; //傳送的字串
    byte[] recvData = new byte[4096]; //接收的資料，必須為位元組
    //byte[] sendData = new byte[1024]; //傳送的資料，必須為位元組
    int recvLen; //接收的資料長度
    Thread connectThread; //連線執行緒

    //初始化
    public void InitSocket(string ipAddr) {
        //定義伺服器的IP和埠，埠與伺服器對應
        ip = IPAddress.Parse(ipAddr); //可以是區域網或網際網路ip，此處是本機
        ipEnd = new IPEndPoint(ip, 5566);


        //開啟一個執行緒連線，必須的，否則主執行緒卡死
        connectThread = new Thread(new ThreadStart(SocketReceive));
        connectThread.Start();
    }

    void SocketConnet() {
        if (serverSocket != null)
            serverSocket.Close();
        //定義套接字型別,必須在子執行緒中定義
        serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        Debug.Log("ready to connect");
        //連線
        try {
            serverSocket.Connect(ipEnd);
        }
        catch (SocketException e) {
            Debug.Log("error" + e);
            MainMgr.inst.panelWaitingList.Enqueue("NetErrorPanel");
            //UIMgr.inst.generatePanel("NetErrorPanel");
        }
        
        ////輸出初次連線收到的字串
        //recvLen = serverSocket.Receive(recvData);
        //recvStr = Encoding.ASCII.GetString(recvData, 0, recvLen);
        //print(recvStr);
    }

    public void SocketSend(byte[] sendMsg) {
        //傳送
        Debug.Log("SocketSend");
        serverSocket.Send(sendMsg, sendMsg.Length, SocketFlags.None);
    }

    void SocketReceive() {
        SocketConnet();
        //不斷接收伺服器發來的資料
        while (true) {
            if (!serverSocket.Connected)
                continue;
            recvData = new byte[4096];
            recvLen = 0;
            recvLen = serverSocket.Receive(recvData);
            if (recvLen == 0) {
                SocketConnet();
                continue;
            }
            recvStr = Encoding.ASCII.GetString(recvData, 0, recvLen);
            Debug.Log("/////SOCKET rcv : " + recvStr);
            //MainMgr.inst.onMsgRcv(recvData);
            NetMgr.OnMsgRcv(recvData,true);
        }

    }

    void SocketQuit() {
        //關閉執行緒
        if (connectThread != null) {
            connectThread.Interrupt();
            connectThread.Abort();
        }
        //最後關閉伺服器
        if (serverSocket != null)
            serverSocket.Close();
        print("diconnect");
    }

    void Awake() {
        //set persist node
        DontDestroyOnLoad(this);
    }

    // Use this for initialization
    void Start() {
    }

    void OnGUI() {
        editString = GUI.TextField(new Rect(10, 10, 100, 20), editString);
        if (GUI.Button(new Rect(10, 30, 60, 20), "send"))
            SocketSend(Encoding.ASCII.GetBytes(editString));
    }

    // Update is called once per frame
    void Update() {

    }

    //程式退出則關閉連線
    void OnApplicationQuit() {
        SocketQuit();
    }

    //get data from remote
    public byte[] getData() {
        return recvData;
    }
}