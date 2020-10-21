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

[CLSCompliant(false)]
[Serializable]
public struct playerPose {
    public string UID;
    //public Skeleton skeleton;
    public SerializableTransform headTransform;
    //only use if VR valid
    public SerializableTransform leftHandTransform;
    public SerializableTransform rightHandTransform;
    public SerializableTransform leftLegTransform;
    public SerializableTransform rightLegTransform;
    public SerializableTransform pelvisTransform;
    public SerializablePos leftArmGoal;
    public SerializablePos rightArmGoal;
    public int modelType;
}

[CLSCompliant(false)]
[Serializable]
public struct register {
    public string UID;
    public int modelType;
    public SerializableTransform headInitTransform;
    public bool hasVR;
    //only use if VR valid
    public SerializableTransform leftHandInitTransform;
    public SerializableTransform rightHandInitTransform;
    public SerializableTransform leftLegInitTransform;
    public SerializableTransform rightLegInitTransform;
    public SerializableTransform pelvisInitTransform;
    public float handDist;
}

[Serializable]
public enum packageType {
    playerPose = 0,
    echoMsg,
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
            case packageType.playerPose:
                Debug.Log("[NetMgr]receive camModel package type");
                playerPose msg = Utility.byte2Origin<playerPose>(socketPackage.data);
                int index = MainMgr.inst.getIndexfromUID(msg.UID);
                if (index != -1) {

                    Debug.Log("[NetMgr]index" + index);
                    //if (MainMgr.inst.skeletons.Count > index)
                    //    MainMgr.inst.skeletons[index] = msg.skeleton;
                    //if (MainMgr.inst.isFirstDataGet.Count > index)
                    //    MainMgr.inst.isFirstDataGet[index] = true;
                    if (MainMgr.inst.headPos.Count > index)
                        MainMgr.inst.headPos[index] = msg.headTransform;
                    MainMgr.inst.modelType[index] = msg.modelType;
                    if (MainMgr.inst.hasVR[index]) {
                        Debug.Log("[NetMgr]update server model");
                        //for ik
                        MainMgr.inst.leftCtr[index] = msg.leftHandTransform;
                        MainMgr.inst.rightCtr[index] = msg.rightHandTransform;
                        MainMgr.inst.leftTkr[index] = msg.leftLegTransform;
                        MainMgr.inst.rightTkr[index] = msg.rightLegTransform;
                        MainMgr.inst.pelvisTkr[index] = msg.pelvisTransform;
                    }
                    if (msg.leftArmGoal.v3() != new Vector3(0, 0, 0)) {
                        MainMgr.inst.leftArmGoal[index] = msg.leftArmGoal;
                    }
                    if (msg.rightArmGoal.v3() != new Vector3(0, 0, 0)) {
                        MainMgr.inst.rightArmGoal[index] = msg.rightArmGoal;
                    }
                }

                Debug.Log("[NetMgr]receive complete");
                sendMsg(packageType.echoMsg, Utility.Trans2byte("model"));
                break;
            case packageType.register:

                //if is client, give it all exist player init data
                if (!MainMgr.isClient) {
                    Debug.Log("[NetMgr]receive new observer get in");
                    foreach (var playerID in MainMgr.inst.getUIDDect()) {
                        int i = playerID.Value;
                        register reg = new register();
                        reg.UID = playerID.Key;
                        reg.hasVR = MainMgr.inst.hasVR[i];
                        Debug.Log("id " + playerID.Key + ", has vr " + reg.hasVR);
                        reg.headInitTransform = MainMgr.inst.headPos[i];
                        reg.leftHandInitTransform = MainMgr.inst.leftInitCtr[i];
                        reg.rightHandInitTransform = MainMgr.inst.rightInitCtr[i];
                        reg.leftLegInitTransform = MainMgr.inst.leftInitTkr[i];
                        reg.rightLegInitTransform = MainMgr.inst.rightInitTkr[i];
                        reg.pelvisInitTransform = MainMgr.inst.pelvisInitTkr[i];
                        //hand dist for scale
                        reg.handDist = MainMgr.inst.handDist[i];
                        //model type
                        reg.modelType = MainMgr.inst.modelType[i];
                        //send from net
                        byte[] registerDataByte = Utility.Trans2byte(reg);
                        sendMsg(packageType.register, registerDataByte);

                    }
                }
                register registerMsg = Utility.byte2Origin<register>(socketPackage.data);
                int registerIndex = MainMgr.inst.getIndexfromUID(registerMsg.UID);
                Debug.Log("[NetMgr]index" + registerIndex + "init get, name : " + registerMsg.UID + "type" + registerMsg.modelType);
                if (MainMgr.inst.headPos.Count > registerIndex)
                    MainMgr.inst.headPos[registerIndex] = registerMsg.headInitTransform;
                if (MainMgr.inst.modelType.Count > registerIndex)
                    MainMgr.inst.modelType[registerIndex] = registerMsg.modelType;
                MainMgr.inst.hasVR[registerIndex] = registerMsg.hasVR;
                if (registerMsg.hasVR) {
                    MainMgr.inst.handDist[registerIndex] = registerMsg.handDist;
                    MainMgr.inst.leftInitCtr[registerIndex] = registerMsg.leftHandInitTransform;
                    MainMgr.inst.rightInitCtr[registerIndex] = registerMsg.rightHandInitTransform;
                    MainMgr.inst.leftInitTkr[registerIndex] = registerMsg.leftLegInitTransform;
                    MainMgr.inst.rightInitTkr[registerIndex] = registerMsg.rightLegInitTransform;
                    MainMgr.inst.pelvisInitTkr[registerIndex] = registerMsg.pelvisInitTransform;
                }
                sendMsg(packageType.echoMsg, Utility.Trans2byte("register"));
                break;
            case packageType.echoMsg:
                string echoMsg = Utility.byte2Origin<String>(socketPackage.data);
                if (echoMsg == "requestRegister")
                    UserListener.sendRegister();
                Debug.Log("[echo]type" + echoMsg);
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

        byte[] dataByte = Utility.Trans2byte(sendData);

            
        try {
            SocketPackage socketPackage = new SocketPackage();
            MemoryStream ms = new MemoryStream(dataByte);
            BinaryFormatter bf = new BinaryFormatter();
            ms.Position = 0;
            socketPackage = (SocketPackage)bf.Deserialize(ms);
        }
        catch (Exception) {
            Debug.LogError("send type" + type + "error");
        }

        if (MainMgr.isClient) {
            MainMgr.inst.client.SocketSend(Utility.Trans2byte(sendData));
        } else {
            MainMgr.inst.server.SocketSend(Utility.Trans2byte(sendData));
        }
    }
}
