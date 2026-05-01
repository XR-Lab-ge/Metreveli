/*
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

public class PushingChair : UdonSharpBehaviour
{
    public float resetTime = 5f;

    private Vector3 startPos;
    private Quaternion startRot;
    private Rigidbody rb;
    private bool isHeld = false;
    private float timer = 0f;
    private float startY;

    void Start()
    {
        startPos = transform.position;
        startRot = transform.rotation;
        startY = transform.position.y;
        rb = GetComponent<Rigidbody>();
    }

    public override void OnPickup()
    {
        isHeld = true;
        timer = 0f;
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }
    }

    public override void OnDrop()
    {
        isHeld = false;
        timer = 0f;

        Vector3 pos = transform.position;
        pos.y = startY;
        transform.position = pos;
        transform.rotation = startRot;

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    public override void OnPickupUseDown()
    {
        VRCPlayerApi player = Networking.LocalPlayer;
        if (!Utilities.IsValid(player)) return;

        Vector3 fwd = player.GetTrackingData(
            VRCPlayerApi.TrackingDataType.Head).rotation * Vector3.forward;
        fwd.y = 0f;
        fwd.Normalize();

        Vector3 targetPos = player.GetPosition() + fwd * 1.2f;
        targetPos.y = startY;
        transform.position = targetPos;
        transform.rotation = startRot;
    }

    void Update()
    {
        if (isHeld)
        {
            VRCPlayerApi player = Networking.LocalPlayer;
            if (!Utilities.IsValid(player)) return;

            // Head-ის მონაცემები
            VRCPlayerApi.TrackingData head = player.GetTrackingData(
                VRCPlayerApi.TrackingDataType.Head);

            float headY = head.rotation.eulerAngles.y;
            Quaternion flatRot = Quaternion.Euler(0f, headY, 0f);

            Vector3 fwd = flatRot * Vector3.forward;
            Vector3 targetPos = player.GetPosition()
                              + fwd * 1.0f        
                              + Vector3.up * 0.9f;  

            transform.position = targetPos;
            transform.rotation = startRot; 
            return;
        }

        if (Vector3.Distance(transform.position, startPos) > 0.1f)
        {
            timer += Time.deltaTime;
            if (timer >= resetTime)
            {
                transform.position = startPos;
                transform.rotation = startRot;
                if (rb != null)
                {
                    rb.velocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }
                timer = 0f;
            }
        }
    }

}

*/




using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

public class PushingChair : UdonSharpBehaviour
{
    public float resetTime = 5f;

    private Vector3 startPos;
    private Quaternion startRot;
    private Rigidbody rb;
    private bool isHeld = false;
    private float timer = 0f;
    private float startY;

    void Start()
    {
        startPos = transform.position;
        startRot = transform.rotation;
        startY = transform.position.y;
        rb = GetComponent<Rigidbody>();
    }

    public override void OnPickup()
    {
        isHeld = true;
        timer = 0f;
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }
    }

    public override void OnDrop()
    {
        isHeld = false;
        timer = 0f;

        // Y პოზიცია დააბრუნე — სკამი არ დაფრინავს
        Vector3 pos = transform.position;
        pos.y = startY;
        transform.position = pos;
        transform.rotation = startRot;

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    // Desktop: სკამი წინ მოდის — არ გვერდზე
    public override void OnPickupUseDown()
    {
        VRCPlayerApi player = Networking.LocalPlayer;
        if (!Utilities.IsValid(player)) return;

        Vector3 fwd = player.GetTrackingData(
            VRCPlayerApi.TrackingDataType.Head).rotation * Vector3.forward;
        fwd.y = 0f;
        fwd.Normalize();

        Vector3 targetPos = player.GetPosition() + fwd * 1.2f;
        targetPos.y = startY;
        transform.position = targetPos;
        transform.rotation = startRot;
    }

    void Update()
    {
        if (isHeld)
        {
            VRCPlayerApi player = Networking.LocalPlayer;
            if (!Utilities.IsValid(player)) return;

            VRCPlayerApi.TrackingData head = player.GetTrackingData(
                VRCPlayerApi.TrackingDataType.Head);

            float headY = head.rotation.eulerAngles.y;
            Quaternion flatRot = Quaternion.Euler(0f, headY, 0f);

            Vector3 fwd = flatRot * Vector3.forward;
            Vector3 targetPos = player.GetPosition()
                              + fwd * 1.0f
                              + Vector3.up * 0.9f;

            transform.position = targetPos;

            // ← ეს ხაზი შეცვალე:
            transform.rotation = Quaternion.identity; // სამყაროს 0 rotation
            return;
        }

        if (Vector3.Distance(transform.position, startPos) > 0.1f)
        {
            timer += Time.deltaTime;
            if (timer >= resetTime)
            {
                transform.position = startPos;
                transform.rotation = startRot;
                if (rb != null)
                {
                    rb.velocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }
                timer = 0f;
            }
        }
    }

}