using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PlayerColliderController : MonoBehaviour
{
    public CapsuleCollider myCollider;
    public LocomotionProvider myLocomotionProvider;

    private XROrigin myXROrigin;
    
    public float minHeight;
    public float maxHeight;
    public float climbingHeight;

    public float headBuffer;

    public ClimbingObject leftClimbingObject;
    public ClimbingObject rightClimbingObject;

    private void Start()
    {
        myXROrigin = myLocomotionProvider.system.xrOrigin;
    }
    
    private void Update()
    {
        var height = Mathf.Clamp(myXROrigin.CameraInOriginSpaceHeight, minHeight, maxHeight);

        Vector3 center = myXROrigin.CameraInOriginSpacePos;
        center.y = height / 2f + headBuffer;
        
        
        if (leftClimbingObject.isLocked || rightClimbingObject.isLocked)
        {
            height = climbingHeight;
            center = myXROrigin.CameraInOriginSpacePos;
            center.y += headBuffer;
        }

        myCollider.height = height;
        myCollider.center = center;
    }
}
