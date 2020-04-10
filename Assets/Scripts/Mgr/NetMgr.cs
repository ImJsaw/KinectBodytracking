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
    public int id;

    public float Vecx;
    public float Vecy;
    public float Vecz;

    public float Rotx;
    public float Roty;
    public float Rotz;
    public float Rotw;
}


[Serializable]
public struct playerPose {
    public string UID;
    public Skeleton skeleton;
    public float posX;
    public float posY;
    public float posZ;
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
                playerPose msg = Utility.byte2Origin<playerPose>(socketPackage.data);
                int index = MainMgr.inst.getIndexfromUID(msg.UID);
                Debug.Log("[NetMgr]index" + index);
                if (MainMgr.inst.skeletons.Count > index)
                    MainMgr.inst.skeletons[index] = msg.skeleton;
                if (MainMgr.inst.mapPos.Count > index)
                    MainMgr.inst.mapPos[index] = new Vector3(msg.posX, msg.posY, msg.posZ);
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
                Cube Cubemsg = Utility.byte2Origin<Cube>(socketPackage.data);
                Moveable target = MainMgr.inst.moveableList.Find(x => Cubemsg.id == x.id);
                target.rcvCube(Cubemsg);
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
