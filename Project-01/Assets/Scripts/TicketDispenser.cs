using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class TicketDispenser : UdonSharpBehaviour
{
    [Header("Tickets (size 5)")]
    public GameObject[] tickets;

    [Header("Door reference")]
    public DoorController door;

    [Header("Spawn point for new tickets")]
    public Transform spawnPoint;

    [Header("Door open duration (seconds)")]
    public float doorOpenTime = 5f;  // ← ახალი ცვლადი

    [UdonSynced] private bool[] taken = new bool[5];
    [UdonSynced] private int occupiedCount = 0;

    void Start()
    {
        for (int i = 0; i < tickets.Length; i++)
            if (tickets[i] != null) tickets[i].SetActive(false);
    }

    public void RequestEntry()
    {
        if (occupiedCount >= 5)
        {
            Debug.Log("[Dispenser] Pavilion full.");
            return;
        }

        if (!Networking.IsOwner(gameObject))
            Networking.SetOwner(Networking.LocalPlayer, gameObject);

        door.OpenDoor();
    }

    public void IssueTicket()
    {
        if (!Networking.IsOwner(gameObject))
            Networking.SetOwner(Networking.LocalPlayer, gameObject);

        int freeIndex = -1;
        for (int i = 0; i < taken.Length; i++)
            if (!taken[i]) { freeIndex = i; break; }

        if (freeIndex == -1) return;

        taken[freeIndex] = true;
        occupiedCount++;

        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(SpawnTicketRPC));
        RequestSerialization();

        SendCustomEventDelayedSeconds(nameof(CloseDoorDelayed), doorOpenTime);

        ActivateTicketLocal(freeIndex);
    }

    public void SpawnTicketRPC()
    {
        for (int i = 0; i < taken.Length; i++)
        {
            if (taken[i] && tickets[i] != null && !tickets[i].activeSelf)
                ActivateTicketLocal(i);
        }
    }

    private void ActivateTicketLocal(int index)
    {
        GameObject t = tickets[index];

        Rigidbody rb = (Rigidbody)t.GetComponent(typeof(Rigidbody));
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        t.transform.position = spawnPoint.position;
        t.transform.rotation = spawnPoint.rotation;
        t.SetActive(true);

        if (Networking.IsOwner(gameObject))
            Networking.SetOwner(Networking.LocalPlayer, t);
    }

    public void ReturnTicket(int ticketNumber)
    {
        if (ticketNumber < 0 || ticketNumber >= taken.Length) return;
        if (!taken[ticketNumber]) return;

        if (!Networking.IsOwner(gameObject))
            Networking.SetOwner(Networking.LocalPlayer, gameObject);

        taken[ticketNumber] = false;
        occupiedCount = Mathf.Max(0, occupiedCount - 1);

        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(DeactivateTicketRPC));

        door.OpenDoor();

        RequestSerialization();
    }

    public void DeactivateTicketRPC()
    {
        for (int i = 0; i < taken.Length; i++)
        {
            if (!taken[i] && tickets[i] != null && tickets[i].activeSelf)
                tickets[i].SetActive(false);
        }
    }

    public void CloseDoorDelayed()
    {
        door.CloseDoor();
    }

    public bool IsTicketTaken(int number)
    {
        if (number < 0 || number >= taken.Length) return false;
        return taken[number];
    }

    public override void OnDeserialization()
    {
        for (int i = 0; i < taken.Length; i++)
        {
            if (tickets[i] == null) continue;
            bool shouldBeActive = taken[i];
            if (tickets[i].activeSelf != shouldBeActive)
            {
                tickets[i].SetActive(shouldBeActive);
                if (shouldBeActive)
                {
                    tickets[i].transform.position = spawnPoint.position;
                    tickets[i].transform.rotation = spawnPoint.rotation;
                }
            }
        }
    }
}