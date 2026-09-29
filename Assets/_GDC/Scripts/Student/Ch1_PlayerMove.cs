using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 第 1 章練習：讓玩家可以左右移動與跳躍。
///
/// ═══ 為什麼要分成 Update 和 FixedUpdate？ ═══
///
///   Update       每畫一張畫面呼叫一次。張數會隨電腦效能浮動（60、144、30…）
///                → 適合「讀玩家按了什麼」
///   FixedUpdate  固定每秒 50 次，不受畫面張數影響
///                → 適合「改物理」，這樣不同效能的電腦手感才會一致
///
///   所以分工是：Update 記錄玩家的操作，FixedUpdate 才真的推動身體。
///
/// ═══ 你會用到的東西 ═══
///
///   Keyboard.current.dKey.isPressed
///       D 鍵「現在」是不是被按著。回傳 true / false。
///       只要按著不放，每一幀都是 true。
///
///   Keyboard.current.spaceKey.wasPressedThisFrame
///       空白鍵是不是「這一幀剛被按下去」。按著不放的話只有第一幀是 true。
///
///   new Vector2(x, y)
///       建立一個二維向量。Vector2 是「值型別」（struct），
///       不能只改它的一半，要整個換一個新的 —— 所以前面一定要加 new。
///       試試看直接寫 rb.linearVelocity.x = 5f，Unity 會直接不讓你編譯。
///
///   rb.linearVelocity
///       Rigidbody2D 目前的速度，型別就是 Vector2，有 .x 和 .y 兩個分量。
///       ⚠ Unity 6 之前這個叫 velocity。網路上的教學寫 rb.velocity，
///         貼到這裡會編譯失敗，要自己換成 linearVelocity。
///
///   rb.AddForce(力, ForceMode2D.Impulse)
///       對身體施加一道「瞬間」的力（Impulse = 撞一下，不是持續推）。
///       力也是 Vector2。Vector2.up 就等於 new Vector2(0f, 1f)。
///
/// ⚠ 本專案使用「新版 Input System」，沒有 Input.GetKeyDown()。
/// </summary>
public class Ch1_PlayerMove : MonoBehaviour
{
    [Header("移動")]
    [Tooltip("左右移動速度（單位 / 秒），試試 3 ~ 12")]
    public float moveSpeed = 6f;

    [Header("跳躍")]
    [Tooltip("跳躍力道，越大跳越高，試試 6 ~ 16")]
    public float jumpForce = 11f;

    Rigidbody2D rb;

    // Update 寫進這兩個變數，FixedUpdate 再拿去用
    float moveInput;       // -1 = 往左、0 = 不動、1 = 往右
    bool  jumpRequested;   // 玩家有沒有要求跳躍

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // ── 只負責讀輸入，不要在這裡改物理 ──
    void Update()
    {
        Keyboard kb = Keyboard.current;
        if (kb == null) return;

        moveInput = 0f;

        // ────────── TODO 1-A ──────────
        // 讀鍵盤，把 moveInput 設成 -1（往左）、0（不動）或 1（往右）。
        // A 和 ← 都算往左，D 和 → 都算往右。


        if (kb.spaceKey.wasPressedThisFrame) jumpRequested = true;
    }

    // ── 真正改變身體的速度 ──
    void FixedUpdate()
    {
        // ────────── TODO 1-B ──────────
        // 把水平速度設成 moveInput * moveSpeed，
        // 但「垂直速度要保持原本的值」，不可以歸零。
        //
        // 想一想：如果你寫成 rb.linearVelocity = new Vector2(moveInput * moveSpeed, 0f)，
        //         跳到一半的時候會發生什麼事？寫完可以故意試一次，看看差別。


        if (jumpRequested)
        {
            jumpRequested = false;

            // ────────── TODO 1-C ──────────
            // 往正上方施加一道「瞬間」的力，大小用 jumpForce。

        }
    }
}
