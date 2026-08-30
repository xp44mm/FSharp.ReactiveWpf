# FSharp.Idioms.Reactive

System.Reactive 的 F# 惯用（Idioms）工具库，独立于 WPF、可跨平台复用（目标框架 netstandard2.0）。本库是 FSharp.ReactiveWpf 响应式绑定工具集中与 UI 无关的公共核心，提供可观察数组、Subject 与 BehaviorSubject 的常用便捷操作。

---

## 特性

- **可观察数组（ObservableArray）**：像 `Panel.Children` 一样增删元素，变更以 `IObservable` 流对外广播；元素与 `IDisposable` 句柄平行存放，删除即自动释放，整体 `Dispose` 即聚合释放器。
- **去重推送（BehaviorSubject.tryNext 系列）**：仅在“值确实变化”时才调用 `OnNext`，避免无意义的重复通知（支持自定义相等、阈值、四舍五入三种策略）。
- **纯 System.Reactive、零 WPF 依赖**：可在控制台、服务端、测试及任意 .NET 平台复用。

---

## 引用

### 项目引用（推荐）

在另一个 `.fsproj` 中添加：

```xml
<ItemGroup>
  <ProjectReference Include="..\FSharp.Idioms.Reactive\FSharp.Idioms.Reactive.fsproj" />
</ItemGroup>
```

### NuGet

发布后通过包 ID `FSharp.Idioms.Reactive` 引用（当前版本 `0.0.1`，`PackageTags: fsharp;reactive;rx`）。

---

## 模块 API

### ObservableArray —— 可观察数组

命名空间 `FSharp.Idioms.Reactive` 下提供两个类型。

#### 变更通知 `UIElementCollectionChange<'T>`

镜像 `System.Windows.Controls.UIElementCollection`（`Panel.Children`）的增删操作，命名与参数输入严格对齐：

| 用例 | 说明 |
| --- | --- |
| `Add item` | 尾部追加（对应 `Children.Add`） |
| `AddRange items` | 批量追加（对应 `Children.AddRange`） |
| `Insert(index, item)` | 指定索引插入（对应 `Children.Insert`） |
| `RemoveAt index` | 删除指定索引（对应 `Children.RemoveAt`） |
| `RemoveRange(index, count)` | 删除指定区间（对应 `Children.RemoveRange`） |
| `Clear` | 清空（对应 `Children.Clear`） |

#### `ObservableArray<'T>`

| 成员 | 签名 | 说明 |
| --- | --- | --- |
| `Count` | `int` | 当前元素个数 |
| `Add` | `item: 'T * disp: IDisposable -> unit` | 尾部追加元素及其释放句柄，并广播 `Add` |
| `AddRange` | `pairs: seq<'T * IDisposable> -> unit` | 批量追加，并广播 `AddRange` |
| `Insert` | `index: int * item: 'T * disp: IDisposable -> unit` | 指定索引插入，并广播 `Insert` |
| `RemoveAt` | `index: int -> unit` | 删除元素，通知后自动释放对应句柄 |
| `RemoveRange` | `index: int * count: int -> unit` | 删除区间，批量通知后逐一释放句柄 |
| `Clear` | `unit -> unit` | 清空全部，通知后释放全部句柄 |
| `IndexOf` | `item: 'T -> int` | 返回元素索引，找不到返回 `-1` |
| `Item` | `index: int -> 'T`（索引器） | 按索引读取元素 |
| `Changes` | `IObservable<UIElementCollectionChange<'T>>` | 变更通知流 |
| `ToArray` | `unit -> 'T[]` | 当前数组快照 |
| `Dispose` | `IDisposable` | 释放全部存活句柄并完成变更流（聚合释放器） |

示例：

```fsharp
open System
open System.Reactive.Disposables
open FSharp.Idioms.Reactive

let array = new ObservableArray<string>()

// 订阅变更通知
use sub =
    array.Changes.Subscribe(fun change ->
        printfn "%A" change)

array.Add("a", Disposable.Empty)              // 广播 Add "a"
array.AddRange [ "b", Disposable.Empty        // 广播 AddRange ["b"; "c"]
                 "c", Disposable.Empty ]
array.Insert(0, "x", Disposable.Empty)        // 广播 Insert(0, "x")
array.RemoveAt(1)                             // 广播 RemoveAt 1，并释放该位置句柄
array.Clear()                                 // 广播 Clear，并释放全部句柄
```

> 提示：`ObservableArray` 的 `Item` 只有 getter，修改请通过 `RemoveAt` + `Insert`，与 `UIElementCollection` 语义一致。

### Subject —— 便捷推送

模块 `FSharp.Idioms.Reactive.Subject`：

| 函数 | 签名 | 说明 |
| --- | --- | --- |
| `OnNext` | `value: 't -> subject: ISubject<'t> -> unit` | 向 `ISubject` 推送一个值（即 `subject.OnNext value`） |

### BehaviorSubject —— 去重推送

模块 `FSharp.Idioms.Reactive.BehaviorSubject`。`BehaviorSubject` 会缓存最近值，以下辅助函数都遵循同一约定：**仅在“新值相对当前值确实不同”时调用 `OnNext`**，相同则不推送，用于避免触发无意义的级联更新。

| 函数 | 签名 | 判定条件 |
| --- | --- | --- |
| `tryNextWith` | `equals: ('t -> 't -> bool) -> newValue: 't -> bs: BehaviorSubject<'t> -> unit` | 使用自定义相等函数，`not (equals bs.Value newValue)` 时推送 |
| `tryNext` | `newValue: 't -> bs: BehaviorSubject<'t> -> unit` | 使用结构相等 `(=)`，值不同时推送 |
| `tryNextDelta` | `delta: float -> newValue: float -> bs: BehaviorSubject<float> -> unit` | 实际差值 `> delta` 时推送 |
| `tryNextRound` | `decimals: int -> newValue: float -> bs: BehaviorSubject<float> -> unit` | 按 `Math.Round(x, decimals, MidpointRounding.AwayFromZero)` 舍入后不同时推送 |

示例：

```fsharp
open System.Reactive.Subjects
open FSharp.Idioms.Reactive.BehaviorSubject

let bs = new BehaviorSubject<float>(0.0)

tryNext 1.0 bs    // 0.0 -> 1.0，推送
tryNext 1.0 bs    // 值未变，不推送

tryNextDelta 0.01 1.005 bs   // 差值 0.005 <= 0.01，不推送
tryNextDelta 0.01 1.011 bs   // 差值 0.011 >  0.01，推送

tryNextRound 2 1.2346 bs     // 舍入后 1.23 = 1.23，不推送
tryNextRound 2 1.2350 bs     // 舍入后 1.23 <> 1.24，推送
```

---

## 设计要点

- **UI 无关的分层**：本库只依赖 `System.Reactive`（v7.0.0），所有“控件绑定”逻辑都在上层库 `FSharp.ReactiveWpf` 中；需要 WPF 绑定时组合两者，纯逻辑场景只引用本库。
- **生命周期集中管理**：`ObservableArray` 把元素与 `IDisposable` 句柄平行存放，删除即释放、整体 `Dispose` 即聚合释放器，避免订阅/句柄泄漏。
- **变更通知与 UIElementCollection 对齐**：变更类型命名与参数输入镜像 `Panel.Children` 的增删操作，便于把响应式数组直接驱动 UI 集合。
- **减少无效通知**：`BehaviorSubject.tryNext` 系列把“是否值得推送”的判断收敛到一处，默认结构相等、可自定义阈值与精度。

---

## 测试

`FSharp.ReactiveWpf.Test` 项目中的 `BehaviorSubjectTest.fs` 覆盖：

- `tryNext`：值相等不更新、值不等更新；
- `tryNextDelta`：差值大于阈值才更新（`<= delta` 不更新）；
- `tryNextRound`：四舍五入后不同才更新；
- `tryNextRound` 负数 `decimals` 抛出 `ArgumentOutOfRangeException`。

运行：`dotnet test FSharp.ReactiveWpf.Test`

---

## 相关项目

- [FSharp.ReactiveWpf](https://github.com/xp44mm/FSharp.ReactiveWpf)：WPF 响应式绑定工具库，本库是其与 UI 无关的公共核心依赖。

## 许可证

[LGPL-3.0-or-later](https://www.gnu.org/licenses/lgpl-3.0.html)