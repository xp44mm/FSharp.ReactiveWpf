module FSharp.ReactiveWpf.TextBoxWindow

open System
open System.Windows.Controls

open System.Reactive.Subjects
open System.Reactive.Disposables
open System.Reactive.Linq
open MahApps.Metro.Controls
open FSharp.Idioms
open System.Windows

let private main
    (textAlignment: TextAlignment)
    (binder: CompositeDisposable -> TextBox -> ISubject<'t> -> unit)
    (initialValue: 't)
    =
    let window = Internal.loadXaml "TextBoxWindow.xaml" :?> MetroWindow
    let textbox = window.FindName("textbox") :?> TextBox
    let confirm = window.FindName("confirm") :?> Button
    let cancel = window.FindName("cancel") :?> Button

    textbox.TextAlignment <- textAlignment
    let disposable = new CompositeDisposable()

    let value = new BehaviorSubject<'t>(initialValue)
    let mutable output = initialValue

    //仅有的不同部分
    binder disposable textbox value

    (confirm.Click :?> IObservable<_>)
        .Do(fun _ -> confirm.Focus() |> ignore)
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

let getText (initialValue: string) =
    let binder textbox disposable textValue =
        TextBox.bindFocus textbox disposable textValue
        TextBox.bindLostFocus textbox disposable textValue
    main TextAlignment.Left binder initialValue

let getFloat (initialValue: float) =
    let binder disposable textbox textValue =
        NumberBox.bindFocus disposable textbox textValue
        NumberBox.bindLostFocus disposable textbox JsonNumber.tryParse textValue
    main TextAlignment.Right binder initialValue

let getInt64 (initialValue: int64) =
    let binder disposable textbox textValue =
        NumberBox.bindFocus disposable textbox textValue
        NumberBox.bindLostFocus disposable textbox Int64.tryParse textValue
    main TextAlignment.Right binder initialValue

let getInt (initialValue: int) =
    let binder disposable textbox textValue =
        NumberBox.bindFocus disposable textbox textValue
        NumberBox.bindLostFocus disposable textbox (Int64.tryParse >> Option.map int) textValue
    main TextAlignment.Right binder initialValue
