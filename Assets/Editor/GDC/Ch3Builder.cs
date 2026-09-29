using UnityEditor;
using UnityEngine;

namespace GDCBuild
{
    /// <summary>
    /// 第 3 章 · 圖層 Layer：單向平台、碰撞矩陣、LayerMask 感應門。
    /// 這一章大部分是 Inspector 與 Project Settings 的操作。
    /// </summary>
    public static class Ch3Builder
    {
        const string BoardText =
@"第 3 章 · 圖層 Layer　　Tag 是「這是什麼」，Layer 是「誰能碰到誰」
1. OneWayPlatform_Task1（黃色）：新增 OneWay 圖層並指派給它 → 加 Platform Effector 2D
　 → 在 Box Collider 2D 勾 Used By Effector，就能從下面跳上去拿金幣
2. GhostWall_Task2 擋路：Project Settings > Physics 2D，取消 Player × GhostWall 的勾選
3. ProximityDoor_Task3：填完 Ch3_ProximityDoor.cs 的 TODO，並把 Detect Layers 設成 Player";

        public static void Build(bool answer = false)
        {
            var scene = GDCSceneKit.NewScene(new Vector2(2.7f, 1.3f), 6f);

            GDCSceneKit.BoardOnCamera("GuideBoard", BoardText, 3.4f);

            GDCSceneKit.PlatformTop("Ground", -8f, 36f, 0f);

            var player = GDCSceneKit.Player(new Vector2(-6f, 1.2f), true, -6f);
            if (answer)
            {
                player.AddComponent<Ch1_PlayerMove_Answer>();
                player.AddComponent<Ch2_PlayerTouch_Answer>();
            }
            else
            {
                player.AddComponent<Ch1_PlayerMove>();
                player.AddComponent<Ch2_PlayerTouch>();
            }

            BuildOneWayLedge(answer);
            BuildGhostWall(answer);
            BuildProximityDoor(answer);

            GDCSceneKit.Goal(new Vector2(33f, 0.9f), player.GetComponent<Rigidbody2D>(),
                             "過關！你搞懂 Layer 了", new Vector2(1.6f, 1.8f));

            GDCSceneKit.FollowPlayer(player, 2.7f, 25.3f);
            GDCSceneKit.Managers("第 3 章 · 圖層 Layer");
            GDCSceneKit.Save(scene, GDCSceneKit.ScenePath("Ch3_Layer", answer));
        }

        // 練習 1：單向平台。學生要自己建 OneWay 圖層並加上 Platform Effector 2D。
        static void BuildOneWayLedge(bool answer)
        {
            var ledge = GDCSceneKit.PlatformTop("OneWayPlatform_Task1", 6f, 12f, 3.4f);
            ledge.GetComponent<SpriteRenderer>().color = GDCPalette.Coin * 0.8f;

            if (answer)
            {
                ledge.layer = LayerMask.NameToLayer("OneWay");
                ledge.GetComponent<BoxCollider2D>().usedByEffector = true;

                var eff = ledge.AddComponent<PlatformEffector2D>();
                eff.useOneWay       = true;
                eff.surfaceArc      = 140f;
                eff.useSideFriction = false;
            }

            // 平台上放三枚金幣當獎勵
            for (int i = 0; i < 3; i++)
            {
                var coin = GDCSceneKit.Shape("Coin", "Star", GDCPalette.Coin,
                                             new Vector2(7.5f + i * 1.5f, 4.3f),
                                             new Vector2(0.55f, 0.55f), GDCSceneKit.OrderObject);
                coin.layer = LayerMask.NameToLayer("Pickup");
                coin.tag   = "Coin";
                coin.AddComponent<CircleCollider2D>().isTrigger = true;
            }
        }

        // 練習 2：紅牆。學生要在碰撞矩陣取消 Player × GhostWall 才能穿過去。
        static void BuildGhostWall(bool answer)
        {
            var wall = GDCSceneKit.Platform("GhostWall_Task2", new Vector2(18f, 1.6f), new Vector2(0.6f, 3.2f));
            wall.layer = LayerMask.NameToLayer("GhostWall");
            wall.GetComponent<SpriteRenderer>().color = GDCPalette.Hazard;

            if (answer) wall.AddComponent<Ch3_IgnoreLayer_Answer>();

            GDCSceneKit.Text("GhostWallLabel", "取消 Player × GhostWall\n的碰撞才能穿過",
                             new Vector2(18f, 4.4f), new Vector2(6f, 1.6f),
                             3.4f, GDCPalette.Hazard, GDCSceneKit.OrderObject,
                             TMPro.TextAlignmentOptions.Center);
        }

        // 練習 3：LayerMask 感應門
        static void BuildProximityDoor(bool answer)
        {
            var door = GDCSceneKit.Platform("ProximityDoor_Task3", new Vector2(27f, 1.6f), new Vector2(0.8f, 3.2f));
            door.GetComponent<SpriteRenderer>().color = GDCPalette.Grapple;

            if (answer)
            {
                var d = door.AddComponent<Ch3_ProximityDoor_Answer>();
                d.detectLayers = 1 << LayerMask.NameToLayer("Player");
            }
            else
            {
                door.AddComponent<Ch3_ProximityDoor>();   // Detect Layers 留空，讓學生自己勾
            }

            GDCSceneKit.Text("ProximityDoorLabel", "靠近時應該要升起來",
                             new Vector2(27f, 4.4f), new Vector2(6f, 1.2f),
                             3.4f, GDCPalette.Grapple, GDCSceneKit.OrderObject,
                             TMPro.TextAlignmentOptions.Center);
        }
    }
}
