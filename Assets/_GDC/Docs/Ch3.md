# 第 3 章 · 圖層 Layer

這一章**大部分是設定，不太需要寫程式**，但觀念很重要。

場景：`Assets/_GDC/Scenes/Ch3_Layer.unity`
要改的檔案：`Assets/_GDC/Scripts/Student/Ch3_ProximityDoor.cs`

---

## Tag 和 Layer 差在哪？

| | 回答的問題 | 誰在用 |
|---|---|---|
| **Tag** | 「這是什麼？」 | 你的程式（`CompareTag`） |
| **Layer** | 「誰能碰到誰？誰看得見誰？」 | 物理引擎、Raycast、攝影機 |

一個物件可以同時有 Tag `Coin` 和 Layer `Pickup`，兩者互不干擾。

**一個物件只能屬於一個 Layer**，而且總共只有 32 個，所以要省著用。

---

## 練習 1：單向平台（黃色平台）

現在從下面跳會撞到頭。我們要讓它變成「可以從下面穿上去，但站上去不會掉下來」。

1. 選取 Hierarchy 裡的 **OneWayPlatform_Task1**（黃色那塊）
2. Inspector 最上方 **Layer** → 選 `OneWay`
3. **Box Collider 2D** → 勾選 **Used By Effector**
4. **Add Component** → 搜尋 **Platform Effector 2D**
   - 勾選 **Use One Way**
   - **Surface Arc** 設 `140`（決定「哪個角度範圍算是上表面」）
   - 取消 **Use Side Friction**（不然會黏在側邊）

現在跳上去拿那三枚金幣。

> Surface Arc 調成 360 會怎樣？調成 20 呢？試試看就懂它在幹嘛了。

## 練習 2：穿過紅牆（碰撞矩陣）

紅牆（**GhostWall_Task2**）在 `GhostWall` 圖層，擋住去路。

1. **Edit > Project Settings > Physics 2D**
2. 找到最下面的 **Layer Collision Matrix**（一個三角形的勾勾表）
3. 找到 `Player` 那一列和 `GhostWall` 那一行的交叉點，**取消勾選**

現在玩家可以直接穿過紅牆。

> ⚠️ 這是**整個專案共用**的設定，不是單一場景的。改了以後每個場景都會生效。
> 實務上常用來做「敵人的子彈不會打到敵人自己」「玩家不會被隊友卡住」。

## 練習 3：LayerMask 感應門（紫色門）

`Ch3_ProximityDoor.cs` 的 TODO 3-A 要用 `Physics2D.OverlapCircle`：

```
Collider2D OverlapCircle(Vector2 point, float radius, int layerMask)
```

| 參數 | 意思 |
|---|---|
| `point` | 圓心的**世界座標**。想用自己的位置就傳 `transform.position` |
| `radius` | 半徑（世界單位） |
| `layerMask` | 只在乎哪些圖層。不給的話所有圖層都算 |

意思是「在 `point` 畫一個半徑 `radius` 的圓，看看圓內有沒有碰撞器」。

⚠️ **回傳的是 `Collider2D`，不是 `bool`：**

- 圈到東西 → 回傳其中**一個**碰到的碰撞器
- 沒圈到 → 回傳 `null`

所以要得到「有沒有」這個 true / false，你得自己跟 `null` 比較一下。
（想一次拿到全部，用 `OverlapCircleAll`，它回傳 `Collider2D[]`。）

寫完之後，**還要在 Inspector 把 Detect Layers 勾成 Player**，
不然那個 LayerMask 是空的，代表「什麼都不找」，玩家走再近也偵測不到。

選取門的時候，Scene 視窗會畫出青色圓圈顯示偵測範圍，可以邊調 Radius 邊看。

---

## 觀念：LayerMask 是什麼

LayerMask 就是「一份圖層的勾選清單」。

- **留空** = 什麼都不偵測 → 永遠找不到東西（最常見的錯誤）
- **Everything** = 全部都偵測 → 會偵測到地板、牆壁、自己
- **只勾需要的** ← 正確做法

---

## 加分題

1. 自己新增一個圖層（Layer 16 以後都是空的），做一個「只有木箱會被擋住、玩家可以穿過」的柵欄
2. 把感應門的 Radius 調到 15，觀察它會不會提早開
3. 讓門在打開時變色（提示：`GetComponent<SpriteRenderer>().color`）

---

## 常見問題

**Q：門完全不動？**
A：九成是 **Detect Layers 留空**了。

**Q：門一直開著不關？**
A：Detect Layers 勾成 Everything 了，它偵測到地板。只勾 Player。

**Q：勾了 Used By Effector 之後平台完全變成空氣？**
A：Platform Effector 2D 還沒加，或 Surface Arc 設成 0 了。

**Q：改了碰撞矩陣，結果第 2 章的尖刺也壞了？**
A：你可能改到 `Hazard` 那一列了。這一章要改的是 `GhostWall`。

---

卡住了 → `Assets/_GDC/Teacher/Answers/Ch3_ProximityDoor.txt`

下一章 → [Ch4.md](Ch4.md)
