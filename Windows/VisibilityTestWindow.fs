module Windows.VisibilityTestWindow

open System
open System.Windows
open System.Windows.Controls

open System.Reactive.Disposables
open System.Reactive.Subjects
open System.Reactive.Linq

open MahApps.Metro.Controls

open FSharp.ReactiveWpf

/// 初始化 Visibility 测试窗口，通过 toggleButton 切换可见性，
/// targetBorder 的 Visibility 由 UIElement.bindVisible 动态绑定。
let createWindow () =
    let window = App.loadXaml "VisibilityTestWindow.xaml" :?> MetroWindow

    let toggleButton = window.FindName("toggleButton") :?> Button
    let targetBorder = window.FindName("targetBorder") :?> Border

    let disposable = new CompositeDisposable()

    let visibleSubject = new BehaviorSubject<bool>(true)

    (toggleButton.Click :?> IObservable<_>)
        .Subscribe(fun _ ->
            visibleSubject.OnNext(not visibleSubject.Value)
        )
    |> disposable.Add

    UIElement.bindVisible disposable targetBorder visibleSubject

    window.Closing.Add(fun _ -> disposable.Dispose())
    window
