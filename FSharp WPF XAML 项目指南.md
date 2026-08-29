# F# 手动加载 WPF XAML 项目完整指南

下面我将详细演示如何用 F# 从零开始构建一个手动加载 XAML 的 WPF 应用程序。

首先，新建一个F#控制台应用。

Code and Uncompiled XAML: A way to use XAML is to parse it on the fly with the XamlReader.

## 项目结构说明

```
AnqiEnglish.fsproj  # 项目配置文件
App.xaml           # 应用程序资源文件
MainWindow.xaml    # 主窗口布局文件
Program.fs         # 主程序入口文件
```

## 第一步：创建项目文件 (AnqiEnglish.fsproj)

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
		<OutputType>WinExe</OutputType>
		<TargetFramework>net10.0-windows</TargetFramework>
		<UseWPF>true</UseWPF>
		<UseWindowsForms>False</UseWindowsForms>
      
		<SatelliteResourceLanguages>zh-Hans;en</SatelliteResourceLanguages>
		<PublishDir>bin\$(MSBuildProjectName)\</PublishDir>
		<CleanPublishFolder>true</CleanPublishFolder>
  </PropertyGroup>

  <ItemGroup>
    <Page Remove="App.xaml" />
    <Page Remove="MainWindow.xaml" />
  </ItemGroup>
  
  <ItemGroup>
    <EmbeddedResource Include="App.xaml" />
    <EmbeddedResource Include="MainWindow.xaml" />
    <Compile Include="Program.fs" />
  </ItemGroup>
  
  <ItemGroup>
    <PackageReference Update="FSharp.Core" Version="9.0.300" />
  </ItemGroup>
</Project>
```

在这个F# WPF项目中，将XAML文件从`页(Page`)类型改为嵌入的资源(EmbeddedResource)类型，将会导致.fsproj这样配置文件：
```xml
  <ItemGroup>
    <Page Remove="App.xaml" />
    <EmbeddedResource Include="App.xaml" />
  </ItemGroup>
```

从`页(Page`)类型改为嵌入的资源(EmbeddedResource)类型是出于以下几个关键原因：

### 1. **避免自动编译冲突**
- **默认`Page`类型**：WPF会尝试自动将`.xaml`编译为`.g.cs`代码后置文件（C#风格）
- **F#兼容问题**：F#没有C#的代码后置机制，会导致编译错误
- **解决方案**：嵌入的资源(EmbeddedResource)，XAML保持原始文件形式，由开发者手动控制加载逻辑

### 技巧

1. 从已有项目复制`<PropertyGroup>`整个。
2. 用复制粘贴的方法，而不是Ctrl+拖拽的方法复制文件。比如：从已有项目复制app.xaml粘贴到本项目，vs会自动设置好app.xaml的项目属性。
3. 




## 第二步：创建App.xaml (应用程序定义)

```xml
<Application 
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" 
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" />
```

这是一个最简化的WPF应用程序定义，不包含任何资源。

## 第三步：创建MainWindow.xaml (主窗口)

```xml
<Window
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
    xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
    Title="MainWindow"
    Width="800"
    Height="450"
    WindowStartupLocation="CenterScreen"
    mc:Ignorable="d">

    <StackPanel HorizontalAlignment="Center" VerticalAlignment="Center">
        <TextBlock
            Margin="10"
            FontSize="24"
            Text="Hello .NET 9.0!" />
        <Button
            x:Name="MyButton"
            Width="120"
            Height="40"
            Margin="10"
            Content="Click Me!" />
    </StackPanel>
</Window>
```

注意：
- 给按钮定义了 `x:Name="MyButton"` 以便代码中查找
- 使用了简单的垂直堆叠布局

## 第四步：创建Program.fs (主程序)

```fsharp
module AnqiEnglish.Program

open System
open System.Xml
open System.IO
open System.Windows
open System.Windows.Controls
open System.Windows.Markup

/// 辅助函数：从文件加载XAML对象
let getXamlObj xaml =
    let path = Path.Combine(__SOURCE_DIRECTORY__, xaml)
    let reader = XmlReader.Create(path)
    XamlReader.Load(reader)

/// 加载App.xaml创建Application实例
let app = 
    getXamlObj "App.xaml"
    :?> Application

/// 加载并配置主窗口
let mainWindow = 
    let window = 
        getXamlObj "MainWindow.xaml"
        :?> Window

    // 查找按钮并添加点击事件
    let btn = window.FindName("MyButton") :?> Button
    btn.Click.Add(fun _ ->
        MessageBox.Show("按钮点击事件来自代码！") 
        |> ignore
    )
    window

[<STAThread>]
[<EntryPoint>]
let main _ = app.Run(mainWindow) 
```

## 代码解析

- 文件路径可通过`AppContext.BaseDirectory`可靠获取：
  ```fsharp
  let path = Path.Combine(AppContext.BaseDirectory, "MainWindow.xaml")
  ```

- **XAML加载机制**
   - 使用 `XamlReader.Load` 手动解析XAML文件
   - 通过 `XmlReader` 读取XAML内容
   - 转换为具体的WPF类型 (`Application`/`Window`)

- **控件绑定**
  
   - 通过 `FindName` 方法查找命名控件
   - 强制转换为具体控件类型 (`Button`)
   - 使用F#的事件处理语法添加点击事件
   
- **应用程序启动**
   - `[<STAThread>]` 是WPF必需的线程模型属性
   - 调用 `Application.Run` 启动消息循环

## 运行流程

1. 加载App.xaml创建Application实例
2. 加载MainWindow.xaml创建窗口实例
3. 查找窗口中的按钮并绑定事件
4. 启动WPF消息循环显示窗口





