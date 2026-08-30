# FSharp.ReactiveWpf 新项目搭建指南（cli + ui 双项目模板）

> 用途：在任意新项目里快速复刻「纯逻辑层（netstandard2.0）+ WPF 界面层（net10.0-windows）」的 F# 响应式应用结构。
> 本文件可直接当作提示词交给 AI，或照此手工创建。以仓库中的 PipeSections.cli / PipeSections.ui 为活样例。

---

## 一、整体结构（一个业务 = cli + ui 两个项目）

```
MyApp/
├── MyApp.cli/MyApp.cli.fsproj   # 纯逻辑层：netstandard2.0，无 WPF，可跨平台、可单元测试
├── MyApp.ui/MyApp.ui.fsproj     # 界面层：net10.0-windows，WinExe，UseWPF
└── 加入解决方案：dotnet sln add
```

依赖关系：

```
MyApp.ui ──► MyApp.cli ──► FSharp.Idioms.Reactive（纯 Rx 核心，netstandard2.0，无 UI）
MyApp.ui ──► FSharp.ReactiveWpf（WPF 绑定库）──► FSharp.Idioms.Reactive
```

---

## 二、关键设计要点（先理解再动手）

1. **F# 没有 C# 的代码后置**：XAML 不能按默认 `Page` 编译（会尝试生成 `.g.cs` 报错）。解决办法：XAML 从 `Page` 移除、改 `EmbeddedResource`，由 `App.fs` 用 `XamlLoader.loadXaml` 手动加载。
2. **RootNamespace 必须设为项目名**：使嵌入资源名 = `"MyApp." + 文件名`，与 `App.fs` 里的加载前缀匹配。
3. **编译顺序严格按依赖排列**：cli 内 `Json → Calculations → ViewModels → Bindings`；ui 内 `App.fs → Views → MainWindow.fs → Program.fs`。
4. **数据流范式**：VM 字段用 `BehaviorSubject`；每条绑定拆成「值→控件」+「控件→值」两条 Rx 流；所有订阅注册进 `CompositeDisposable`，窗口 `Closing` 时统一 `Dispose`。
5. **按钮事件流**：`btn.Click :?> IObservable<_>` 拿到事件流再 `Subscribe`。
6. **cli 用 netstandard2.0**：只能用标准 API，禁止碰 WPF 与 net10 专属类型；好处是可跨平台复用、可脱离 UI 测试。

---

## 三、MyApp.cli.fsproj（完整模板）

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>Library</OutputType>
    <TargetFramework>netstandard2.0</TargetFramework>
    <SatelliteResourceLanguages>zh-Hans;en</SatelliteResourceLanguages>
  </PropertyGroup>

  <!-- 编译顺序：Json → Calculations → ViewModels → Bindings -->
  <ItemGroup>
    <Compile Include="Json.fs" />
    <Compile Include="Calculations\Calculation.fs" />
    <Compile Include="ViewModels\XxxViewModel.fs" />
    <Compile Include="ViewModels\MainViewModel.fs" />
    <Compile Include="Bindings\Binding.fs" />
    <Compile Include="Bindings\Collection.fs" />
  </ItemGroup>

  <ItemGroup>
    <PackageReference Update="FSharp.Core" Version="10.1.400" />
    <PackageReference Include="FSharp.Idioms" Version="1.5.17" />
    <PackageReference Include="System.Reactive" Version="7.0.0" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\FSharp.Idioms.Reactive\FSharp.Idioms.Reactive.fsproj" />
  </ItemGroup>

</Project>
```

---

## 四、MyApp.ui.fsproj（完整模板）

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>WinExe</OutputType>
    <TargetFramework>net10.0-windows</TargetFramework>
    <UseWPF>true</UseWPF>
    <UseWindowsForms>False</UseWindowsForms>
    <!-- 关键：保持嵌入资源名 = MyApp.*.xaml，与 App.fs 中 "MyApp." + filename 匹配 -->
    <RootNamespace>MyApp</RootNamespace>
    <SatelliteResourceLanguages>zh-Hans;en</SatelliteResourceLanguages>
  </PropertyGroup>

  <!-- XAML 从 Page 改为 EmbeddedResource：F# 无代码后置，避免 .g.cs 编译错误 -->
  <ItemGroup>
    <Page Remove="App.xaml" />
    <Page Remove="MainWindow.xaml" />
  </ItemGroup>

  <ItemGroup>
    <EmbeddedResource Include="App.xaml" />
    <EmbeddedResource Include="MainWindow.xaml" />
    <Compile Include="App.fs" />
    <Compile Include="Views\Row.fs" />
    <Compile Include="MainWindow.fs" />
    <Compile Include="Program.fs" />
  </ItemGroup>

  <ItemGroup>
    <PackageReference Update="FSharp.Core" Version="10.1.400" />
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include="FSharp.Idioms" Version="1.5.17" />
    <PackageReference Include="FSharp.RfcJson" Version="0.0.4" />
    <PackageReference Include="MahApps.Metro" Version="2.4.11" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\FSharp.ReactiveWpf\FSharp.ReactiveWpf.fsproj" />
    <ProjectReference Include="..\MyApp.cli\MyApp.cli.fsproj" />
  </ItemGroup>

</Project>
```

---

## 五、入口文件四件套（骨架）

### App.xaml（合并 MahApps 资源）

```xml
<Application xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <Application.Resources>
        <ResourceDictionary>
            <ResourceDictionary.MergedDictionaries>
                <ResourceDictionary Source="pack://application:,,,/MahApps.Metro;component/Styles/Controls.xaml" />
                <ResourceDictionary Source="pack://application:,,,/MahApps.Metro;component/Styles/Fonts.xaml" />
                <ResourceDictionary Source="pack://application:,,,/MahApps.Metro;component/Styles/Themes/Dark.Blue.xaml" />
            </ResourceDictionary.MergedDictionaries>
        </ResourceDictionary>
    </Application.Resources>
</Application>
```

### App.fs（从嵌入资源手动加载 XAML）

```fsharp
module MyApp.App

open System.Reflection
open System.Windows
open FSharp.ReactiveWpf

let assy = Assembly.GetExecutingAssembly()

let loadXaml filename =
    "MyApp." + filename
    |> XamlLoader.loadXaml assy

let app =
    loadXaml "App.xaml"
    :?> Application
```

### MainWindow.xaml（根元素 metro:MetroWindow，用 x:Name 标记要操作的控件）

```xml
<metro:MetroWindow
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:metro="http://metro.mahapps.com/winfx/xaml/controls"
    Title="MyApp" Width="800" Height="500"
    WindowStartupLocation="CenterScreen">
    <Grid Margin="10">
        <!-- 用 x:Name 命名需操作的控件，例如 <Button x:Name="myButton" Content="确定"/> -->
    </Grid>
</metro:MetroWindow>
```

### MainWindow.fs（骨架）

```fsharp
module MyApp.MainWindow

open System
open System.Windows
open System.Windows.Controls
open System.Reactive.Disposables
open MahApps.Metro.Controls
open FSharp.Idioms.Reactive

let createWindow () =
    let window = App.loadXaml "MainWindow.xaml" :?> MetroWindow
    let btn = window.FindName("myButton") :?> Button
    let disposable = new CompositeDisposable()

    let vm = MainViewModel.empty()

    // 按钮事件流
    (btn.Click :?> IObservable<_>)
    |> Observable.subscribe(fun _ -> ())
    |> disposable.Add

    // 数据流 → UI（示例：只读文本绑定）
    // vm.someText
    // |> TextBlock.bind disposable textBlock

    window.Closing.Add(fun _ -> disposable.Dispose())
    window
```

### Program.fs（入口）

```fsharp
module MyApp.Program

open System
open System.Threading
open System.Windows.Threading

SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext())

[<STAThread>]
[<EntryPoint>]
let main _ =
    let app = App.app
    let window = MainWindow.createWindow()
    app.Run(window)
```

---

## 六、快速创建命令（6 步）

1. 建两个 classlib 骨架（F# 无内置 WPF 模板）：

```powershell
dotnet new classlib -lang F# -n MyApp.cli
dotnet new classlib -lang F# -n MyApp.ui
```

2. 用第三节的 `MyApp.cli.fsproj` 覆盖 cli 的 fsproj（netstandard2.0 + 依赖 + 编译序）。
3. 用第四节的 `MyApp.ui.fsproj` 覆盖 ui 的 fsproj（WinExe / net10.0-windows / UseWPF / EmbeddedResource + 依赖 + 项目引用）。
4. 补齐 `App.xaml/App.fs`、`MainWindow.xaml/MainWindow.fs`、`Program.fs`。
5. 加入解决方案：

```powershell
dotnet sln Your.slnx add MyApp.cli\MyApp.cli.fsproj MyApp.ui\MyApp.ui.fsproj
```

6. 构建运行验证：

```powershell
dotnet build
dotnet run --project MyApp.ui
```

> 更快：直接把 `PipeSections.cli` / `PipeSections.ui` 两个文件夹复制改名当模板，全局替换 `PipeSections` → `MyApp`（fsproj 名、命名空间、RootNamespace、`App.fs` 里的 `"PipeSections."` 前缀）即可。

---

## 七、验收标准（照此逐项检查）

- [ ] cli 编译产物在 `bin/Debug/netstandard2.0/` 下，0 警告 0 错误
- [ ] ui 引用 cli 与 FSharp.ReactiveWpf 正常，0 警告 0 错误
- [ ] 运行时窗口正常弹出，无 XAML 解析异常（证明嵌入资源加载成功）
- [ ] 所有订阅都注册进 `CompositeDisposable`，窗口关闭即释放，无订阅泄漏
- [ ] 业务逻辑都在 cli 层，ui 层只做装配与渲染，可脱离 UI 做单元测试

---

## 参考

- 仓库：https://github.com/xp44mm/FSharp.ReactiveWpf
- 活样例：`PipeSections.cli` / `PipeSections.ui`（本指南第 3、4 节即其真实 fsproj）
- 绑定库 API：见仓库根 `readme.md`