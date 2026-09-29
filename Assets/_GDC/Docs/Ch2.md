# 第 2 章 · 標籤 Tag

目標：吃光所有金幣、避開紅色尖刺、走到終點。

場景：`Assets/_GDC/Scenes/Ch2_Tag.unity`
要改的檔案：`Assets/_GDC/Scripts/Student/Ch2_PlayerTouch.cs`

> 這一章會用到你在第 1 章寫的 `Ch1_PlayerMove.cs`。如果那支還沒寫完，先回去補。

---

## Tag 是什麼

Tag 就是**貼在物件上的一張名牌**，讓程式可以問：「我碰到的這個東西，是什麼？」

金幣、尖刺、終點長得不一樣，但對程式來說它們都只是「一個 Collider」。
有了 Tag，程式才知道該加分、該扣血，還是該過關。

---

## 任務一：把 Tag 指派給物件

場景裡的金幣、尖刺、終點目前都是 **Untagged**。

1. 在 Hierarchy 點選一枚**金幣**
2. Inspector **最上方**有個 `Tag` 下拉選單，選 **Coin**
3. 其他五枚金幣也一樣（可以按住 `Ctrl` 一次選多個，一起改）
4. 兩根**尖刺** → `Spike`
5. **終點** → `Goal`

---

## 任務二：把五個 TODO 填完

打開 `Ch2_PlayerTouch.cs`，**先看檔案最上方那一大段說明**。
裡面寫了三個 Trigger 事件的差別、`other` 到底是什麼、`Destroy` 的效果，
以及你可以呼叫 `GameManager` 與 `Respawner` 的哪些功能。

| TODO | 事件 | 要做的事 |
|---|---|---|
| 2-A | Enter | 吃到金幣 → 加分、讓金幣消失 |
| 2-B | Enter | 踩到尖刺 → 扣分、回到起點 |
| 2-C | Enter | 抵達終點 → 過關 |
| 2-D | **Stay** | 站在感應板上 → 累計停留秒數並顯示 |
| 2-E | **Exit** | 離開感應板 → 把它的顏色變回去 |

2-D 和 2-E 在檔案裡有一段「已經幫你寫好」的 Enter 範例可以照著看。

---

## 觀念：`other` 是什麼？為什麼能改到對方的東西？

`other` 的型別是 `Collider2D` —— 它是**碰到我的那個東西身上的碰撞器「元件」**，
不是那個物件本身。這個區別很重要。

在 Unity 裡，只要拿到任何一個元件，就能從它身上找到所屬的物件與其他元件：

| 寫法 | 拿到什麼 |
|---|---|
| `other.gameObject` | 那個東西本身（GameObject） |
| `other.gameObject.name` | 它在 Hierarchy 裡的名字 |
| `other.CompareTag("Coin")` | 它的 Tag 是不是 Coin |
| `other.GetComponent<SpriteRenderer>()` | 從它身上再拿出繪製元件 |

所以「碰到感應板，然後改感應板的顏色」是做得到的 ——
先從 `other` 找到它的 `SpriteRenderer`，再改那個元件的 `color`。
**這是 Unity 裡最常用的一種寫法，之後你會一直用到。**

---

## 觀念：Destroy 到底刪掉了什麼？

| 寫法 | 效果 |
|---|---|
| `Destroy(other.gameObject)` | 整個物件從場景移除，直接消失 ← 金幣要用這個 |
| `Destroy(other)` | **只**移除那個碰撞器元件，物件還在，只是不再會被碰到 |
| `Destroy(物件, 3f)` | 3 秒後才刪除 |

刪除不是立刻發生的，而是在這一幀結束後才真的執行。

---

## 觀念：Enter / Stay / Exit 差在哪

| 事件 | 呼叫時機 |
|---|---|
| `OnTriggerEnter2D` | 剛碰到的那一幀，呼叫**一次** |
| `OnTriggerStay2D` | 只要還重疊著，**每一幀**都呼叫 |
| `OnTriggerExit2D` | 離開的那一幀，呼叫**一次** |

寫完 2-D 之後播放，盯著 Inspector 上的 **Pad Seconds** ——
踩上感應板它就開始一直往上跳，這就是 Stay 每幀都在執行的證據。

> ⚠️ 因為 Stay 每幀都跑，裡面**不要**直接加分，不然站著不動就能一直得分。
> 要跟時間有關的東西，就像 2-D 那樣累加 `Time.deltaTime`。

---

## 觀念：為什麼要用 CompareTag？

你可能會想這樣寫：

```csharp
if (other.tag == "Coin")     // 可以動，但不建議
```

用 `CompareTag` 有兩個好處：

1. **比較快**（不會產生垃圾記憶體）
2. **拼錯字會直接報錯**告訴你「這個 Tag 不存在」

用 `==` 的話，你把 `"Coin"` 打成 `"Coln"`，程式不會報錯，只會安靜地永遠不成立 ——
你可能要找半小時才發現。

---

## 觀念：OnTriggerEnter2D 什麼時候會被呼叫？

要同時滿足三個條件：

1. 兩個物件都有 **Collider 2D**
2. 其中至少一個 Collider 勾了 **Is Trigger**
3. 其中至少一個物件有 **Rigidbody 2D**

金幣沒反應時，就照這三點檢查。

---

## 加分題（這題會用到「自己新增 Tag」）

1. Edit > Project Settings > Tags and Layers > Tags，按 `+` 新增一個 **Bonus**
2. 複製一枚金幣（`Ctrl+D`），把 Tag 改成 Bonus，顏色改成別的
3. 在腳本裡加一段「碰到 Bonus 加 5 分」

其他挑戰：

- 讓尖刺不只扣分，還把分數歸零
- 加一個「必須吃滿 6 分才能過關」的判斷（提示：`if (gm.score >= 6)`）

---

## 常見問題

**Q：`Tag: Coin is not defined`**
A：拼錯字了。Tag 有大小寫之分，`coin` 和 `Coin` 是不一樣的。

**Q：碰到金幣沒反應？**
A：依序檢查 ① 金幣的 Tag 有沒有指派 ② 金幣的 Collider 有沒有勾 Is Trigger
③ 玩家有沒有 Rigidbody 2D。

**Q：走過尖刺沒事？**
A：尖刺的 Tag 忘了指派，或 TODO 2-B 還沒寫。

**Q：分數不會變？**
A：右上角 HUD 顯示的是 `GameManager.score`。確認你呼叫的是 `gm.AddScore(...)`。

---

卡住了 → `Assets/_GDC/Teacher/Answers/Ch2_PlayerTouch.txt`

下一章 → [Ch3.md](Ch3.md)
