module FSharp.ReactiveWpf.TextBoxWindow

open System
open System.Windows.Controls
open System.Reflection

open System.Reactive.Subjects
open System.Reactive.Disposables
open System.Reactive.Linq
open MahApps.Metro.Controls

let assy = Assembly.GetExecutingAssembly()

let private main (initialValue: 't) binder =
    let window =
        XamlLoader.loadXaml assy "FSharp.ReactiveWpf.TextBoxWindow.xaml" :?> MetroWindow
    let textbox = window.FindName("textbox") :?> TextBox
    let confirm = window.FindName("confirm") :?> Button
    let cancel = window.FindName("cancel") :?> Button

    let disposable = new CompositeDisposable()

    let value = new BehaviorSubject<'t>(initialValue)
    let mutable output = initialValue

    //仅有的不同部分
    binder disposable textbox value

    (confirm.Click :?> IObservable<_>)
        .WithLatestFrom(value)
        .Subscribe(fun struct (_, v) ->
            output <- v
            window.DialogResult <- Nullable(true)
        )
    |> disposable.Add

    (cancel.Click :?> IObservable<_>).Subscribe(fun _ -> window.DialogResult <- Nullable(false))
    |> disposable.Add

    window.Closed.Add(fun _ ->
        disposable.Dispose()
        value.Dispose()
    )
    window, fun () -> output

let getFloat (initialValue: float) =
    let binder disposable textbox textValue =
        WpfSubscriber.bindingNumberBox disposable textValue textbox
    main initialValue binder

let getInt64 (initialValue: int64) =
    let binder disposable textbox textValue =
        WpfSubscriber.bindingInt64Box disposable textValue textbox
    main initialValue binder

let getInt (initialValue: int) =
    let binder disposable textbox textValue =
        WpfSubscriber.bindingIntegerBox disposable textValue textbox
    main initialValue binder
