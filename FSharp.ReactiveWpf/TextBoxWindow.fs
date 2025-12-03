module FSharp.ReactiveWpf.TextBoxWindow

open System
open System.Windows.Controls
open System.Reflection

open System.Reactive.Subjects
open System.Reactive.Disposables
open System.Reactive.Linq
open MahApps.Metro.Controls

let getFloat (initialValue: float) =
    let assy = Assembly.GetExecutingAssembly()
    let window =
        XamlLoader.loadXaml assy "FSharp.ReactiveWpf.TextBoxWindow.xaml" :?> MetroWindow
    let textbox = window.FindName("textbox") :?> TextBox
    let confirm = window.FindName("confirm") :?> Button
    let cancel = window.FindName("cancel") :?> Button

    let disposable = new CompositeDisposable()

    let innerValue = new BehaviorSubject<float>(initialValue)
    WpfSubscriber.bindingNumberBox disposable innerValue textbox

    (cancel.Click :?> IObservable<_>).Subscribe(fun _ -> window.DialogResult <- Nullable(false))
    |> disposable.Add
    let mutable output = 0.0
    (confirm.Click :?> IObservable<_>)
        .WithLatestFrom(innerValue)
        .Subscribe(fun struct (_, v) ->
            output <- v
            window.DialogResult <- Nullable(true)
        )
    |> disposable.Add

    window.Closed.Add(fun _ ->
        disposable.Dispose()
        innerValue.Dispose()
    )
    window, fun () -> output

let getInt64 (initialValue: int64) =
    let assy = Assembly.GetExecutingAssembly()
    let window =
        XamlLoader.loadXaml assy "FSharp.ReactiveWpf.TextBoxWindow.xaml" :?> MetroWindow
    let textbox = window.FindName("textbox") :?> TextBox
    let confirm = window.FindName("confirm") :?> Button
    let cancel = window.FindName("cancel") :?> Button

    let disposable = new CompositeDisposable()

    let innerValue = new BehaviorSubject<int64>(initialValue)
    WpfSubscriber.bindingInt64Box disposable innerValue textbox

    (cancel.Click :?> IObservable<_>).Subscribe(fun _ -> window.DialogResult <- Nullable(false))
    |> disposable.Add

    let mutable output = 0L
    (confirm.Click :?> IObservable<_>)
        .WithLatestFrom(innerValue)
        .Subscribe(fun struct (_, v) ->
            output <- v
            window.DialogResult <- Nullable(true)
        )
    |> disposable.Add

    window.Closed.Add(fun _ ->
        disposable.Dispose()
        innerValue.Dispose()
    )
    window, fun () -> output

let getInt (initialValue: int) =
    let assy = Assembly.GetExecutingAssembly()
    let window =
        XamlLoader.loadXaml assy "FSharp.ReactiveWpf.TextBoxWindow.xaml" :?> MetroWindow
    let textbox = window.FindName("textbox") :?> TextBox
    let confirm = window.FindName("confirm") :?> Button
    let cancel = window.FindName("cancel") :?> Button

    let disposable = new CompositeDisposable()

    let innerValue = new BehaviorSubject<int>(initialValue)
    WpfSubscriber.bindingIntegerBox disposable innerValue textbox

    (cancel.Click :?> IObservable<_>).Subscribe(fun _ -> window.DialogResult <- Nullable(false))
    |> disposable.Add

    let mutable output = 0
    (confirm.Click :?> IObservable<_>)
        .WithLatestFrom(innerValue)
        .Subscribe(fun struct (_, v) ->
            output <- v
            window.DialogResult <- Nullable(true)
        )
    |> disposable.Add

    window.Closed.Add(fun _ ->
        disposable.Dispose()
        innerValue.Dispose()
    )
    window, fun () -> output
