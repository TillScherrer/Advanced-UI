using OculusSampleFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RetargetHand : MonoBehaviour
{
    public bool retargetPosition = true;
    public bool retargetRotation = true;
    public Transform Hand;
    public bool isLeftHand = true;
    public OVRSkeleton skeleton;
    public GameObject retargetedColliderKit;
    [HideInInspector]
    public Transform[] retargetedInteractionFingers = new Transform[5];
    private Transform HeadJoint;
    private float HeightDistanceFromMouthToElbow= 0.4f;
    private float HeightDistanceFromMouthToLoweredHand = 0.7f;
    private InteractableToolsCreator fingerSignal;
    private InteractableToolsInputRouter fingerKnower;
    private Transform[] originalFingers = new Transform[5];
    private bool fingersAssigned = true;
    // Start is called before the first frame update
    void Start()
    {
        HeadJoint = transform.parent.GetChild(1).GetChild(0);
        //if(retargetRotation)Hand.localRotation = Quaternion.Euler(-40, -0, -20);


        //fingerSignal = GameObject.Find("InteractableToolsSDKDriver").GetComponent<InteractableToolsCreator>();
        //fingerKnower = GameObject.Find("InteractableToolsSDKDriver").GetComponent<InteractableToolsInputRouter>();
        for (int i= 0; i<retargetedColliderKit.transform.childCount; i++)
        {
            retargetedInteractionFingers[i] = retargetedColliderKit.transform.GetChild(i);
        }
        Invoke("AssignFingers", 2); 
        
    }



    void AssignFingers()
    {
        while (false)//fingerSignal.isSettenUp<1)
        {
           // yield return null;
        }

        
        foreach ( OVRBone bone in skeleton.Bones)
        {
            
            if (bone != null)
            {
                //Debug.Log("fingerForeachCall with name: " + finger.name);
                //Debug.Log("fingerForeachCall with name: " + finger.ToolTags);
                if (bone.Id == OVRSkeleton.BoneId.Hand_ThumbTip)
                {
                    originalFingers[0] = bone.Transform;
                    Debug.Log("Thumb added!");
                }
                else if (bone.Id == OVRSkeleton.BoneId.Hand_IndexTip)
                {
                    originalFingers[1] = bone.Transform;
                    Debug.Log("FingerTipPokeToolIndex added!");
                }
                else if (bone.Id == OVRSkeleton.BoneId.Hand_MiddleTip)
                {
                    originalFingers[2] = bone.Transform;
                    Debug.Log("FingerTipPokeToolMiddle added!");
                }
                else if (bone.Id == OVRSkeleton.BoneId.Hand_RingTip)
                {
                    originalFingers[3] = bone.Transform;
                    Debug.Log("FingerTipPokeToolRing added!");
                }
                else if (bone.Id == OVRSkeleton.BoneId.Hand_PinkyTip)
                {
                    originalFingers[4] = bone.Transform;
                    Debug.Log("FingerTipPokeToolPinky added!");
                }
            }

        }
        fingersAssigned = true;
    }

    private void Update()
    {
        if (fingersAssigned)
        {
            //StartCoroutine(AssignFingers());
            for (int i = 0; i < retargetedInteractionFingers.Length; i++)
            {
                if (originalFingers[i] != null)
                {
                    retargetedInteractionFingers[i].position = originalFingers[i].position;
                    //Debug.Log("setRetargetedFinger" + i + " to " + originalFingers[i].position);
                }
            }
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (fingersAssigned)
        {
            if (retargetPosition) Hand.position = transform.position + new Vector3(0, 0.2f, 0.2f) + ((HeadJoint.position.y - transform.position.y > HeightDistanceFromMouthToElbow) ? (HeadJoint.position.y - transform.position.y - HeightDistanceFromMouthToElbow) : 0) * Vector3.forward * 0.5f;
            float lowHandRotMultiplier = (((HeadJoint.position.y - transform.position.y) / HeightDistanceFromMouthToLoweredHand) + 1f) / 2f;
            if (retargetRotation) Hand.localRotation = Quaternion.Euler(-40 * lowHandRotMultiplier, -0, -20 * lowHandRotMultiplier);
        }
    }
}
