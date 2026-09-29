using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using TMPro;

namespace GDCBuild
{
    /// <summary>建構章節場景時共用的小工具。</summary>
    public static class GDCSceneKit
    {
        // 統一的繪製順序（數字越大越前面）
        public const int OrderBackdrop = -100;
        public const int OrderPlatform = -50;
        public const int OrderObject   = 0;
        public const int OrderActor    = 10;
        public const int OrderBoard    = 50;
        public const int OrderHud      = 100;

        /// <summary>開一個全新的空場景，並放好 2D 專案必備的攝影機與全域燈光。</summary>
        public static Scene NewScene(Vector2 camPos, float camSize = 6.5f)
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            var camGo = new GameObject("Main Camera");
            camGo.tag = "MainCamera";
            camGo.transform.position = new Vector3(camPos.x, camPos.y, -10f);
            var cam = camGo.AddComponent<Camera>();
            cam.orthographic      = true;
            cam.orthographicSize  = camSize;
            cam.clearFlags        = CameraClearFlags.SolidColor;
            cam.backgroundColor   = GDCPalette.Background;
            camGo.AddComponent<AudioListener>();

            // URP 2D：沒有全域燈光的話畫面會是全黑的
            var lightGo = new GameObject("Global Light 2D");
            var light = lightGo.AddComponent<Light2D>();
            light.lightType = Light2D.LightType.Global;
            light.intensity = 1f;

            return scene;
        }

        public static Camera MainCamera() => Object.FindFirstObjectByType<Camera>();

        // ── 基本物件 ────────────────────────────────────────────────────

        /// <summary>放一個純色的形狀。</summary>
        public static GameObject Shape(string name, string sprite, Color color,
                                       Vector2 pos, Vector2 size, int order = OrderObject, float angle = 0f)
        {
            var go = new GameObject(name);
            go.transform.position   = pos;
            go.transform.localScale = new Vector3(size.x, size.y, 1f);
            go.transform.rotation   = Quaternion.Euler(0, 0, angle);

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite       = GDCPaths.Sprite(sprite);
            sr.color        = color;
            sr.sortingOrder = order;
            return go;
        }

        /// <summary>地面 / 平台：方塊 + BoxCollider2D，圖層設為 Ground。</summary>
        public static GameObject Platform(string name, Vector2 center, Vector2 size,
                                          float angle = 0f, string physicsMat = null)
        {
            var go = Shape(name, "Square", GDCPalette.Platform, center, size, OrderPlatform, angle);
            go.layer = LayerMask.NameToLayer("Ground");
            var col = go.AddComponent<BoxCollider2D>();
            if (physicsMat != null) col.sharedMaterial = GDCPaths.Phys(physicsMat);
            return go;
        }

        /// <summary>用「上緣 Y」來擺平台，排版時比用中心點直覺。</summary>
        public static GameObject PlatformTop(string name, float left, float right, float topY,
                                             float thickness = 0.7f, string physicsMat = null)
        {
            var center = new Vector2((left + right) * 0.5f, topY - thickness * 0.5f);
            return Platform(name, center, new Vector2(right - left, thickness), 0f, physicsMat);
        }

        // ── 玩家與攝影機 ────────────────────────────────────────────────

        /// <summary>建立玩家：膠囊外型 + Rigidbody2D + CapsuleCollider2D + 重生。</summary>
        public static GameObject Player(Vector2 pos, bool freezeRotation = false, float killY = -8f)
        {
            var go = Shape("Player", "Capsule", GDCPalette.Player, pos, new Vector2(0.8f, 0.8f), OrderActor);
            go.layer = LayerMask.NameToLayer("Player");
            go.tag   = "Player";

            var rb = go.AddComponent<Rigidbody2D>();
            rb.freezeRotation         = freezeRotation;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            rb.linearDamping          = 0f;
            rb.gravityScale           = 2.5f;   // 重一點，跳起來比較俐落不會飄

            var col = go.AddComponent<CapsuleCollider2D>();
            col.size      = new Vector2(1f, 1.5f);
            col.direction = CapsuleDirection2D.Vertical;
            col.sharedMaterial = GDCPaths.Phys("Slippery");   // 免得貼牆卡住

            go.AddComponent<Respawner>().killY = killY;
            return go;
        }

        /// <summary>讓攝影機跟隨玩家，並限制水平範圍。</summary>
        public static void FollowPlayer(GameObject player, float minX, float maxX, bool followY = false)
        {
            var cam = MainCamera();
            if (cam == null) return;

            var f = cam.gameObject.AddComponent<CameraFollow>();
            f.target  = player.transform;
            f.followY = followY;
            f.offset  = new Vector2(1.5f, 1f);
            f.min     = new Vector2(minX, 0f);
            f.max     = new Vector2(maxX, 0f);
        }

        /// <summary>終點：碰到就過關。</summary>
        public static GameObject Goal(Vector2 center, Rigidbody2D player, string message, Vector2 size)
        {
            var go = Shape("Goal", "Square", new Color(0.02f, 0.84f, 0.63f, 0.35f), center, size, OrderObject);
            go.layer = LayerMask.NameToLayer("Goal");
            go.AddComponent<BoxCollider2D>().isTrigger = true;

            var g = go.AddComponent<TouchGoal>();
            g.targetBody   = player;
            g.clearMessage = message;

            Text("GoalLabel", "終點", new Vector2(center.x, center.y + size.y * 0.5f + 0.5f),
                 new Vector2(3f, 0.8f), 4f, GDCPalette.Goal, OrderObject, TextAlignmentOptions.Center);
            return go;
        }

        // ── 文字 ────────────────────────────────────────────────────────

        /// <summary>世界空間的文字（中文靠 TMP 的 fallback 字型顯示）。</summary>
        public static TextMeshPro Text(string name, string content, Vector2 pos, Vector2 boxSize,
                                       float fontSize, Color color, int order = OrderBoard,
                                       TextAlignmentOptions align = TextAlignmentOptions.TopLeft)
        {
            var go = new GameObject(name);
            go.transform.position = pos;

            var tmp = go.AddComponent<TextMeshPro>();
            tmp.text      = content;
            tmp.fontSize  = fontSize;
            tmp.color     = color;
            tmp.alignment = align;
            tmp.textWrappingMode = TextWrappingModes.Normal;
            tmp.rectTransform.sizeDelta = boxSize;

            var mr = go.GetComponent<MeshRenderer>();
            if (mr != null) mr.sortingOrder = order;
            return tmp;
        }

        /// <summary>帶底板的引導看板。底板與文字各自獨立，避免縮放互相影響。</summary>
        public static GuideBoard Board(string name, string content, Vector2 center, Vector2 size,
                                       float fontSize = 0.62f)
        {
            var root = new GameObject(name);
            root.transform.position = center;

            // 底板一律用純正方形：圓角圖被非等比放大時，四角會被拉成橢圓
            var bg = Shape(name + "_BG", "Square", GDCPalette.Board, center, size, OrderBoard - 1);
            bg.transform.SetParent(root.transform, true);

            const float pad = 0.5f;
            var tmp = Text(name + "_Text", content, center,
                           new Vector2(size.x - pad * 2f, size.y - pad * 2f),
                           fontSize, GDCPalette.Text, OrderBoard);
            tmp.transform.SetParent(root.transform, true);

            var board = root.AddComponent<GuideBoard>();
            board.content   = content;
            board.textColor = GDCPalette.Text;
            board.fontSize  = fontSize;
            board.Apply();
            return board;
        }

        /// <summary>
        /// 把引導看板釘在畫面上緣或下緣，跟著攝影機移動。
        /// 看板放在世界座標的話，攝影機一跟著玩家跑，看板就會飄到關卡中間擋住畫面。
        /// 高度依文字實際需要自動撐開，不用手動填。
        /// </summary>
        public static GuideBoard BoardOnCamera(string name, string content,
                                               float fontSize = 3.4f, bool bottom = true)
        {
            const float pad = 0.5f;

            var cam = MainCamera();
            float halfH = cam.orthographicSize;
            float halfW = halfH * 16f / 9f;
            float width = halfW * 2f * 0.94f;

            var board = Board(name, content, Vector2.zero, new Vector2(width, 2f), fontSize);

            // 量出文字真正需要多高，再把底板撐到那個高度
            var tmp = board.GetComponentInChildren<TMP_Text>(true);
            tmp.rectTransform.sizeDelta = new Vector2(width - pad * 2f, 1000f);
            tmp.ForceMeshUpdate();
            float height = tmp.preferredHeight + pad * 2f;
            tmp.rectTransform.sizeDelta = new Vector2(width - pad * 2f, height - pad * 2f);

            var bg = board.transform.Find(name + "_BG");
            if (bg != null) bg.localScale = new Vector3(width, height, 1f);

            float y = bottom ? -halfH + height * 0.5f + 0.2f
                             :  halfH - height * 0.5f - 0.2f;

            board.transform.SetParent(cam.transform, false);
            board.transform.localPosition = new Vector3(0f, y, 1f);
            return board;
        }

        // ── 章節基礎建設 ────────────────────────────────────────────────

        /// <summary>總管物件：GameManager + 章節切換 + 右上角 HUD + 過關橫幅。</summary>
        public static GameManager Managers(string chapterTitle)
        {
            var go = new GameObject("__GDC_System");   // 教材基礎建設，學生不用動
            var gm = go.AddComponent<GameManager>();
            gm.chapterTitle = chapterTitle;
            go.AddComponent<ChapterNavigator>();

            var cam = MainCamera();
            if (cam != null)
            {
                // HUD 掛在攝影機下、貼齊畫面右上角（左上角通常留給引導看板）
                float halfH = cam.orthographicSize;
                float halfW = halfH * 16f / 9f;
                var box = new Vector2(8.5f, 3.0f);

                var hud = Text("HUD", "", Vector2.zero, box, 4.4f, GDCPalette.Text, OrderHud,
                               TextAlignmentOptions.TopRight);
                hud.transform.SetParent(cam.transform, false);
                hud.transform.localPosition = new Vector3(halfW - 0.3f - box.x * 0.5f,
                                                          halfH - 0.3f - box.y * 0.5f, 1f);
                hud.gameObject.AddComponent<HudDisplay>().Refresh();   // 先填好，編輯模式看得到

                // 過關橫幅：平常空白，過關時才顯示
                var banner = Text("ClearBanner", "", Vector2.zero, new Vector2(halfW * 1.6f, 2f),
                                  9f, GDCPalette.Goal, OrderHud, TextAlignmentOptions.Center);
                banner.transform.SetParent(cam.transform, false);
                banner.transform.localPosition = new Vector3(0f, -halfH * 0.55f, 1f);
                banner.gameObject.AddComponent<ClearBanner>();
            }
            return gm;
        }

        // ── 存檔 ────────────────────────────────────────────────────────

        /// <summary>學生版存 Scenes/，教師解答版存 Teacher/Scenes/。</summary>
        public static string ScenePath(string name, bool answer)
            => answer ? $"{GDCPaths.TeacherScenes}/{name}_Answer.unity"
                      : $"{GDCPaths.Scenes}/{name}.unity";

        public static void Save(Scene scene, string path)
        {
            StripTmpSubMeshes();
            EditorSceneManager.SaveScene(scene, path);
            AddToBuildSettings(path);
            Debug.Log($"[GDC] 場景已建立：{path}");
        }

        /// <summary>
        /// 清掉 TMP 為了 fallback 字型自動產生的 sub-mesh 子物件。
        /// 這些子物件的材質是執行期產生的，存進場景後會變成失效參照，
        /// 導致中文在編輯模式顯示成洋紅色方塊。刪掉之後 TMP 會在開啟場景時重建。
        /// </summary>
        static void StripTmpSubMeshes()
        {
            foreach (var t in Object.FindObjectsByType<TMP_Text>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                for (int i = t.transform.childCount - 1; i >= 0; i--)
                {
                    var child = t.transform.GetChild(i);
                    if (child.GetComponent<TMP_SubMesh>() != null || child.GetComponent<TMP_SubMeshUI>() != null)
                        Object.DestroyImmediate(child.gameObject);
                }
            }
        }

        static void AddToBuildSettings(string path)
        {
            // 教師解答版不進 Build Settings，才不會打亂 N / P 的章節順序
            if (path.Contains("/Teacher/")) return;

            var list = new System.Collections.Generic.List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
            if (list.Exists(s => s.path == path)) return;
            list.Add(new EditorBuildSettingsScene(path, true));
            EditorBuildSettings.scenes = list.ToArray();
        }
    }
}
