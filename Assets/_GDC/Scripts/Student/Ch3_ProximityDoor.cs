using UnityEngine;

/// <summary>
/// 第 3 章練習：用 LayerMask 只偵測「特定圖層」的東西。
/// 玩家靠近時門會升起，走遠了又降下來。
///
/// ═══ Physics2D.OverlapCircle ═══
///
///   Collider2D OverlapCircle(Vector2 point, float radius, int layerMask)
///
///   意思是「在 point 畫一個半徑 radius 的圓，看看圓內有沒有碰撞器」。
///
///     point       圓心的世界座標。想用自己的位置就傳 transform.position
///     radius      半徑（世界單位）
///     layerMask   只在乎哪些圖層。不給的話所有圖層都算，
///                 給了就只會找到那些圖層上的東西
///
///   ⚠ 回傳值是 Collider2D，不是 bool。
///     找到東西 → 回傳「其中一個」碰到的碰撞器
///     沒找到   → 回傳 null
///
///   所以要判斷「有沒有」，得自己跟 null 比較。
///   （想一次拿到全部，用 OverlapCircleAll，它回傳 Collider2D[]）
///
/// ═══ LayerMask 是什麼 ═══
///
///   它是一個「勾選清單」，在 Inspector 上可以複選圖層。
///   宣告成 public LayerMask，Unity 就會幫你畫出下拉勾選介面，
///   傳進 Physics2D 的函式時會自動轉成它要的數字。
///
///   ⚠ 沒勾任何東西的 LayerMask 代表「什麼都不找」，
///     這時候不管玩家走多近都偵測不到。
/// </summary>
public class Ch3_ProximityDoor : MonoBehaviour
{
    [Header("偵測設定")]
    [Tooltip("偵測範圍半徑（選取這個物件可以在 Scene 視窗看到圓圈）")]
    public float radius = 3.5f;

    [Tooltip("要偵測哪些圖層？請把 Player 勾起來")]
    public LayerMask detectLayers;

    [Header("門的動作")]
    [Tooltip("門要升多高")]
    public float openHeight = 3.5f;
    [Tooltip("升降速度")]
    public float speed = 8f;

    Vector3 closedPos;

    void Awake()
    {
        closedPos = transform.position;
    }

    void Update()
    {
        bool near = false;

        // ────────── TODO 3-A ──────────
        // 用 Physics2D.OverlapCircle 檢查「以自己為圓心、半徑 radius 的範圍內」，
        // 有沒有屬於 detectLayers 的東西。有的話把 near 設成 true。
        //
        // 回傳值不是 bool，請看上面的說明想一下要怎麼轉成 true / false。


        Vector3 target = near ? closedPos + Vector3.up * openHeight : closedPos;
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
    }

    // 在 Scene 視窗把偵測範圍畫出來，方便調整 radius
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
