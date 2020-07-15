using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System;

[CLSCompliant(false)]
public class TcpClient : MonoBehaviour {

    Socket clientSocket;
    IPEndPoint ipEnd;
    byte[] recvData = new byte[4096];
    Thread connectThread;

    private int port = 5566;

    //init
    public void InitSocket(string ipAddr) {
        IPAddress ip = IPAddress.Parse(ipAddr);
        ipEnd = new IPEndPoint(ip, port);
        // assign a thread for connecting or thread stuck
        connectThread = new Thread(new ThreadStart(SocketReceive));
        connectThread.Start();
    }

    void SocketConnet() {
        if (clientSocket != null)
            clientSocket.Close();
        //create socket instance
        clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        Debug.Log("ready to connect");
        //連線
        try {
            clientSocket.Connect(ipEnd);
        }
        catch (SocketException e) {
            socketErrHandle(e);
        }
    }

    void SocketReceive() {
        SocketConnet();
        while (true) {
            if (!clientSocket.Connected)
                continue;
            int recvLen = 0;
            recvLen = clientSocket.Receive(recvData);
            if (recvLen == 0) {
                SocketConnet();
                continue;
            }
            dataHandle(recvData);
        }
    }

    //////////   custom area /////////////////

    //send data to server
    public void SocketSend(byte[] sendMsg) {
        clientSocket.Send(sendMsg, sendMsg.Length, SocketFlags.None);
    }

    //all data get from server would be handled here
    public void dataHandle(byte[] data) {
        //message from server
        NetMgr.OnMsgRcv(data, true);
    }

    //handle exception
    void socketErrHandle(SocketException e) {
        Debug.Log("error" + e);
        MainMgr.inst.panelWaitingList.Enqueue("NetErrorPanel");
        //UIMgr.inst.generatePanel("NetErrorPanel");
    }
    //////////   custom area /////////////////

    void SocketQuit() {
        //關閉執行緒
        if (connectThread != null) {
            connectThread.Interrupt();
            connectThread.Abort();
        }
        //最後關閉伺服器
        if (clientSocket != null)
            clientSocket.Close();
        print("diconnect");
    }

    void Awake() {
        //set persist node
        DontDestroyOnLoad(this);
    }

    //程式退出則關閉連線
    void OnApplicationQuit() {
        SocketQuit();
    }
}