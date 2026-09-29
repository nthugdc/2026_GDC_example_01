# 第 4 章 · 射線 Raycast

**Raycast 是遊戲程式最常用的工具之一。** 這一章你會用它修好第 1 章留下的 bug，
再做一個雷射感應門。

場景：`Assets/_GDC/Scenes/Ch4_Raycast.unity`
要改的檔案：`Ch4_PlayerMove.cs`、`Ch4_LaserSensor.cs`

---

## Raycast 是什麼

> 從某個點往某個方向，射出一條看不見的線，然後問它：「你打到什麼了？」

遊戲裡幾乎每個功能背後都有 Raycast：

- 角色**站在地上了嗎**（往腳下射）
- 槍**打中誰了**（往準心射）
- 敵人**看得到玩家嗎**（往玩家射，中間有牆就看不到）
- 滑鼠**點到哪個物件**（從攝影機往滑鼠方向射）

```csharp
RaycastHit2D hit = Physics2D.Raycast(起點, 方向, 最長距離, 只偵測哪些圖層);

hit.collider   // 打到的東西，沒打到就是 null ← 最常用
hit.point      // 打中的座標
hit.distance   // 打了多遠
hit.normal     // 打中表面的方向
```

---

## 任務一：修好無限跳（TODO 4-A）

問題在於「按空白鍵就跳」，沒有檢查腳下有沒有地板。

打開 `Ch4_PlayerMove.cs`，看 `IsGrounded()`：

```csharp
RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, checkDistance, groundLayers);
return hit.collider != null;
```

寫完把最後那行 `return true;` 刪掉。

**然後在 Inspector 把 Ground Layers 勾成 Ground。**

### 一定要做的參數實驗

進 Play Mode，切到 **Scene 分頁**，你會看到玩家腳下有一條紅線（那是 `Debug.DrawRay` 畫的）。

| Check Distance | 結果 |
|---|---|
| `0.02` | 太短，站在地上也偵測不到 → **完全跳不起來** |
| `0.15` | 剛好 |
| `3.0` | 太長，人在半空中也算「站在地上」→ **又變回無限跳** |

親手把這三個值都試一次。這就是為什麼遊戲的數值要一個一個調出來。

## 任務二：雷射感應門（TODO 4-B）

打開 `Ch4_LaserSensor.cs`：

```csharp
hit = Physics2D.Raycast(transform.position, dir, maxDistance, detectLayers);
```

**然後在 Inspector 把 Detect Layers 勾成 Player。**

畫線的部分已經幫你寫好了：雷射沒打到東西是紅色，打到玩家會變綠色，
同時門會升起來。走到雷射下面試試。

門的開闔是三個參數在控制，可以自己調調看手感：

| 參數 | 預設 | 意思 |
|---|---|---|
| **Open Speed** | 8 | 門升起的速度，快一點才有即時回饋 |
| **Close Speed** | 1.6 | 門落下的速度，慢一點才來得及跑過去 |
| **Close Delay** | 1.2 | 離開雷射後先等幾秒才開始關門 |

> 試試把 Close Delay 改成 0、Close Speed 改成 8 —— 你會發現一離開雷射門就砸下來，
> 根本過不去。**「給玩家反應時間」就是關卡設計**，不是程式問題。

---

## 觀念：為什麼一定要給 LayerMask？

如果不給圖層過濾，往腳下射的那條線**第一個打到的會是玩家自己的 Collider**，
`IsGrounded()` 就永遠回傳 true。

玩家在 `Player` 圖層，地板在 `Ground` 圖層。
Ground Layers 只勾 `Ground`，射線就會自動忽略玩家自己。

---

## 加分題

1. 做**土狼時間**（Coyote Time）：離開地面後 0.1 秒內還是可以跳。
   幾乎所有平台遊戲都有這個，手感差很多。
2. 用 `hit.distance` 讓玩家快落地時自動變色（預告著地）
3. 把雷射改成**水平**發射（`direction` 改成 `(1, 0)`），做成一道橫向的紅外線
4. 用 `Physics2D.RaycastAll` 讓雷射可以同時偵測到好幾個東西

---

## 常見問題

**Q：完全跳不起來？**
A：① Ground Layers 留空了 ② Check Distance 太短 ③ Feet Offset 太大，射線起點跑到地板下面了。

**Q：看不到紅色射線？**
A：`Debug.DrawRay` 只畫在 **Scene 視窗**，Game 視窗看不到。進 Play Mode 後切到 Scene 分頁。

**Q：雷射永遠是紅色，門不開？**
A：Detect Layers 沒勾 Player。

**Q：`hit` 是 null 不能用？**
A：`RaycastHit2D` 是結構（struct）不會是 null，要判斷的是 `hit.collider != null`。

---

卡住了 → `Assets/_GDC/Teacher/Answers/Ch4_Raycast.txt`

下一章 → [Ch5.md](Ch5.md)
