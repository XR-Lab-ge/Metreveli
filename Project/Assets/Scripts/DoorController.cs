using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class DoorController : UdonSharpBehaviour
{
    [Header("Door halves")]
    public Transform leftDoor;
    public Transform rightDoor;

    [Header("Closed positions (local)")]
    public Vector3 leftClosedPos;
    public Vector3 rightClosedPos;

    [Header("Phase 1 — pull back")]
    public Vector3 leftPulledBackPos; 
    public Vector3 rightPulledBackPos;

    [Header("Phase 2 — slide aside")]
    public Vector3 leftOpenPos;  
    public Vector3 rightOpenPos;    

    [Header("Timing")]
    public float pullBackDuration = 0.8f;  
    public float slideDuration = 1.2f;     
    public float pullBackSpeed = 4f; 
    public float slideSpeed = 5f;

    [UdonSynced] private bool isOpen = false;
    [UdonSynced] private float stateChangeTime = 0f;

    private float localTimer = 0f;
    private bool wasOpen = false;

    void Start()
    {
        leftDoor.localPosition = leftClosedPos;
        rightDoor.localPosition = rightClosedPos;
    }

    void Update()
    {
        if (isOpen != wasOpen)
        {
            wasOpen = isOpen;
            localTimer = 0f;
        }

        localTimer += Time.deltaTime;

        if (isOpen)
        {
            if (localTimer < pullBackDuration)
            {
                leftDoor.localPosition = Vector3.Lerp(
                    leftDoor.localPosition, leftPulledBackPos, Time.deltaTime * pullBackSpeed);
                rightDoor.localPosition = Vector3.Lerp(
                    rightDoor.localPosition, rightPulledBackPos, Time.deltaTime * pullBackSpeed);
            }
            else
            {
                leftDoor.localPosition = Vector3.Lerp(
                    leftDoor.localPosition, leftOpenPos, Time.deltaTime * slideSpeed);
                rightDoor.localPosition = Vector3.Lerp(
                    rightDoor.localPosition, rightOpenPos, Time.deltaTime * slideSpeed);
            }
        }
        else
        {
            if (localTimer < slideDuration)
            {
                leftDoor.localPosition = Vector3.Lerp(
                    leftDoor.localPosition, leftPulledBackPos, Time.deltaTime * slideSpeed);
                rightDoor.localPosition = Vector3.Lerp(
                    rightDoor.localPosition, rightPulledBackPos, Time.deltaTime * slideSpeed);
            }
            else
            {
                leftDoor.localPosition = Vector3.Lerp(
                    leftDoor.localPosition, leftClosedPos, Time.deltaTime * pullBackSpeed);
                rightDoor.localPosition = Vector3.Lerp(
                    rightDoor.localPosition, rightClosedPos, Time.deltaTime * pullBackSpeed);
            }
        }
    }

    public void OpenDoor()
    {
        if (!Networking.IsOwner(gameObject))
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
        isOpen = true;
        stateChangeTime = Time.time;
        RequestSerialization();
    }

    public void CloseDoor()
    {
        if (!Networking.IsOwner(gameObject))
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
        isOpen = false;
        stateChangeTime = Time.time;
        RequestSerialization();
    }

    public override void OnDeserialization()
    {
        if (isOpen != wasOpen) localTimer = 0f;
    }
}