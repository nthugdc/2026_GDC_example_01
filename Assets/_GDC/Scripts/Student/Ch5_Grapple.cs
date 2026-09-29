using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 第 5 章練習：鉤爪。用滑鼠瞄準天花板的紫色鉤點，按住左鍵盪過深淵。
///
/// ═══ DistanceJoint2D：這一章的主角 ═══
///
///   它的工作是「把我和某個點之間的距離限制住」，就像一條繩子。
///   你要動到的欄位只有這幾個：
///
///     enabled                  繩子存不存在。false = 沒鉤住，true = 正在吊著
///     connectedBody            繩子另一端綁在哪個 Rigidbody2D 上。
///                              留成 null 代表「綁在世界上的一個固定點」，正是我們要的
///     connectedAnchor          那個固定點的座標。
///                              因為 connectedBody 是 null，這裡填的是「世界座標」
///     distance                 繩子的長度
///     autoConfigureDistance    如果是 true，Unity 每幀會自己把 distance 改成
///                              「目前的實際距離」，於是你永遠盪不起來 ←【坑 1】
///                              本專案已經在 Awake 幫你關掉了
///     maxDistanceOnly          true = 只限制「最遠」距離（像繩子，可以靠近）
///                              false = 距離被鎖死（像鐵棍）
///
/// ═══ 滑鼠座標：螢幕 vs 世界 ═══
///
///   Mouse.current.position.ReadValue()
///       滑鼠在「螢幕」上的位置，單位是像素，左下角是 (0, 0)。
///       這個數字不能直接拿來跟場景裡的物件比較。
///
///   Camera.main.ScreenToWorldPoint(Vector3 screenPos)
///       把螢幕座標換算成世界座標。
///       ⚠【坑 2】參數是 Vector3，第三個值 z 代表「離攝影機多遠」。
///         z 留成 0 的話，算出來的點會全部貼在攝影機平面上，
///         結果就是不管滑鼠指哪裡，方向都怪怪的。
///         本專案攝影機在 z = -10，所以 z 要給 10（也就是 -攝影機的 z）。
///
/// ═══ Raycast ═══
///
///   跟第 4 章同一個函式：
///   Physics2D.Raycast(起點, 方向, 距離, 圖層) → RaycastHit2D
///   一樣用 hit.collider 是不是 null 來判斷有沒有打到。
///   ⚠【坑 3】Grapple Layers 沒勾 Grappleable 的話，不是鉤到地板就是什麼都鉤不到。
///
/// ═══ 好用的小工具 ═══
///
///   (a - b).normalized            從 b 指向 a 的「單位方向向量」（長度固定為 1）
///   Vector2.Distance(a, b)        兩點之間的距離
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
        joint.autoConfigureDistance = false;   // 坑 1 已經幫你關掉了

        line = GetComponent<LineRenderer>();
        line.positionCount = 0;
    }

    void Update()
    {
        Mouse mouse = Mouse.current;
        if (mouse == null) return;

        if (mouse.leftButton.wasPressedThisFrame)  TryGrapple();
        if (mouse.leftButton.wasReleasedThisFrame) Release();

        // 按住 W 收繩，把自己往鉤點拉近
        Keyboard kb = Keyboard.current;
        if (joint.enabled && kb != null && kb.wKey.isPressed)
            joint.distance = Mathf.Max(minRopeLength, joint.distance - ropeSpeed * Time.deltaTime);

        DrawRope();
    }

    void TryGrapple()
    {
        Vector2 mouseWorld = transform.position;

        // ────────── TODO 5-A ──────────
        // 讀出滑鼠的螢幕座標，換算成世界座標，放進 mouseWorld。
        // 上面【坑 2】的說明有寫 z 要給什麼。


        // ────────── TODO 5-B ──────────
        // 算出「從自己指向 mouseWorld」的方向，
        // 然後往那個方向射一條長度 maxDistance 的射線，只偵測 grappleLayers。
        // 沒打到任何東西的話就直接 return，不要鉤。

        RaycastHit2D hit = default;


        if (hit.collider == null) return;

        // ────────── TODO 5-C ──────────
        // 把繩子接到打中的那個點上，然後讓繩子生效。
        // 需要設定的欄位：接點在哪、繩子多長、還有讓它開始作用。
        // 三個欄位的名字都寫在最上方的說明裡。

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
