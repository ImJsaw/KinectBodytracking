using UnityEngine;
using Microsoft.Azure.Kinect.Sensor.BodyTracking;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

[CLSCompliant(false)]
public class ClientListener : ListenerBase {

    bool startUpdate = false;

    // Update is called once per frame
    void Update() {
        checkError();
        if (client == null)
            Debug.LogWarning("null server");
        if (startUpdate) {
            updateModel();
            //updateModelFromSkeleton();
        }

        updateChatRoom();
    }

    public void updateBody(byte[] bodyData) { //get data from net
        //MemoryStream ms = new MemoryStream(bodyData);
        //BinaryFormatter bf = new BinaryFormatter();
        //ms.Position = 0;
        //skeleton = (Skeleton)bf.Deserialize(ms);
        skeleton = Utility.byte2Origin<Skeleton>(bodyData);
        startUpdate = true;
    }

}