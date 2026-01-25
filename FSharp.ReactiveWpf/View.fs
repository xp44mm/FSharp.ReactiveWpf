module FSharp.ReactiveWpf.View

open System
open System.Windows.Controls

open System.Reactive.Subjects
open System.Reactive.Linq

open System.Threading

open FSharp.Idioms
open System.Reactive.Disposables

let formatValue (value: 't) =
    match box value with
    | :? float as f -> sprintf "%.2f" f
    | :? float32 as f32 -> sprintf "%.2f" f32
    | :? int as i -> string i
    | :? int64 as i64 -> string i64
    | :? decimal as d -> sprintf "%.2M" d
    | :? DateTime as dt -> dt.ToString("yyyy-MM-dd HH:mm:ss")
    | :? bool as b -> if b then "是" else "否"
    | :? string as s -> s
    | null -> ""
    | _ -> sprintf "%A" value

let textBlock (ob: IObservable<'t>) =
    let tb = TextBlock()
    let sub =
        ob.ObserveOn(SynchronizationContext.Current).Subscribe(fun s -> tb.Text <- formatValue s)
    tb.Unloaded.Add(fun _ -> sub.Dispose())
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
    let sub2 =
        value
            .DistinctUntilChanged()
            .Select(fun f -> f.ToString())
            .ObserveOn(SynchronizationContext.Current)
            .Subscribe(fun text ->
                if not textbox.IsFocused then
                    textbox.Text <- text
            )
    textbox.Unloaded.Add(fun _ ->
        sub1.Dispose()
        sub2.Dispose()
    )
    textbox

let checkBox (value: ISubject<bool>) =
    let cb = CheckBox()
    cb.IsThreeState <- false
    let c = (cb.Checked :?> IObservable<_>).Select(fun _ -> true)
    let u = (cb.Unchecked :?> IObservable<_>).Select(fun _ -> false)
    let sub1 = c.Merge(u).Subscribe(value)
    let sub2 =
        value
            .DistinctUntilChanged()
            .ObserveOn(SynchronizationContext.Current)
            .Subscribe(fun x ->
                if cb.IsChecked.HasValue && cb.IsChecked.Value <> x then
                    cb.IsChecked <- Nullable(x)
            )
    cb.Unloaded.Add(fun _ ->
        sub1.Dispose()
        sub2.Dispose()
    )
    cb

/// 
let comboBox (index: ISubject<int>) =
    let comboBox = ComboBox()
    let disposable = new CompositeDisposable()
    WpfSubscriber.bindingComboBox disposable index comboBox
    comboBox.Unloaded.Add(fun _ -> disposable.Dispose())
    comboBox
