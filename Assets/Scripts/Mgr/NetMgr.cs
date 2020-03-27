using Microsoft.Azure.Kinect.Sensor.BodyTracking;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[Serializable]
public struct SocketPackage {
    public packageType type;
    public byte[] data;
}

[Serializable]
public struct Messege {
    public string username;
    public string text;
}

[Serializable]
public struct Cube
{
    public Transform[] cubeTransform;
}


[Serializable]
public struct CamModel {
    public int index;
    public Skeleton skeleton;
}


public enum packageType {
    model = 0,
    messege,
    camModel,
    cube
}

public static class NetMgr {

    public static void OnMsgRcv(byte[] socketData, Boolean isCient) {
        SocketPackage socketPackage = new SocketPackage();

        MemoryStream ms = new MemoryStream(socketData);
        BinaryFormatter bf = new BinaryFormatter();
        ms.Position = 0;
        socketPackage = (SocketPackage)bf.Deserialize(ms);


        switch (socketPackage.type) {
            case packageType.model:
                if (!MainMgr.inst.getListenerComplete || MainMgr.inst.clientListener == null) {
                    Debug.Log("[NetMgr]null client");
                    break;
                }
                MainMgr.inst.clientListener.updateBody(socketPackage.data);
                break;
            case packageType.messege:
                if (!isCient) {
                    if (!MainMgr.inst.getListenerComplete || MainMgr.inst.serverListener == null) {
                        Debug.Log("[NetMgr]null server");
                        break;
                    }
                    MainMgr.inst.serverListener.updateChatRoom(socketPackage.data);
                } else {
                    if (!MainMgr.inst.getListenerComplete || MainMgr.inst.clientListener == null) {
                        Debug.Log("[NetMgr]null client");
                        break;
                    }
                    MainMgr.inst.clientListener.updateChatRoom(socketPackage.data);
                }

                break;
            case packageType.camModel:
                Debug.Log("[NetMgr]receive camModel package type");
                CamModel msg = Utility.byte2Origin<CamModel>(socketPackage.data);
                int index = msg.index;
                Debug.Log("[NetMgr]index" + index);
                if (MainMgr.inst.skeletons.Count > index)
                    MainMgr.inst.skeletons[index] = msg.skeleton;
                if (MainMgr.inst.isFirstDataGet.Count > index)
                    MainMgr.inst.isFirstDataGet[index] = true;
                Debug.Log("[NetMgr]receive complete");
                break;
            case packageType.cube:
                if (!MainMgr.inst.getListenerComplete || MainMgr.inst.serverListener == null)
                {
                    Debug.Log("[NetMgr]null server");
                    break;
                }
                MainMgr.inst.serverListener.rcvCube(socketPackage.data);
                break;
            default:
                Debug.Log("[NetMgr]receive unknown package type");
                break;
        }
    }

    public static void sendMsg(packageType type, byte[] data, bool isClient = true) {
        SocketPackage sendData = new SocketPackage();
        sendData.type = type;
        sendData.data = data;

        if (isClient) {
            MainMgr.inst.client.SocketSend(Utility.Trans2byte(sendData));
        } else {
            MainMgr.inst.server.SocketSend(Utility.Trans2byte(sendData));
        }
    }
}
