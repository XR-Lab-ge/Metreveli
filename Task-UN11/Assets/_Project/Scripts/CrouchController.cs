using UnityEngine;
using StarterAssets;

public class CrouchController : MonoBehaviour
{
    public Transform cameraRoot;

    public float standHeight = 1.8f;
    public float crouchHeight = 1.1f;
    public float proneHeight = 0.6f;

    public float standCenterY = 0.93f;
    public float crouchCenterY = 0.55f;
    public float proneCenterY = 0.3f;

    public float standCamY = 1.375f;
    public float crouchCamY = 0.85f;
    public float proneCamY = 0.35f;

    public float standSpeed = 4f;
    public float crouchSpeed = 2f;
    public float proneSpeed = 1f;

    CharacterController cc;
    FirstPersonController fpc;
    enum Stance { Standing, Crouching, Prone }
    Stance stance = Stance.Standing;

    void Start()
    {
        cc = GetComponent<CharacterController>();
        fpc = GetComponent<FirstPersonController>();
        Apply();
    }

    void Update()
    {
        if (Time.timeScale == 0) return;
        if (Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.LeftControl))
        {
            stance = (stance == Stance.Crouching) ? Stance.Standing : Stance.Crouching;
            Apply();
        }
        else if (Input.GetKeyDown(KeyCode.Z))
        {
            stance = (stance == Stance.Prone) ? Stance.Standing : Stance.Prone;
            Apply();
        }
    }

    void Apply()
    {
        float h = standHeight, cy = standCenterY, camY = standCamY, sp = standSpeed;
        switch (stance)
        {
            case Stance.Crouching: h = crouchHeight; cy = crouchCenterY; camY = crouchCamY; sp = crouchSpeed; break;
            case Stance.Prone: h = proneHeight; cy = proneCenterY; camY = proneCamY; sp = proneSpeed; break;
        }
        if (cc) { cc.height = h; cc.center = new Vector3(0, cy, 0); }
        if (cameraRoot) { Vector3 p = cameraRoot.localPosition; p.y = camY; cameraRoot.localPosition = p; }
        if (fpc) { fpc.MoveSpeed = sp; fpc.SprintSpeed = sp * 1.5f; }
    }
}