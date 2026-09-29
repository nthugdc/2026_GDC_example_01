# 第 5 章 · 鉤爪

**本教材最好玩的一章。** 用滑鼠瞄準天花板的紫色鉤點，盪過深淵到對岸。

場景：`Assets/_GDC/Scenes/Ch5_Grapple.unity`
要改的檔案：`Assets/_GDC/Scripts/Student/Ch5_Grapple.cs`

操作：**滑鼠左鍵**射出鉤爪（按住不放）、**W** 收繩、放開左鍵解開。

> 這一章的移動腳本已經幫你寫好了（`PlayerMoveBasic`），專心做鉤爪就好。

---

## 鉤爪拆成三步

1. **滑鼠在世界的哪裡？** → `ScreenToWorldPoint`
2. **那個方向上有東西可以鉤嗎？** → `Physics2D.Raycast`
3. **有的話，把繩子接上去** → `DistanceJoint2D`

## 先認識 DistanceJoint2D

它的工作是「把我和某個點之間的距離限制住」，就像一條繩子。
這一章你會動到的欄位只有這幾個：

| 欄位 | 意思 |
|---|---|
| `enabled` | 繩子存不存在。`false` = 沒鉤住，`true` = 正在吊著 |
| `connectedBody` | 繩子另一端綁在哪個 Rigidbody2D 上。**留 `null` 代表綁在世界上的固定點**，正是我們要的 |
| `connectedAnchor` | 那個固定點的座標。因為 `connectedBody` 是 `null`，這裡填的是**世界座標** |
| `distance` | 繩子的長度 |
| `autoConfigureDistance` | `true` 的話 Unity 每幀會自己改 `distance` ← **坑 1**，已經幫你關掉 |
| `maxDistanceOnly` | `true` = 只限制最遠距離（像繩子）；`false` = 距離鎖死（像鐵棍） |

### TODO 5-A：滑鼠螢幕座標 → 世界座標

`Mouse.current.position.ReadValue()` 給你的是**螢幕座標**，單位是像素，左下角是 (0, 0)。
這個數字不能直接拿來跟場景裡的物件比較，要先用
`Camera.main.ScreenToWorldPoint()` 換算成世界座標。

⚠️ 它的參數是 `Vector3`，第三個值 `z` 代表「離攝影機多遠」——
這就是**坑 2**，往下看。

（記得把上面那行 `Vector2 mouseWorld = transform.position;` 改掉，不然變數會重複宣告。）

### TODO 5-B：射線

跟第 4 章同一個 `Physics2D.Raycast`。起點是自己、距離用 `maxDistance`、
圖層用 `grappleLayers`。

方向要怎麼算？你有「自己的位置」和「滑鼠的世界座標」兩個點，
需要的是一個**長度為 1 的方向向量** —— 想想 `.normalized`。

沒打到任何東西的話就直接 `return`，不要鉤。

### TODO 5-C：接上繩子

看上面那張表，你需要設定三件事：**接點在哪**、**繩子多長**、以及**讓它開始作用**。

繩長要設成「現在的我」到「鉤中的那個點」之間的距離。
`hit.point` 是打中的座標，`Vector2.Distance(a, b)` 算兩點距離。

**最後在 Inspector 把 Grapple Layers 勾成 Grappleable。**

---

## 三個坑（先看過再開始寫，會省你很多時間）

### 坑 1：Auto Configure Distance 一定要關掉

`DistanceJoint2D` 有一個 **Auto Configure Distance**，預設是**勾選**的，
意思是「繩長讓 Unity 自己算」。

這樣你設的 `joint.distance` 會馬上被蓋掉，結果就是玩家**瞬間被吸到鉤點上**，
或是完全不動。

本教材已經在 `Awake()` 幫你關掉了。但如果你之後自己從零做鉤爪，
這是第一個會卡住的地方。

### 坑 2：ScreenToWorldPoint 一定要給 z

螢幕座標只有 x, y；世界座標有 x, y, z。

不給 z 的話 Unity 會當作 `z = 0`，換算出來的點**全部落在攝影機自己的平面上** ——
症狀是「滑鼠指哪裡都往同一個方向鉤」。

攝影機放在 `z = -10`，所以要給 `sp.z = 10`，也就是 `-Camera.main.transform.position.z`。

### 坑 3：Grapple Layers 沒勾

- **留空** → 什麼都鉤不到
- **Everything** → 會鉤到地板、鉤到自己，繩子亂接
- **只勾 Grappleable** ← 正確

---

## 怎麼盪才盪得遠

鉤爪不是「拉過去」，是**擺盪**。

1. 先跑起來，帶著水平速度
2. 在快到崖邊時鉤住**前上方**的鉤點
3. 盪到最低點附近再放開（這時速度最快）
4. 空中接下一個鉤點

按住 `W` 收繩可以把自己往上拉，過高處時很有用。

> 繩子最短只會收到 `Min Rope Length`（預設 2.5）。
> 收太短的話整個人會貼在鉤點上，擺盪的半徑趨近於零，自然就盪不動了。
> 想體驗這個現象可以把它調成 0.5 試試看。

場景裡有三個鉤點，正常玩法是連續鉤三次過去。

---

## 加分題

1. 按 `S` 放長繩子
2. 加冷卻時間：放開後 0.5 秒內不能再鉤
3. 鉤中的時候讓鉤點放大一下（提示：`transform.localScale`）
4. 用 `LineRenderer` 的 `startWidth` / `endWidth` 讓繩子有粗細變化
5. 自己再加兩個鉤點，設計一條更難的路線

---

## 常見問題

**Q：按左鍵完全沒反應？**
A：① Grapple Layers 有沒有勾 Grappleable ② 你離鉤點是不是超過 13 單位了。

**Q：一按就被瞬移到鉤點上？**
A：Auto Configure Distance 沒關掉（坑 1）。

**Q：滑鼠指哪都鉤同一個方向？**
A：`ScreenToWorldPoint` 沒給 z（坑 2）。

**Q：鉤到了但盪不起來，只是吊在那邊？**
A：先跑再鉤 —— 擺盪靠的是鉤之前累積的水平速度。
另外按 `A` / `D` 在空中可以幫擺盪加力，像盪鞦韆一樣配合節奏推。

**Q：用 W 收繩之後就變得很慢很鈍？**
A：把 `Min Rope Length` 調大一點（例如 3）。繩子越短，擺盪半徑越小，
在鉤點正下方幾乎就只剩上下晃動了。

**Q：繩子看不見？**
A：`LineRenderer` 的 Material 要用 `Sprites/Default`，本教材已經設好。
如果你自己加了新的 LineRenderer 記得也要設。

---

卡住了 → `Assets/_GDC/Teacher/Answers/Ch5_Grapple.txt`

下一章 → [Ch6.md](Ch6.md)
