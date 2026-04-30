using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

public class PushingChair : UdonSharpBehaviour
{
    public float smoothTime = 0.05f;
    public float resetTime = 5f;
    public float followDistance = 1.5f;

    private Vector3 startPos;
    private Quaternion startRot;
    private Rigidbody rb;
    private Collider chairCollider;
    private bool isHeld = false;
    private float timer = 0f;

    private Vector3 currentVelocity;

    void Start()
    {
        startPos = transform.position;
        startRot = transform.rotation;
        rb = GetComponent<Rigidbody>();
        chairCollider = GetComponent<Collider>();
    }

    public override void OnPickup()
    {
        isHeld = true;
        if (rb != null)
        {
            rb.isKinematic = true; // ფიზიკას სრულად ვთიშავთ
            rb.useGravity = false;
        }
        if (chairCollider != null) chairCollider.enabled = false;
    }

    public override void OnDrop()
    {
        isHeld = false;
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.velocity = Vector3.zero;
        }
        if (chairCollider != null) chairCollider.enabled = true;
        timer = 0f;
    }

    // მოძრაობისთვის ვიყენებთ FixedUpdate-ს, რადგან VRChat-ის ფიზიკა ასე მუშაობს
    void FixedUpdate()
    {
        if (isHeld)
        {
            VRCPlayerApi player = Networking.LocalPlayer;
            if (!Utilities.IsValid(player)) return;

            VRCPlayerApi.TrackingData headData = player.GetTrackingData(VRCPlayerApi.TrackingDataType.Head);
            Vector3 forward = headData.rotation * Vector3.forward;
            forward.y = 0;
            forward.Normalize();

            Vector3 targetPos = headData.position + (forward * followDistance);
            targetPos.y = startPos.y;

            // SmoothDamp
            transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref currentVelocity, smoothTime);

            // დავამატოთ როტაციის გასწორებაც, რომ სკამი ყოველთვის წინ იყურებოდეს
            transform.rotation = Quaternion.LookRotation(forward);
        }
        else if (Vector3.Distance(transform.position, startPos) > 0.1f)
        {
            timer += Time.deltaTime;
            if (timer >= resetTime) ResetChair();
        }
    }

    private void ResetChair()
    {
        transform.position = startPos;
        transform.rotation = startRot;
        if (rb != null) { rb.velocity = Vector3.zero; rb.angularVelocity = Vector3.zero; }
    }
}