module Windows.StyleTestWindow

open System
open System.Windows
open System.Windows.Controls

open System.Reactive.Disposables
open System.Reactive.Subjects
open System.Reactive.Linq

open MahApps.Metro.Controls

open FSharp.ReactiveWpf

/// 初始化 Style 测试窗口，通过 toggleButton 在两种 Style 间切换，
/// targetButton 的 Style 由 FrameworkElement.bindStyle 动态绑定。
let createWindow () =
    let window = App.loadXaml "StyleTestWindow.xaml" :?> MetroWindow

    let toggleButton = window.FindName("toggleButton") :?> Button
    let targetButton = window.FindName("targetButton") :?> Button

    let normalStyle = window.FindResource("NormalStyle") :?> Style
    let highlightStyle = window.FindResource("HighlightStyle") :?> Style

    let disposable = new CompositeDisposable()

    let styleSubject = new BehaviorSubject<Style>(normalStyle)

    (toggleButton.Click :?> IObservable<_>)
        .Subscribe(fun _ ->
            let next =
                if styleSubject.Value = normalStyle then
                    highlightStyle
                else
                    normalStyle
            styleSubject.OnNext(next)
        )
    |> disposable.Add

    FrameworkElement.bindStyle disposable targetButton styleSubject

    window.Closing.Add(fun _ -> disposable.Dispose())
    window
