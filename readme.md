# FSharp.ReactiveWpf

一个用于 F# 与 WPF 的轻量级响应式绑定工具库。它以 `System.Reactive` 的 `IObservable` / `ISubject` 为纽带，把 WPF 控件的事件与属性转换为可组合、可测试的数据流，用函数式风格搭建界面。

---

## 特性

- **双向绑定**：文本、数值、下拉框、开关、单选等控件与 `ISubject` 双向同步。值驱动界面、界面驱动值各走一条独立数据流，无需手工维护中间状态。
- **Rx 原生**：所有绑定建立在 `IObservable` / `ISubject` 之上，可自由组合 `Select` / `DistinctUntilChanged` / `Throttle` / `ObserveOn` 等操作符。
- **生命周期集中管理**：绑定统一向调用方传入的 `CompositeDisposable` 注册，窗口关闭或界面销毁时一次性释放，杜绝订阅泄漏。
- **类型安全**：数值框内置 `float` / `float32` / `int` / `int64` 解析，非法输入不会进入数据流。
- **文档排版支持**：提供 `FlowDocument` 表格（`Table` / `Row`）与 `Run` / `Paragraph` 的构建辅助，便于在代码中生成富文本。
- **实用工具**：内置文件打开/保存对话框（`TextFileOpener` / `TextFileSaver`）、输入弹窗（`TextBoxWindow`）、媒体播放列表（`MediaPlayer`）等。

---

## 引用

### 项目引用（推荐）

在另一个 `.fsproj` 中添加：

```xml
<ItemGroup>
  <ProjectReference Include="..\FSharp.ReactiveWpf\FSharp.ReactiveWpf.fsproj" />
</ItemGroup>
```

### NuGet

通过包 ID `FSharp.ReactiveWpf` 引用（当前版本 `0.1.10`）。包内嵌 `readme.md` 作为包说明。

---

## 快速开始

以下示例创建数据源并绑定到各类控件：

```fsharp
open System.Reactive.Subjects
open System.Reactive.Disposables
open System.Windows
open System.Windows.Controls
open FSharp.ReactiveWpf

let disposable = new CompositeDisposable()

// 数值输入框（浮点），双向绑定
let numberSubject = new BehaviorSubject<float>(0.0)
let numberTextBox = NumberBox.createFloat disposable numberSubject

// 文本输入框，双向绑定
let textSubject = new BehaviorSubject<string>("hello")
let textBox = TextBox.create disposable textSubject

// 复选框
let boolSubject = new BehaviorSubject<bool>(false)
let check = CheckBox.create disposable boolSubject "启用"

// 单选按钮组（按索引绑定）
let radioIndex = new BehaviorSubject<int>(-1)
let r1, r2, r3 = RadioButton(), RadioButton(), RadioButton()
r1.Content <- "A"; r2.Content <- "B"; r3.Content <- "C"
RadioButton.bindingRadioButtonGroup disposable [| r1; r2; r3 |] radioIndex

// 下拉框（按项目值绑定）
let comboItem = new BehaviorSubject<string>("A")
let combo = ComboBox.itemCreate disposable [ "A"; "B"; "C" ] comboItem

// 文本块：订阅派生数据流
let data = numberSubject.Select(fun f -> f.ToString("0.##"))
let textBlock = TextBlock.create disposable data

// Run 文本元素（FlowDocument 行内）
let run = Run.create disposable textSubject

// 播放列表（MediaPlayer）
let mediaPlayer = new MediaPlayer()
let playlistSubject = new BehaviorSubject<seq<string>>(Seq.empty)
let playback = MediaPlayer.createPlaylistObservable mediaPlayer (playlistSubject :> IObservable<_>)
let sub = playback.Subscribe() // 触发播放副作用

// 窗口关闭时统一释放
// window.Closing.Add(fun _ -> disposable.Dispose())
```

---

## 模块 API 参考

> 签名取自库的公开 API。除特别说明外，参数中 `CompositeDisposable`（下文简称 `disp`）用于注册所有订阅；`ISubject<'t>` 既是数据源也是数据汇（双向绑定），`IObservable<'t>` 仅作数据源（单向）。

### 文本 / 数值绑定

#### NumberBox —— 数值输入框

| 函数 | 签名 | 说明 |
| --- | --- | --- |
| `createFloat` | `disp -> ISubject<float> -> TextBox` | 创建浮点输入框（右对齐，失焦时解析写入） |
| `createSingle` | `disp -> ISubject<float32> -> TextBox` | 创建 `float32` 输入框 |
| `createInt` | `disp -> ISubject<int> -> TextBox` | 创建整型输入框 |
| `createInt64` | `disp -> ISubject<int64> -> TextBox` | 创建 `int64` 输入框 |
| `createBase` | `disp -> (string -> 'n option) -> ISubject<'n> -> TextBox` | 使用自定义解析函数创建输入框 |
| `bindFocus` | `disp -> TextBox -> ISubject<'t> -> unit` | 值→文本框；仅未聚焦时写入（50ms 节流） |
| `bindLostFocus` | `disp -> TextBox -> (string -> 'T option) -> ISubject<'T> -> unit` | 文本框→值；失焦时解析写入，解析失败忽略 |

#### TextBox —— 文本框

| 函数 | 签名 | 说明 |
| --- | --- | --- |
| `bindFocus` | `disp -> TextBox -> IObservable<string> -> unit` | 值→文本框；仅未聚焦时写入（50ms 节流） |
| `bindLostFocus` | `disp -> TextBox -> ISubject<string> -> unit` | 文本框→值；失焦时写入 |
| `create` | `disp -> ISubject<string> -> TextBox` | 创建并双向绑定 |
| `readonly` | `bool -> TextBox -> TextBox` | 设置只读并返回原控件 |

样式：`defaultStyle` / `successStyle` / `dangerStyle`（默认 / 成功绿边框 / 失败红边框），以及按布尔选样式的 `successDangerStyle : bool -> Style`、`normalDangerStyle : bool -> Style`。

#### ComboBox —— 下拉框

| 函数 | 签名 | 说明 |
| --- | --- | --- |
| `bindIndex` | `disp -> ComboBox -> ISubject<int> -> unit` | 按选中索引双向绑定 |
| `bindItem` | `disp -> ComboBox -> ISubject<'t> -> unit` | 按选中项目值双向绑定 |
| `indexCreate` | `disp -> #seq<string> -> ISubject<int> -> ComboBox` | 创建带项目列表的下拉框并按索引绑定 |
| `itemCreate` | `disp -> #seq<string> -> ISubject<string> -> ComboBox` | 创建带项目列表的下拉框并按值绑定 |

### 开关 / 单选

#### ToggleButton / CheckBox

| 函数 | 签名 | 说明 |
| --- | --- | --- |
| `ToggleButton.bind` | `disp -> ToggleButton -> ISubject<bool> -> unit` | 开关双向绑定 |
| `ToggleButton.create` | `disp -> ISubject<bool> -> ToggleButton` | 创建开关并绑定 |
| `CheckBox.bind` | `disp -> CheckBox -> ISubject<bool> -> unit` | 复选框双向绑定 |
| `CheckBox.create` | `disp -> ISubject<bool> -> obj -> CheckBox` | 创建复选框并绑定（第三参数为显示内容） |

#### RadioButton

| 函数 | 签名 | 说明 |
| --- | --- | --- |
| `bindingRadioButton` | `disp -> RadioButton -> ISubject<bool> -> unit` | 单个单选按钮绑定 |
| `bindingRadioButtonGroup` | `disp -> RadioButton[] -> ISubject<int> -> unit` | 单选组按索引绑定 |
| `bindingRadioButtonGroupUsingContent` | `disp -> RadioButton[] -> ISubject<string> -> unit` | 单选组按 `Content` 字符串绑定 |

### 只读文本

#### TextBlock

| 函数 | 签名 | 说明 |
| --- | --- | --- |
| `bind` | `disp -> TextBlock -> IObservable<string> -> unit` | 文本流→TextBlock（错误信息也写入） |
| `create` | `disp -> IObservable<string> -> TextBlock` | 创建 TextBlock 并绑定 |
| `textAlignment` | `TextAlignment -> TextBlock -> TextBlock` | 设置对齐并返回原控件 |

#### Run（FlowDocument 行内元素）

| 函数 | 签名 | 说明 |
| --- | --- | --- |
| `bindText` | `disp -> Run -> IObservable<string> -> unit` | 文本流→Run |
| `create` | `disp -> IObservable<string> -> Run` | 创建 Run 并绑定 |
| `bindVisible` | `disp -> Run -> IObservable<bool> -> unit` | 按布尔值切换显隐样式 |
| `bindSuccess` | `disp -> Run -> IObservable<bool> -> unit` | 成功绿 / 失败红 |
| `setVisible` / `setSuccess` | `disp -> IObservable<bool> -> Run -> Run` | 管道版本，返回原 Run |

样式：`defaultStyle` / `TransparentStyle` / `DangerStyle` / `SuccessStyle` / `visibleRunStyle` / `normalDangerRunStyle` / `successDangerRunStyle`。

### 布局 / 样式

#### UIElement

| 函数 | 签名 | 说明 |
| --- | --- | --- |
| `bindVisible` | `disp -> #UIElement -> IObservable<bool> -> unit` | 按布尔值切换 `Visibility`（Visible / Hidden） |
| `setVisible` | `disp -> IObservable<bool> -> #UIElement -> #UIElement` | 管道版本 |

#### FrameworkElement

| 函数 | 签名 | 说明 |
| --- | --- | --- |
| `bindStyle` | `disp -> #FrameworkElement -> IObservable<Style> -> unit` | 按样式流切换控件 `Style` |
| `setStyle` | `disp -> IObservable<Style> -> #FrameworkElement -> #FrameworkElement` | 管道版本 |

#### Brushes —— 语义色板

| 值 | 颜色 |
| --- | --- |
| `Success` | 绿 |
| `Danger` | 红 |
| `Warning` | 橙 |
| `Info` | 青 |

### 文档排版（FlowDocument）

以下模块均采用「参数先行、对象在后」的管道风格，便于 `|>` 组合。

| 模块 | 函数 | 说明 |
| --- | --- | --- |
| `Paragraph` | `alignment` / `add` | 设置对齐 / 追加 `Inline`，返回 `Paragraph` |
| `Table` | `addColumn` / `addRowGroup` / `addRow` / `cellSpacing` / `borderThickness` / `borderBrush` | 列、行组、行、间距与边框，均返回 `Table` |
| `TableRow` | `addCell` / `addCells` / `create` | 追加单元格 / 批量追加 / 由 `seq<TableCell>` 创建 |
| `TableCell` | `alignment` / `addBlock` / `columnSpan` / `rowSpan` / `background` / `borderThickness` / `borderBrush` | 对齐、内容、跨列、跨行、背景、边框，均返回 `TableCell` |
| `TableRowGroup` | `addRow` / `addRows` / `background` / `fontSize` / `fontFamily` / `fontWeight` / `fontStyle` / `foreground` | 行与文字样式，均返回 `TableRowGroup` |
| `TableColumn` | `width` / `widthPixels` / `widthStar` / `widthAuto` / `background` | 列宽（`GridLength` / 像素 / 星号 / 自适应）与背景，均返回 `TableColumn` |

#### Row —— 表单行容器

`Row` 类型（命名空间 `FSharp.ReactiveWpf`）由内嵌 `Row.xaml` 模板定义，是一个含「名称 / 单位 / 数值 / 备注」四栏的 `DockPanel`：

| 成员 | 签名 | 说明 |
| --- | --- | --- |
| `Row.empty` | `unit -> Row` | 加载 `Row.xaml` 模板 |
| `Row.fill` | `?name -> ?measure -> ?value -> ?spec -> Row`（均为 `UIElement`） | 填充四栏内容 |
| 字段 | `Root` / `Name` / `Measure` / `Value` / `Spec`（均为 `Border`） | 通过 `.Child` 设置内容 |

示例：

```fsharp
let row =
    Row.fill(
        TextBlock(Text = "流速"),
        TextBlock(Text = "m/s"),
        value = textBlockForFloat section.velocity
    )
panel.Children.Add(row.Root) |> ignore
```

### 窗口 / 文件对话框

#### TextBoxWindow —— 输入弹窗

基于 `TextBoxWindow.xaml`（MahApps `MetroWindow`）的模态输入对话框，返回窗口与取值函数（`ShowDialog` 结果为 `true` 后再调用取值函数取得输入值）：

| 函数 | 签名 | 说明 |
| --- | --- | --- |
| `getText` | `string -> MetroWindow * (unit -> string)` | 文本输入 |
| `getInt` | `int -> MetroWindow * (unit -> int)` | 整型输入（右对齐） |
| `getInt64` | `int64 -> MetroWindow * (unit -> int64)` | `int64` 输入 |
| `getFloat` | `float -> MetroWindow * (unit -> float)` | 浮点输入 |

示例：

```fsharp
let window, getResult = TextBoxWindow.getFloat 0.0
if window.ShowDialog() = System.Nullable(true) then
    let value = getResult()
    f.OnNext(value)
```

#### TextFileOpener / TextFileSaver —— 文件对话框

| 类型 | 构造 | 成员 |
| --- | --- | --- |
| `TextFileOpener` | `(defaultExt, filter)` 或 `(defaultExt, filter, encoding)`；`static openJson()` | `Open() : string option`（取消返回 `None`） |
| `TextFileSaver` | 同上；`static jsonSaver()` | `Save(text: string)`（成功/失败弹提示框） |

默认 `openJson` / `jsonSaver` 使用 UTF-8（带 BOM）编码、`.json` 扩展名、JSON/文本过滤。

### 媒体播放

#### MediaPlayer —— 播放列表

| 函数 | 签名 | 说明 |
| --- | --- | --- |
| `playMany` | `MediaPlayer -> string[] -> IDisposable` | 依次播放文件列表（自动跳过不存在的文件） |
| `createPlaylistObservable` | `MediaPlayer -> IObservable<#seq<string>> -> IObservable<unit>` | 播放列表流→顺序播放，列表变化自动切换 |
| `createPlaylistObservable2` | `(string -> unit) -> MediaPlayer -> IObservable<#seq<string>> -> IObservable<unit>` | 同上，附带日志回调 |

### 富文本定位（GreenWhite）

面向「打字机 / 字幕」场景的「绿色已读 + 白色未读」定位工具：

| 函数 | 签名 | 说明 |
| --- | --- | --- |
| `initialArticle` | `FlowDocument -> string -> unit` | 将文档初始化为一段文本 |
| `updateParagraph` | `Paragraph -> string -> int -> unit` | 按字符索引生成「绿 + 白」两段 `Run` |
| `getPos` | `Paragraph -> int -> TextPointer` | 白绿交界处的文本指针 |
| `scrollToCurrent` | `RichTextBox -> int -> unit` | 滚动到指定字符索引（自动跟随） |
| `getRunOffsetInParagraph` | `Paragraph -> Run -> int` | 计算 `Run` 在段落中的字符偏移 |

### 其它工具

| 模块 | 函数 | 说明 |
| --- | --- | --- |
| `XamlLoader` | `loadXaml : Assembly -> string -> obj` | 从嵌入式资源加载 XAML 并解析 |
| `DependencyObjectUtils` | `getInfiniteHierarchy` / `getParentHierarchy` / `info` / `getClickedElement` / `verifyTag` | 可视化树遍历与命中测试辅助 |

`FSharp.ReactiveWpf.Internal` 为库内部的 XAML 加载辅助（`Row`、`TextBoxWindow` 等依赖），一般无需直接使用。

---

## 设计要点

- **UI 分层**：控件创建/绑定逻辑与业务数据分离——业务侧持有 `ISubject` / `IObservable`，UI 侧通过 `CompositeDisposable` 注册绑定，便于单元测试与界面重建。
- **数据流双向但单向传播**：每个绑定拆成「值→控件」与「控件→值」两条 Rx 流，用 `DistinctUntilChanged` 防止回环抖动，用 `Throttle` 降低高频更新开销。
- **生命周期集中管理**：所有订阅汇入调用方传入的 `CompositeDisposable`，由窗口/页面关闭事件统一 `Dispose`。
- **依赖明确**：纯 Rx 公共核心在 [FSharp.Idioms.Reactive](FSharp.Idioms.Reactive)（`netstandard2.0`，与 UI 无关）；本库只负责 WPF 绑定，并通过 `MahApps.Metro` 提供 Metro 风格控件。

---

## 测试与示例

- `FSharp.ReactiveWpf.Test`：运行 `dotnet test FSharp.ReactiveWpf.Test`。
- `Windows`：`TextBoxWindow`、`FrameworkElement.bindStyle`、`UIElement.bindVisible` 的演示窗口。
- `section`：截面计算的响应式表单示例（`Row`、`NumberBox`、`TextBlock`、`ComboBox` 的组合用法）。
- `PipeSections.cli` / `PipeSections.ui`：管道截面应用的命令行与 UI 版本。

---

## 相关项目

- [FSharp.Idioms.Reactive](FSharp.Idioms.Reactive)：System.Reactive 的惯用工具库（`ObservableArray`、`BehaviorSubject.tryNext` 系列），本库与 UI 无关的公共核心依赖。
- 仓库：<https://github.com/xp44mm/FSharp.ReactiveWpf>

## 许可证

[LGPL-3.0-or-later](https://www.gnu.org/licenses/lgpl-3.0.html)

---

## 参考

WPF 的属性值优先级：

1. 本地值（Local Value）- 最高优先级
2. 样式触发器（Style Triggers）
3. 模板触发器（Template Triggers）
4. 样式设置器（Style Setters）
5. 主题样式（Theme Style）
6. 继承值（Inherited Value）
7. 默认值（Default Value）- 最低优先级
