using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
[CLSCompliant(false)]
public class SerializableTransform {
    SerializableTransform(Vector3 position, Quaternion rotation) {
        _pos = new SerializablePos(position);
        _rot = new SerializableRot(rotation);
    }
    private SerializablePos _pos;
    private SerializableRot _rot;
    public Vector3 pos { get { return _pos.v3(); } }
    public Quaternion rot { get { return _rot.Quat(); } }
}

public class SerializablePos {
    public float x;
    public float y;
    public float z;

    public SerializablePos(Vector3 vector) {
        x = vector.x;
        y = vector.y;
        z = vector.z;
    }

    public Vector3 v3() {
        return new Vector3(x, y, z);
    }
}

public class SerializableRot {
    public float x;
    public float y;
    public float z;
    public float w;


    public SerializableRot(Quaternion r) {
        x = r.x;
        y = r.y;
        z = r.z;
        w = r.w;
    }

    public Quaternion Quat() {
        return new Quaternion(x, y, z, w);
    }
}

public class Utility{

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
    public static T resLoad<T>(string name) where T : UnityEngine.Object {
        T res = Resources.Load<T>(name);
        if (res == null) {
            Debug.LogError("Resources.Load [ " + name + " ] is Null !!");
            return default(T);
        }
        return res;
    }
}
