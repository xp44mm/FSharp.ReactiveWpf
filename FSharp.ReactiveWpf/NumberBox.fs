module FSharp.ReactiveWpf.NumberBox

open System
open System.Reactive.Linq
open System.Reactive.Subjects
open System.Reactive.Disposables

open System.Windows.Controls
open FSharp.Idioms
open System.Threading

/// 从值到文本框
let bindFocus<'t> (disposable: CompositeDisposable) (textbox: TextBox) (value: ISubject<'t>) =
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
    (disposable: CompositeDisposable)
    (textbox: TextBox)
    (parse: string -> 'T option)
    (value: ISubject<'T>)
    =
    (textbox.LostFocus :?> IObservable<_>)
        .Select(fun _ -> textbox.Text)
        .Select(fun txt -> parse txt)
        .Where(Option.isSome)
        .Select(Option.get)
        .DistinctUntilChanged()
        .Subscribe(value)
    |> disposable.Add

let createBase (disposable: CompositeDisposable) (parse: string -> 'n option) (value: ISubject<'n>) =
    let textbox = TextBox()
    bindLostFocus disposable textbox parse value
    bindFocus disposable textbox value
    textbox

let createFloat (disposable: CompositeDisposable) (value: ISubject<float>) =
    createBase disposable JsonNumber.tryParse value

let createSingle (disposable: CompositeDisposable) (value: ISubject<float32>) =
    let parseSingle =
        JsonNumber.tryParse
        >> Option.map float32
    createBase disposable parseSingle value

let createInt64 (disposable: CompositeDisposable) (value: ISubject<int64>) = 
    createBase disposable Int64.tryParse value

let createInt (disposable: CompositeDisposable) (value: ISubject<int>) =
    let parseInt = Int64.tryParse >> Option.map int
    createBase disposable parseInt value
