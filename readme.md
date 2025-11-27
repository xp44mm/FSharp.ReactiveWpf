# FSharp.ReactiveWpf

一个用于 F# 和 WPF 的响应式数据绑定库，提供类型安全且函数式的 UI 绑定解决方案。

## 功能特性

- 🚀 **响应式数据绑定** - 基于 Reactive Extensions (Rx) 构建
- 🎯 **类型安全** - 完整的 F# 类型支持
- 🔄 **双向绑定** - 支持控件与数据源的双向同步
- 📦 **即插即用** - 简单的 API 设计，易于集成
- 🎨 **MahApps.Metro 支持** - 现代化 UI 控件支持

## 安装

```bash
dotnet add package FSharp.ReactiveWpf
```

## 快速开始

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
bindingNumberBox disposables numberValue myTextBox

// 绑定文本框（字符串）
bindingTextBox disposables textValue myStringTextBox

// 绑定复选框
bindingCheckBox disposables boolValue myCheckBox

// 绑定单选按钮组
let radioButtons = [| radio1; radio2; radio3 |]
bindingRadioButtonGroup disposables selectedIndex radioButtons
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
- `bindingNumberBox` - 浮点数文本框
- `bindingIntegerBox` - 整数文本框  
- `bindingInt64Box` - 64位整数文本框
- `bindingTextBox` - 字符串文本框

#### 选择控件绑定
- `bindingComboBox` - 组合框（索引绑定）
- `bindingComboBoxItem` - 组合框（项绑定）
- `bindingRadioButtonGroup` - 单选按钮组

#### 切换控件绑定
- `bindingCheckBox` - 复选框
- `bindingRadioButton` - 单选按钮

#### 文本显示
- `bindingRun` - Run 文本元素绑定

### 对话框函数

- `getFloat` - 获取浮点数输入
- `getInt` - 获取整数输入  
- `getInt64` - 获取64位整数输入

## 完整示例

```fsharp
open System.Windows
open FSharp.ReactiveWpf.Binding
open System.Reactive.Subjects
open System.Reactive.Disposables

type MainWindow() as this =
    inherit Window()
    
    let disposables = new CompositeDisposable()
    
    // 创建数据模型
    let temperature = new BehaviorSubject<float>(20.0)
    let name = new BehaviorSubject<string>("")
    let isActive = new BehaviorSubject<bool>(true)
    let selectedIndex = new BehaviorSubject<int>(0)
    
    do
        // 初始化 UI 组件
        this.InitializeComponent()
        
        // 绑定控件
        bindingNumberBox disposables temperature this.temperatureTextBox
        bindingTextBox disposables name this.nameTextBox  
        bindingCheckBox disposables isActive this.activeCheckBox
        bindingComboBox disposables selectedIndex this.categoryComboBox
        
    interface IDisposable with
        member this.Dispose() = disposables.Dispose()
```

## 设计理念

### 响应式编程
库基于观察者模式，使用 `BehaviorSubject<T>` 作为数据源，确保数据流的实时性和一致性。

### 资源管理
使用 `CompositeDisposable` 统一管理订阅，避免内存泄漏。

### 关注点分离
UI 逻辑与业务逻辑清晰分离，便于测试和维护。

## 依赖项

- .NET 6.0+ / .NET Framework 4.7.2+
- FSharp.Core
- System.Reactive
- MahApps.Metro (用于对话框)
- WindowsBase
- PresentationFramework

## 贡献

欢迎提交 Issue 和 Pull Request！

## 许可证

LGPL-3.0-or-later