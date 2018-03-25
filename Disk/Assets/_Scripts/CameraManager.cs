using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{

    public static CameraManager singleton;

    public bool lockon = false;
    public float followSpeed = 9;
    public float mouseSpeed = 2;
    public float controllerSpeed = 7;

    public Transform target;

    [HideInInspector] public Transform pivot;
    [HideInInspector] public Transform cameraTransform;

    float turnSmothing = 0.1f;
    [SerializeField] float minAngle = -35;
    [SerializeField] float maxAngle = 35;

    float smoothX;
    float smoothY;
    float smoothXVelocity;
    float smoothYVelocity;

    [SerializeField] float lookAngle;
    [SerializeField] float tiltAngle;

    private void Awake() {
        if (singleton != null) {
            Destroy(gameObject);
        }

        singleton = this;
    }

    public void Init(Transform p_target) {
        target = p_target;

        cameraTransform = Camera.main.transform;
        pivot = cameraTransform.parent;
    }

    public void FixedTick(float delta) {
        float horizontal = Input.GetAxis("Mouse X");
        float vertical = Input.GetAxis("Mouse Y");

        float c_horizontal = Input.GetAxis("HorizontalR");
        float c_vertical = Input.GetAxis("VerticalR");

        float targetSpeed = mouseSpeed;

        if (c_horizontal != 0 || c_vertical != 0) {
            horizontal = c_horizontal;
            vertical = c_vertical;

            targetSpeed = controllerSpeed;
        }

        FollowTarget(delta);
        HandleRotation(delta, vertical, horizontal, targetSpeed);
    }

    void FollowTarget(float delta) {
        float speed = delta * followSpeed;

        Vector3 targetPosition = Vector3.Lerp(transform.position, target.position, speed);
        transform.position = targetPosition;
    }

    void HandleRotation(float d, float p_vertical, float p_horizontal, float targetSpeed) {
        if (turnSmothing > 0) {

            smoothY = Mathf.SmoothDamp(smoothY, p_vertical, ref smoothYVelocity, turnSmothing);
            smoothX = Mathf.SmoothDamp(smoothX, p_horizontal, ref smoothXVelocity, turnSmothing);
        }
        else {
            smoothX = p_horizontal;
            smoothY = p_vertical;
        }

        // TODO
        if (lockon) {

        }

        lookAngle += smoothX * targetSpeed;
        transform.rotation = Quaternion.Euler(0, lookAngle, 0);

        tiltAngle -= smoothY * targetSpeed;
        tiltAngle = Mathf.Clamp(tiltAngle, minAngle, maxAngle);
        pivot.localRotation = Quaternion.Euler(tiltAngle, 0, 0);
    }

}

