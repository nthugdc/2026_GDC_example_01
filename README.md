# 《物理實驗室大逃脫》

NTHU GDC（清華大學遊戲創作社）2D 物理練習專案 — 給**完全沒有程式基礎**的新社員。

七個章節，帶你認識 Unity 2D 的四個核心：**2D 物理**、**Raycast**、**Tag**、**Layer**。

## 取得專案

> ⚠️ **請先安裝 Git LFS，再 clone。**
> 本專案的中文字型與圖檔存放在 Git LFS。沒裝就 clone 的話，
> 拉下來的會是幾行文字的「指標檔」而不是真正的檔案，
> Unity 會載不到字型，**場景裡的中文看板會整片變成方框（□□□）**。

```bash
# 1. 安裝 Git LFS（每台電腦只需要做一次）
git lfs install

# 2. 下載專案
git clone https://github.com/nthugdc/2026_GDC_example_01.git
```

### 怎麼裝 Git LFS

| 系統 | 做法 |
|---|---|
| Windows | 安裝 [Git for Windows](https://git-scm.com/download/win)（已內含 LFS），然後跑 `git lfs install` |
| macOS | `brew install git-lfs` |
| Ubuntu / Debian | `sudo apt install git-lfs` |

### 已經 clone 了才發現忘記裝？

不用重新 clone，補救兩行就好：

```bash
git lfs install
git lfs pull
```

### 怎麼確認 LFS 有正常運作

```bash
git lfs ls-files
```

有列出 `GDC_CJK.ttf` 之類的檔案就對了。
或者直接看 `Assets/_GDC/Art/Fonts/GDC_CJK.ttf` 的檔案大小 —
應該是 **約 4 MB**；如果只有 1 KB 左右，代表那是指標檔，LFS 沒生效。

## 開啟專案

1. 用 **Unity 6000.3.10f1** 開啟本專案
2. 打開 `Assets/_GDC/Scenes/Ch0_Sandbox.unity`
3. 按 ▶

> 第一次開啟會花幾分鐘匯入資源，屬正常現象。

## 📖 完整講義

**→ [Assets/_GDC/Docs/README.md](Assets/_GDC/Docs/README.md) ←**

裡面有環境須知、七章的逐章講義、社課排程建議與除錯指引。

> ⚠️ 開始寫程式前請務必先看講義開頭的兩張對照表：
> 本專案只能用**新版 Input System**（不能用 `Input.GetKeyDown`），
> 而且 Unity 6 把 `rb.velocity` 改名成 `rb.linearVelocity` 了。

## 章節一覽

| 章 | 主題 | 要做什麼 |
|---|---|---|
| 0 | 物理沙盒 | 完全不用寫程式，調參數讓球進籃子 |
| 1 | 移動與跳躍 | 第一支腳本，3 個 TODO |
| 2 | 標籤 Tag | 用 `CompareTag` 分辨碰到什麼 |
| 3 | 圖層 Layer | 單向平台、碰撞矩陣、LayerMask |
| 4 | 射線 Raycast | 修好無限跳、做雷射感應門 |
| 5 | 鉤爪 | 滑鼠瞄準 + Raycast + DistanceJoint2D |
| 6 | 自由創作 | 自己建物件、自己寫腳本 |

## 資料夾

```
Assets/_GDC/
├── Scenes/        七章場景（要玩的）
├── Scripts/
│   ├── Student/   有 TODO 的腳本（要改的）
│   └── Core/      基礎建設（不用改）
├── Art/           程式產生的圖 + 中文字型
├── Physics/       五種物理材質
├── Teacher/       完成版場景 + 參考解答
└── Docs/          講義

Assets/Editor/GDC/ 教材建置工具（學生可忽略）
```

## 給幹部

選單 **GDC**：

- **建置 > 全部重建** — Layer/Tag、美術素材、全部章節（含教師版）一次重做
- **重建章節 > 第 N 章** — 學生把某一章改壞了，一鍵還原

## 授權

`Assets/_GDC/Art/Fonts/GDC_CJK.ttf` 為 Droid Sans Fallback 的副本，
授權 Apache License 2.0，用於顯示繁體中文。詳見同資料夾的 `LICENSE-GDC_CJK.txt`。
