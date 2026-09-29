using UnityEditor;
using UnityEngine;

namespace GDCBuild
{
    /// <summary>第 0 章 · 物理沙盒：完全不用寫程式，只調 Inspector 參數。</summary>
    public static class Ch0Builder
    {
        const string BoardText =
@"第 0 章 · 物理沙盒
────────────────
目標：讓黃球滾進右邊的籃子。
這一章不用寫程式，只要調參數！

【重要】要先「停止播放」再改參數
播放中改的數值，一停止就會被還原。

1. 按 ■ 停止播放
2. 在 Hierarchy 點選「Ball」
3. 在 Inspector 改 Rigidbody 2D 的參數
4. 再按 ▶ 播放，看結果

・Linear Damping（空氣阻力）
　預設 2，太大了衝不過缺口
・Gravity Scale（重力倍率）
・Mass（質量）改改看有差嗎？

材質練習：把 _GDC/Physics 裡的
Slippery 拖到 Ball 的 Circle
Collider 2D 的 Material 欄位。";

        public static void Build(bool answer = false)
        {
            var scene = GDCSceneKit.NewScene(new Vector2(0f, 0.5f), 6.5f);

            GDCSceneKit.Board("GuideBoard", BoardText,
                              new Vector2(-7.2f, 1.2f), new Vector2(8.2f, 10.2f), 4.0f);

            BuildPuzzle(answer);
            BuildBounceDemo();

            GDCSceneKit.Managers("第 0 章 · 物理沙盒");
            GDCSceneKit.Save(scene, GDCSceneKit.ScenePath("Ch0_Sandbox", answer));
        }

        // ── 主謎題：斜坡 → 缺口 → 籃子 ──────────────────────────────────
        static void BuildPuzzle(bool answer)
        {
            // 斜坡
            GDCSceneKit.Platform("Ramp", new Vector2(0.3f, 2.2f), new Vector2(7.2f, 0.4f), -36f);

            // 起跳平台與缺口
            GDCSceneKit.PlatformTop("PlatformA", 2.6f, 5.6f, 0f);
            GDCSceneKit.PlatformTop("PlatformB_BasketFloor", 6.9f, 10.9f, 0f);
            GDCSceneKit.Platform("BasketWall", new Vector2(11.05f, 0.8f), new Vector2(0.3f, 1.6f));

            // 缺口提示
            GDCSceneKit.Text("GapLabel", "缺口", new Vector2(6.25f, 1.0f), new Vector2(3f, 0.8f),
                             3.5f, GDCPalette.Hazard, GDCSceneKit.OrderObject,
                             TMPro.TextAlignmentOptions.Center);

            // 球
            var ball = GDCSceneKit.Shape("Ball", "Circle", GDCPalette.Player,
                                         new Vector2(-2.4f, 5.6f), new Vector2(0.7f, 0.7f),
                                         GDCSceneKit.OrderActor);
            var rb = ball.AddComponent<Rigidbody2D>();
            rb.gravityScale           = 1f;
            rb.mass                   = 1f;
            rb.linearDamping          = answer ? 0f : 2f;  // 學生版刻意設太大，要自己調小
            rb.angularDamping         = 0.05f;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

            var col = ball.AddComponent<CircleCollider2D>();
            col.sharedMaterial = GDCPaths.Phys("Normal");

            var respawn = ball.AddComponent<Respawner>();
            respawn.killY = -1.6f;               // 掉進缺口就回到起點

            // 目標區
            var goal = GDCSceneKit.Shape("BasketZone", "Square", new Color(0.02f, 0.84f, 0.63f, 0.18f),
                                         new Vector2(8.9f, 0.6f), new Vector2(3.6f, 1.2f),
                                         GDCSceneKit.OrderObject);
            goal.layer = LayerMask.NameToLayer("Goal");
            var goalCol = goal.AddComponent<BoxCollider2D>();
            goalCol.isTrigger = true;

            var settle = goal.AddComponent<SettleGoal>();
            settle.targetBody   = rb;
            settle.clearMessage = "過關！球成功停進籃子了";

            GDCSceneKit.Text("BasketLabel", "籃子", new Vector2(8.9f, 1.9f), new Vector2(3f, 0.8f),
                             3.5f, GDCPalette.Goal, GDCSceneKit.OrderObject,
                             TMPro.TextAlignmentOptions.Center);
        }

        // ── 彈跳材質觀察區 ──────────────────────────────────────────────
        static void BuildBounceDemo()
        {
            GDCSceneKit.Text("BounceDemoTitle", "彈跳材質觀察區：三顆球的 Material 不同，按 R 可重看一次",
                             new Vector2(4.5f, -1.1f), new Vector2(11f, 0.7f),
                             4f, GDCPalette.TextDim, GDCSceneKit.OrderObject,
                             TMPro.TextAlignmentOptions.Center);

            GDCSceneKit.PlatformTop("BounceDemoFloor", -1.0f, 10.0f, -5.0f);

            AddDemoBall("Normal",      "Normal\n彈性 0",       1.0f);
            AddDemoBall("Bouncy",      "Bouncy\n彈性 0.6",     4.5f);
            AddDemoBall("SuperBouncy", "SuperBouncy\n彈性 0.95", 8.0f);
        }

        static void AddDemoBall(string mat, string label, float x)
        {
            GDCSceneKit.Text("Label_" + mat, label, new Vector2(x, -2.15f), new Vector2(3.2f, 0.9f),
                             3.6f, GDCPalette.TextDim, GDCSceneKit.OrderObject,
                             TMPro.TextAlignmentOptions.Center);

            var ball = GDCSceneKit.Shape("DemoBall_" + mat, "Circle", GDCPalette.Coin,
                                         new Vector2(x, -3.1f), new Vector2(0.55f, 0.55f),
                                         GDCSceneKit.OrderActor);
            ball.AddComponent<Rigidbody2D>();
            ball.AddComponent<CircleCollider2D>().sharedMaterial = GDCPaths.Phys(mat);
        }
    }
}
