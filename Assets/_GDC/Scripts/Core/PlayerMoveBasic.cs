using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 完整可用的基本移動腳本（第 1 章與第 4 章的答案合起來）。
/// 第 5、6 章直接用這支，讓你專心在該章的主題上。
/// 分工跟第 1 章一樣：Update 讀輸入、FixedUpdate 改物理。
/// </summary>
public class PlayerMoveBasic : MonoBehaviour
{
    [Header("移動")]
    public float moveSpeed = 6f;
    public float jumpForce = 11f;

    [Header("盪繩時的操控（第 5 章用得到）")]
    [Tooltip("掛在繩子上時，左右鍵改成「加速度」而不是直接設定速度。太大會像裝了火箭，建議 5 ~ 10")]
    public float swingAccel = 7f;
    [Tooltip("左右鍵最多只能把水平速度推到這麼快，超過就不再加力（擺盪本身的速度不受限制）")]
    public float swingMaxAssist = 8f;

    [Header("地面偵測")]
    public float feetOffset = 0.6f;
    public float checkDistance = 0.15f;
    public LayerMask groundLayers;

    Rigidbody2D rb;
    DistanceJoint2D rope;   // 鉤爪用的繩子，沒有這個元件就是 null

    float moveInput;
    bool  jumpRequested;

    void Awake()
    {
        rb   = GetComponent<Rigidbody2D>();
        rope = GetComponent<DistanceJoint2D>();
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
        if (rope != null && rope.enabled)
        {
            // 掛在繩子上時只「加速」，沒按鍵就完全不碰速度。
            // 如果照平常那樣每幀寫入速度，等於把擺盪累積的動能一直歸零，
            // 盪起來會又慢又沒力。
            //
            // 但也不能無限加速，不然按著不放就變成裝了火箭。
            // 只有「往你要推的方向還不夠快」時才繼續加力 ——
            // 這樣既能像盪鞦韆那樣配合節奏助推，也能反向煞車，
            // 而擺盪本身盪出來的速度不會被這個上限影響。
            float speedInPushDir = moveInput * rb.linearVelocity.x;
            if (moveInput != 0f && speedInPushDir < swingMaxAssist)
                rb.linearVelocityX += moveInput * swingAccel * Time.fixedDeltaTime;
        }
        else
        {
            rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
        }

        if (jumpRequested)
        {
            jumpRequested = false;
            if (IsGrounded())
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    public bool IsGrounded()
    {
        Vector2 origin = (Vector2)transform.position + Vector2.down * feetOffset;
        return Physics2D.Raycast(origin, Vector2.down, checkDistance, groundLayers).collider != null;
    }
}
