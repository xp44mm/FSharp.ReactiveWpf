module FSharp.ReactiveWpf.NumberBox

open System
open System.Reactive.Linq
open System.Reactive.Subjects
open System.Reactive.Disposables

open System.Windows.Controls
open FSharp.Idioms
open System.Threading

/// 从值到文本框
let bindFocus<'t> (textbox: TextBox) (value: ISubject<'t>) (disposable: CompositeDisposable) =
    value
        .Select(fun f -> f.ToString())
        .DistinctUntilChanged()
        .Throttle(TimeSpan.FromMilliseconds(100.0))
        .ObserveOn(SynchronizationContext.Current)
        .Subscribe(fun text ->
            if not textbox.IsFocused then
                textbox.Text <- text
        )
    |> disposable.Add

/// 从文本框到值
let bindLostFocus<'T>
    (parse: string -> 'T option)
    (textbox: TextBox)
    (value: ISubject<'T>)
    (disposable: CompositeDisposable)
    =
    (textbox.LostFocus :?> IObservable<_>)
        .Select(fun _ -> textbox.Text)
        .Select(fun txt -> parse txt)
        .Where(Option.isSome)
        .Select(Option.get)
        .DistinctUntilChanged()
        .Subscribe(value)
    |> disposable.Add

let createNumber (parse: string -> 'n option) (value: ISubject<'n>) (disposable: CompositeDisposable) =
    let textbox = TextBox()
    bindLostFocus parse textbox value disposable
    bindFocus textbox value disposable
    textbox

let createFloat (value: ISubject<float>) (disposable: CompositeDisposable) =
    createNumber FSharp.Idioms.Decimal.tryFloat value disposable

let createSingle (value: ISubject<float32>) (disposable: CompositeDisposable) =
    let parseSingle (s: string) =
        FSharp.Idioms.Decimal.tryFloat s
        |> Option.map float32
    createNumber parseSingle value disposable

let createInt64 (value: ISubject<int64>) (disposable: CompositeDisposable) =
    let parseInt64 = FSharp.Idioms.Decimal.tryInt
    createNumber parseInt64 value disposable

let createInt (value: ISubject<int>) (disposable: CompositeDisposable) =
    let parseInt (s: string) =
        FSharp.Idioms.Decimal.tryInt s
        |> Option.map int
    createNumber parseInt value disposable  

