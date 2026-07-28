module FSharp.ReactiveWpf.TextBox

open System
open System.Windows.Controls


open System.Reactive.Linq
open System.Reactive.Subjects
open System.Reactive.Disposables

open System.Threading


let bindFocus
    (tb: TextBox)
    (value: IObservable<string>)
    (disposable: CompositeDisposable)
    =
    value
        .DistinctUntilChanged()
        .Throttle(TimeSpan.FromMilliseconds(100.0))
        .ObserveOn(SynchronizationContext.Current)
        .Subscribe(fun text -> 
            if not tb.IsFocused then
                tb.Text <- text
            )
    |> disposable.Add

let bindLostFocus
    (textbox: TextBox)
    (value: ISubject<string>)
    (disposable: CompositeDisposable)
    =
    (textbox.LostFocus :?> IObservable<_>)
        .Select(fun _ -> textbox.Text)
        .DistinctUntilChanged()
        .Subscribe(value)
    |> disposable.Add


let create
    (textbox: TextBox)
    (value: ISubject<string>)
    (disposable: CompositeDisposable)
    =
    (textbox.LostFocus :?> IObservable<_>)
        .Select(fun _ -> textbox.Text)
        .DistinctUntilChanged()
        .Subscribe(value)
    |> disposable.Add

    value
        .DistinctUntilChanged()
        .ObserveOn(SynchronizationContext.Current)
        .Subscribe(fun text ->
            if not textbox.IsFocused then
                textbox.Text <- text
        )
    |> disposable.Add
