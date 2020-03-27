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
public struct CamModel {
    public int index;
    //public Quaternion[] rot;
    //public Vector3 pos;
    public Skeleton skeleton;
}


public enum packageType {
    model = 0,
    messege,
    camModel
}

public static class NetMgr{

    public static void OnMsgRcv(byte[] socketData , Boolean isCient) {
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
                if(!isCient)
                {
                    if (!MainMgr.inst.getListenerComplete || MainMgr.inst.serverListener == null)
                    {
                        Debug.Log("[NetMgr]null server");
                        break;
                    }
                    MainMgr.inst.serverListener.updateChatRoom(socketPackage.data);
                }
                else
                {
                    if (!MainMgr.inst.getListenerComplete || MainMgr.inst.clientListener == null)
                    {
                        Debug.Log("[NetMgr]null client");
                        break;
                    }
                    MainMgr.inst.clientListener.updateChatRoom(socketPackage.data);
                }

                break;
            case packageType.camModel:
                CamModel msg = Utility.byte2Origin<CamModel>(socketPackage.data);
                int index = msg.index;
                //MainMgr.inst.modelRot[index] = msg.rot;
                //MainMgr.inst.modelPos[index] = msg.pos;
                MainMgr.inst.skeletons[index] = msg.skeleton;
                MainMgr.inst.isFirstDataGet[index] = true;
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
