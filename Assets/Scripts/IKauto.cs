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
        private string prefix;
        IKModelController modelContoller;

        IKauto(string prefix = "mixamorig:") {
            this.prefix = prefix;
        }

        public void setPrefix(string prefix) {

            this.prefix = prefix;
        }

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
            string rightHandPath = ""+prefix+"Hips/"+prefix+"Spine/"+prefix+"Spine1/"+prefix+"Spine2/"+prefix+"RightShoulder/"+prefix+"RightArm";
            GameObject rightHandTargetBone1;
            GameObject rightHandTargetBone2;
            GameObject rightHandTargetBone3;
            rightHandTargetBone1 = GameObject.Find(rightHandPath);
            rightHandTargetBone2 = GameObject.Find(rightHandPath + "/"+prefix+"RightForeArm");
            rightHandTargetBone3 = GameObject.Find(rightHandPath + "/"+prefix+"RightForeArm/"+prefix+"RightHand");
            RightArm.solver.SetChain(rightHandTargetBone1.transform, rightHandTargetBone2.transform, rightHandTargetBone3.transform, this.transform);


            //RightArm setting
            RightArm.solver.bendModifier = IKSolverLimb.BendModifier.Goal;
            RightArm.solver.target = modelContoller.rightHandTarget;
            RightArm.solver.bendGoal = modelContoller.rightHandGoal;
            RightArm.solver.maintainRotationWeight = 0;
            RightArm.solver.IKRotationWeight = 1;
        }

        private void leftArmSettig()
        {
            //LeftArmLimbIK
            string leftHandPath = ""+prefix+"Hips/"+prefix+"Spine/"+prefix+"Spine1/"+prefix+"Spine2/"+prefix+"LeftShoulder/"+prefix+"LeftArm";
            GameObject leftHandTargetBone1;
            GameObject leftHandTargetBone2;
            GameObject leftHandTargetBone3;
            leftHandTargetBone1 = GameObject.Find(leftHandPath);
            leftHandTargetBone2 = GameObject.Find(leftHandPath + "/"+prefix+"LeftForeArm");
            leftHandTargetBone3 = GameObject.Find(leftHandPath + "/"+prefix+"LeftForeArm/"+prefix+"LeftHand");
            LeftArm.solver.SetChain(leftHandTargetBone1.transform, leftHandTargetBone2.transform, leftHandTargetBone3.transform, this.transform);

            //LeftArm setting
            LeftArm.solver.bendModifier = IKSolverLimb.BendModifier.Goal;
            LeftArm.solver.target = modelContoller.leftHandTarget;
            LeftArm.solver.bendGoal = modelContoller.leftHandGoal;
            LeftArm.solver.maintainRotationWeight = 0;
            LeftArm.solver.IKRotationWeight = 1;
        }

        private void leftLegSetting()
        {
            //LeftLegLimbIK
            string leftLegPath = ""+prefix+"Hips/"+prefix+"LeftUpLeg";
            GameObject leftLegTargetBone1;
            GameObject leftLegTargetBone2;
            GameObject leftLegTargetBone3;
            leftLegTargetBone1 = GameObject.Find(leftLegPath);
            leftLegTargetBone2 = GameObject.Find(leftLegPath + "/"+prefix+"LeftLeg");
            leftLegTargetBone3 = GameObject.Find(leftLegPath + "/"+prefix+"LeftLeg/"+prefix+"LeftFoot");
            LeftLeg.solver.SetChain(leftLegTargetBone1.transform, leftLegTargetBone2.transform, leftLegTargetBone3.transform, this.transform);

            //LeftLegIKsetting
            LeftLeg.solver.bendModifier = IKSolverLimb.BendModifier.Target;
            LeftLeg.solver.target = modelContoller.leftLegTarget;
            LeftLeg.solver.maintainRotationWeight = 1; 
        }

        private void rightLegSetting()
        {
            //RightLegLimbIK
            string RightLegPath = ""+prefix+"Hips/"+prefix+"RightUpLeg";
            GameObject rightLegTargetBone1;
            GameObject rightLegTargetBone2;
            GameObject rightLegTargetBone3;
            rightLegTargetBone1 = GameObject.Find(RightLegPath);
            rightLegTargetBone2 = GameObject.Find(RightLegPath + "/"+prefix+"RightLeg");
            rightLegTargetBone3 = GameObject.Find(RightLegPath + "/"+prefix+"RightLeg/"+prefix+"RightFoot");
            RightLeg.solver.SetChain(rightLegTargetBone1.transform, rightLegTargetBone2.transform, rightLegTargetBone3.transform, this.transform);

            RightLeg.solver.bendModifier = IKSolverLimb.BendModifier.Target;
            RightLeg.solver.target = modelContoller.rightLegTarget;
            RightLeg.solver.maintainRotationWeight = 1;
        }

        private void headSetting()
        {
            string HeadPath = ""+prefix+"Hips/"+prefix+"Spine";
            GameObject HeadTargetBone1;
            GameObject HeadTargetBone2;
            GameObject HeadTargetBone3;
            HeadTargetBone1 = GameObject.Find(HeadPath);
            HeadTargetBone2 = GameObject.Find(HeadPath + "/"+prefix+"Spine1/"+prefix+"Spine2");
            HeadTargetBone3 = GameObject.Find(HeadPath + "/"+prefix+"Spine1/"+prefix+"Spine2/"+prefix+"Neck/"+prefix+"Head");
            Head.solver.SetChain(HeadTargetBone1.transform, HeadTargetBone2.transform, HeadTargetBone3.transform, this.transform);


            Head.solver.bendModifier = IKSolverLimb.BendModifier.Target;
            Head.solver.target = modelContoller.headTarget;
            Head.solver.maintainRotationWeight = 0.5f;
            Head.fixTransforms = false;
        }
    }
}