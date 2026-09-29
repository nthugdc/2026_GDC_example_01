# 第 1 章 · 移動與跳躍

**你的第一支腳本。** 目標是讓玩家走到最右邊的綠色終點。

場景：`Assets/_GDC/Scenes/Ch1_Move.unity`
要改的檔案：`Assets/_GDC/Scripts/Student/Ch1_PlayerMove.cs`

---

## 先玩一次

按 ▶。按 `A` `D` 完全沒反應 —— 因為程式還沒寫。

---

## 任務一：把三個 TODO 填完

用滑鼠雙擊 `Ch1_PlayerMove.cs` 打開它。

**先把檔案最上方那一大段註解看完。** 裡面列出了你會用到的每一個東西：
它叫什麼、參數是什麼意思、回傳什麼。TODO 區塊只會告訴你「要達成什麼」，
不會把答案寫給你 —— 那才是你要寫的部分。

### TODO 1-A：讀鍵盤（在 `Update` 裡）

`moveInput` 代表「要往哪走」：`-1` 是左、`1` 是右、`0` 是不動。

你需要的是「某個鍵現在是不是被按著」。A 和 ← 都算往左，D 和 → 都算往右，
所以一個方向要判斷兩個鍵 —— 想想 `||`。

### TODO 1-B：設定速度（在 `FixedUpdate` 裡）

這題有兩個坑，都值得自己踩一次：

**坑一：不能只改一半。** 試著寫 `rb.linearVelocity.x = 5f`，
Unity 會直接不讓你編譯，錯誤訊息是 *Cannot modify the return value*。
原因是 `Vector2` 是**值型別（struct）**，`rb.linearVelocity` 給你的是一份複本，
改複本對真正的速度沒有任何影響。所以要換速度，就得**整個給一個新的 `Vector2`**，
這也是為什麼要寫 `new`。

**坑二：y 不能填 0。** 水平速度要換成新的，但**垂直速度必須保留原本的值**。
填 0 的話等於每秒 50 次把垂直速度歸零 —— 跳不起來，掉落時也會像貼著牆慢慢滑。
**寫完之後故意改成 `0f` 跑一次**，這個錯誤親眼看過一遍就永遠記得了。

⚠️ 欄位叫 `linearVelocity` 不是 `velocity`。Unity 6 改名了，詳見 [README](README.md)。

### TODO 1-C：跳躍（在 `FixedUpdate` 裡）

用 `AddForce` 往正上方施力。第二個參數決定施力方式：

| | 效果 | 適合 |
|---|---|---|
| `ForceMode2D.Impulse` | 瞬間推一下 | 跳躍 |
| `ForceMode2D.Force` | 持續施力 | 推進器、風 |

寫完可以把 Impulse 改成 Force 試試看，玩家會像坐火箭一樣慢慢升空。

存檔後切回 Unity，等左下角的轉圈圈跑完（那是在編譯），再按 ▶。

---

## 觀念：為什麼有 Update 又有 FixedUpdate？

這支腳本刻意分成兩半，這是 Unity 專案的標準結構：

| | 何時執行 | 負責什麼 |
|---|---|---|
| `Update` | 每畫一張畫面一次，**張數會浮動**（60、144、30…） | 讀輸入 |
| `FixedUpdate` | **固定每秒 50 次**，不受畫面張數影響 | 改物理 |

如果把物理寫在 `Update`，在 144Hz 的電腦上每秒會執行 144 次、30Hz 只有 30 次，
同一款遊戲在不同電腦上手感就會不一樣。物理交給 `FixedUpdate` 才穩定。

那為什麼輸入不能也放 `FixedUpdate`？因為 `wasPressedThisFrame`（按下的那一瞬間）
**只有那一幀是 true**，而那一幀不一定剛好輪到 `FixedUpdate` 執行，會漏掉。

所以腳本裡的 `jumpRequested` 是一個「傳話用」的旗標：
`Update` 發現玩家按了跳躍就把它設成 `true`，`FixedUpdate` 拿去用完再設回 `false`。

---

## 任務二：玩家一直翻倒？

因為膠囊有物理，撞到東西會轉。

修法：選取 **Player** → Rigidbody 2D → 展開 **Constraints** → 勾選 **Freeze Rotation** 的 **Z**。

（2D 遊戲只有 Z 軸會轉，所以只要凍結 Z。）

---

## 任務三：調出好手感

| 參數 | 太小 | 太大 |
|---|---|---|
| **Move Speed** | 走起來很拖 | 難以控制、容易衝過頭 |
| **Jump Force** | 跳不過缺口 | 飛得太高看不到路 |

沒有標準答案。試 5～15 之間，找出你覺得最順的組合。
**這就是遊戲設計裡的「手感調校」，職業開發者也是這樣一個一個試出來的。**

---

## 加分題

1. 加一個「衝刺」：按住 `Shift` 時速度變兩倍
2. 讓玩家面向移動方向（提示：`transform.localScale` 的 x 改成 -1 可以左右翻轉）
3. 跳躍時把顏色改掉（提示：`GetComponent<SpriteRenderer>().color`）

---

## 常見問題

**Q：`Input` does not contain a definition for `GetKeyDown`**
A：本專案只能用新版 Input System，要用 `Keyboard.current`。見 [README](README.md)。

**Q：`Rigidbody2D` does not contain a definition for `velocity`**
A：Unity 6 改名成 `linearVelocity` / `linearVelocityX` 了。

**Q：Cannot modify the return value of `Rigidbody2D.linearVelocity`**
A：你寫了 `rb.linearVelocity.x = ???`。`Vector2` 是值型別，不能只改一半，
要整個換成 `new Vector2(...)`。見上面 TODO 1-B 的說明。

**Q：跳起來一下就掉下去，跳不高？**
A：TODO 1-B 的 y 填成 0 了。垂直速度每秒被歸零 50 次，跳躍的力剛給就被清掉。

**Q：改完存檔了，但遊戲沒變？**
A：切回 Unity 等它編譯完（右下角有轉圈圈）。如果 Console 有紅字，先修好錯誤。

**Q：可以無限連跳耶？**
A：對，這是**故意留的 bug**。第 4 章會用 Raycast 修好它。

---

卡住了 → `Assets/_GDC/Teacher/Answers/Ch1_PlayerMove.txt`

下一章 → [Ch2.md](Ch2.md)
