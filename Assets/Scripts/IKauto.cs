using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RootMotion.FinalIK
{
    public class IKauto : MonoBehaviour
    {
        // Start is called before the first frame update

        LimbIK RightArm;
        LimbIK RightLeg;
        LimbIK LeftArm;
        LimbIK LeftLeg;
        LimbIK Head;

        IKModelController modelContoller;

        void Start()
        {
            RightArm = this.gameObject.AddComponent<LimbIK>();
            RightLeg = this.gameObject.AddComponent<LimbIK>();
            LeftArm = this.gameObject.AddComponent<LimbIK>();
            LeftLeg = this.gameObject.AddComponent<LimbIK>();
            Head = this.gameObject.AddComponent<LimbIK>();
            modelContoller = this.GetComponent<IKModelController>();

  
        }

        // Update is called once per frame
        void Update()
        {
            if(modelContoller.is_catch)
            {
                setLimbIK();
                modelContoller.is_catch = false;
            }
        }


        private void setLimbIK()
        {
            rightArmSetting();
            leftArmSettig();
            leftLegSetting();
            rightLegSetting();
            headSetting();
        }

        private void rightArmSetting()
        {
            string rightHandPath = "mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:RightShoulder/mixamorig:RightArm";
            GameObject rightHandTargetBone1;
            GameObject rightHandTargetBone2;
            GameObject rightHandTargetBone3;
            rightHandTargetBone1 = GameObject.Find(rightHandPath);
            rightHandTargetBone2 = GameObject.Find(rightHandPath + "/mixamorig:RightForeArm");
            rightHandTargetBone3 = GameObject.Find(rightHandPath + "/mixamorig:RightForeArm/mixamorig:RightHand");
            RightArm.solver.SetChain(rightHandTargetBone1.transform, rightHandTargetBone2.transform, rightHandTargetBone3.transform, this.transform);


            //RightArm setting
            RightArm.solver.bendModifier = IKSolverLimb.BendModifier.Goal;
            RightArm.solver.target = modelContoller.rightHandTarget;
            RightArm.solver.bendGoal = modelContoller.rightHandGoal;
            RightArm.solver.maintainRotationWeight = 1;
        }

        private void leftArmSettig()
        {
            //LeftArmLimbIK
            string leftHandPath = "mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:LeftShoulder/mixamorig:LeftArm";
            GameObject leftHandTargetBone1;
            GameObject leftHandTargetBone2;
            GameObject leftHandTargetBone3;
            leftHandTargetBone1 = GameObject.Find(leftHandPath);
            leftHandTargetBone2 = GameObject.Find(leftHandPath + "/mixamorig:LeftForeArm");
            leftHandTargetBone3 = GameObject.Find(leftHandPath + "/mixamorig:LeftForeArm/mixamorig:LeftHand");
            LeftArm.solver.SetChain(leftHandTargetBone1.transform, leftHandTargetBone2.transform, leftHandTargetBone3.transform, this.transform);

            //LeftArm setting
            LeftArm.solver.bendModifier = IKSolverLimb.BendModifier.Goal;
            LeftArm.solver.target = modelContoller.leftHandTarget;
            LeftArm.solver.bendGoal = modelContoller.leftHandGoal;
            LeftArm.solver.maintainRotationWeight = 1;

        }

        private void leftLegSetting()
        {
            //LeftLegLimbIK
            string leftLegPath = "mixamorig:Hips/mixamorig:LeftUpLeg";
            GameObject leftLegTargetBone1;
            GameObject leftLegTargetBone2;
            GameObject leftLegTargetBone3;
            leftLegTargetBone1 = GameObject.Find(leftLegPath);
            leftLegTargetBone2 = GameObject.Find(leftLegPath + "/mixamorig:LeftLeg");
            leftLegTargetBone3 = GameObject.Find(leftLegPath + "/mixamorig:LeftLeg/mixamorig:LeftFoot");
            LeftLeg.solver.SetChain(leftLegTargetBone1.transform, leftLegTargetBone2.transform, leftLegTargetBone3.transform, this.transform);

            //LeftLegIKsetting
            LeftLeg.solver.bendModifier = IKSolverLimb.BendModifier.Target;
            LeftLeg.solver.target = modelContoller.leftLegTarget;
            LeftLeg.solver.maintainRotationWeight = 1; 
        }

        private void rightLegSetting()
        {
            //RightLegLimbIK
            string RightLegPath = "mixamorig:Hips/mixamorig:RightUpLeg";
            GameObject rightLegTargetBone1;
            GameObject rightLegTargetBone2;
            GameObject rightLegTargetBone3;
            rightLegTargetBone1 = GameObject.Find(RightLegPath);
            rightLegTargetBone2 = GameObject.Find(RightLegPath + "/mixamorig:RightLeg");
            rightLegTargetBone3 = GameObject.Find(RightLegPath + "/mixamorig:RightLeg/mixamorig:RightFoot");
            RightLeg.solver.SetChain(rightLegTargetBone1.transform, rightLegTargetBone2.transform, rightLegTargetBone3.transform, this.transform);

            RightLeg.solver.bendModifier = IKSolverLimb.BendModifier.Target;
            RightLeg.solver.target = modelContoller.rightLegTarget;
            RightLeg.solver.maintainRotationWeight = 1;
        }

        private void headSetting()
        {
            string HeadPath = "mixamorig:Hips/mixamorig:Spine";
            GameObject HeadTargetBone1;
            GameObject HeadTargetBone2;
            GameObject HeadTargetBone3;
            HeadTargetBone1 = GameObject.Find(HeadPath);
            HeadTargetBone2 = GameObject.Find(HeadPath + "/mixamorig:Spine1/mixamorig:Spine2");
            HeadTargetBone3 = GameObject.Find(HeadPath + "/mixamorig:Spine1/mixamorig:Spine2/mixamorig:Neck/mixamorig:Head");
            Head.solver.SetChain(HeadTargetBone1.transform, HeadTargetBone2.transform, HeadTargetBone3.transform, this.transform);


            Head.solver.bendModifier = IKSolverLimb.BendModifier.Target;
            Head.solver.target = modelContoller.headTarget;
            Head.solver.maintainRotationWeight = 1;
            Head.fixTransforms = false;
        }
    }
}