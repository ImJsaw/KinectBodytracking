using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using PoseList = System.Collections.Generic.List<PoseEstimator.Pose>;


public static class PoseEstimator {
    private static readonly float m_ExpNumber = 200;//差異度轉相似度函數(要與差異度成正相關)

    [System.Serializable]
    public struct Pose {
        public List<Vector3> joints;
        public Pose Clone() {
            Pose pose = new Pose { joints = new List<Vector3>() };
            for (int i = 0; i < joints.Count; i++) {
                Vector3 v = new Vector3(joints[i].x, joints[i].y, joints[i].z);
                pose.joints.Add(v);
            }
            return pose;
        }
    }

    public static PoseList ReadDataset(string _string) {
        PoseList peopleList = new PoseList();
        string[] Linedata = _string.Split('\n');

        int peopleCount = 0;
        int peopleJointCount = 0;

        string t_Line = "";
        string[] data;
        int lineNumber = 0;
        try {
            if ((t_Line = Linedata[lineNumber]) != "")//讀第1行
            {
                lineNumber++;
                data = t_Line.Split(' ');                       //用空白切割字串
                peopleCount = Convert.ToInt32(data[0]);         //有幾個人
                peopleJointCount = Convert.ToInt32(data[1]);    //每個人有幾個點
            }

            for (int i = 0; i < peopleCount; i++) {
                t_Line = Linedata[lineNumber];//讀人物編號//不需做處理
                lineNumber++;

                data = t_Line.Split(' ');//用空白切割字串

                ////////////////////////////
                Pose pose = new Pose {
                    joints = new List<Vector3>()
                };
                for (int j = 0; j < peopleJointCount; j++)//x y weight
                {
                    t_Line = Linedata[lineNumber];
                    lineNumber++;
                    data = t_Line.Split(' ');//用空白切割字串

                    Vector3 joint = new Vector3(Convert.ToSingle(data[0]), Convert.ToSingle(data[1]), Convert.ToSingle(data[2]));
                    pose.joints.Add(joint);
                }

                peopleList.Add(pose);
            }
        }
        catch (Exception e) {
            Debug.Log(e);
            return new PoseList();
        }
        return peopleList;
    }

    public static PoseList ReadFile(string path) {
        StreamReader reader = null;//打開文字檔用的
        reader = File.OpenText(path);
        string _string = reader.ReadToEnd();
        reader.Close();
        reader.Dispose();
        return ReadDataset(_string);
    }

    public static bool SaveFile(PoseList data, string path) {
        if (path != null) {
            //先刪除原本的檔案
            if (File.Exists(path)) {
                string[] filePaths = Directory.GetFiles(path);
                foreach (string filePath in filePaths) { File.Delete(filePath); }
            }

            FileStream fsFile = new FileStream(path, FileMode.OpenOrCreate);
            StreamWriter writer = new StreamWriter(fsFile);

            writer.WriteLine(data.Count.ToString() + " " + data[0].joints.Count);
            for (int i = 0; i < data.Count; i++) {
                writer.WriteLine(i.ToString());
                for (int j = 0; j < data[i].joints.Count; j++) {
                    string NewData =
                        data[i].joints[j].x + " " +
                        data[i].joints[j].y + " " +
                        data[i].joints[j].z;
                    writer.WriteLine(NewData);
                }
            }
            writer.Close();
            return true;
        }
        else { return false; }
    }

    public static float YDif(Pose data) {
        Pose pose = data.Clone();
        float highest = -100000;
        float lowest = 100000;

        foreach (Vector3 p in pose.joints) {
            if (p.z == 0) { continue; }//權重0表示沒算到那個點所以忽略
            if (p.y > highest) { highest = p.y; }
            if (p.y < lowest) { lowest = p.y; }
        }
        return highest - lowest;
    }

    public static Pose Normalize(Pose data) {
        Pose pose = data.Clone();
        float highest = -100000;
        float lowest = 100000;
        float leftest = 100000;
        float rightest = -100000;

        foreach (Vector3 p in pose.joints) {
            if (p.z == 0) { continue; }//權重0表示沒算到那個點所以忽略
            if (p.y > highest) { highest = p.y; }
            if (p.y < lowest) { lowest = p.y; }
            if (p.x > rightest) { rightest = p.x; }
            if (p.x < leftest) { leftest = p.x; }
        }
        float oringinSizeX = rightest - leftest;
        float oringinSizeY = highest - lowest;

        for (int i = 0; i < pose.joints.Count; i++)//將長寬拉伸為100
        {
            float x = (pose.joints[i].x - leftest) / oringinSizeX * 100;
            float y = (pose.joints[i].y - lowest) / oringinSizeY * 100;

            pose.joints[i] = new Vector3(x, y, pose.joints[i].z);
        }
        return pose;
    }

    public static float Comparision(Pose target, Pose challenger)//單人比較//回傳差異度
    {
        Pose sample, people;
        /*------------------Normalize------------------*/
        sample = Normalize(target);
        people = Normalize(challenger);
        /*------------------Normalize------------------*/

        /////////////////////////////////////////
        float deviation = 0;
        float allWeight = 0;
        foreach (Vector3 p in people.joints) { allWeight += p.z; }

        for (int i = 0; i < people.joints.Count; i++) {
            float dx = people.joints[i].x - sample.joints[i].x;
            float dy = people.joints[i].y - sample.joints[i].y;

            float distance = Mathf.Sqrt(Mathf.Pow(dx, 2) + Mathf.Pow(dy, 2));
            deviation += distance * people.joints[i].z;
        }
        deviation /= allWeight;
        float deviation1 = deviation;


        //////////////////////////比對模式2(去掉所有沒權重的點)
        sample = target.Clone();
        people = challenger.Clone();

        float orginPointCount = 0;
        float deletePointCount = 0;

        allWeight = 0;
        deviation = 0;

        for (int i = 0; i < sample.joints.Count; i++) {
            if (sample.joints[i].z > 0.01) { orginPointCount++; }
        }

        for (int i = 0; i < sample.joints.Count; i++) {
            for (int j = 0; j < sample.joints.Count; j++) {
                if (people.joints[j].z < 0.0001) { deletePointCount++; }
                if (sample.joints[j].z < 0.0001 || people.joints[j].z < 0.0001) {
                    sample.joints.RemoveAt(j);
                    people.joints.RemoveAt(j);
                }
            }
        }

        foreach (Vector3 p in people.joints) { allWeight += p.z; }

        sample = Normalize(sample);
        people = Normalize(people);

        for (int i = 0; i < people.joints.Count; i++)//距離相減
        {
            float dx = people.joints[i].x - sample.joints[i].x;
            float dy = people.joints[i].y - sample.joints[i].y;

            float distance = Mathf.Sqrt(Mathf.Pow(dx, 2) + Mathf.Pow(dy, 2));
            deviation += distance * people.joints[i].z;
        }
        deviation /= allWeight;

        //Debug.Log(deletePointCount);
        //Debug.Log(orginPointCount);

        deviation = deviation * (orginPointCount + deletePointCount) / orginPointCount;
        float deviation2 = deviation;

        //Debug.Log(deviation1 + " " + deviation2);
        if (deviation1 > deviation2) { return deviation2; }
        else { return deviation1; }
    }

    public static float Score(PoseList target, PoseList challenger) {
        if (challenger.Count == 0) { return 0.0f; }

        bool[] compareList_sample = new bool[target.Count];
        for (int i = 0; i < compareList_sample.Length; i++) { compareList_sample[i] = false; }

        bool[] compareList_camera = new bool[challenger.Count];
        for (int i = 0; i < compareList_camera.Length; i++) { compareList_camera[i] = false; }

        float allDeviation = 0;
        for (int i = 0; i < target.Count; i++) {
            int mostSimilar = 0;
            float lowDeviation = 10000;//差異度
            for (int j = 0; j < challenger.Count; j++) {
                if (compareList_camera[j] == true) { continue; }//不跟比過的人比

                float deviation = Comparision(target[i], challenger[j]);
                //Debug.Log(deviation);
                if (deviation < lowDeviation) {
                    lowDeviation = deviation;
                    mostSimilar = j;
                }
            }

            if (lowDeviation == 10000) { continue; }
            compareList_camera[mostSimilar] = true;//已被比較過
            compareList_sample[i] = true;
            allDeviation += lowDeviation;
        }

        for (int i = 0; i < target.Count; i++)//防止camera中人太少所以在比一次
        {
            if (compareList_sample[i] == true) { continue; }

            int mostSimilar = 0;
            float lowDeviation = 10000;//差異度
            for (int j = 0; j < challenger.Count; j++) {
                float deviation = Comparision(target[i], challenger[j]);
                if (deviation < lowDeviation) {
                    lowDeviation = deviation;
                    mostSimilar = j;
                }
            }
            compareList_camera[mostSimilar] = true;//已被比較過
            allDeviation += lowDeviation;
        }

        allDeviation /= target.Count;//算出來的數字最大值在30左右

        //Debug.Log(allDeviation);

        float finalDeviation = Mathf.Exp(-Mathf.Pow(allDeviation, 2) / (2 * m_ExpNumber));//用normal distribution 將數值mapping到0-1的區間內
        return finalDeviation;
    }

}
