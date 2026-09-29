using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>第 1 章 TODO 1-A ~ 1-C 的參考解答。</summary>
public class Ch1_PlayerMove_Answer : MonoBehaviour
{
    [Header("移動")]
    public float moveSpeed = 6f;

    [Header("跳躍")]
    public float jumpForce = 11f;

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

        // 解答 1-A
        if (kb.aKey.isPressed || kb.leftArrowKey.isPressed)  moveInput = -1f;
        if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) moveInput =  1f;

        if (kb.spaceKey.wasPressedThisFrame) jumpRequested = true;
    }

    void FixedUpdate()
    {
        // 解答 1-B：保留原本的 y，不然跳到一半會被歸零變成瞬間掉下來
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        if (jumpRequested)
        {
            jumpRequested = false;

            // 解答 1-C
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }
}
