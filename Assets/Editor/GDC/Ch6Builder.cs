using UnityEngine;

namespace GDCBuild
{
    /// <summary>第 6 章 · 自由創作：只給起點、終點與素材，中間的關卡由學生自己蓋。</summary>
    public static class Ch6Builder
    {
        const string BoardText =
@"第 6 章 · 自由創作　　用前面五章學到的東西，自己把這一關做出來！
起點在左邊、終點在右邊，中間的路要你自己蓋。下方是素材倉庫，複製（Ctrl+D）來用。
1. 會來回移動的平台（自己寫一支腳本）　　2. 可以推的箱子（只調 Inspector）
3. 彈簧墊（Trigger + AddForce，自己寫腳本）4. 巡邏敵人（用 Raycast 偵測前方的牆就轉向）
5. 自己設計一個機關　　　　　　　　　　　完整說明與提示見 Docs/Ch6.md";

        public static void Build(bool answer = false)
        {
            var scene = GDCSceneKit.NewScene(new Vector2(6f, 2f), 7.5f);

            GDCSceneKit.BoardOnCamera("GuideBoard", BoardText, 3.4f);

            GDCSceneKit.PlatformTop("StartPlatform", -8f, -2f, 0f);
            GDCSceneKit.PlatformTop("GoalPlatform", 26f, 34f, 0f);

            int groundMask = 1 << LayerMask.NameToLayer("Ground");

            var player = GDCSceneKit.Player(new Vector2(-6f, 1.2f), true, -10f);
            player.AddComponent<PlayerMoveBasic>().groundLayers = groundMask;

            GDCSceneKit.Goal(new Vector2(31f, 0.9f), player.GetComponent<Rigidbody2D>(),
                             "太強了！你自己做出了一整關", new Vector2(1.6f, 1.8f));

            BuildWarehouse();

            GDCSceneKit.FollowPlayer(player, 6f, 20f);
            GDCSceneKit.Managers("第 6 章 · 自由創作");
            GDCSceneKit.Save(scene, GDCSceneKit.ScenePath("Ch6_Create", answer));
        }

        const float ShelfY = -0.6f;   // 素材擺放高度（畫面下緣留給引導看板）

        /// <summary>素材倉庫：擺在畫面下方，學生 Ctrl+D 複製出來用。</summary>
        static void BuildWarehouse()
        {
            var root = new GameObject("Warehouse");
            root.transform.position = new Vector2(9f, ShelfY);

            GDCSceneKit.Text("WarehouseTitle", "素材倉庫：選一個按 Ctrl+D 複製，再拖到你要的位置",
                             new Vector2(9f, 0.9f), new Vector2(20f, 0.8f),
                             3.4f, GDCPalette.TextDim, GDCSceneKit.OrderObject,
                             TMPro.TextAlignmentOptions.Center)
                        .transform.SetParent(root.transform, true);

            //     物件名稱（英文）        看板顯示   圖形            顏色
            Sample(root, "Sample_Platform",     "平台",   "Square",        GDCPalette.Platform,  1f, new Vector2(3f, 0.6f),   true,  false);
            Sample(root, "Sample_Box",          "木箱",   "RoundedSquare", GDCPalette.Box,       5f, new Vector2(1f, 1f),     true,  true);
            Sample(root, "Sample_Coin",         "金幣",   "Star",          GDCPalette.Coin,      8f, new Vector2(0.6f, 0.6f), true,  false);
            Sample(root, "Sample_Spike",        "尖刺",   "Triangle",      GDCPalette.Hazard,   11f, new Vector2(0.8f, 0.8f), true,  false);
            Sample(root, "Sample_GrapplePoint", "鉤點",   "Ring",          GDCPalette.Grapple,  14f, new Vector2(0.9f, 0.9f), true,  false);
            Sample(root, "Sample_GoalZone",     "終點區", "Square",        GDCPalette.Goal,     17f, new Vector2(1.4f, 1.4f), true,  false);
        }

        static void Sample(GameObject root, string name, string label, string sprite, Color color,
                           float x, Vector2 size, bool collider, bool rigidbody)
        {
            var go = GDCSceneKit.Shape(name, sprite, color, new Vector2(x, ShelfY), size,
                                       GDCSceneKit.OrderObject);
            if (collider)  go.AddComponent<BoxCollider2D>();
            if (rigidbody)
            {
                // 倉庫裡先設成 Static，才不會沒有地板就掉下去。
                // 學生複製出去之後要自己改回 Dynamic（Docs/Ch6.md 有說明）。
                var srb = go.AddComponent<Rigidbody2D>();
                srb.bodyType       = RigidbodyType2D.Static;
                srb.freezeRotation = true;
            }
            go.transform.SetParent(root.transform, true);

            GDCSceneKit.Text(name + "_Label", label,
                             new Vector2(x, ShelfY - 1.0f), new Vector2(3f, 0.7f),
                             3f, GDCPalette.TextDim, GDCSceneKit.OrderObject,
                             TMPro.TextAlignmentOptions.Center)
                        .transform.SetParent(root.transform, true);
        }
    }
}
