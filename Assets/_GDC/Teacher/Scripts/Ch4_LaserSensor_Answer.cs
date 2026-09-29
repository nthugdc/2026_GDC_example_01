using UnityEngine;

/// <summary>第 4 章 TODO 4-B 的參考解答。</summary>
[RequireComponent(typeof(LineRenderer))]
public class Ch4_LaserSensor_Answer : MonoBehaviour
{
    [Header("射線設定")]
    public Vector2 direction = Vector2.down;
    public float maxDistance = 6f;
    public LayerMask detectLayers;

    [Header("連動的門")]
    public Transform door;
    public float openHeight = 3.6f;
    public float openSpeed  = 8f;
    public float closeSpeed = 1.6f;
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
        Vector2 dir = direction.normalized;

        // 解答 4-B
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, maxDistance, detectLayers);

        bool detected = hit.collider != null;

        float length = detected ? hit.distance : maxDistance;
        line.positionCount = 2;
        line.SetPosition(0, transform.position);
        line.SetPosition(1, transform.position + (Vector3)(dir * length));
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
