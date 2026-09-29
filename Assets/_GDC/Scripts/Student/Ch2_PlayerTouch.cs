using UnityEngine;

/// <summary>
/// 第 2 章練習：用 Tag 分辨「碰到的是什麼東西」。
///
/// Tag 回答的是「這是什麼」；第 3 章的 Layer 回答的是「誰能碰到誰」。
///
/// ═══ 三個 Trigger 事件的差別 ═══
///
///   OnTriggerEnter2D   剛碰到的那一幀，呼叫「一次」
///   OnTriggerStay2D    只要還重疊著，「每一幀」都呼叫
///   OnTriggerExit2D    離開的那一幀，呼叫「一次」
///
///   要收得到這些事件，對方的 Collider 2D 必須勾選 Is Trigger。
///
///   ⚠ Stay 有一個很容易踩到的坑：
///     Rigidbody 2D 靜止超過 0.5 秒會自動「休眠」以節省效能，
///     而休眠中的剛體不會再發出 Stay 事件 —— 站著不動計時就卡住了，
///     稍微走動又恢復。本專案已經把玩家的 Sleeping Mode 設成 Never Sleep。
///     （Rigidbody 2D → Sleeping Mode，可以改回 Start Awake 親眼看一次）
///
/// ═══ 參數 other 是什麼？ ═══
///
///   other 的型別是 Collider2D，它是「碰到我的那個東西身上的碰撞器元件」。
///   注意它是一個「元件（Component）」，不是那個物件本身。
///
///   在 Unity 裡，只要拿到任何一個元件，就能從它身上找到所屬的物件與其他元件：
///
///     other.gameObject            那個東西本身（GameObject）
///     other.gameObject.name       它在 Hierarchy 裡的名字
///     other.CompareTag("Coin")    它的 Tag 是不是 Coin
///     other.GetComponent<T>()     從它身上再拿出別的元件，例如 SpriteRenderer
///
///   這就是為什麼你可以「碰到對方，然後去改對方的顏色」。
///
/// ═══ Destroy 的效果 ═══
///
///   Destroy(other.gameObject)  把整個物件從場景中移除，它會直接消失。
///   Destroy(other)             只移除那個碰撞器元件 —— 物件還在，只是不再會被碰到。
///   想讓金幣消失要用前者。（刪除會在這一幀結束後才真的發生）
///
/// ═══ 你可以呼叫的東西 ═══
///
///   GameManager.Instance        場景裡的總管，任何腳本都能拿到
///     .AddScore(int amount)     加分（給負數就是扣分）
///     .Clear(string message)    過關，畫面會跳出綠色訊息。只會生效一次
///     .Fail(string message)     顯示一行紅色訊息，不會鎖住關卡
///     .score                    目前分數（int）
///     .statusMessage            HUD 上顯示的那一行字，可以直接指定
///
///   Respawner                   掛在玩家身上的重生元件
///     .Respawn()                把玩家送回起點，速度歸零
/// </summary>
public class Ch2_PlayerTouch : MonoBehaviour
{
    [Header("分數設定")]
    [Tooltip("吃到一枚金幣加幾分")]
    public int coinScore = 1;
    [Tooltip("踩到尖刺扣幾分")]
    public int spikePenalty = 2;

    [Header("感應板顏色")]
    [Tooltip("踩上去時的顏色")]
    public Color padOnColor  = new Color(0.02f, 0.84f, 0.63f);
    [Tooltip("離開後要變回的顏色")]
    public Color padOffColor = new Color(0.29f, 0.35f, 0.42f);

    [Header("執行時觀察用（不用自己改）")]
    [Tooltip("站在感應板上的累計秒數，播放時看這個數字怎麼跳")]
    public float padSeconds;

    Respawner respawner;

    void Awake()
    {
        respawner = GetComponent<Respawner>();
    }

    // ══════════ 剛碰到的那一幀，呼叫一次 ══════════
    void OnTriggerEnter2D(Collider2D other)
    {
        GameManager gm = GameManager.Instance;

        // ────────── TODO 2-A ──────────
        // 碰到 Tag 是 Coin 的東西：加 coinScore 分，然後讓那枚金幣消失。
        //  ⚠ 如果出現「Tag: Coin is not defined」，代表你還沒在
        //    Project Settings > Tags and Layers 把這個 Tag 指派給金幣。


        // ────────── TODO 2-B ──────────
        // 碰到 Tag 是 Spike 的東西：扣 spikePenalty 分，並把玩家送回起點。


        // ────────── TODO 2-C ──────────
        // 碰到 Tag 是 Goal 的東西：過關，訊息自己寫。


        // ── 以下是「已經幫你寫好」的範例，Stay 和 Exit 請照這個樣子寫 ──
        // 踩上感應板 → 把「感應板自己」的顏色換成亮色。
        // 注意我們是從 other 身上拿到它的 SpriteRenderer，再改它的 color。
        if (other.CompareTag("SensorPad"))
        {
            other.GetComponent<SpriteRenderer>().color = padOnColor;
            padSeconds = 0f;
        }
    }

    // ══════════ 只要還重疊著，每一幀都會呼叫 ══════════
    void OnTriggerStay2D(Collider2D other)
    {
        // ────────── TODO 2-D ──────────
        // 站在感應板（Tag 是 SensorPad）上的時候：
        //   1. 把 padSeconds 累加這一幀經過的時間（Time.deltaTime）
        //   2. 把停留秒數顯示到 HUD 上（用 GameManager.Instance.statusMessage）
        //
        // 寫完播放看看：Inspector 裡的 Pad Seconds 會一直往上跳，
        // 這就是 Stay 和 Enter 最大的差別 —— 它每一幀都在跑。

    }

    // ══════════ 離開的那一幀，呼叫一次 ══════════
    void OnTriggerExit2D(Collider2D other)
    {
        // ────────── TODO 2-E ──────────
        // 離開感應板的時候：
        //   1. 把感應板的顏色變回 padOffColor
        //   2. 把 HUD 訊息換成「離開感應板，總共站了 ? 秒」
        //
        // 顏色的改法跟上面 Enter 裡的範例一模一樣。

    }
}
