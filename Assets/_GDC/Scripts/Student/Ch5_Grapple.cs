using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 第 5 章練習：鉤爪。用滑鼠瞄準紫色鉤點，按住左鍵盪過深淵。
///
/// 這一章有三個常見的坑，先看過再開始寫：
///   1. DistanceJoint2D 的 Auto Configure Distance 一定要「取消勾選」
///   2. ScreenToWorldPoint 一定要給 z 值，不然結果會全部落在攝影機平面上
///   3. Grapple Layers 要勾 Grappleable，不然會鉤到地板或什麼都鉤不到
/// </summary>
[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(DistanceJoint2D))]
public class Ch5_Grapple : MonoBehaviour
{
    [Header("鉤爪設定")]
    [Tooltip("鉤爪最遠能射多遠")]
    public float maxDistance = 13f;
    [Tooltip("可以鉤的圖層，請勾選 Grappleable")]
    public LayerMask grappleLayers;
    [Tooltip("按 W 收繩的速度")]
    public float ropeSpeed = 6f;
    [Tooltip("繩子最短只能收到這麼短。太短會整個人貼在鉤點上，就盪不動了")]
    public float minRopeLength = 2.5f;

    DistanceJoint2D joint;
    LineRenderer line;

    void Awake()
    {
        joint = GetComponent<DistanceJoint2D>();
        joint.enabled = false;
        joint.autoConfigureDistance = false;   // 坑 1：不關掉的話繩長會自己亂變

        line = GetComponent<LineRenderer>();
        line.positionCount = 0;
    }

    void Update()
    {
        var mouse = Mouse.current;
        if (mouse == null) return;

        if (mouse.leftButton.wasPressedThisFrame)  TryGrapple();
        if (mouse.leftButton.wasReleasedThisFrame) Release();

        // 按住 W 收繩，把自己往鉤點拉近
        var kb = Keyboard.current;
        if (joint.enabled && kb != null && kb.wKey.isPressed)
            joint.distance = Mathf.Max(minRopeLength, joint.distance - ropeSpeed * Time.deltaTime);

        DrawRope();
    }

    void TryGrapple()
    {
        Vector2 mouseWorld = transform.position;

        // ────────── TODO 5-A ──────────
        // 把滑鼠的「螢幕座標」轉成「世界座標」。
        // 提示：
        //   Vector3 sp = Mouse.current.position.ReadValue();
        //   sp.z = -Camera.main.transform.position.z;   // 坑 2：一定要給 z
        //   mouseWorld = Camera.main.ScreenToWorldPoint(sp);


        Vector2 dir = ((Vector2)mouseWorld - (Vector2)transform.position).normalized;
        if (dir == Vector2.zero) return;

        RaycastHit2D hit = default;

        // ────────── TODO 5-B ──────────
        // 從自己往 dir 射一條長度 maxDistance 的射線，只偵測 grappleLayers。
        // 提示：hit = Physics2D.Raycast(transform.position, dir, maxDistance, grappleLayers);


        if (hit.collider == null) return;

        // ────────── TODO 5-C ──────────
        // 把繩子接到打中的點上，然後啟動 joint。
        // 提示：
        //   joint.connectedAnchor = hit.point;
        //   joint.distance = Vector2.Distance(transform.position, hit.point);
        //   joint.enabled = true;

    }

    void Release()
    {
        joint.enabled = false;
    }

    /// <summary>把繩子畫出來（這段已經寫好了）。</summary>
    void DrawRope()
    {
        if (!joint.enabled) { line.positionCount = 0; return; }

        line.positionCount = 2;
        line.SetPosition(0, transform.position);
        line.SetPosition(1, joint.connectedAnchor);
    }
}
