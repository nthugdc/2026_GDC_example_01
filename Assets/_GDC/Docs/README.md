# 《物理實驗室大逃脫》— NTHU GDC 2D 物理練習專案

給**完全沒有程式基礎**的新社員，用七個章節認識 Unity 2D 的四個核心：
**2D 物理**、**Raycast 射線**、**Tag 標籤**、**Layer 圖層**。

每一章都是一個可以玩的小關卡。你會做三件事：

1. **調參數** — 在 Inspector 改數值，馬上看到結果
2. **填空** — 腳本裡留了 `TODO`，把提示照著寫進去
3. **自己做** — 最後一章由你自己建立物件與腳本

---

## 開始之前（很重要，只有兩件事）

### 1. 這個專案只能用「新版 Input System」

你在網路上找到的 Unity 教學，九成會寫這樣：

```csharp
if (Input.GetKeyDown(KeyCode.Space))   // ❌ 這個專案不能用，會編譯失敗
```

本專案要這樣寫：

```csharp
if (Keyboard.current.spaceKey.wasPressedThisFrame)   // ✅
```

對照表：

| 網路上的舊寫法 | 本專案的寫法 |
|---|---|
| `Input.GetKey(KeyCode.D)` | `Keyboard.current.dKey.isPressed` |
| `Input.GetKeyDown(KeyCode.Space)` | `Keyboard.current.spaceKey.wasPressedThisFrame` |
| `Input.GetKeyUp(KeyCode.Space)` | `Keyboard.current.spaceKey.wasReleasedThisFrame` |
| `Input.GetMouseButtonDown(0)` | `Mouse.current.leftButton.wasPressedThisFrame` |
| `Input.mousePosition` | `Mouse.current.position.ReadValue()` |

檔案最上面要加 `using UnityEngine.InputSystem;`。

- `isPressed` = 現在**按著**（一直是 true）
- `wasPressedThisFrame` = **剛剛按下去的那一瞬間**（只有一幀是 true）

### 2. Unity 6 把 Rigidbody2D 的欄位改名了

| 舊名（網路教學常見） | Unity 6 的新名 |
|---|---|
| `rb.velocity` | `rb.linearVelocity` |
| `rb.velocity.x` | `rb.linearVelocityX` |
| `rb.drag` | `rb.linearDamping` |
| `rb.angularDrag` | `rb.angularDamping` |
| `rb.isKinematic = true` | `rb.bodyType = RigidbodyType2D.Kinematic` |

Inspector 裡也一樣，找不到 Drag 是正常的，它現在叫 **Linear Damping**。

---

## 怎麼玩

1. 用 Unity 開啟這個專案（Unity **6000.3.10f1**）
2. 打開 `Assets/_GDC/Scenes/Ch0_Sandbox.unity`
3. 按 ▶ 開始

遊戲中的按鍵：

| 鍵 | 功能 |
|---|---|
| `A` / `D` 或 `←` / `→` | 移動 |
| `空白鍵` | 跳躍 |
| `R` | 重玩本章 |
| `N` | 下一章 |
| `P` | 上一章 |

---

## 七個章節

| 章 | 主題 | 你要做什麼 | 講義 |
|---|---|---|---|
| 0 | 物理沙盒 | **完全不用寫程式**，調參數讓球進籃子 | [Ch0.md](Ch0.md) |
| 1 | 移動與跳躍 | 你的第一支腳本，3 個 TODO | [Ch1.md](Ch1.md) |
| 2 | 標籤 Tag | 用 `CompareTag` 分辨碰到什麼 | [Ch2.md](Ch2.md) |
| 3 | 圖層 Layer | 單向平台、碰撞矩陣、LayerMask | [Ch3.md](Ch3.md) |
| 4 | 射線 Raycast | 修好無限跳、做雷射感應門 | [Ch4.md](Ch4.md) |
| 5 | 鉤爪 | 滑鼠瞄準 + Raycast + DistanceJoint2D | [Ch5.md](Ch5.md) |
| 6 | 自由創作 | 自己建物件、自己寫腳本 | [Ch6.md](Ch6.md) |

### 社課建議排程（每次約 2 小時）

| 次 | 內容 |
|---|---|
| 第 1 次 | Ch0 + Ch1 — 熟悉介面、物理參數、寫出第一支腳本 |
| 第 2 次 | Ch2 + Ch3 — Tag 與 Layer，大量 Inspector 操作 |
| 第 3 次 | Ch4 + Ch5 — Raycast 與鉤爪，本教材的重頭戲 |
| 第 4 次 | Ch6 — 自由創作 + 互相試玩 |

---

## 卡住了怎麼辦

1. **先看 Console**（Window > General > Console）。紅色訊息會告訴你哪個檔案第幾行出錯。
2. 看該章的講義 `Docs/ChN.md`，最後都有「常見問題」。
3. 還是卡住 → `Assets/_GDC/Teacher/Answers/` 裡有每一章的完整參考解答（純文字檔）。
4. 想直接看「做完長怎樣」→ `Assets/_GDC/Teacher/Scenes/` 有每一章的完成版，可以直接玩。

> 看解答不丟臉。看懂了以後把自己的檔案關掉重寫一次，那才是真的學會。

### 特例：畫面上的中文全變成方框（□□□）

這不是你寫錯，是 **clone 時沒有裝 Git LFS**，中文字型檔沒有真的下載下來。

在專案資料夾開終端機跑這兩行，然後回 Unity 等它重新匯入：

```bash
git lfs install
git lfs pull
```

確認方式：`Assets/_GDC/Art/Fonts/GDC_CJK.ttf` 應該是**約 4 MB**，
只有 1 KB 左右就代表還沒下載到。詳見專案根目錄的 [README](../../../README.md)。

---

## 專案結構

```
Assets/_GDC/
├── Scenes/      七章的場景（← 你要玩的）
├── Scripts/
│   ├── Student/ 有 TODO 的腳本（← 你要改的）
│   └── Core/    教材的基礎建設（不用改）
├── Art/         程式產生的圖與中文字型
├── Physics/     五種物理材質，可以拖到 Collider 上比較
├── Teacher/     完成版場景與參考解答
└── Docs/        就是你現在看的講義

Assets/Editor/   教材建置工具（學生完全可以忽略）
```

### 已經幫你建好的 Tag 與 Layer

**Tag**：`Player` `Coin` `Spike` `Goal` `Box` `GrapplePoint` `Bounce`

**Layer**：

| # | 名稱 | 用途 |
|---|---|---|
| 6 | Ground | 地面、平台 |
| 7 | Player | 玩家 |
| 8 | Pickup | 可撿的東西 |
| 9 | Hazard | 危險物 |
| 10 | Goal | 終點 |
| 11 | OneWay | 單向平台 |
| 12 | Grappleable | 可以鉤的東西 |
| 13 | Bullet | 子彈 |
| 14 | MovingPlatform | 移動平台 |
| 15 | GhostWall | 第 3 章的紅牆 |

---

## 給幹部：重建教材

如果學生把場景改壞了，選單 **GDC > 重建章節 > 第 N 章** 就能把那一章還原。
**GDC > 建置 > 全部重建** 會連 Layer/Tag、美術素材、教師版一起重做。

美術素材（`Art/Sprites/*.png`）全部是 `Assets/Editor/GDC/SpriteFactory.cs` 程式產生的純白圖，
顏色由 `SpriteRenderer.color` 決定，改配色只要動 `Scripts/Core/GDCPalette.cs`。

---

## 授權

`Art/Fonts/GDC_CJK.ttf` 是 Droid Sans Fallback（Apache License 2.0）的副本，
用來顯示繁體中文。詳見同資料夾的 `LICENSE-GDC_CJK.txt`。
