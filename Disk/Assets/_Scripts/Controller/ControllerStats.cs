using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Controller/Stats")]
public class ControllerStats : ScriptableObject
{
    public float moveSpeed = 4;
    public float boostSpeed = 6;
    public float drag = 4;
    public float downForce = 10;

    public float torque = 8;
    public float maxAngularVelocity = 7;
    public float angularDrag = 4;

    public float rotationAccelerationSpeed = 0.1f;
    public float rotationUpForce = 5f;
    public float decelerationFactor = 1.5f;

    [Header("Jump params")]
    public bool jumpUseWorldSpace = false;
    public float jumpTrust = 4;


    [Header("Hover params")]
    public bool hoverUseWorldSpace = false;
    public float hoverRayDistance = 1.4f;
    public float restingHeight = 1.5f;
    public float forceDamping = -0.2f;
    [Tooltip("Damp the vertical velocity ; hoverForce -= hoverDamping * rigid.velocity.y;")]
    public float hoverDamping = 1f;

    [Header("Flip params")]
    public float flipTorqueForce = 5;
    public float flipSideForce = 5;
}
