using UnityEngine;

namespace GDCBuild
{
    /// <summary>第 1 章 · 移動與跳躍：學生的第一支腳本，填 3 個 TODO。</summary>
    public static class Ch1Builder
    {
        const string BoardText =
@"第 1 章 · 移動與跳躍　　　目標：走到最右邊的綠色終點
1. 填完 Scripts/Student/Ch1_PlayerMove.cs 裡的 3 個 TODO
2. 玩家一直翻倒？勾選 Rigidbody 2D 的 Freeze Rotation Z
3. 調 Move Speed 與 Jump Force，找出最好操作的手感
操作：A / D 移動　空白鍵 跳躍　（詳細說明見 Docs/Ch1.md）";

        public static void Build(bool answer = false)
        {
            var scene = GDCSceneKit.NewScene(new Vector2(2.7f, 1.3f), 6f);

            GDCSceneKit.BoardOnCamera("GuideBoard", BoardText, 3.4f);

            // 地形：四段平台，中間有缺口要跳過去
            GDCSceneKit.PlatformTop("GroundA", -8f,   3f,   0f);
            GDCSceneKit.PlatformTop("PlatformB",  5.2f, 10f,  1.2f);
            GDCSceneKit.PlatformTop("PlatformC", 12.2f, 17f,  2.4f);
            GDCSceneKit.PlatformTop("PlatformD", 19.2f, 26f,  1.2f);

            var player = GDCSceneKit.Player(new Vector2(-6f, 1.2f), answer, -6f);
            if (answer) player.AddComponent<Ch1_PlayerMove_Answer>();
            else        player.AddComponent<Ch1_PlayerMove>();

            GDCSceneKit.Goal(new Vector2(24f, 2.1f), player.GetComponent<Rigidbody2D>(),
                             "過關！你完成了第一支腳本", new Vector2(1.6f, 1.8f));

            GDCSceneKit.FollowPlayer(player, 2.7f, 15.3f);
            GDCSceneKit.Managers("第 1 章 · 移動與跳躍");
            GDCSceneKit.Save(scene, GDCSceneKit.ScenePath("Ch1_Move", answer));
        }
    }
}
