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

[CLSCompliant(false)]
[Serializable]
public struct playerPose {
    public string UID;
    public Skeleton skeleton;
    public float posX;
    public float posY;
    public float posZ;
    //only use if VR valid
    public SerializableTransform leftHandTransform;
    public SerializableTransform rightHandTransform;
}


[Serializable]
public struct register {
    public string UID;
    //cam position
    public float posX;
    public float posY;
    public float posZ;
    //init model rotation
    public float rotX;
    public float rotY;
    public float rotZ;
    public float rotW;
}


public enum packageType {
    camModel = 0,
    cube,
    register
}

public static class NetMgr {

    public static void OnMsgRcv(byte[] socketData, Boolean isCient) {
        SocketPackage socketPackage = new SocketPackage();

        MemoryStream ms = new MemoryStream(socketData);
        BinaryFormatter bf = new BinaryFormatter();
        ms.Position = 0;
        socketPackage = (SocketPackage)bf.Deserialize(ms);


        switch (socketPackage.type) {
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
                if (MainMgr.inst.hasVR[index]) {
                    //for ik
                    MainMgr.inst.leftCtr[index] = msg.leftHandTransform;
                    MainMgr.inst.rightCtr[index] = msg.rightHandTransform;
                }
                Debug.Log("[NetMgr]receive complete");
                break;
            case packageType.cube:
                Cube Cubemsg = Utility.byte2Origin<Cube>(socketPackage.data);
                Moveable target = MainMgr.inst.moveableList.Find(x => Cubemsg.id == x.id);
                target.rcvCube(Cubemsg);
                break;
            case packageType.register:
                Debug.Log("[NetMgr]receive new observer get in");
                register registerMsg = Utility.byte2Origin<register>(socketPackage.data);
                int registerIndex = MainMgr.inst.getIndexfromUID(registerMsg.UID);
                Debug.Log("[NetMgr]index" + registerIndex);
                if (MainMgr.inst.mapPos.Count > registerIndex)
                    MainMgr.inst.mapPos[registerIndex] = new Vector3(registerMsg.posX, registerMsg.posY, registerMsg.posZ);
                if (MainMgr.inst.initRot.Count > registerIndex)
                    MainMgr.inst.initRot[registerIndex] = new Quaternion(registerMsg.rotX, registerMsg.rotY, registerMsg.rotZ, registerMsg.rotW);
                break;
            default:
                Debug.Log("[NetMgr]receive unknown package type");
                break;
        }
    }

    public static void sendMsg(packageType type, byte[] data) {
        SocketPackage sendData = new SocketPackage();
        sendData.type = type;
        sendData.data = data;

        if (MainMgr.isClient) {
            MainMgr.inst.client.SocketSend(Utility.Trans2byte(sendData));
        } else {
            MainMgr.inst.server.SocketSend(Utility.Trans2byte(sendData));
        }
    }
}
