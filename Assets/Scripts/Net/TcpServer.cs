using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;

[CLSCompliant(false)]
public class TcpServer : MonoBehaviour {
    Socket serverSocket;

    public class SocketPack {
        public Socket currentSocket;
        public byte[] dataBuffer = new byte[4096];
    }

    private int port = 5566;
    private const int MAX_CLIENTS = 20;
    private Socket[] socketChannel = new Socket[MAX_CLIENTS];

    //init
    void InitSocket() {
        //listen any IP
        IPEndPoint ipEnd = new IPEndPoint(IPAddress.Any, port);
        //create socket instance
        serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        serverSocket.Bind(ipEnd);

        //開始偵聽 "同時要求連線最大值" 10
        serverSocket.Listen(10);
        serverSocket.BeginAccept(new AsyncCallback(OnClientConnect), null);
    }

    public class NoSocketAvailableException : Exception { }

    // find avaliable channel
    private int FindEmptyChannel() {
        for (int i = 0; i < MAX_CLIENTS; i++) {
            if (socketChannel[i] == null || !socketChannel[i].Connected)
                return i;
        }
        return -1;
    }

    public void OnClientConnect(IAsyncResult async) {
        try {
            int emptyChannelIndex = -1;
            if (serverSocket == null)
                return;

            Socket tmpSocket = serverSocket.EndAccept(async);
            EndPoint remoteEndPoint = tmpSocket.RemoteEndPoint;
            //get avaliable channel
            emptyChannelIndex = FindEmptyChannel();
            if (emptyChannelIndex == -1)
                throw new NoSocketAvailableException();
            //handle tmp socket
            socketChannel[emptyChannelIndex] = tmpSocket;
            tmpSocket = null;

            waitData(socketChannel[emptyChannelIndex]);
        }
        catch (ObjectDisposedException) {
            Debug.Log("[TCP SERVER] ObjectDisposedException");
        }
        catch (SocketException) {
            Debug.Log("[TCP SERVER] SocketException");
        }
        catch (NoSocketAvailableException) {
            Debug.Log("[TCP SERVER] NoSocketAvailableException");
        }
        finally {
            //release lock
            serverSocket.BeginAccept(new AsyncCallback(OnClientConnect), null);
        }
    }

    public AsyncCallback socketCallBack;
    public void waitData(Socket socket) {
        try {
            if (socketCallBack == null)
                socketCallBack = new AsyncCallback(onDataReceive);

            SocketPack socketPack = new SocketPack();
            socketPack.currentSocket = socket;
            socket.BeginReceive(socketPack.dataBuffer, 0, socketPack.dataBuffer.Length, SocketFlags.None, socketCallBack, socketPack);
        }
        catch (SocketException) {
            Debug.Log("[TCP SERVER] SocketException");
        }
    }

    public void onDataReceive(IAsyncResult async) {
        try {
            SocketPack socketData = (SocketPack)async.AsyncState;
            socketData.currentSocket.EndReceive(async);
            dataHandle(socketData.dataBuffer);
        }
        catch (SocketException) {
            Debug.Log("[TCP SERVER] SocketException");
        }
    }

    //////////   custom area /////////////////

    //all data get from client would be handled here
    public void dataHandle(byte[] data) {
        //message from client
        NetMgr.OnMsgRcv(data, false);
    }

    //send data to all client
    public void SocketSend(byte[] sendMsg) {
        for (int i = 0; i < MAX_CLIENTS; i++) {
            if (socketChannel[i]!= null && socketChannel[i].Connected)
                socketChannel[i].Send(sendMsg, sendMsg.Length, SocketFlags.None);
        }
    }

    //////////   custom area /////////////////

    //連線關閉
    void SocketQuit() {
        //close client
        for (int i = 0; i < MAX_CLIENTS; i++) {
            socketChannel[i].Close();
        }
        //close server
        serverSocket.Close();
        print("diconnect");
    }

    void Awake() {
        //set persist node
        DontDestroyOnLoad(this);
    }

    void Start() {
        //init server
        InitSocket();
    }

    void OnApplicationQuit() {
        SocketQuit();
    }
}
