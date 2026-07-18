module FSharp.ReactiveWpf.NumberBox

open System
open System.Reactive.Linq
open System.Reactive.Subjects
open System.Reactive.Disposables

open System.Windows.Controls
open FSharp.Idioms
open System.Threading

//一个双向绑定的文本输入框，专门用于处理浮点数输入。主要功能是：
//将文本框的文本内容转换为浮点数
//将浮点数更新同步回文本框
let bind
    (disposable: CompositeDisposable)
    (value: ISubject<float>)
    (textbox: TextBox)
    =
    (textbox.LostFocus :?> IObservable<_>)
        .Select(fun _ -> textbox.Text)
        .Select(fun t -> Decimal.tryFloat t)
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

let create (disposable: CompositeDisposable) (value: ISubject<float>) =
    let textbox = TextBox()

    bind disposable value textbox

    //textbox.Unloaded.Add(fun _ ->
    //    disposable.Dispose()
    //)
    textbox

let createLocal (value: ISubject<float>) =
    let disposable = new CompositeDisposable()
    create disposable value
