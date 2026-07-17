module FSharp.ReactiveWpf.IntegerBox

open System
open System.Reactive.Linq
open System.Reactive.Subjects
open System.Reactive.Disposables

open System.Windows.Controls
open FSharp.Idioms
open System.Threading

let bind (disposable: CompositeDisposable) (value: ISubject<int>) (textbox: TextBox) =
    (textbox.LostFocus :?> IObservable<_>)
        .Select(fun _ -> textbox.Text)
        .Select(fun t -> Decimal.tryInt t |> Option.map int)
        .Where(Option.isSome)
        .Select(Option.get)
        .DistinctUntilChanged()
        .Subscribe(value)
    |> disposable.Add

    value
        .DistinctUntilChanged()
        .Select(fun f -> f.ToString())
        .ObserveOn(SynchronizationContext.Current)
        .Subscribe(fun text ->
            if not textbox.IsFocused then
                textbox.Text <- text
        )
    |> disposable.Add

let create (disposable: CompositeDisposable) (value: ISubject<int>) =
    let textbox = TextBox()
    bind disposable value textbox
    textbox.Unloaded.Add(fun _ -> disposable.Dispose())
    textbox

let createLocal (value: ISubject<int>) =
    let disposable = new CompositeDisposable()
    create disposable value
