using UnityEngine;

/// <summary>
/// 第 4 章練習：雷射感應器。射線打到玩家時，門就升起來。
/// 這台發射器固定裝在天花板上朝正下方射，所以方向直接用 Vector2.down。
/// 畫線與開關門的部分都寫好了，你只要完成那一行 Raycast。
///
/// ═══ 這裡的 Raycast ═══
///
///   RaycastHit2D Physics2D.Raycast(Vector2 origin, Vector2 direction,
///                                  float distance, int layerMask)
///
///   跟 Ch4_PlayerMove 裡用的是同一個函式，差別只在：
///     ・起點是這台發射器自己（transform.position）
///     ・距離用 maxDistance
///     ・圖層用 detectLayers —— 這次要勾的是 Player，不是 Ground
///
///   一樣記得：hit 是 struct，判斷有沒有打到要看 hit.collider 是不是 null。
///
/// ═══ 為什麼一定要給 layerMask ═══
///
///   不給的話射線會打到它遇到的第一個東西 —— 包括地板、金幣、甚至門本身，
///   那扇門就永遠不會為了玩家打開。LayerMask 讓射線「只看得見」你在乎的東西。
/// </summary>
[RequireComponent(typeof(LineRenderer))]
public class Ch4_LaserSensor : MonoBehaviour
{
    [Header("射線設定")]
    [Tooltip("射線最長射多遠")]
    public float maxDistance = 6f;
    [Tooltip("要偵測哪些圖層？請勾選 Player")]
    public LayerMask detectLayers;

    [Header("連動的門")]
    public Transform door;
    [Tooltip("門升起的高度")]
    public float openHeight = 3.6f;
    [Tooltip("門升起的速度（快一點，踩到雷射馬上有反應）")]
    public float openSpeed = 8f;
    [Tooltip("門落下的速度（慢一點，才來得及跑過去）")]
    public float closeSpeed = 1.6f;
    [Tooltip("離開雷射後，等幾秒才開始關門")]
    public float closeDelay = 1.2f;

    LineRenderer line;
    Vector3 doorClosedPos;
    float closeTimer;      // 還要維持開啟幾秒

    void Awake()
    {
        line = GetComponent<LineRenderer>();
        if (door != null) doorClosedPos = door.position;
    }

    void Update()
    {
        RaycastHit2D hit = default;

        // ────────── TODO 4-B ──────────
        // 從這台發射器的位置往正下方射一條長度 maxDistance 的射線，
        // 只偵測 detectLayers，把結果放進上面的 hit。


        bool detected = hit.collider != null;

        // 畫出雷射：打到東西就畫到接觸點，沒打到就畫滿長度
        float length = detected ? hit.distance : maxDistance;
        line.positionCount = 2;
        line.SetPosition(0, transform.position);
        line.SetPosition(1, transform.position + Vector3.down * length);
        line.startColor = line.endColor = detected ? Color.green : Color.red;

        // 開關門：離開雷射後先等 closeDelay 秒，再用較慢的速度落下，
        // 玩家才來得及跑過去（不然一離開射線門就砸下來）
        if (detected) closeTimer = closeDelay;
        else          closeTimer -= Time.deltaTime;

        if (door != null)
        {
            bool open = detected || closeTimer > 0f;
            Vector3 target = open ? doorClosedPos + Vector3.up * openHeight : doorClosedPos;
            float speed = open ? openSpeed : closeSpeed;
            door.position = Vector3.MoveTowards(door.position, target, speed * Time.deltaTime);
        }
    }
}
