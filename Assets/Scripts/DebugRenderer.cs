using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.Azure.Kinect.Sensor;
using Microsoft.Azure.Kinect.Sensor.BodyTracking;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class DebugRenderer : MonoBehaviour
{
    Device device;
    BodyTracker tracker;
    Skeleton skeleton;
    GameObject[] debugObjects;
    public Renderer renderer;
    public JointChan chan;
    int tempx = 0, tempy = 0, tempz = 0;
    bool isRotate = true;
    Queue<string> dataQueue = new Queue<string>();
    bool readComplete = false;

    Vector3 pos;
    Quaternion rot;

    [System.Serializable]
    public struct JointChan
    {
        public Transform Pelvis;// id = 0
        public Transform SpinNaval;// id = 1
        public Transform SpinChest;// id = 2
        public Transform Neck;// id = 3
        public Transform ClavicleLeft;// id = 4
        public Transform ShoulderLeft;// id = 5
        public Transform ElbowLeft;// id = 6
        public Transform WristLeft;// id = 7
        public Transform ClavicleRight;// id = 8
        public Transform ShoulderRight;// id = 9
        public Transform ElbowRight;// id = 10
        public Transform WristRight;// id = 11
        public Transform HipLeft;// id = 12
        public Transform KneeLeft;// id = 13
        public Transform AnkleLeft;// id = 14
        public Transform FootLeft;// id = 15
        public Transform HipRight;// id = 16
        public Transform KneeRight;// id = 17
        public Transform AnkleRight;// id = 18
        public Transform FootRight;// id = 19
        public Transform Head;// id = 20
    }

    private void OnEnable()
    {
        /*
        // KINECT INITIALIZE
        this.device = Device.Open(0);
        var config = new DeviceConfiguration
        {
            ColorResolution = ColorResolution.r720p,
            ColorFormat = ImageFormat.ColorBGRA32,
            DepthMode = DepthMode.NFOV_Unbinned
        };
        device.StartCameras(config);

        var calibration = device.GetCalibration(config.DepthMode, config.ColorResolution);
        this.tracker = BodyTracker.Create(calibration);
        */
        //cubes?
        debugObjects = new GameObject[(int)JointId.Count];
        for (var i = 0; i < (int)JointId.Count; i++)
        {
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = Enum.GetName(typeof(JointId), i);
            cube.transform.localScale = Vector3.one * 0.4f;
            debugObjects[i] = cube;
            Debug.DrawLine(debugObjects[i].transform.position, debugObjects[i].transform.TransformPoint(Vector3.forward * 5.0f), Color.blue);
            Debug.DrawLine(debugObjects[i].transform.position, debugObjects[i].transform.TransformPoint(Vector3.up * 5.0f), Color.green);
            Debug.DrawLine(debugObjects[i].transform.position, debugObjects[i].transform.TransformPoint(Vector3.right * 5.0f), Color.red);
        }

        readData();
    }

    private void OnDisable()
    {
        if (tracker != null)
        {
            tracker.Dispose();
        }
        if (device != null)
        {
            device.Dispose();
        }
    }

    void Update()
    {
        //SCREEN
        /*
        using (Capture capture = device.GetCapture())
        {
            tracker.EnqueueCapture(capture);
            var color = capture.Color;
            if (color.WidthPixels > 0)
            {
                Texture2D tex = new Texture2D(color.WidthPixels, color.HeightPixels, TextureFormat.BGRA32, false);
                tex.LoadRawTextureData(color.GetBufferCopy());
                tex.Apply();
                renderer.material.mainTexture = tex;
            }
        }
        */
        //split for force update
        //updateBody();
        //selfUpdate();
        updateFromData();

    }

    void updateBody() {
        using (var frame = tracker.PopResult()) {
            Debug.LogFormat("{0} bodies found.", frame.NumBodies);
            if (frame.NumBodies > 0) {
                var bodyId = frame.GetBodyId(0);
                Debug.LogFormat("bodyId={0}", bodyId);
                //send skeleton
                //this.skeleton = frame.GetSkeleton(0);

                //send from net
                byte[] userDataBytes;
                MemoryStream ms = new MemoryStream();
                BinaryFormatter bf1 = new BinaryFormatter();
                bf1.Serialize(ms, frame.GetSkeleton(0));
                userDataBytes = ms.ToArray();

                //get data from net
                MemoryStream ms2 = new MemoryStream(userDataBytes);
                BinaryFormatter bf2 = new BinaryFormatter();
                ms.Position = 0;
                object rawObj = bf1.Deserialize(ms);
                this.skeleton = (Skeleton)rawObj;

                for (var i = 0; i < 6; i++) {
                    var joint = this.skeleton.Joints[i];
                    var pos = joint.Position;
                    var rot = joint.Orientation;
                    var v = new Vector3(pos[0], -pos[1], pos[2]) * 0.004f;
                    var r = new Quaternion(rot[1], rot[2], rot[3], rot[0]);
                    var obj = debugObjects[i];
                    obj.transform.SetPositionAndRotation(v, r);
                }
            }
        }
    }

    void selfUpdate() {
        using (var frame = tracker.PopResult()) {
            Debug.LogFormat("{0} bodies found.", frame.NumBodies);
            if (frame.NumBodies > 0) {
                var bodyId = frame.GetBodyId(0);
                Debug.LogFormat("bodyId={0}", bodyId);
                //send skeleton
               this.skeleton = frame.GetSkeleton(0);

                if (Input.GetKeyDown(KeyCode.R))
                {
                    isRotate = !isRotate;
                }

                for (var i = 0; i < 26; i++) {
                    var joint = this.skeleton.Joints[i];
                    var pos = joint.Position;
                    var rot = joint.Orientation;
                    var v1 = new Vector3(pos[0], -pos[1], pos[2]) * 0.004f;
                    var r1 = new Quaternion();
                    if (isRotate)
                    {
                        r1.Set(rot[1], rot[2], rot[3], rot[0]);
                    }
                    else
                    {
                        r1.Set(0, 0, 0, rot[0]);
                    }
                    var obj = debugObjects[i];
                    obj.transform.SetPositionAndRotation(v1, r1);
 
                }

                for(var i=0;i<4;i++)
                {
                    Debug.DrawLine(debugObjects[i].transform.position, debugObjects[i].transform.TransformPoint(Vector3.forward * 5.0f), Color.blue);
                    Debug.DrawLine(debugObjects[i].transform.position, debugObjects[i].transform.TransformPoint(Vector3.up * 5.0f), Color.green);
                    Debug.DrawLine(debugObjects[i].transform.position, debugObjects[i].transform.TransformPoint(Vector3.right * 5.0f), Color.red);
                }

                //       0            
                var joint1 = this.skeleton.Joints[0];
                var rot1 = joint1.Orientation;
                var r = new Quaternion(rot1[1], rot1[2], rot1[3], rot1[0]);
                var qx = Quaternion.AngleAxis(90, chan.Pelvis.right);
                // var qz = Quaternion.AngleAxis(180, chan.Pelvis.up);
                //var r = new Quaternion(-rot1[2], -rot1[1], -rot1[3], rot1[0]);
                chan.Pelvis.rotation = Quaternion.Euler(r.eulerAngles.z, r.eulerAngles.x , r.eulerAngles.y);
                chan.Pelvis.Rotate(-90, 0, 0);
                chan.Pelvis.Rotate(0, 180, 0);

                Debug.DrawLine(chan.Pelvis.transform.position, chan.Pelvis.transform.TransformPoint(Vector3.forward * 5.0f), Color.blue);
                Debug.DrawLine(chan.Pelvis.transform.position, chan.Pelvis.transform.TransformPoint(Vector3.up * 5.0f), Color.green);
                Debug.DrawLine(chan.Pelvis.transform.position, chan.Pelvis.transform.TransformPoint(Vector3.right * 5.0f), Color.red);
                {
                    /*
                    ////         1          
                    //joint1 = this.skeleton.Joints[1];
                    //rot1 = joint1.Orientation;
                    ////var r = new Quaternion(rot1[2], -rot1[3], rot1[1], rot1[0]);
                    //r = new Quaternion(-rot1[1], rot1[2], -rot1[3], rot1[0]);
                    //chan.SpinNaval.localRotation = r;
                    ////        2           
                    //joint1 = this.skeleton.Joints[2];
                    //rot1 = joint1.Orientation;
                    //// chan.SpinChest.rotation = Quaternion.Euler(-r.eulerAngles.x, r.eulerAngles.y, -r.eulerAngles.z - 180);
                    ////var r = new Quaternion(rot1[2], -rot1[3], rot1[1], rot1[0]);
                    //r = new Quaternion(-rot1[1], rot1[2], -rot1[3], rot1[0]);
                    //chan.SpinChest.localRotation = r;
                    ////        3           
                    //joint1 = this.skeleton.Joints[3];
                    //rot1 = joint1.Orientation;
                    //r = new Quaternion(-rot1[1], rot1[2], -rot1[3], rot1[0]);
                    //chan.Neck.rotation = r;

                    ////        4           
                    //joint1 = this.skeleton.Joints[4];
                    //rot1 = joint1.Orientation;
                    //r = new Quaternion(rot1[1], rot1[2], rot1[3], rot1[0]);
                    //chan.ClavicleLeft.rotation = r;

                    ////         5         
                    //joint1 = this.skeleton.Joints[5];
                    //rot1 = joint1.Orientation;
                    //r = new Quaternion(rot1[1], rot1[2], rot1[3], rot1[0]);
                    //chan.ShoulderLeft.rotation = r;

                    ////         6         
                    //joint1 = this.skeleton.Joints[6];
                    //rot1 = joint1.Orientation;
                    //r = new Quaternion(rot1[1], rot1[2], rot1[3], rot1[0]);
                    //chan.ElbowLeft.rotation =r;

                    ////         7         
                    //joint1 = this.skeleton.Joints[7];
                    //rot1 = joint1.Orientation;
                    //r = new Quaternion(rot1[1], rot1[2], rot1[3], rot1[0]);
                    //chan.WristLeft.rotation = r;

                    //Debug.DrawLine(chan.WristLeft.position, chan.WristLeft.forward * 2.5f, Color.blue);
                    //Debug.DrawLine(chan.WristLeft.position, chan.WristLeft.up * 2.5f, Color.green);
                    //Debug.DrawLine(chan.WristLeft.position, chan.WristLeft.right * 2.5f, Color.red);

                    //        8           
                    //joint1 = this.skeleton.Joints[8];
                    //rot1 = joint1.Orientation;
                    //r = new Quaternion(rot1[1], rot1[2], rot1[3], rot1[0]);
                    //chan.ClavicleRight.localRotation =r;

                    ////Debug.DrawLine(chan.ClavicleRight.position, chan.ClavicleRight.forward * 2.5f, Color.blue);
                    ////Debug.DrawLine(chan.ClavicleRight.position, chan.ClavicleRight.up * 2.5f, Color.green);
                    ////Debug.DrawLine(chan.ClavicleRight.position, chan.ClavicleRight.right * 2.5f, Color.red);
                    //Debug.Log("chan.ClavicleRight.rotation " + chan.ClavicleRight.rotation);
                    //Debug.Log("tempx: " + tempx);
                    //Debug.Log("tempy: " + tempy);
                    //Debug.Log("tempz: " + tempz);

                    //if (Input.GetKeyDown(KeyCode.Z))
                    //{
                    //    tempx += 90;
                    //    if (tempx >= 360)
                    //        tempx %= 360;
                    //}
                    //if (Input.GetKeyDown(KeyCode.X))
                    //{
                    //    tempy += 90;
                    //    if (tempy >= 360)
                    //        tempy %= 360;
                    //}
                    //if (Input.GetKeyDown(KeyCode.C))
                    //{
                    //    tempz += 90;
                    //    if (tempz >= 360)
                    //        tempz %= 360;
                    //}

                    ////        9           
                    //joint1 = this.skeleton.Joints[9];
                    //rot1 = joint1.Orientation;
                    //r = new Quaternion(rot1[1], rot1[2], rot1[3], rot1[0]);
                    //chan.ShoulderRight.localRotation = Quaternion.Euler(r.eulerAngles.x, r.eulerAngles.y, r.eulerAngles.z);

                    ////        10           
                    //joint1 = this.skeleton.Joints[10];
                    //rot1 = joint1.Orientation;
                    //r = new Quaternion(rot1[1], rot1[2], rot1[3], rot1[0]);
                    //chan.ElbowRight.rotation = Quaternion.Euler(r.eulerAngles.x, r.eulerAngles.y, r.eulerAngles.z);

                    ////        11 
                    //joint1 = this.skeleton.Joints[11];
                    //rot1 = joint1.Orientation;
                    //r = new Quaternion(rot1[1], rot1[2], rot1[3], rot1[0]);
                    //chan.WristRight.rotation = Quaternion.Euler(r.eulerAngles.x, r.eulerAngles.y, r.eulerAngles.z);

                    //////        12           
                    ////joint1 = this.skeleton.Joints[12];
                    ////pos1 = joint1.Position;
                    ////rot1 = joint1.Orientation;
                    ////v = new Vector3(pos1[0], -pos1[1], pos1[2]) * 0.004f;
                    ////r = new Quaternion(rot1[1], rot1[2], rot1[3], rot1[0]);
                    ////chan.HipLeft.rotation = r;

                    //////        13           
                    ////joint1 = this.skeleton.Joints[13];
                    ////pos1 = joint1.Position;
                    ////rot1 = joint1.Orientation;
                    ////v = new Vector3(pos1[0], -pos1[1], pos1[2]) * 0.004f;
                    ////r = new Quaternion(rot1[1], rot1[2], rot1[3], rot1[0]);
                    ////chan.KneeLeft.rotation = r;

                    //////        14           
                    ////joint1 = this.skeleton.Joints[14];
                    ////pos1 = joint1.Position;
                    ////rot1 = joint1.Orientation;
                    ////v = new Vector3(pos1[0], -pos1[1], pos1[2]) * 0.004f;
                    ////r = new Quaternion(rot1[1], rot1[2], rot1[3], rot1[0]);
                    ////chan.AnkleLeft.rotation = r;

                    //////        15           
                    ////joint1 = this.skeleton.Joints[15];
                    ////pos1 = joint1.Position;
                    ////rot1 = joint1.Orientation;
                    ////v = new Vector3(pos1[0], -pos1[1], pos1[2]) * 0.004f;
                    ////r = new Quaternion(rot1[1], rot1[2], rot1[3], rot1[0]);
                    ////chan.FootLeft.rotation = r;

                    //////        16           
                    ////joint1 = this.skeleton.Joints[16];
                    ////rot1 = joint1.Orientation;
                    ////r = new Quaternion(rot1[1], rot1[2], rot1[3], rot1[0]);
                    ////chan.HipRight.rotation = Quaternion.Euler(-r.eulerAngles.x + 180, r.eulerAngles.y , -r.eulerAngles.z );

                    //////        17           
                    ////joint1 = this.skeleton.Joints[17];
                    ////rot1 = joint1.Orientation;
                    ////r = new Quaternion(rot1[1], rot1[2], rot1[3], rot1[0]);
                    ////chan.KneeRight.rotation = Quaternion.Euler(-r.eulerAngles.x +180,r.eulerAngles.y , -r.eulerAngles.z );

                    //////        18           
                    ////joint1 = this.skeleton.Joints[18];
                    ////rot1 = joint1.Orientation;
                    ////r = new Quaternion(rot1[1], rot1[2], rot1[3], rot1[0]);
                    ////chan.AnkleRight.rotation = Quaternion.Euler(-r.eulerAngles.x +180, r.eulerAngles.y, -r.eulerAngles.z );

                    //////        19           
                    ////joint1 = this.skeleton.Joints[19];
                    ////rot1 = joint1.Orientation;
                    ////r = new Quaternion(rot1[1], rot1[2], rot1[3], rot1[0]);
                    ////chan.FootRight.rotation = Quaternion.Euler(-r.eulerAngles.x +180, r.eulerAngles.y, -r.eulerAngles.z );

                    ////        20           
                    //joint1 = this.skeleton.Joints[20];
                    //rot1 = joint1.Orientation;
                    //r = new Quaternion(rot1[1], rot1[2], rot1[3], rot1[0]);
                    //chan.Head.rotation = Quaternion.Euler(r.eulerAngles.x , r.eulerAngles.y, r.eulerAngles.z);
                    */
                }
            }
        }
    }

    void updateFromData() {
        if (!readComplete) return;


        for (var i = 0; i < 26; i++) {
            getData();
            var obj = debugObjects[i];
            obj.transform.SetPositionAndRotation(pos, rot);

        }

        for (var i = 0; i < 1; i++) {
            Debug.DrawLine(debugObjects[i].transform.position, debugObjects[i].transform.TransformPoint(Vector3.forward * 5.0f), Color.blue);
            Debug.DrawLine(debugObjects[i].transform.position, debugObjects[i].transform.TransformPoint(Vector3.up * 5.0f), Color.green);
            Debug.DrawLine(debugObjects[i].transform.position, debugObjects[i].transform.TransformPoint(Vector3.right * 5.0f), Color.red);
        }

        //       0            
        GameObject joint1 = debugObjects[0];
        Quaternion rot1 = joint1.transform.rotation;

        Vector3 r = (Quaternion.Inverse(Quaternion.Euler(0,-150,-90)) * rot1).eulerAngles;
        Quaternion q = Quaternion.Euler(r.z, -r.x, r.y);
        chan.Pelvis.rotation = q;

        Debug.DrawLine(chan.Pelvis.transform.position, chan.Pelvis.transform.TransformPoint(Vector3.forward * 5.0f), Color.blue);
        Debug.DrawLine(chan.Pelvis.transform.position, chan.Pelvis.transform.TransformPoint(Vector3.up * 5.0f), Color.green);
        Debug.DrawLine(chan.Pelvis.transform.position, chan.Pelvis.transform.TransformPoint(Vector3.right * 5.0f), Color.red);

        {
            ////         1          
            //joint1 = this.skeleton.Joints[1];
            //rot1 = joint1.Orientation;
            ////var r = new Quaternion(rot1[2], -rot1[3], rot1[1], rot1[0]);
            //r = new Quaternion(-rot1[1], rot1[2], -rot1[3], rot1[0]);
            //chan.SpinNaval.localRotation = r;
            ////        2           
            //joint1 = this.skeleton.Joints[2];
            //rot1 = joint1.Orientation;
            //// chan.SpinChest.rotation = Quaternion.Euler(-r.eulerAngles.x, r.eulerAngles.y, -r.eulerAngles.z - 180);
            ////var r = new Quaternion(rot1[2], -rot1[3], rot1[1], rot1[0]);
            //r = new Quaternion(-rot1[1], rot1[2], -rot1[3], rot1[0]);
            //chan.SpinChest.localRotation = r;
            ////        3           
            //joint1 = this.skeleton.Joints[3];
            //rot1 = joint1.Orientation;
            //r = new Quaternion(-rot1[1], rot1[2], -rot1[3], rot1[0]);
            //chan.Neck.rotation = r;

            ////        4           
            //joint1 = this.skeleton.Joints[4];
            //rot1 = joint1.Orientation;
            //r = new Quaternion(rot1[1], rot1[2], rot1[3], rot1[0]);
            //chan.ClavicleLeft.rotation = r;

            ////         5         
            //joint1 = this.skeleton.Joints[5];
            //rot1 = joint1.Orientation;
            //r = new Quaternion(rot1[1], rot1[2], rot1[3], rot1[0]);
            //chan.ShoulderLeft.rotation = r;

            ////         6         
            //joint1 = this.skeleton.Joints[6];
            //rot1 = joint1.Orientation;
            //r = new Quaternion(rot1[1], rot1[2], rot1[3], rot1[0]);
            //chan.ElbowLeft.rotation =r;

            ////         7         
            //joint1 = this.skeleton.Joints[7];
            //rot1 = joint1.Orientation;
            //r = new Quaternion(rot1[1], rot1[2], rot1[3], rot1[0]);
            //chan.WristLeft.rotation = r;

            //Debug.DrawLine(chan.WristLeft.position, chan.WristLeft.forward * 2.5f, Color.blue);
            //Debug.DrawLine(chan.WristLeft.position, chan.WristLeft.up * 2.5f, Color.green);
            //Debug.DrawLine(chan.WristLeft.position, chan.WristLeft.right * 2.5f, Color.red);

            //        8           
            //joint1 = this.skeleton.Joints[8];
            //rot1 = joint1.Orientation;
            //r = new Quaternion(rot1[1], rot1[2], rot1[3], rot1[0]);
            //chan.ClavicleRight.localRotation =r;

            ////Debug.DrawLine(chan.ClavicleRight.position, chan.ClavicleRight.forward * 2.5f, Color.blue);
            ////Debug.DrawLine(chan.ClavicleRight.position, chan.ClavicleRight.up * 2.5f, Color.green);
            ////Debug.DrawLine(chan.ClavicleRight.position, chan.ClavicleRight.right * 2.5f, Color.red);
            //Debug.Log("chan.ClavicleRight.rotation " + chan.ClavicleRight.rotation);
            //Debug.Log("tempx: " + tempx);
            //Debug.Log("tempy: " + tempy);
            //Debug.Log("tempz: " + tempz);

            //if (Input.GetKeyDown(KeyCode.Z))
            //{
            //    tempx += 90;
            //    if (tempx >= 360)
            //        tempx %= 360;
            //}
            //if (Input.GetKeyDown(KeyCode.X))
            //{
            //    tempy += 90;
            //    if (tempy >= 360)
            //        tempy %= 360;
            //}
            //if (Input.GetKeyDown(KeyCode.C))
            //{
            //    tempz += 90;
            //    if (tempz >= 360)
            //        tempz %= 360;
            //}

            ////        9           
            //joint1 = this.skeleton.Joints[9];
            //rot1 = joint1.Orientation;
            //r = new Quaternion(rot1[1], rot1[2], rot1[3], rot1[0]);
            //chan.ShoulderRight.localRotation = Quaternion.Euler(r.eulerAngles.x, r.eulerAngles.y, r.eulerAngles.z);

            ////        10           
            //joint1 = this.skeleton.Joints[10];
            //rot1 = joint1.Orientation;
            //r = new Quaternion(rot1[1], rot1[2], rot1[3], rot1[0]);
            //chan.ElbowRight.rotation = Quaternion.Euler(r.eulerAngles.x, r.eulerAngles.y, r.eulerAngles.z);

            ////        11 
            //joint1 = this.skeleton.Joints[11];
            //rot1 = joint1.Orientation;
            //r = new Quaternion(rot1[1], rot1[2], rot1[3], rot1[0]);
            //chan.WristRight.rotation = Quaternion.Euler(r.eulerAngles.x, r.eulerAngles.y, r.eulerAngles.z);

            //////        12           
            ////joint1 = this.skeleton.Joints[12];
            ////pos1 = joint1.Position;
            ////rot1 = joint1.Orientation;
            ////v = new Vector3(pos1[0], -pos1[1], pos1[2]) * 0.004f;
            ////r = new Quaternion(rot1[1], rot1[2], rot1[3], rot1[0]);
            ////chan.HipLeft.rotation = r;

            //////        13           
            ////joint1 = this.skeleton.Joints[13];
            ////pos1 = joint1.Position;
            ////rot1 = joint1.Orientation;
            ////v = new Vector3(pos1[0], -pos1[1], pos1[2]) * 0.004f;
            ////r = new Quaternion(rot1[1], rot1[2], rot1[3], rot1[0]);
            ////chan.KneeLeft.rotation = r;

            //////        14           
            ////joint1 = this.skeleton.Joints[14];
            ////pos1 = joint1.Position;
            ////rot1 = joint1.Orientation;
            ////v = new Vector3(pos1[0], -pos1[1], pos1[2]) * 0.004f;
            ////r = new Quaternion(rot1[1], rot1[2], rot1[3], rot1[0]);
            ////chan.AnkleLeft.rotation = r;

            //////        15           
            ////joint1 = this.skeleton.Joints[15];
            ////pos1 = joint1.Position;
            ////rot1 = joint1.Orientation;
            ////v = new Vector3(pos1[0], -pos1[1], pos1[2]) * 0.004f;
            ////r = new Quaternion(rot1[1], rot1[2], rot1[3], rot1[0]);
            ////chan.FootLeft.rotation = r;

            //////        16           
            ////joint1 = this.skeleton.Joints[16];
            ////rot1 = joint1.Orientation;
            ////r = new Quaternion(rot1[1], rot1[2], rot1[3], rot1[0]);
            ////chan.HipRight.rotation = Quaternion.Euler(-r.eulerAngles.x + 180, r.eulerAngles.y , -r.eulerAngles.z );

            //////        17           
            ////joint1 = this.skeleton.Joints[17];
            ////rot1 = joint1.Orientation;
            ////r = new Quaternion(rot1[1], rot1[2], rot1[3], rot1[0]);
            ////chan.KneeRight.rotation = Quaternion.Euler(-r.eulerAngles.x +180,r.eulerAngles.y , -r.eulerAngles.z );

            //////        18           
            ////joint1 = this.skeleton.Joints[18];
            ////rot1 = joint1.Orientation;
            ////r = new Quaternion(rot1[1], rot1[2], rot1[3], rot1[0]);
            ////chan.AnkleRight.rotation = Quaternion.Euler(-r.eulerAngles.x +180, r.eulerAngles.y, -r.eulerAngles.z );

            //////        19           
            ////joint1 = this.skeleton.Joints[19];
            ////rot1 = joint1.Orientation;
            ////r = new Quaternion(rot1[1], rot1[2], rot1[3], rot1[0]);
            ////chan.FootRight.rotation = Quaternion.Euler(-r.eulerAngles.x +180, r.eulerAngles.y, -r.eulerAngles.z );

            ////        20           
            //joint1 = this.skeleton.Joints[20];
            //rot1 = joint1.Orientation;
            //r = new Quaternion(rot1[1], rot1[2], rot1[3], rot1[0]);
            //chan.Head.rotation = Quaternion.Euler(r.eulerAngles.x , r.eulerAngles.y, r.eulerAngles.z);
        }
    }

    void readData() {
        string text = File.ReadAllText(@"C:\Users\CGAL\Downloads\Moving.txt");
        string[] txts = text.Split('|');
        for(int i = 0;i < txts.Length; i++) {
            dataQueue.Enqueue(txts[i]);
        }
        Debug.Log("readData complete");
        readComplete = true;
    }

    void getData() {
        string data = dataQueue.Dequeue();

        string[] spData = data.Split('*');
        Debug.Log("pos." + spData[0]+".");
        Debug.Log("rot." + spData[1]+".");

        // Remove the parentheses
        if (spData[0].StartsWith("(") && spData[0].EndsWith(")")) {
            spData[0] = spData[0].Substring(1, spData[0].Length - 2);
        }
        // split the items
        string[] sArray = spData[0].Split(',');
        Debug.Log(sArray[0]+", "+ sArray[1]+", "+ sArray[2]);
        // store as a Vector3
        pos = new Vector3(
            float.Parse(sArray[0]),
            float.Parse(sArray[1]),
            float.Parse(sArray[2]));
        Debug.Log("pos fin");
        // Remove the parentheses
        if (spData[1].StartsWith("(") && spData[1].EndsWith(")")) {
            spData[1] = spData[1].Substring(1, spData[1].Length - 2);
        }

        // split the items
         sArray = spData[1].Split(',');

        // store as a Vector3
        rot = new Quaternion(
            float.Parse(sArray[0]),
            float.Parse(sArray[1]),
            float.Parse(sArray[2]),
            float.Parse(sArray[3]));
        Debug.Log("rot fin");
    }
}

    

