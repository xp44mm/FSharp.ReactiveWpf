module PipeSections.App

open System.Reflection
open System.Windows

open FSharp.ReactiveWpf

/// 当前程序集，XAML 以嵌入资源形式打包在其中
let assy = Assembly.GetExecutingAssembly()

/// 辅助函数：从嵌入资源加载 XAML 对象（手动加载，不使用 Page 编译）
let loadXaml filename =
    "PipeSections." + filename
    |> XamlLoader.loadXaml assy

/// 加载 App.xaml 创建 Application 实例
let app =
    loadXaml "App.xaml"
    :?> Application
