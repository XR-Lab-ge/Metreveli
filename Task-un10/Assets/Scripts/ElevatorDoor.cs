using UdonSharp;
using UnityEngine;

public class ElevatorDoor : UdonSharpBehaviour
{
    [Header("Panels")]
    public Transform leftPanel;
    public Transform rightPanel;

    [Header("Settings")]
    public float slideDistance = 0.5f;
    public float pullDepth = 0.15f;
    public float moveSpeed = 1.6f;
    public float snapThreshold = 0.015f;

    private Vector3 leftClosed, rightClosed;
    private Vector3 leftBack, rightBack;
    private Vector3 leftOpen, rightOpen;

    private const int PH_CLOSED = 0;
    private const int PH_PULLING = 1;
    private const int PH_SLIDING = 2;
    private const int PH_OPEN = 3;
    private const int PH_JOINING = 4;
    private const int PH_PUSHING = 5;
    private int phase = PH_CLOSED;

    void Start()
    {
        if (leftPanel == null || rightPanel == null)
        {
            Debug.LogError($"[ElevatorDoor] {name}: panels missing!");
            return;
        }
        leftClosed = leftPanel.localPosition;
        rightClosed = rightPanel.localPosition;
        leftBack = leftClosed + new Vector3(0, 0, pullDepth);
        rightBack = rightClosed + new Vector3(0, 0, pullDepth);
        leftOpen = leftBack + Vector3.left * slideDistance;
        rightOpen = rightBack + Vector3.right * slideDistance;
        Snap(leftClosed, rightClosed);
    }

    void Update()
    {
        if (leftPanel == null || rightPanel == null) return;

        switch (phase)
        {
            case PH_CLOSED: case PH_OPEN: break;
            case PH_PULLING:
                MoveTo(leftBack, rightBack);
                if (Near(leftBack, rightBack)) { Snap(leftBack, rightBack); phase = PH_SLIDING; }
                break;
            case PH_SLIDING:
                MoveTo(leftOpen, rightOpen);
                if (Near(leftOpen, rightOpen)) { Snap(leftOpen, rightOpen); phase = PH_OPEN; }
                break;
            case PH_JOINING:
                MoveTo(leftBack, rightBack);
                if (Near(leftBack, rightBack)) { Snap(leftBack, rightBack); phase = PH_PUSHING; }
                break;
            case PH_PUSHING:
                MoveTo(leftClosed, rightClosed);
                if (Near(leftClosed, rightClosed)) { Snap(leftClosed, rightClosed); phase = PH_CLOSED; }
                break;
        }
    }

    void MoveTo(Vector3 l, Vector3 r)
    {
        leftPanel.localPosition = Vector3.MoveTowards(leftPanel.localPosition, l, moveSpeed * Time.deltaTime);
        rightPanel.localPosition = Vector3.MoveTowards(rightPanel.localPosition, r, moveSpeed * Time.deltaTime);
    }
    bool Near(Vector3 l, Vector3 r)
    {
        return Vector3.Distance(leftPanel.localPosition, l) < snapThreshold &&
               Vector3.Distance(rightPanel.localPosition, r) < snapThreshold;
    }
    void Snap(Vector3 l, Vector3 r)
    { leftPanel.localPosition = l; rightPanel.localPosition = r; }

    public void Open()
    {
        if (phase == PH_CLOSED || phase == PH_PUSHING || phase == PH_JOINING)
            phase = PH_PULLING;
    }
    public void Close()
    {
        if (phase == PH_OPEN || phase == PH_SLIDING) phase = PH_JOINING;
        else if (phase == PH_PULLING) phase = PH_PUSHING;
    }
}