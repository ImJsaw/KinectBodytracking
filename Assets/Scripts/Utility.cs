using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[CLSCompliant(false)]
[Serializable]
public class SerializableTransform {
    public SerializableTransform(Vector3 position, Quaternion rotation) {
        _pos = new SerializablePos(position);
        _rot = new SerializableRot(rotation);
    }
    public SerializableTransform() {
        _pos = new SerializablePos(new Vector3(0, 0, 0));
        _rot = new SerializableRot(Quaternion.identity);
    }
    private SerializablePos _pos;
    private SerializableRot _rot;
    public Vector3 pos { get { return _pos.v3(); } set { _pos.setPos(value); } }
    public Quaternion rot { get { return _rot.Quat(); } set { _rot.setRot(value); } }
}

[CLSCompliant(false)]
[Serializable]
public class SerializablePos {
    public float x;
    public float y;
    public float z;

    public SerializablePos(Vector3 vector) {
        setPos(vector);
    }

    public Vector3 v3() {
        return new Vector3(x, y, z);
    }

    public void setPos(Vector3 vector) {
        x = vector.x;
        y = vector.y;
        z = vector.z;
    }
}

[CLSCompliant(false)]
[Serializable]
public class SerializableRot {
    public float x;
    public float y;
    public float z;
    public float w;


    public SerializableRot(Quaternion r) {
        setRot(r);
    }

    public Quaternion Quat() {
        return new Quaternion(x, y, z, w);
    }

    public void setRot(Quaternion r) {
        x = r.x;
        y = r.y;
        z = r.z;
        w = r.w;
    }
}

public class Utility {

    public static byte[] Trans2byte<T>(T data) {
        byte[] dataBytes;
        using (MemoryStream ms = new MemoryStream()) {
            BinaryFormatter bf1 = new BinaryFormatter();
            bf1.Serialize(ms, data);
            dataBytes = ms.ToArray();
        }
        return dataBytes;
    }

    public static T byte2Origin<T>(byte[] data) {
        MemoryStream ms = new MemoryStream(data);
        BinaryFormatter bf = new BinaryFormatter();
        ms.Position = 0;
        return (T)bf.Deserialize(ms);
    }

    public static GameObject instantiateGameObject(GameObject parent, GameObject prefab) {

        GameObject inst = GameObject.Instantiate(prefab);

        if (inst != null && parent != null) {
            Transform t = inst.transform;
            t.SetParent(parent.transform);
            t.localPosition = Vector3.zero;
            t.localRotation = Quaternion.identity;
            t.localScale = Vector3.one;

            RectTransform rect = inst.transform as RectTransform;
            if (rect != null) {
                rect.anchoredPosition = Vector3.zero;
                rect.localRotation = Quaternion.identity;
                rect.localScale = Vector3.one;

                //判斷anchor是否在同一點
                if (rect.anchorMin.x != rect.anchorMax.x && rect.anchorMin.y != rect.anchorMax.y) {
                    rect.offsetMin = Vector2.zero;
                    rect.offsetMax = Vector2.zero;
                }
            }

            inst.layer = parent.layer;
        }
        return inst;
    }

    //where ==> 限制 T 繼承自object
	//  path : "Asset/Resources/"+ name
    public static T resLoad<T>(string name) where T : UnityEngine.Object {
        T res = Resources.Load<T>(name);
        if (res == null) {
            Debug.LogError("Resources.Load [ " + name + " ] is Null !!");
            return default(T);
        }
        return res;
    }
	
	public static GameObject loadModelWithTex(string modelPath, string texPath){
        GameObject model = resLoad<GameObject>(modelPath);
        GameObject mesh = model.transform.Find("unamed").gameObject;
        Texture texImg = resLoad<Texture>(texPath);
        if(texImg != null) {
            Debug.Log("load tex" + texPath);
            mesh.GetComponent<Renderer>().sharedMaterial.SetTexture("_MainTex", texImg);
        }
		return model;
	}


    public static GameObject loadModelWithTex(string modelPath)
    {
        string RootPath = "Model/"+ modelPath+"/";
        GameObject model = resLoad<GameObject>(RootPath+modelPath + "_UV");
        GameObject mesh = model.transform.Find("unamed").gameObject;
        Texture texImg = resLoad<Texture>(RootPath + modelPath + "_tex");
        if (texImg != null)
        {
            Debug.Log("load tex");
            mesh.GetComponent<Renderer>().sharedMaterial.SetTexture("_MainTex", texImg);
        }
        return model;
    }

    // Multiply a 4x4Matrix with a Vector3 (position i.e. translation)
    static Vector3 Matrix4x4_Mult_Translation(Vector3 Translation, Matrix4x4 TransformationMatrix) {
        Vector4 TempV4 = new Vector4(Translation.x, Translation.y, Translation.z, 1);
        TempV4 = TransformationMatrix * TempV4;
        Translation = new Vector3(TempV4.x, TempV4.y, TempV4.z);
        return Translation;
    }

    static public void CCDIK(Transform[] KinematicJoints, Vector3 targetPos, bool isArm, bool isRight = true) {
        // Iteration from end-effector to root in the kinematic chain
        for (int i = KinematicJoints.Length - 1; i >= 0; i--) {
            //世界 ->local 座標轉換矩陣
            Matrix4x4 InverseTransformMatrix = KinematicJoints[i].localToWorldMatrix.inverse;
            //把末端座標轉為本地 並normalize
            Vector3 EndEffectorDirection = Matrix4x4_Mult_Translation(KinematicJoints[KinematicJoints.Length - 1].position, InverseTransformMatrix).normalized;
            //目標座標=>本地座標
            Vector3 TargetDirection = Matrix4x4_Mult_Translation(targetPos, InverseTransformMatrix).normalized;
            //當前點到末端的向量 & 當前點到目標的向量  (cos)
            float DotProduct = Vector3.Dot(EndEffectorDirection, TargetDirection);
            //若不共線
            if (DotProduct < 1.0f - 1.0e-6f) {
                //求出cos theta 的theta角
                float RotationAngle = Mathf.Acos(DotProduct) * Mathf.Rad2Deg;
                //求出旋轉軸
                Vector3 RotationAxis = Vector3.Cross(EndEffectorDirection, TargetDirection).normalized;
                //轉到目標方向
                KinematicJoints[i].Rotate(RotationAxis, RotationAngle);
                //Debug.Log("[CCDIK] rotate" + RotationAngle);

                //min/max rotate, default not limit
                Vector3 min = new Vector3(-180, -180, -180);
                Vector3 max = new Vector3(180, 180, 180);

                //handle human limit

                if (isArm) {

                    switch (i) {
                        //clavicle
                        case 0:
                            min = new Vector3(-20, -20, -20);
                            max = new Vector3(20, 20, 20);
                            break;
                        //shoulder
                        case 1:
                            if (isRight) {
                                min = new Vector3(-90, -90, -90);
                                max = new Vector3(90, 25, 90);
                            } else {
                                min = new Vector3(-90, -25, -90);
                                max = new Vector3(90, 90, 90);
                            }
                            break;
                        //elbow
                        case 2:
                            Vector3 sholder_elbow = (KinematicJoints[1].position - KinematicJoints[2].position).normalized;
                            Vector3 elbow_target = (targetPos - KinematicJoints[2].position).normalized;
                            //Debug.Log(Vector3.Cross(sholder_elbow, elbow_target));
                            if (Vector3.Cross(sholder_elbow, elbow_target).y < 0 && isRight) {
                                KinematicJoints[i].localRotation = Quaternion.identity;
                            }
                            if (Vector3.Cross(sholder_elbow, elbow_target).y > 0 && !isRight) {
                                KinematicJoints[i].localRotation = Quaternion.identity;
                            }
                            break;
                        default:
                            break;
                    }
                } else {
                    switch (i) {
                        //大腿
                        case 0:
                            break;
                        //膝蓋
                        case 1:
                            min = new Vector3(-10, -90, -25);
                            max = new Vector3(135, 90, 25);
                            break;
                        default:
                            break;
                    }
                }
                //limit max rotate of each
                limRot(KinematicJoints[i], min, max);

            }
        }
    }

    public static void limRot(Transform obj, Vector3 min, Vector3 max) {
        Vector3 localRot = obj.localEulerAngles;
        Debug.Log("origin : " + localRot.ToString());
        obj.localEulerAngles = limVec(localRot, min, max);
    }

    static Vector3 limVec(Vector3 origin, Vector3 min, Vector3 max) {
        Vector3 limitedVec = formatVec(origin);
        //lim +-45
        limitedVec.x = Mathf.Clamp(limitedVec.x, min.x, max.x);
        limitedVec.y = Mathf.Clamp(limitedVec.y, min.y, max.y);
        limitedVec.z = Mathf.Clamp(limitedVec.z, min.z, max.z);
        return limitedVec;
    }

    static Vector3 formatVec(Vector3 ori) {
        Vector3 formatedVec = new Vector3(0, 0, 0);
        formatedVec.x = ori.x > 180 ? ori.x - 360 : ori.x;
        formatedVec.y = ori.y > 180 ? ori.y - 360 : ori.y;
        formatedVec.z = ori.z > 180 ? ori.z - 360 : ori.z;
        return formatedVec;
    }

    public static void dropOutRot(Transform obj, Vector3 min, Vector3 max) {
        Vector3 localRot = formatVec(obj.localEulerAngles);
        Debug.Log("origin : " + localRot.ToString());

        if (localRot.x > max.x || localRot.y > max.y || localRot.z > max.z || localRot.x < min.x || localRot.y < min.y || localRot.z < min.z) {
            obj.localEulerAngles = new Vector3(0, 0, 0);
        }
    }

    public static bool testRotValid(Transform obj, Vector3 min, Vector3 max) {
        Vector3 localRot = formatVec(obj.localEulerAngles);
        //Debug.Log("origin : " + localRot.ToString());

        if (localRot.x > max.x || localRot.y > max.y || localRot.z > max.z || localRot.x < min.x || localRot.y < min.y || localRot.z < min.z) {
            return false;
        }

        return true;
    }

    public static void saveModel(string fileName)
    {
        GameObject customModel = Utility.loadModelWithTex(fileName);
        //customModel.transform.localScale = new Vector3(100, 100, 100);
        MainMgr.inst.customModelList.Add(customModel);
        MainMgr.inst.is_custom = true;
    }
}
