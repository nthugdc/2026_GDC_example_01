using UnityEngine;

namespace GDCBuild
{
    /// <summary>第 5 章 · 鉤爪：滑鼠瞄準 + Raycast + DistanceJoint2D。</summary>
    public static class Ch5Builder
    {
        const string BoardText =
@"第 5 章 · 鉤爪　　目標：盪過深淵到對岸　　操作：滑鼠左鍵射出鉤爪、按住 W 收繩
填完 Scripts/Student/Ch5_Grapple.cs 的 3 個 TODO，並把 Grapple Layers 勾成 Grappleable
三個常見的坑：1) Auto Configure Distance 要取消勾選　2) ScreenToWorldPoint 一定要給 z
3) Grapple Layers 沒勾好就什麼都鉤不到（詳細除錯說明見 Docs/Ch5.md）";

        static readonly Vector2[] Anchors =
        {
            new Vector2( 9f, 6.2f),
            new Vector2(14f, 6.8f),
            new Vector2(19f, 6.2f),
        };

        public static void Build(bool answer = false)
        {
            var scene = GDCSceneKit.NewScene(new Vector2(2.7f, 1.3f), 6f);

            GDCSceneKit.BoardOnCamera("GuideBoard", BoardText, 3.3f);

            GDCSceneKit.PlatformTop("GroundA", -8f,  6f, 0f);
            GDCSceneKit.PlatformTop("GroundB", 22f, 42f, 0f);

            foreach (var a in Anchors) AddAnchor(a);

            int groundMask  = 1 << LayerMask.NameToLayer("Ground");
            int grappleMask = 1 << LayerMask.NameToLayer("Grappleable");

            var player = GDCSceneKit.Player(new Vector2(-4f, 1.2f), true, -8f);
            player.AddComponent<PlayerMoveBasic>().groundLayers = groundMask;

            var line = player.AddComponent<LineRenderer>();
            line.material        = new Material(Shader.Find("Sprites/Default"));
            line.widthMultiplier = 0.08f;
            line.useWorldSpace   = true;
            line.sortingOrder    = GDCSceneKit.OrderActor;
            line.startColor = line.endColor = GDCPalette.Grapple;

            var joint = player.AddComponent<DistanceJoint2D>();
            joint.enabled = false;
            joint.autoConfigureConnectedAnchor = false;
            joint.autoConfigureDistance = false;
            joint.maxDistanceOnly = true;   // 只限制最遠距離，盪起來比較像繩子

            if (answer) player.AddComponent<Ch5_Grapple_Answer>().grappleLayers = grappleMask;
            else        player.AddComponent<Ch5_Grapple>();   // Grapple Layers 留給學生

            GDCSceneKit.Goal(new Vector2(39f, 0.9f), player.GetComponent<Rigidbody2D>(),
                             "過關！你做出鉤爪了", new Vector2(1.6f, 1.8f));

            GDCSceneKit.FollowPlayer(player, 2.7f, 31.3f);
            GDCSceneKit.Managers("第 5 章 · 鉤爪");
            GDCSceneKit.Save(scene, GDCSceneKit.ScenePath("Ch5_Grapple", answer));
        }

        /// <summary>天花板上的紫色鉤點。</summary>
        static void AddAnchor(Vector2 pos)
        {
            var go = GDCSceneKit.Shape("GrapplePoint", "Ring", GDCPalette.Grapple, pos,
                                       new Vector2(0.9f, 0.9f), GDCSceneKit.OrderObject);
            go.layer = LayerMask.NameToLayer("Grappleable");
            go.tag   = "GrapplePoint";
            go.AddComponent<CircleCollider2D>();
        }
    }
}
