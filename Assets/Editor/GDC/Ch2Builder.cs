using UnityEngine;

namespace GDCBuild
{
    /// <summary>第 2 章 · 標籤 Tag：用 CompareTag 分辨碰到的是什麼。</summary>
    public static class Ch2Builder
    {
        const string BoardText =
@"第 2 章 · 標籤 Tag　　目標：吃光金幣、避開紅色尖刺、踩上感應板、走到終點
1. 金幣 / 尖刺 / 終點目前都還是 Untagged，請在 Inspector 最上方的
　 Tag 欄位分別指派 Coin / Spike / Goal（感應板已經標好了）
2. 填完 Scripts/Student/Ch2_PlayerTouch.cs 的 5 個 TODO
　 腳本最上方有 Trigger 三個事件的差別與可用 API 的說明，先看過再寫";

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

            AddSensorPad(25f);
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

        /// <summary>感應板：練習 OnTriggerStay2D / OnTriggerExit2D 用。
        /// Tag 直接標好，這一關的重點是三個事件的差別，不是再標一次 Tag。</summary>
        static void AddSensorPad(float x)
        {
            var pad = GDCSceneKit.Shape("SensorPad", "Square", new Color(0.29f, 0.35f, 0.42f),
                                        new Vector2(x, 0.12f), new Vector2(3.2f, 0.24f),
                                        GDCSceneKit.OrderObject);
            pad.tag = "SensorPad";
            // BoxCollider2D 的 size / offset 會再乘上物件縮放，
            // 所以這裡用「想要的世界尺寸 ÷ 縮放」回推。
            // 玩家站在地面上時大約佔 y = 0.0 ~ 1.2，判定範圍要蓋住這一段。
            const float padScaleY = 0.24f;
            var col = pad.AddComponent<BoxCollider2D>();
            col.isTrigger = true;
            col.size   = new Vector2(1f, 1.6f / padScaleY);   // 世界高度 1.6
            col.offset = new Vector2(0f, 0.63f / padScaleY);  // 世界中心抬到 y ≈ 0.75

            GDCSceneKit.Text("SensorPadLabel", "感應板：站上去看看", new Vector2(x, 1.6f),
                             new Vector2(5f, 0.8f), 3.4f, GDCPalette.TextDim,
                             GDCSceneKit.OrderObject, TMPro.TextAlignmentOptions.Center);
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
