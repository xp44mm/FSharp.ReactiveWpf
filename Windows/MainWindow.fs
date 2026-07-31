module Windows.MainWindow

open System
open System.Windows
open System.Windows.Controls

open System.Reactive.Disposables
open System.Reactive.Subjects
open System.Reactive.Linq

open MahApps.Metro.Controls

open FSharp.ReactiveWpf

/// 初始化主窗口并绑定事件
let createWindow () =
    let window = App.loadXaml "MainWindow.xaml" :?> MetroWindow

    let stringButton = window.FindName("stringButton") :?> Button
    let intButton = window.FindName("intButton") :?> Button
    let floatButton = window.FindName("floatButton") :?> Button
    let textBlock = window.FindName("textBlock") :?> TextBlock

    let disposable = new CompositeDisposable()

    let f = new BehaviorSubject<float>(0.0)

    (stringButton.Click :?> IObservable<_>)
        .Subscribe(fun _ ->
            let window, getResult = TextBoxWindow.getText "Enter a string value"
            window.Title <- "string test"
            if window.ShowDialog() = System.Nullable(true) then
                let result = getResult()
                MessageBox.Show(
                    sprintf "String value: %s" result,
                    "Input Result",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                )
                |> ignore
                ()
        )
    |> disposable.Add

    // Int 按钮
    (intButton.Click :?> IObservable<_>)
        .Subscribe(fun _ ->
            let window, getResult = TextBoxWindow.getInt 0
            window.Title <- "integer test"
            if window.ShowDialog() = System.Nullable(true) then
                let input = getResult()
                MessageBox.Show(
                    sprintf "Integer value: %d" input,
                    "Input Result",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                )
                |> ignore
                ()
        )
    |> disposable.Add

    // Float 按钮
    (floatButton.Click :?> IObservable<_>)
        .Subscribe(fun _ ->
            let window, getResult = TextBoxWindow.getFloat 0.0
            window.Title <- "float test"
            if window.ShowDialog() = System.Nullable(true) then
                let value = getResult()
                f.OnNext(value)
        )
    |> disposable.Add

    let data =
        f.Select(fun value -> value.ToString("0.##"))

    TextBlock.bind disposable textBlock data

    window.Closing.Add(fun _ -> disposable.Dispose())
    window

