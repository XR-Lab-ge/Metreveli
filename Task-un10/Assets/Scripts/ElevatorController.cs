using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using TMPro;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class ElevatorController : UdonSharpBehaviour
{
    [Header("=== CAR ===")]
    public Transform elevatorCar;
    public ElevatorDoor carDoor;
    public TextMeshPro floorIndicator;

    [Header("=== FLOORS ===")]
    public float[] floorPositionsY = { 0.1f, 5f };
    public ElevatorDoor[] floorDoors;

    [Header("=== TIMING ===")]
    public float moveSpeed = 1.5f;
    public float doorOpenDuration = 3.0f;
    public float doorHoldTime = 7.0f;
    public float doorCloseDuration = 3.0f;
    public float preMoveDelay = 0.2f;

    [UdonSynced] private int syncCurrentFloor = 0;
    [UdonSynced] private int syncDestFloor = -1;
    [UdonSynced] private bool syncMoving = false;
    [UdonSynced] private float syncCarY = 0.1f;

    private const int S_IDLE = 0;
    private const int S_DOOR_OPENING = 1;
    private const int S_DOOR_OPEN = 2;
    private const int S_DOOR_CLOSING_PRE = 3;
    private const int S_MOVING = 4;
    private const int S_DOOR_CLOSING = 5;
    private int state = S_IDLE;

    private float stateTimer;
    private int pendingDestination = -1;
    private bool passengerInCar = false;

    private float lastCarY;

    void Start()
    {
        SetCarY(floorPositionsY[0]);
        UpdateDisplay(0);
        lastCarY = floorPositionsY[0];
    }

    void Update()
    {
        if (!Networking.IsOwner(gameObject))
        {
            SmoothSyncCar();
            CarryPlayer();
            return;
        }

        switch (state)
        {
            case S_IDLE: break;
            case S_DOOR_OPENING: TickDoorOpening(); break;
            case S_DOOR_OPEN: TickDoorOpen(); break;
            case S_DOOR_CLOSING_PRE: TickClosingPre(); break;
            case S_MOVING: TickMoving(); break;
            case S_DOOR_CLOSING: TickClosing(); break;
        }

        CarryPlayer();
    }

    void CarryPlayer()
    {
        if (!passengerInCar) { lastCarY = elevatorCar.position.y; return; }
        if (Networking.LocalPlayer == null || !Networking.LocalPlayer.IsValid()) return;

        float currentCarY = elevatorCar.position.y;
        float deltaY = currentCarY - lastCarY;

        if (Mathf.Abs(deltaY) > 0.0001f)
        {
            Vector3 playerPos = Networking.LocalPlayer.GetPosition();
            playerPos.y += deltaY;
            Networking.LocalPlayer.TeleportTo(
                playerPos,
                Networking.LocalPlayer.GetRotation(),
                VRC_SceneDescriptor.SpawnOrientation.AlignPlayerWithSpawnPoint,
                /*lerpOnRemote*/ true
            );
        }

        lastCarY = currentCarY;
    }


    void TickDoorOpening()
    {
        stateTimer -= Time.deltaTime;
        if (stateTimer <= 0f) { state = S_DOOR_OPEN; stateTimer = doorHoldTime; Sync(); }
    }

    void TickDoorOpen()
    {
        stateTimer -= Time.deltaTime;
        if (stateTimer <= 0f) CloseAndStay();
    }

    void TickClosingPre()
    {
        stateTimer -= Time.deltaTime;
        if (stateTimer <= 0f)
        {
            syncDestFloor = pendingDestination;
            pendingDestination = -1;
            syncMoving = true;
            state = S_MOVING;
            Sync();
        }
    }

    void TickMoving()
    {
        float targetY = floorPositionsY[syncDestFloor];
        float newY = Mathf.MoveTowards(elevatorCar.position.y, targetY, moveSpeed * Time.deltaTime);
        SetCarY(newY);

        if (Mathf.Abs(newY - targetY) < 0.005f)
        {
            SetCarY(targetY);
            syncCurrentFloor = syncDestFloor;
            syncDestFloor = -1;
            syncMoving = false;
            UpdateDisplay(syncCurrentFloor);
            Sync();
            OpenDoors();
        }
    }

    void TickClosing()
    {
        stateTimer -= Time.deltaTime;
        if (stateTimer <= 0f) { state = S_IDLE; Sync(); }
    }

    void OpenDoors()
    {
        state = S_DOOR_OPENING;
        stateTimer = doorOpenDuration;
        if (carDoor != null) carDoor.Open();
        if (syncCurrentFloor < floorDoors.Length && floorDoors[syncCurrentFloor] != null)
            floorDoors[syncCurrentFloor].Open();
        Sync();
    }

    void CloseForMove(int destFloor)
    {
        state = S_DOOR_CLOSING_PRE;
        stateTimer = doorCloseDuration + preMoveDelay;
        pendingDestination = destFloor;
        if (carDoor != null) carDoor.Close();
        if (syncCurrentFloor < floorDoors.Length && floorDoors[syncCurrentFloor] != null)
            floorDoors[syncCurrentFloor].Close();
        Sync();
    }

    void CloseAndStay()
    {
        state = S_DOOR_CLOSING;
        stateTimer = doorCloseDuration;
        if (carDoor != null) carDoor.Close();
        if (syncCurrentFloor < floorDoors.Length && floorDoors[syncCurrentFloor] != null)
            floorDoors[syncCurrentFloor].Close();
        Sync();
    }

    public void RequestFloor(int floor)
    {
        if (floor < 0 || floor >= floorPositionsY.Length) return;
        TakeOwnership();

        if (state == S_MOVING || state == S_DOOR_OPENING ||
            state == S_DOOR_CLOSING || state == S_DOOR_CLOSING_PRE) return;

        if (floor == syncCurrentFloor)
        {
            if (state == S_IDLE) OpenDoors();
            else if (state == S_DOOR_OPEN) CloseAndStay();
            return;
        }

        if (state == S_IDLE)
        {
            pendingDestination = floor;
            syncDestFloor = floor;
            syncMoving = true;
            state = S_MOVING;
            Sync();
        }
        else if (state == S_DOOR_OPEN)
        {
            CloseForMove(floor);
        }
    }

    public void OnPassengerEntered()
    {
        passengerInCar = true;
        lastCarY = elevatorCar.position.y;
    }
    public void OnPassengerExited() { passengerInCar = false; }

    void SetCarY(float y)
    {
        Vector3 p = elevatorCar.position; p.y = y;
        elevatorCar.position = p; syncCarY = y;
    }

    void SmoothSyncCar()
    {
        Vector3 p = elevatorCar.position;
        p.y = Mathf.Lerp(p.y, syncCarY, Time.deltaTime * 12f);
        elevatorCar.position = p;
    }

    void UpdateDisplay(int floor)
    { if (floorIndicator != null) floorIndicator.text = (floor + 1).ToString(); }

    void TakeOwnership()
    { if (!Networking.IsOwner(gameObject)) Networking.SetOwner(Networking.LocalPlayer, gameObject); }

    void Sync() { RequestSerialization(); }

    public override void OnDeserialization()
    {
        UpdateDisplay(syncCurrentFloor);
        if (!syncMoving) SetCarY(floorPositionsY[syncCurrentFloor]);
    }
}