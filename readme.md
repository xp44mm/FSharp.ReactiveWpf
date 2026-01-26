# FSharp.ReactiveWpf

一个用于 F# 和 WPF 的响应式数据绑定库，提供类型安全且函数式的 UI 绑定解决方案。


### 基本数据绑定

```fsharp
open FSharp.ReactiveWpf.Binding
open System.Reactive.Subjects
open System.Reactive.Disposables

// 创建可观察数据源
let numberValue = new BehaviorSubject<float>(0.0)
let textValue = new BehaviorSubject<string>("Hello")
let boolValue = new BehaviorSubject<bool>(false)

// 创建 disposable 容器
let disposables = new CompositeDisposable()

// 绑定文本框（浮点数）
NumberBox.bind disposables numberValue myTextBox

// 绑定文本框（字符串）
TextBox.bind disposables textValue myStringTextBox

// 绑定复选框
CheckBox.bind disposables boolValue myCheckBox

// 绑定单选按钮组
let radioButtons = [| radio1; radio2; radio3 |]
RadioButton.bindingRadioButtonGroup disposables selectedIndex radioButtons
```

### 数字输入对话框

```fsharp
open FSharp.ReactiveWpf.TextBoxWindow

// 获取浮点数输入
let (window, getResult) = getFloat 0.0

if window.ShowDialog() = Nullable true then
    let result = getResult()
    printfn "用户输入: %f" result

// 获取整数输入
let (window, getResult) = getInt 42

if window.ShowDialog() = Nullable true then
    let result = getResult()
    printfn "用户输入: %d" result
```

## API 参考

### 绑定函数

#### 文本框绑定
- `NumberBox.bind` - 浮点数文本框
- `TextBox.bindingIntegerBox` - 整数文本框  
- `TextBox.bindingInt64Box` - 64位整数文本框
- `TextBox.bindingTextBox` - 字符串文本框

#### 选择控件绑定
- `ComboBox.bindIndex` - 组合框（索引绑定）
- `ComboBox.bindItem` - 组合框（项绑定）
- `RadioButton.bindingRadioButtonGroup` - 单选按钮组

#### 切换控件绑定
- `CheckBox.bind` - 复选框
- `RadioButton.bindingRadioButton` - 单选按钮

#### 文本显示
- `Run.bind` - Run 文本元素绑定

### 对话框函数

- `TextBoxWindow.getFloat` - 获取浮点数输入
- `TextBoxWindow.getInt` - 获取整数输入  
- `TextBoxWindow.getInt64` - 获取64位整数输入

# MediaPlayer.createPlaylistObservable

## 函数签名

```fsharp
val createPlaylistObservable : 
    mediaPlayer:MediaPlayer -> 
    subject:IObservable<string[]> -> 
    IObservable<unit>
```

## 功能描述

创建一个响应式音频播放器，能够监听播放列表序列并自动按顺序播放其中的音频文件。当接收到新的播放列表时，会自动停止当前播放并开始新的播放列表。

## 参数说明

- `mediaPlayer` : `System.Windows.Media.MediaPlayer`  
  用于实际播放音频的媒体播放器实例

- `subject` : `IObservable<string[]>`  
  播放列表的观察序列，每次发射一个字符串数组，每个字符串代表一个音频文件的完整路径

## 返回值

返回一个 `IObservable<unit>`，表示播放过程的观察序列。主要用于订阅播放生命周期，实际播放是副作用。



## 使用示例

```fsharp
let mediaPlayer = new MediaPlayer()
let playlistSubject = new Subject<string[]>()

// 创建播放器观察序列
let playback = MediaPlayer.createPlaylistObservable mediaPlayer playlistSubject

// 订阅播放过程
use subscription = playback.Subscribe()

// 发送播放列表
playlistSubject.OnNext([|
    @"C:\audio\file1.mp3"
    @"C:\audio\file2.mp3" 
    @"C:\audio\file3.mp3"
|])
```




## 完整示例

```fsharp
```

## 设计理念

### 响应式编程
库基于观察者模式，使用 `BehaviorSubject<T>` 作为数据源，确保数据流的实时性和一致性。

### 资源管理
使用 `CompositeDisposable` 统一管理订阅，避免内存泄漏。

### 关注点分离
UI 逻辑与业务逻辑清晰分离，便于测试和维护。

## 贡献

欢迎提交 Issue 和 Pull Request！

## 许可证

GPLv3

