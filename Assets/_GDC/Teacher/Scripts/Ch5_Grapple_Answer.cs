using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>第 5 章的參考解答（教師版場景使用）。</summary>
[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(DistanceJoint2D))]
public class Ch5_Grapple_Answer : MonoBehaviour
{
    [Header("鉤爪設定")]
    public float maxDistance = 13f;
    public LayerMask grappleLayers;
    public float ropeSpeed = 6f;
    public float minRopeLength = 2.5f;

    DistanceJoint2D joint;
    LineRenderer line;

    void Awake()
    {
        joint = GetComponent<DistanceJoint2D>();
        joint.enabled = false;
        joint.autoConfigureDistance = false;

        line = GetComponent<LineRenderer>();
        line.positionCount = 0;
    }

    void Update()
    {
        var mouse = Mouse.current;
        if (mouse == null) return;

        if (mouse.leftButton.wasPressedThisFrame)  TryGrapple();
        if (mouse.leftButton.wasReleasedThisFrame) Release();

        var kb = Keyboard.current;
        if (joint.enabled && kb != null && kb.wKey.isPressed)
            joint.distance = Mathf.Max(minRopeLength, joint.distance - ropeSpeed * Time.deltaTime);

        DrawRope();
    }

    void TryGrapple()
    {
        // 解答 5-A
        Vector3 sp = Mouse.current.position.ReadValue();
        sp.z = -Camera.main.transform.position.z;
        Vector2 mouseWorld = Camera.main.ScreenToWorldPoint(sp);

        Vector2 dir = (mouseWorld - (Vector2)transform.position).normalized;
        if (dir == Vector2.zero) return;

        // 解答 5-B
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, maxDistance, grappleLayers);
        if (hit.collider == null) return;

        // 解答 5-C
        joint.connectedAnchor = hit.point;
        joint.distance = Vector2.Distance(transform.position, hit.point);
        joint.enabled = true;
    }

    void Release() { joint.enabled = false; }

    void DrawRope()
    {
        if (!joint.enabled) { line.positionCount = 0; return; }
        line.positionCount = 2;
        line.SetPosition(0, transform.position);
        line.SetPosition(1, joint.connectedAnchor);
    }
}
