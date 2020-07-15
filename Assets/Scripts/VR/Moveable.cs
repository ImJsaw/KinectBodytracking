//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class Moveable : MonoBehaviour
//{
//    protected Cube controlCubeTransform = new Cube();
//    private Vector3 lastPos = new Vector3();
//    private Quaternion lastRot = new Quaternion();

//    bool updatecube = false;
//    [HideInInspector]
//    public int id;
//    // Start is called before the first frame update
//    void Start()
//    {
//        MainMgr.inst.moveableList.Add(this);
//        id = MainMgr.inst.moveableid++;
//        iniCube();

//    }

//    // Update is called once per frame
//    void Update()
//    {
//        if (!MainMgr.isClient && is_Change()) //有更改且為server才發送
//            sendCube();

//        if(updatecube) //有收到才跑
//        {
//            updateCube();
//            updatecube = false;
//        }
//    }

//    public void rcvCube(Cube msgData) //接訊息方
//    {
//        Debug.Log("updateCube");
//        //controlCubeTransform = Utility.byte2Origin<Cube>(msgData);
//        controlCubeTransform = msgData;

//        updatecube = true;
//    }

//    protected void sendCube()
//    {


//        controlCubeTransform.Vecx = transform.position.x;
//        controlCubeTransform.Vecy = transform.position.y;
//        controlCubeTransform.Vecz = transform.position.z;

//        controlCubeTransform.Rotx = transform.rotation.x;
//        controlCubeTransform.Roty = transform.rotation.y;
//        controlCubeTransform.Rotz = transform.rotation.z;
//        controlCubeTransform.Rotw = transform.rotation.w;



//        byte[] msg;
//        msg = Utility.Trans2byte<Cube>(controlCubeTransform);
//        NetMgr.sendMsg(packageType.cube, msg);

//    }

//    private void iniCube()
//    {
//        controlCubeTransform.id = id;

//        controlCubeTransform.Vecx = transform.position.x;
//        controlCubeTransform.Vecy = transform.position.y;
//        controlCubeTransform.Vecz = transform.position.z;

//        controlCubeTransform.Rotx = transform.rotation.x;
//        controlCubeTransform.Roty = transform.rotation.y;
//        controlCubeTransform.Rotz = transform.rotation.z;
//        controlCubeTransform.Rotw = transform.rotation.w;

//        lastPos = transform.position;
//        lastRot = transform.rotation;
//    }

//    private bool is_Change()
//    {
//        if (lastPos == transform.position && lastRot == transform.rotation)
//        {
//            lastPos = transform.position;
//            lastRot = transform.rotation;
//            return false;
//        }

//        lastPos = transform.position;
//        lastRot = transform.rotation;

//        return true;
//    }


//    protected void updateCube()
//    {
//        Vector3 cubPosition = new Vector3(controlCubeTransform.Vecx, controlCubeTransform.Vecy, controlCubeTransform.Vecz);
//        transform.position = cubPosition;

//        Quaternion cubRotation = new Quaternion(controlCubeTransform.Rotx, controlCubeTransform.Roty, controlCubeTransform.Rotz, controlCubeTransform.Rotw);
//        transform.rotation = cubRotation;

//    }
//}
