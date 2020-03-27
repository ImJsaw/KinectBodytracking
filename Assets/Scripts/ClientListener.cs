using UnityEngine;
using Microsoft.Azure.Kinect.Sensor.BodyTracking;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

[CLSCompliant(false)]
public class ClientListener : ListenerBase {

    bool startUpdate = false;

    string username = "client";
    bool updatechat = false;
    // Update is called once per frame



    void Update() {
        checkError();
        if (client == null)
            Debug.LogWarning("null server");

        updateChatRoom();
        sendCube();
        if (updatechat)
        {
            Debug.Log("update true");
            UpdateChat();
        }

        if (startUpdate) {
            updateModel();
            //updateModelFromSkeleton();
        }


    }

    protected void updateChatRoom() //接訊息方
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (chatInput.text != "")
            {

                Messege content = new Messege();
                content.username = username;
                content.text = chatInput.text;

                /*string addText = "\n  " + "<color=red>" + username + "</color>: " + chatInput.text;
                chatText.text += addText;
                chatInput.text = "";
                chatInput.ActivateInputField();
                Canvas.ForceUpdateCanvases();
                scrollRect.verticalNormalizedPosition = 1;
                Canvas.ForceUpdateCanvases();*/

                chatInput.ActivateInputField();
                Canvas.ForceUpdateCanvases();

                byte[] modelDataBytes = Utility.Trans2byte(content);
                NetMgr.sendMsg(packageType.messege, modelDataBytes, true);
            }
        }
    }

    public void updateChatRoom(byte[] msgData) //接訊息方
    {
        Debug.Log("updateChatRoom");
        content = Utility.byte2Origin<Messege>(msgData);
        updatechat = true;
    }

    void UpdateChat()
    {
        Debug.Log("UpdateChat()");
        string addText = "\n  " + "<color=red>" + content.username + "</color>: " + content.text;
        chatText.text += addText;

        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 1;
        Canvas.ForceUpdateCanvases();

        updatechat = false;
    }

    public void updateBody(byte[] bodyData) { //get data from net
        //MemoryStream ms = new MemoryStream(bodyData);
        //BinaryFormatter bf = new BinaryFormatter();
        //ms.Position = 0;
        //skeleton = (Skeleton)bf.Deserialize(ms);
        skeleton = Utility.byte2Origin<Skeleton>(bodyData);
        startUpdate = true;
    }

    protected void sendCube()
    {
       controlCubeTransform.cubeVector = controlCube.transform.position;
       controlCubeTransform.cubRot = controlCube.transform.rotation;


        byte[] msg;
        msg = Utility.Trans2byte<Cube>(controlCubeTransform);
        NetMgr.sendMsg(packageType.cube, msg, true);

    }

}