[<System.Obsolete("使用各具体控件模块")>]
module FSharp.ReactiveWpf.View

open System
open System.Windows.Controls

open System.Reactive.Subjects
open System.Reactive.Linq

open System.Threading

open FSharp.Idioms
open System.Reactive.Disposables
open FSharp.Idioms.Literal

let textBlock (ob: IObservable<'t>) =
    let tb = TextBlock()
    //let sub =
    ob.ObserveOn(SynchronizationContext.Current).Subscribe(fun s -> tb.Text <- stringify s)
    |> ignore

    //tb.Unloaded.Add(fun _ -> sub.Dispose())
    tb

let numberBox (value: ISubject<float>) =
    let textbox = TextBox()

    let sub1 =
        (textbox.LostFocus :?> IObservable<_>)
            .Select(fun _ -> textbox.Text)
            .Select(fun t -> Decimal.tryFloat t)
            .Where(Option.isSome)
            .Select(Option.get)
            .DistinctUntilChanged()
            .Subscribe(value)

    //let sub2 =
    value
        .DistinctUntilChanged()
        .Select(fun f -> f.ToString())
        .ObserveOn(SynchronizationContext.Current)
        .Subscribe(fun text ->
            if not textbox.IsFocused then
                textbox.Text <- text
        )
    |> ignore

    textbox

let checkBox (value: ISubject<bool>) =
    let cb = CheckBox()
    cb.IsThreeState <- false
    let c = (cb.Checked :?> IObservable<_>).Select(fun _ -> true)
    let u = (cb.Unchecked :?> IObservable<_>).Select(fun _ -> false)
    c.Merge(u).Subscribe(value)
    |> ignore
    value
        .DistinctUntilChanged()
        .ObserveOn(SynchronizationContext.Current)
        .Subscribe(fun x ->
            if
                cb.IsChecked.HasValue
                && cb.IsChecked.Value <> x
            then
                cb.IsChecked <- Nullable(x)
        )
    |> ignore

    cb

///
let comboBox (index: ISubject<int>) =
    let comboBox = ComboBox()
    let disposable = new CompositeDisposable()
    ComboBox.bindIndex disposable index comboBox
    comboBox
