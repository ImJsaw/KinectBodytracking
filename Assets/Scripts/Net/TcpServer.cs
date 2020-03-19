using UnityEngine;
using System.Collections;
//引入庫
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System;
using System.Collections.Generic;

[CLSCompliant(false)]
public class TcpServer : MonoBehaviour {
    //以下預設都是私有的成員
    Socket serverSocket; //伺服器端socket
    static List<Socket> client = new List<Socket>();
    IPEndPoint ipEnd; //偵聽埠
    string recvStr; //接收的字串
    string sendStr; //傳送的字串
    byte[] recvData = new byte[4096]; //接收的資料，必須為位元組
    //byte[] sendData = new byte[1024]; //傳送的資料，必須為位元組
    int recvLen; //接收的資料長度
    Thread connectThread; //連線執行緒
    Thread connectThread2; //連線執行緒
    Boolean connectlock = true;


    //初始化
    void InitSocket() {
        //定義偵聽埠,偵聽任何IP
        ipEnd = new IPEndPoint(IPAddress.Any, 5566);
        //定義套接字型別,在主執行緒中定義
        serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //連線
        serverSocket.Bind(ipEnd);
        //開始偵聽,最大10個連線
        serverSocket.Listen(10);



        //開啟一個執行緒連線，必須的，否則主執行緒卡死
        connectThread = new Thread(new ThreadStart(SocketReceive));
        connectThread.Start();

    }

    //連線
    Socket SocketConnet() {
        //if (clientSocket != null)
        //   clientSocket.Close();

        Socket clientSocket; //客戶端socket

        //控制檯輸出偵聽狀態
        print("Waiting for a client");
        //一旦接受連線，建立一個客戶端
        clientSocket = serverSocket.Accept();
        //獲取客戶端的IP和埠
        IPEndPoint ipEndClient = (IPEndPoint)clientSocket.RemoteEndPoint;
        //輸出客戶端的IP和埠
        print("Connect with " + ipEndClient.Address.ToString() + ":" + ipEndClient.Port.ToString());

        client.Add(clientSocket);

        return clientSocket;

        //連線成功則傳送資料
        //sendStr = "Welcome to my server";
        //SocketSend(Encoding.ASCII.GetBytes(sendStr));
    }

    public void SocketSend(byte[] sendMsg) {
        //傳送
        foreach(Socket curS in client)
        {
            curS.Send(sendMsg, sendMsg.Length, SocketFlags.None);
        }
    }


    //伺服器接收
    void SocketReceive() {
        //連線
        Socket curSocket = SocketConnet();
        //進入接收迴圈
        while (true) {
            //對data清零
            recvData = new byte[4096];
            //獲取收到的資料的長度
            recvLen = curSocket.Receive(recvData);
            //如果收到的資料長度為0，則重連並進入下一個迴圈
            if (recvLen == 0) {
                Debug.Log("socket close : " + client.Count);
                client.Remove(curSocket);
                SocketConnet();
                continue;
            }
            //輸出接收到的資料
            //recvStr = Encoding.ASCII.GetString(recvData, 0, recvLen);
            //將接收到的資料經過處理再發送出去
            //sendStr = "From Server: " + recvStr;
            //SocketSend(Encoding.ASCII.GetBytes(sendStr));
            //get data
            //MainMgr.inst.onMsgRcv(recvStr);
            Debug.Log("receive");
            NetMgr.OnMsgRcv(recvData,false);
        }
    }

    //連線關閉
    void SocketQuit() {
        //先關閉客戶端

        if (client.Count > 0)
        {
            foreach (Socket curS in client)
            {
                curS.Close();
            }
        }

        //再關閉執行緒
        if (connectThread != null) {
            connectThread.Interrupt();
            connectThread.Abort();
        }
        //最後關閉伺服器
        serverSocket.Close();
        print("diconnect");
    }

    void Awake() {
        //set persist node
        DontDestroyOnLoad(this);
    }

    // Use this for initialization
    void Start() {
        InitSocket(); //在這裡初始化server
    }

    // Update is called once per frame
    void Update() {
        if(client.Count == 1 && connectlock)
        {
            connectThread2 = new Thread(new ThreadStart(SocketReceive));
            connectThread2.Start();
            connectlock = false;
        }
    }

    void OnApplicationQuit() {
        SocketQuit();
    }

    //get data from remote
    public byte[] getData() {
        return recvData;
    }
}
