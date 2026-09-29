using UnityEngine;

namespace GDCBuild
{
    /// <summary>第 2 章 · 標籤 Tag：用 CompareTag 分辨碰到的是什麼。</summary>
    public static class Ch2Builder
    {
        const string BoardText =
@"第 2 章 · 標籤 Tag　　目標：吃光金幣、避開紅色尖刺、走到終點
1. 場景裡的金幣 / 尖刺 / 終點都還是 Untagged，請在 Inspector
　 最上方的 Tag 欄位，分別指派 Coin / Spike / Goal
2. 填完 Scripts/Student/Ch2_PlayerTouch.cs 裡的 3 個 TODO
提示：用 other.CompareTag(""Coin"")，不要用 other.tag == ""Coin""";

        static readonly float[] CoinX  = { 0f, 3f, 6f, 13f, 19f, 22f };
        static readonly float[] SpikeX = { 9.5f, 16.5f };

        public static void Build(bool answer = false)
        {
            var scene = GDCSceneKit.NewScene(new Vector2(2.7f, 1.3f), 6f);

            GDCSceneKit.BoardOnCamera("GuideBoard", BoardText, 3.4f);

            GDCSceneKit.PlatformTop("Ground", -8f, 32f, 0f);

            foreach (var x in CoinX)  AddCoin(x, answer);
            foreach (var x in SpikeX) AddSpike(x, answer);

            var player = GDCSceneKit.Player(new Vector2(-6f, 1.2f), true, -6f);
            if (answer)
            {
                player.AddComponent<Ch1_PlayerMove_Answer>();
                player.AddComponent<Ch2_PlayerTouch_Answer>();
            }
            else
            {
                // 沿用學生自己在第 1 章寫好的移動腳本
                player.AddComponent<Ch1_PlayerMove>();
                player.AddComponent<Ch2_PlayerTouch>();
            }

            AddGoal(29f, answer);

            GDCSceneKit.FollowPlayer(player, 2.7f, 21.3f);
            GDCSceneKit.Managers("第 2 章 · 標籤 Tag");
            GDCSceneKit.Save(scene, GDCSceneKit.ScenePath("Ch2_Tag", answer));
        }

        static void AddCoin(float x, bool answer)
        {
            var go = GDCSceneKit.Shape("Coin", "Star", GDCPalette.Coin,
                                       new Vector2(x, 1.5f), new Vector2(0.55f, 0.55f),
                                       GDCSceneKit.OrderObject);
            go.layer = LayerMask.NameToLayer("Pickup");
            if (answer) go.tag = "Coin";
            go.AddComponent<CircleCollider2D>().isTrigger = true;
        }

        static void AddSpike(float x, bool answer)
        {
            var go = GDCSceneKit.Shape("Spike", "Triangle", GDCPalette.Hazard,
                                       new Vector2(x, 0.35f), new Vector2(0.8f, 0.7f),
                                       GDCSceneKit.OrderObject);
            go.layer = LayerMask.NameToLayer("Hazard");
            if (answer) go.tag = "Spike";

            var col = go.AddComponent<BoxCollider2D>();
            col.isTrigger = true;
            col.size = new Vector2(0.8f, 1f);   // 三角形圖較尖，碰撞框稍微縮一點
        }

        static void AddGoal(float x, bool answer)
        {
            var go = GDCSceneKit.Shape("Goal", "Square", new Color(0.02f, 0.84f, 0.63f, 0.35f),
                                       new Vector2(x, 0.9f), new Vector2(1.6f, 1.8f),
                                       GDCSceneKit.OrderObject);
            go.layer = LayerMask.NameToLayer("Goal");
            if (answer) go.tag = "Goal";
            go.AddComponent<BoxCollider2D>().isTrigger = true;

            GDCSceneKit.Text("GoalLabel", "終點", new Vector2(x, 2.3f), new Vector2(3f, 0.8f),
                             4f, GDCPalette.Goal, GDCSceneKit.OrderObject,
                             TMPro.TextAlignmentOptions.Center);
        }
    }
}
