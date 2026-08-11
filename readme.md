# FSharp.ReactiveWpf

一个用于 F# 和 WPF 的轻量级响应式绑定工具集。库通过 System.Reactive 提供各种控件与 IObservable / ISubject 的绑定辅助函数，便于在函数式风格下构建 WPF 界面。

快速说明：文档中的 API 以模块名.函数名 形式给出，示例使用 System.Reactive.Subjects 和 System.Reactive.Disposables 来管理数据流与生命周期。

---

## 快速开始

示例演示如何创建数据源并将其绑定到控件：

```fsharp
open System.Reactive.Subjects
open System.Reactive.Disposables
open System.Windows
open System.Windows.Controls
open FSharp.ReactiveWpf

let disposable = new CompositeDisposable()

// 数值输入框（浮点）
let numberSubject = new BehaviorSubject<float>(0.0)
let numberTextBox = NumberBox.createFloat disposable numberSubject

// 文本绑定到已有 TextBox
let textSubject = new BehaviorSubject<string>("hello")
let tb = TextBox()
TextBox.bindFocus disposable tb textValue
TextBox.bindLostFocus disposable tb textValue

// 复选框
let boolSubject = new BehaviorSubject<bool>(false)
let check = CheckBox.create disposable boolSubject "启用"

// 单选按钮组（按索引）
let radioIndex = new BehaviorSubject<int>(-1)
let r1, r2, r3 = RadioButton(), RadioButton(), RadioButton()
r1.Content <- "A"; r2.Content <- "B"; r3.Content <- "C"
RadioButton.bindingRadioButtonGroup disposable radioIndex [| r1; r2; r3 |]

// Run 文本元素
let run = Run.create disposable textSubject

// 播放列表 (MediaPlayer)
let mediaPlayer = new MediaPlayer()
let playlistSubject = new BehaviorSubject<seq<string>>(Seq.empty)
let playback = MediaPlayer.createPlaylistObservable mediaPlayer (playlistSubject :> IObservable<_>)
let sub = playback.Subscribe() // 触发播放副作用

// 将控件加入窗口等
```

---

## 主要 API 参考（按模块）

注意：下列签名为库中可见函数的简化描述，实际使用时以代码编辑器的提示为准。

- NumberBox
  - createFloat : CompositeDisposable -> TextBox -> ISubject<float> -> TextBox
  - createSingle : CompositeDisposable -> TextBox -> ISubject<float32> -> TextBox
  - createInt64 : CompositeDisposable -> TextBox -> ISubject<int64> -> TextBox
  - createInt : CompositeDisposable -> TextBox -> ISubject<int> -> TextBox
  - bindFocus / bindLostFocus : 绑定焦点事件，更新状态
- TextBox
  - bindFocus : CompositeDisposable -> TextBox -> IObservable<string> -> unit
  - bindLostFocus : CompositeDisposable -> TextBox -> ISubject<string> -> unit
  - create : CompositeDisposable -> TextBox -> ISubject<string> -> unit
- ComboBox
  - bindIndex : CompositeDisposable -> ComboBox -> ISubject<int> -> unit
  - bindItem : CompositeDisposable -> ComboBox -> ISubject<'t> -> unit
  - indexCreate : CompositeDisposable -> ComboBox -> seq<'t> -> ISubject<int> -> ComboBox
  - itemCreate : CompositeDisposable -> ComboBox -> seq<'t> -> ISubject<'t> -> ComboBox
- ToggleButton / CheckBox
  - ToggleButton.bind : CompositeDisposable -> ToggleButton -> ISubject<bool> -> unit
  - ToggleButton.create :  CompositeDisposable -> ToggleButton -> ISubject<bool>
  - CheckBox.bind : CompositeDisposable -> CheckBox -> ISubject<bool> -> unit
  - CheckBox.create : CompositeDisposable -> CheckBox -> ISubject<bool> -> obj -> CheckBox
- RadioButton
  - bindingRadioButton : CompositeDisposable -> ISubject<bool> -> RadioButton -> unit
  - bindingRadioButtonGroup : CompositeDisposable -> ISubject<int> -> RadioButton[] -> unit
  - bindingRadioButtonGroupUsingContent : CompositeDisposable -> ISubject<string> -> RadioButton[] -> unit
- Run
  - bind : CompositeDisposable -> Run -> IObservable<string> -> unit
  - create : CompositeDisposable -> Run -> IObservable<string> -> Run
- TextBlock
  - bind : CompositeDisposable -> TextBlock -> IObservable<string> -> unit
  - create : CompositeDisposable -> TextBlock -> IObservable<string> -> TextBlock

- MediaPlayer
  - playMany : MediaPlayer -> string[] -> IDisposable
  - createPlaylistObservable : MediaPlayer -> IObservable<#seq<string>> -> IObservable<unit>
  - createPlaylistObservable2 : (string -> unit) -> MediaPlayer -> IObservable<#seq<string>> -> IObservable<unit>

---

## 设计要点

- 基于 System.Reactive：所有数据流使用 IObservable / ISubject，以便函数式组合与订阅管理。
- 关注点分离：UI 控件创建/绑定逻辑与业务数据分离，便于测试。

---

## 贡献

欢迎提交 Issue 与 Pull Request。请确保代码风格与现有文件一致并附带简单说明。

## 许可证

GPLv3

## 参考

WPF 的属性值优先级：

1. 本地值（Local Value）- 最高优先级
2. 样式触发器（Style Triggers）
3. 模板触发器（Template Triggers）
4. 样式设置器（Style Setters）
5. 主题样式（Theme Style）
6. 继承值（Inherited Value）
7. 默认值（Default Value）- 最低优先级