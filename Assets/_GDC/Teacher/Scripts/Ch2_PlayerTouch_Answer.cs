using UnityEngine;

/// <summary>第 2 章 TODO 2-A ~ 2-E 的參考解答。</summary>
public class Ch2_PlayerTouch_Answer : MonoBehaviour
{
    [Header("分數設定")]
    public int coinScore = 1;
    public int spikePenalty = 2;

    [Header("感應板顏色")]
    public Color padOnColor  = new Color(0.02f, 0.84f, 0.63f);
    public Color padOffColor = new Color(0.29f, 0.35f, 0.42f);

    [Header("執行時觀察用")]
    public float padSeconds;

    Respawner respawner;

    void Awake()
    {
        respawner = GetComponent<Respawner>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        GameManager gm = GameManager.Instance;

        // 解答 2-A
        if (other.CompareTag("Coin"))
        {
            gm.AddScore(coinScore);
            Destroy(other.gameObject);
        }

        // 解答 2-B
        if (other.CompareTag("Spike"))
        {
            gm.AddScore(-spikePenalty);
            respawner.Respawn();
        }

        // 解答 2-C
        if (other.CompareTag("Goal"))
        {
            gm.Clear("過關！你學會 Tag 了");
        }

        if (other.CompareTag("SensorPad"))
        {
            other.GetComponent<SpriteRenderer>().color = padOnColor;
            padSeconds = 0f;
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        // 解答 2-D
        if (other.CompareTag("SensorPad"))
        {
            padSeconds += Time.deltaTime;
            GameManager.Instance.statusMessage = "站在感應板上 " + padSeconds.ToString("F1") + " 秒";
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        // 解答 2-E
        if (other.CompareTag("SensorPad"))
        {
            other.GetComponent<SpriteRenderer>().color = padOffColor;
            GameManager.Instance.statusMessage = "離開感應板，總共站了 " + padSeconds.ToString("F1") + " 秒";
        }
    }
}
