module FSharp.ReactiveWpf.TextBox

open System
open System.Windows.Controls

open System.Reactive.Linq
open System.Reactive.Subjects
open System.Reactive.Disposables

open System.Threading

let bindFocus
    (disposable: CompositeDisposable)
    (tb: TextBox)
    (value: IObservable<string>)
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
    (disposable: CompositeDisposable)
    (textbox: TextBox)
    (value: ISubject<string>)
    =
    (textbox.LostFocus :?> IObservable<_>)
        .Select(fun _ -> textbox.Text)
        .DistinctUntilChanged()
        .Subscribe(value)
    |> disposable.Add

let create
    (disposable: CompositeDisposable)
    (value: ISubject<string>)
    =
    let textbox = TextBox()
    bindLostFocus disposable textbox value
    bindFocus disposable textbox value
    textbox
