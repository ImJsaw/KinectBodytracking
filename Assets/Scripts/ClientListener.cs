using UnityEngine;
using Microsoft.Azure.Kinect.Sensor.BodyTracking;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class ClientListener : ListenerBase {

    bool startUpdate = false;

    // Update is called once per frame
    void Update() {
        if (client == null)
            Debug.LogWarning("null server");
        if (startUpdate) {
            updateModel();
            //updateModelFromSkeleton();
        }
    }

    public void updateBody(byte[] bodyData) { //get data from net
        MemoryStream ms = new MemoryStream(bodyData);
        BinaryFormatter bf = new BinaryFormatter();
        ms.Position = 0;
        skeleton = (Skeleton)bf.Deserialize(ms);

        startUpdate = true;
    }

}