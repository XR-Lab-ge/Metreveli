using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

public class DoorOpening : UdonSharpBehaviour
{
    [Header("Pivot Objects")]
    public GameObject leftDoorPivot;
    public GameObject rightDoorPivot;

    public float speed = 3f;
    private bool isOpen = false;
    private float t = 0f;

    // მარცხენა კარის როტაცია
    private Quaternion leftClosed = Quaternion.Euler(90f, -90f, -90f);
    private Quaternion leftOpened = Quaternion.Euler(1f, -90f, -90f);

    // მარჯვენა კარის როტაცია
    private Quaternion rightClosed = Quaternion.Euler(90f, -90f, -90f);
    private Quaternion rightOpened = Quaternion.Euler(180f, -90f, -90f);

    public override void Interact()
    {
        isOpen = !isOpen;
    }

    void Update()
    {
        if (leftDoorPivot == null || rightDoorPivot == null) return;

        t = Mathf.MoveTowards(t, isOpen ? 1f : 0f, Time.deltaTime * speed);
        float smoothT = Mathf.SmoothStep(0f, 1f, t);

        // Slerp
        leftDoorPivot.transform.localRotation = Quaternion.Slerp(leftClosed, leftOpened, smoothT);
        rightDoorPivot.transform.localRotation = Quaternion.Slerp(rightClosed, rightOpened, smoothT);
    }
}