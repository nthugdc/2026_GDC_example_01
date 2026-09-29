using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 第 4 章練習：用 Raycast 判斷腳下有沒有地板，修好「無限連跳」。
/// 移動與跳躍已經幫你寫好了（就是第 1 章的答案），你只要完成 IsGrounded()。
///
/// ═══ Raycast 是什麼 ═══
///
///   從一個點往一個方向射出一條看不見的線，回報它「第一個」打到的東西。
///   2D 的射線只會打到 Collider 2D。
///
///   RaycastHit2D Physics2D.Raycast(Vector2 origin, Vector2 direction,
///                                  float distance, int layerMask)
///
///     origin      起點的世界座標
///     direction   方向。只看方向不看長短，Vector2.down 就是正下方
///     distance    最遠射多遠。超過這個距離就算沒打到
///     layerMask   只在乎哪些圖層（跟第 3 章的 LayerMask 一樣）
///
/// ═══ 回傳的 hit 是什麼 ═══
///
///   RaycastHit2D 是一個「這一次射擊的結果報告」，它是 struct 不是物件，
///   所以「沒打到」的時候不會是 null，而是一份內容全空的報告。
///   要判斷有沒有打到，看它的 collider 欄位：
///
///     hit.collider    打到的碰撞器。沒打到時是 null ← 用這個判斷
///     hit.point       打到的那個「點」的世界座標
///     hit.distance    從起點到打中處的距離
///     hit.normal      被打到的表面朝哪個方向
///
///   ⚠ 不要寫 if (hit != null)，那永遠都成立，因為 struct 不會是 null。
/// </summary>
public class Ch4_PlayerMove : MonoBehaviour
{
    [Header("移動")]
    public float moveSpeed = 6f;
    public float jumpForce = 11f;

    [Header("地面偵測")]
    [Tooltip("射線起點要往下移多少（大約是身體的一半高）")]
    public float feetOffset = 0.6f;
    [Tooltip("射線長度。太短會跳不起來，太長會在空中也能跳")]
    public float checkDistance = 0.15f;
    [Tooltip("哪些圖層算是地板？請勾選 Ground")]
    public LayerMask groundLayers;

    Rigidbody2D rb;
    float moveInput;
    bool  jumpRequested;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Keyboard kb = Keyboard.current;
        if (kb == null) return;

        moveInput = 0f;
        if (kb.aKey.isPressed || kb.leftArrowKey.isPressed)  moveInput = -1f;
        if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) moveInput =  1f;

        if (kb.spaceKey.wasPressedThisFrame) jumpRequested = true;
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        if (jumpRequested)
        {
            jumpRequested = false;
            if (IsGrounded())
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    /// <summary>腳下有沒有踩到地板？</summary>
    bool IsGrounded()
    {
        Vector2 origin = (Vector2)transform.position + Vector2.down * feetOffset;

        // 把射線畫在 Scene 視窗裡，方便你看清楚它射到哪（只有 Scene 視窗看得到）
        Debug.DrawRay(origin, Vector2.down * checkDistance, Color.red);

        // ────────── TODO 4-A ──────────
        // 從 origin 往正下方射一條長度 checkDistance 的射線，只偵測 groundLayers，
        // 然後回傳「有沒有打到東西」。
        //
        // 上面的說明有寫該看 hit 的哪個欄位。


        return true;   // ← 還沒寫之前永遠回傳 true，所以現在可以無限連跳
    }
}
