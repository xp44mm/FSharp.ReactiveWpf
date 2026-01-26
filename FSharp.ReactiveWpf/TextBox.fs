module FSharp.ReactiveWpf.TextBox

open System
open System.Reactive.Linq
open System.Reactive.Subjects
open System.Reactive.Disposables

open System.Windows.Controls
open FSharp.Idioms
open System.Threading

let bindingReadOnlyTextBox
    (disposable: CompositeDisposable)
    (value: IObservable<string>)
    (tb: TextBox)
    =
    value
        .DistinctUntilChanged()
        .ObserveOn(SynchronizationContext.Current)
        .Subscribe(fun text -> tb.Text <- text)
    |> disposable.Add

let bindingTextBox
    (disposable: CompositeDisposable)
    (value: ISubject<string>)
    (textbox: TextBox)
    =
    (textbox.LostFocus :?> IObservable<_>)
        .Select(fun _ -> textbox.Text)
        .DistinctUntilChanged()
        .Subscribe(value)
    |> disposable.Add

    bindingReadOnlyTextBox disposable value textbox

let readOnlyTextBox (value: IObservable<string>) = 
    let textbox = TextBox()
    let disposable = new CompositeDisposable()
    bindingReadOnlyTextBox disposable value textbox
    textbox.Unloaded.Add(fun _ ->
        disposable.Dispose()
    )
    textbox

let textBox (value: ISubject<string>) =
    let textbox = TextBox()
    let disposable = new CompositeDisposable()

    bindingTextBox disposable value textbox

    textbox.Unloaded.Add(fun _ ->
        disposable.Dispose()
    )
    textbox

let bindingIntegerBox
    (disposable: CompositeDisposable)
    (value: ISubject<int>)
    (textbox: TextBox)
    =
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

let bindingInt64Box
    (disposable: CompositeDisposable)
    (value: ISubject<int64>)
    (textbox: TextBox)
    =
    (textbox.LostFocus :?> IObservable<_>)
        .Select(fun _ -> textbox.Text)
        .Select(fun t -> Decimal.tryInt t)
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
