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
    public SerializableTransform headTransform;
    //only use if VR valid
    public SerializableTransform leftHandTransform;
    public SerializableTransform rightHandTransform;
    public SerializableTransform leftLegTransform;
    public SerializableTransform rightLegTransform;
    public SerializableTransform pelvisTransform;
}

[CLSCompliant(false)]
[Serializable]
public struct register {
    public string UID;
    public SerializableTransform headInitTransform;
    //only use if VR valid
    public SerializableTransform leftHandInitTransform;
    public SerializableTransform rightHandInitTransform;
    public SerializableTransform leftLegInitTransform;
    public SerializableTransform rightLegInitTransform;
    public SerializableTransform pelvisInitTransform;
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
                if (MainMgr.inst.headPos.Count > index)
                    MainMgr.inst.headPos[index] = msg.headTransform;
                if (MainMgr.inst.isFirstDataGet.Count > index)
                    MainMgr.inst.isFirstDataGet[index] = true;
                if (MainMgr.inst.hasVR[index]) {
                    //for ik
                    MainMgr.inst.leftCtr[index] = msg.leftHandTransform;
                    MainMgr.inst.rightCtr[index] = msg.rightHandTransform;
                    MainMgr.inst.leftTkr[index] = msg.leftLegTransform;
                    MainMgr.inst.rightTkr[index] = msg.rightLegTransform;
                    MainMgr.inst.pelvisTkr[index] = msg.pelvisTransform;
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
                if (MainMgr.inst.headPos.Count > registerIndex)
                    MainMgr.inst.headPos[registerIndex] = registerMsg.headInitTransform;
                if (MainMgr.inst.leftInitCtr.Count > registerIndex)
                    MainMgr.inst.leftInitCtr[registerIndex] = registerMsg.leftHandInitTransform;
                if (MainMgr.inst.rightInitCtr.Count > registerIndex)
                    MainMgr.inst.rightInitCtr[registerIndex] = registerMsg.rightHandInitTransform;
                if (MainMgr.inst.leftInitTkr.Count > registerIndex)
                    MainMgr.inst.leftInitTkr[registerIndex] = registerMsg.leftLegInitTransform;
                if (MainMgr.inst.rightInitTkr.Count > registerIndex)
                    MainMgr.inst.rightInitTkr[registerIndex] = registerMsg.rightLegInitTransform;
                if (MainMgr.inst.pelvisInitTkr.Count > registerIndex)
                    MainMgr.inst.pelvisInitTkr[registerIndex] = registerMsg.pelvisInitTransform;
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
