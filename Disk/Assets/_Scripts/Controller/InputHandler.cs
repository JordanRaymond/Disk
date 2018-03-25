using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public StatesManager states;
    private CameraManager camManager;

    float horizontal;
    float vertical;

    bool sprintInput;
    bool shootInput;
    bool reloadInput;
    bool switchInput;
    bool pivotInput;

    bool isInit;

    float delta;

    void Start() {
        InitInGame();
    }

    public void InitInGame() {
        camManager = CameraManager.singleton;

        states.Init();
        camManager.Init(transform);

        states.camHolder = camManager.cameraTransform;

        isInit = true;
    }

    private void FixedUpdate() {
        if (!isInit) return;

        delta = Time.fixedDeltaTime;
        InGame_UpdateStates_FixedUpdate();

        states.FixedTick(delta);
        camManager.FixedTick(delta);
    }

    void Update() {
        if (!isInit) return;

        delta = Time.deltaTime;

        GetInput();
        states.Tick(delta);
    }

    void GetInput() {
        if (Input.GetButtonDown("Jump"))
        {
            states.controllerStates.IsJumping = true;
        }
        // TODO ^^ vv
        if (Input.GetButtonDown("ActionButton2"))
        {
            states.inp.ActionButton2Down = true;
        }

        vertical = Input.GetAxis("Vertical");
        horizontal = Input.GetAxis("Horizontal");
        states.inp.fireLDown = Input.GetButtonDown("FireL");
        states.inp.fireRDown = Input.GetButtonDown("FireR");

    }

    void InGame_UpdateStates_FixedUpdate() {
        //Record input values - handled every frame by StatesManager
        states.inp.horizontal = horizontal;
        states.inp.vertical = vertical;
     
        states.inp.moveAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));

        Vector3 moveDir = states.camHolder.forward * vertical;
        moveDir.y = 0;
        moveDir += states.camHolder.right * horizontal;
        moveDir.Normalize();

        states.inp.moveDirection = moveDir;    
    }
}

public enum GamePhase
{
    inGame, inMenu
}
