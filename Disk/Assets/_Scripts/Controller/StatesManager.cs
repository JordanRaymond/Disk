using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class StatesManager : MonoBehaviour
{
    // TODO : Check if not null or get the ControllerStats in init ( I hate feading a var through the inspector)
    public ControllerStats stats;
    public ControllerStates controllerStates;
    public Transform camHolder;

    public InputVariables inp;

    [System.Serializable]
    public class InputVariables
    {
        public float horizontal;
        public float vertical;
        public float moveAmount;
        public Vector3 moveDirection;

        public bool fireLDown = false;
        public bool fireRDown = false;
        public bool ActionButton2Down = false;
    }

    [System.Serializable]
    public class ControllerStates
    {
        public bool onGround;
        public bool isAiming;
        public bool IsJumping;
        public bool isRunning; // Disk cant run x)
    }

    // public Animator anim;
    // public GameObject activeModel;
    [HideInInspector] public Rigidbody rigid;
    [HideInInspector] public Collider controllerCollider;

    public Transform tTransform;
    public DiskStates currentState;

    public LayerMask ignoreLayers;
    public LayerMask ignoreForGround;

    public float delta;

    [Header("Color ring params")]
    public Color color1;
    public float color1Emision;
    public Color color2;
    public float color2Emision;

    private Material extMatRing;
    private CameraManager camManager;

    void Start()
    {
    }

    public void Init()
    {
        tTransform = transform;

        // SetupAnimator();
        rigid = GetComponent<Rigidbody>();
        rigid.isKinematic = false;

        rigid.drag = stats.drag;
        rigid.angularDrag = stats.angularDrag;
        // rigid.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        controllerCollider = GetComponent<Collider>();

        ignoreLayers = ~(1 << 9);
        ignoreForGround = ~(1 << 9 | 1 << 2);

        extMatRing = GetExternalRingMaterial();
    }

    public void FixedTick(float p_delta)
    {
        delta = p_delta;

        switch (currentState)
        {
            case DiskStates.normal:
                controllerStates.onGround = OnGround();

                RotationNormal();
                Hovering();
                MovementNormal();

                break;
            case DiskStates.noHovering:
                controllerStates.onGround = OnGround();

                RotationNormal();
                MovementNormal();

                break;
            case DiskStates.onAir:
                break;
            case DiskStates.rolling:
                MovementNormal();
                rigid.AddForce(Vector3.down * stats.downForce, ForceMode.Force);

                break;
            default:
                break;
        }
    }

    public void Tick(float p_delta)
    {
        delta = p_delta;

        InputsHandle();

        switch (currentState)
        {
            case DiskStates.normal:
                controllerStates.onGround = OnGround();

                break;
            case DiskStates.onAir:
                rigid.drag = 0;
                controllerStates.onGround = OnGround();
                break;
            default:
                break;
        }

        // ColorLerp();
    }

    private Coroutine colorLerpCo;
    void InputsHandle()
    {
        if (inp.ActionButton2Down)
        {
            inp.ActionButton2Down = false;

            if (colorLerpCo !=null) { StopCoroutine(colorLerpCo); }

            if (currentState == DiskStates.normal)
            {
                currentState = DiskStates.noHovering;
                colorLerpCo = StartCoroutine(ColorLerpTo(color2, color2Emision, 2));
            }
            else if (currentState == DiskStates.noHovering)
            {
                currentState = DiskStates.normal;
                colorLerpCo = StartCoroutine(ColorLerpTo(color1, color1Emision, 2));
            }

        }

        //if (controllerStates.IsFliping) {
        //    ToRollState();
        //}

        if (inp.fireLDown || inp.fireRDown)
        {
            Flip(inp.fireLDown, inp.fireRDown);
        }

        if (controllerStates.IsJumping)
        {
            if (controllerStates.onGround)
            {
                Jump();
            }

            controllerStates.IsJumping = false;
        }
    }

    // Need a rework so it can work with the wipout controlls
    void ToRollState()
    {
        // rigid.AddForce(Vector3.up * stats.jumpTrust, ForceMode.Impulse);
    }

    void MovementNormal()
    {
        rigid.drag = stats.drag;

        // TODO : Acceleration?
        float speed = stats.moveSpeed;
        if (controllerStates.isRunning)
        {
            speed = stats.boostSpeed;
        }

        Vector3 dir = Vector3.zero;
        dir = inp.moveDirection * (speed * inp.moveAmount);

        // float angle = Vector3.Angle(tTransform.position, Vector3.down);
        // Quaternion tQuaternion = Quaternion.AngleAxis(angle, Vector3.left);
        // Debug.Log("Angle : " + angle);

        // dir = tQuaternion * dir;
        // Debug.DrawRay(tTransform.position + (Vector3.up), dir, Color.cyan);

        rigid.AddForce(dir, ForceMode.Force);
    }

    // TODO : MOVE VAR
    // TODO : If you accel in one direction and try to spin the other direction, the acceleration factor is still at 
    private float accelerationFactor = 0;
    private int sign = 1;
    void RotationNormal()
    {
        //rigid.maxAngularVelocity = stats.maxAngularVelocity;
        //rigid.angularDrag = stats.angularDrag;

        //// To simulate acceleration
        //if (Input.GetButton("FireL") || Input.GetButton("FireR"))
        //{
        //    sign = Input.GetButton("FireL") ? 1 : -1;

        //    accelerationFactor += (stats.rotationAccelerationSpeed * sign);

        //    // Rotation/
        //    rigid.AddTorque(Vector3.up * Mathf.Lerp(0, stats.torque, Mathf.Abs(accelerationFactor)) * sign, ForceMode.Force);
        //    // Upward force
        //    rigid.AddForce(Vector3.up * Mathf.Lerp(0, stats.rotationUpForce, Mathf.Abs(accelerationFactor)), ForceMode.Force);
        //}
        //else
        //{
        //    float deceleration = stats.decelerationFactor * (accelerationFactor >= 0 ? -1 : 1);
        //    accelerationFactor += stats.rotationAccelerationSpeed * deceleration;
        //}

        //accelerationFactor = Mathf.Clamp(accelerationFactor, -1, 1);
    }

    void Flip(bool isFireLdown, bool isFireRdown)
    {
        // Todo move this logic out of flip and hover
        Vector3 dir = stats.jumpUseWorldSpace ? Vector3.down : tTransform.up * -1;

        // Vector3.Dot ; The closer the two vector are, the dot product will be closer to 1
        if (!stats.jumpUseWorldSpace && Vector3.Dot(tTransform.up, Vector3.down) > 0.1f)
        {
            dir = tTransform.up;
        }

        if (isFireLdown)
        {

            rigid.AddForce(-camHolder.right * stats.flipSideForce, ForceMode.Impulse);
            rigid.AddTorque(camHolder.forward * stats.flipTorqueForce, ForceMode.Impulse);
        }
        if (isFireRdown)
        {
            rigid.AddForce(camHolder.right * stats.flipSideForce, ForceMode.Impulse);
            rigid.AddTorque(camHolder.forward * -stats.flipTorqueForce, ForceMode.Impulse);
        }
    }

    public Transform[] raysPositions;

    void Hovering()
    {
        Vector3 origin = tTransform.position;
        Vector3 dir = stats.hoverUseWorldSpace ? Vector3.down : tTransform.up * -1;

        // Vector3.Dot ; The closer the two vector are, the dot product will be closer to 1
        if (!stats.hoverUseWorldSpace && Vector3.Dot(tTransform.up, Vector3.down) > 0.1f)
        {
            dir = tTransform.up;
        }

        RaycastHit hit;

        for (int i = 0; i < raysPositions.Length; i++)
        {
            Debug.DrawRay(raysPositions[i].position, dir * stats.hoverRayDistance, Color.red);

            if (Physics.Raycast(raysPositions[i].position, dir, out hit, stats.hoverRayDistance, ignoreForGround))
            {
                float distanceToGround = hit.distance;

                // How close we are to the target height (-0.5 under or 1.2 over) * 
                float hoverForce = ((distanceToGround - stats.restingHeight) / Time.deltaTime) * (stats.forceDamping * (stats.restingHeight - distanceToGround));
                hoverForce -= stats.hoverDamping * rigid.velocity.y;

                rigid.AddForceAtPosition(dir * hoverForce, raysPositions[i].position, ForceMode.Force);
            }
        }
    }

    void Jump()
    {
        Vector3 origin = tTransform.position;
        Vector3 dir = stats.jumpUseWorldSpace ? Vector3.up : tTransform.up;

        // Vector3.Dot ; The closer the two vector are, the dot product will be closer to 1
        if (!stats.jumpUseWorldSpace && Vector3.Dot(tTransform.up, Vector3.down) > 0.1f)
        {
            dir = -tTransform.up;
        }

        for (int i = 0; i < raysPositions.Length; i++)
        {
            float jumpForce = stats.jumpTrust;

            rigid.AddForceAtPosition(dir * jumpForce, raysPositions[i].position, ForceMode.Impulse);
        }
    }

    private Material GetExternalRingMaterial()
    {
        Renderer rend = GetComponentInChildren<Renderer>();
        Material[] mats = rend.materials;

        for (int i = 0; i < mats.Length; i++)
        {
            if (mats[i].name == "Light 2 (Instance)")
            {
                return mats[i];
            }
        }

        return rend.material;
    }

    void ColorLerp()
    {
        Color finalColor1 = color1 * Mathf.LinearToGammaSpace(color1Emision);
        Color finalColor2 = color2 * Mathf.LinearToGammaSpace(color2Emision);
        extMatRing.SetColor("_EmissionColor", Color.Lerp(finalColor1, finalColor2, Mathf.PingPong(Time.time, 1)));
    }

    IEnumerator ColorLerpTo(Color p_color, float emision, float lerpDuration)
    {
        Color finalColor = p_color * Mathf.LinearToGammaSpace(emision);
        Color initialColor = extMatRing.GetColor("_EmissionColor");
        float startTime = Time.time;
        float elapsedTime = 0;

        while (elapsedTime <= lerpDuration)
        {
            elapsedTime = Time.time - startTime;
            extMatRing.SetColor("_EmissionColor", Color.Lerp(initialColor, finalColor, elapsedTime / lerpDuration));

            yield return null;
        }
    }

    bool OnGround()
    {
        Vector3 origin = tTransform.position;
        Vector3 dir = stats.jumpUseWorldSpace ? Vector3.down : -tTransform.up;

        // Vector3.Dot ; The closer the two vector are, the dot product will be closer to 1
        if (!stats.jumpUseWorldSpace && Vector3.Dot(tTransform.up, Vector3.down) > 0.1f)
        {
            dir = tTransform.up;
        }

        float dis = stats.hoverRayDistance;
        RaycastHit hit;

        if (Physics.Raycast(origin, dir, out hit, dis, ignoreForGround))
        {
            return true;
        }

        return false;
    }

    public enum DiskStates
    {
        normal, noHovering, onAir, shield, charging, rolling
    }

    //void SetupAnimator() {
    //    if (activeModel == null) {
    //        anim = GetComponentInChildren<Animator>();
    //        activeModel = anim.gameObject;
    //    }

    //    if (anim == null) {
    //        anim = activeModel.GetComponent<Animator>();
    //    }

    //    anim.applyRootMotion = false;
    //}

    //void HandleAnimationNormal() {
    //    float anim_v = inp.moveAmount;
    //    anim.SetFloat("vertical", anim_v, 0.15f, delta);
    //}
}

