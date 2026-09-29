using UnityEngine;

namespace GDCBuild
{
    /// <summary>第 4 章 · 射線 Raycast：修好無限跳，並做出雷射感應門。</summary>
    public static class Ch4Builder
    {
        const string BoardText =
@"第 4 章 · 射線 Raycast　　Raycast 就是「往某個方向射一條看不見的線，問它打到什麼」
1. 修好無限跳：填完 Ch4_PlayerMove.cs 的 TODO 4-A，並把 Ground Layers 勾成 Ground
　 調 Check Distance 看看：太短跳不起來，太長會在空中也能跳
2. 雷射門：填完 Ch4_LaserSensor.cs 的 TODO 4-B，把 Detect Layers 勾成 Player
　 站到紅色雷射下面，門就會升起來（射線會在 Scene 視窗畫出來）";

        public static void Build(bool answer = false)
        {
            var scene = GDCSceneKit.NewScene(new Vector2(2.7f, 1.3f), 6f);

            GDCSceneKit.BoardOnCamera("GuideBoard", BoardText, 3.4f);

            GDCSceneKit.PlatformTop("GroundA", -8f, 16f, 0f);
            GDCSceneKit.PlatformTop("Step1", 6f, 10f, 1.4f);
            GDCSceneKit.PlatformTop("Step2", 12f, 16f, 2.8f);
            GDCSceneKit.PlatformTop("GroundB", 20f, 42f, 0f);

            int groundMask = 1 << LayerMask.NameToLayer("Ground");
            int playerMask = 1 << LayerMask.NameToLayer("Player");

            var player = GDCSceneKit.Player(new Vector2(-6f, 1.2f), true, -6f);
            if (answer)
            {
                var m = player.AddComponent<Ch4_PlayerMove_Answer>();
                m.groundLayers = groundMask;
            }
            else
            {
                player.AddComponent<Ch4_PlayerMove>();   // Ground Layers 留空，讓學生自己勾
            }

            BuildLaserGate(answer, playerMask);

            GDCSceneKit.Goal(new Vector2(39f, 0.9f), player.GetComponent<Rigidbody2D>(),
                             "過關！Raycast 是遊戲程式最常用的工具之一", new Vector2(1.6f, 1.8f));

            GDCSceneKit.FollowPlayer(player, 2.7f, 31.3f);
            GDCSceneKit.Managers("第 4 章 · 射線 Raycast");
            GDCSceneKit.Save(scene, GDCSceneKit.ScenePath("Ch4_Raycast", answer));
        }

        /// <summary>雷射感應門：走到雷射下面，門就升起來。</summary>
        static void BuildLaserGate(bool answer, int playerMask)
        {
            // 擋路的門
            var door = GDCSceneKit.Platform("LaserDoor", new Vector2(31f, 1.7f), new Vector2(0.8f, 3.4f));
            door.GetComponent<SpriteRenderer>().color = GDCPalette.Grapple;

            // 雷射發射器（掛在門左邊的天花板上）
            var emitter = GDCSceneKit.Shape("LaserEmitter", "Square", GDCPalette.Hazard,
                                            new Vector2(26f, 5.2f), new Vector2(0.9f, 0.5f),
                                            GDCSceneKit.OrderObject);

            var line = emitter.AddComponent<LineRenderer>();
            line.material      = new Material(Shader.Find("Sprites/Default"));
            line.widthMultiplier = 0.09f;
            line.useWorldSpace = true;
            line.sortingOrder  = GDCSceneKit.OrderObject;
            line.numCapVertices = 4;

            if (answer)
            {
                var s = emitter.AddComponent<Ch4_LaserSensor_Answer>();
                s.detectLayers = playerMask;
                s.door         = door.transform;
                s.maxDistance  = 5.5f;
            }
            else
            {
                var s = emitter.AddComponent<Ch4_LaserSensor>();
                s.door        = door.transform;   // 門先接好，Detect Layers 留給學生
                s.maxDistance = 5.5f;
            }

            GDCSceneKit.Text("LaserLabel", "走到雷射下面，門會升起",
                             new Vector2(26f, 6.2f), new Vector2(8f, 1f),
                             3.4f, GDCPalette.TextDim, GDCSceneKit.OrderObject,
                             TMPro.TextAlignmentOptions.Center);
        }
    }
}
