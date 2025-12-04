module FSharp.ReactiveWpf.WpfSubscriber

open System
open System.Reactive.Linq
open System.Reactive.Subjects
open System.Reactive.Disposables

open System.Windows.Controls
open System.Windows.Documents
open FSharp.Idioms

//一个双向绑定的文本输入框，专门用于处理浮点数输入。主要功能是：
//将文本框的文本内容转换为浮点数
//将浮点数更新同步回文本框
let bindingNumberBox
    (disposable: CompositeDisposable)
    (value: BehaviorSubject<float>)
    (textbox: TextBox)
    =
    (textbox.LostFocus :?> IObservable<_>)
        .Select(fun _ -> textbox.Text)
        .Select(fun t -> Decimal.tryFloat t)
        .Where(Option.isSome)
        .Select(Option.get)
        .DistinctUntilChanged()
        .Subscribe(fun x -> value.OnNext x)
    |> disposable.Add

    value
        .DistinctUntilChanged()
        .Select(fun f -> f.ToString())
        .Subscribe(fun text ->
            if not textbox.IsFocused then
                textbox.Text <- text
        )
    |> disposable.Add

let bindingIntegerBox
    (disposable: CompositeDisposable)
    (value: BehaviorSubject<int>)
    (textbox: TextBox)
    =
    (textbox.LostFocus :?> IObservable<_>)
        .Select(fun _ -> textbox.Text)
        .Select(fun t -> Decimal.tryInt t |> Option.map int)
        .Where(Option.isSome)
        .Select(Option.get)
        .DistinctUntilChanged()
        .Subscribe(fun x -> value.OnNext x)
    |> disposable.Add

    value
        .DistinctUntilChanged()
        .Select(fun f -> f.ToString())
        .Subscribe(fun text ->
            if not textbox.IsFocused then
                textbox.Text <- text
        )
    |> disposable.Add

let bindingInt64Box
    (disposable: CompositeDisposable)
    (value: BehaviorSubject<int64>)
    (textbox: TextBox)
    =
    (textbox.LostFocus :?> IObservable<_>)
        .Select(fun _ -> textbox.Text)
        .Select(fun t -> Decimal.tryInt t)
        .Where(Option.isSome)
        .Select(Option.get)
        .DistinctUntilChanged()
        .Subscribe(fun x -> value.OnNext x)
    |> disposable.Add

    value
        .DistinctUntilChanged()
        .Select(fun f -> f.ToString())
        .Subscribe(fun text ->
            if not textbox.IsFocused then
                textbox.Text <- text
        )
    |> disposable.Add

let bindingTextBox
    (disposable: CompositeDisposable)
    (value: BehaviorSubject<string>)
    (textbox: TextBox)
    =
    (textbox.LostFocus :?> IObservable<_>)
        .Select(fun _ -> textbox.Text)
        .DistinctUntilChanged()
        .Subscribe(fun x -> value.OnNext x)
    |> disposable.Add

    value
        .DistinctUntilChanged()
        .Subscribe(fun text ->
            if not textbox.IsFocused then
                textbox.Text <- text
        )
    |> disposable.Add

/// 绑定到索引，因为Item和Value过于复杂可以从外部数组查询中解耦。
let bindingComboBox
    (disposable: CompositeDisposable)
    (index: BehaviorSubject<int>)
    (comboBox: ComboBox)
    =

    (comboBox.SelectionChanged :?> IObservable<_>)
        .Select(fun _ -> comboBox.SelectedIndex)
        .DistinctUntilChanged()
        .Subscribe(fun i -> index.OnNext i)
    |> disposable.Add

    index
        .DistinctUntilChanged()
        .Subscribe(fun i ->
            if
                i >= 0
                && i < comboBox.Items.Count
                && comboBox.SelectedIndex <> i
            then
                comboBox.SelectedIndex <- i
            elif i = -1 && comboBox.SelectedIndex <> -1 then
                comboBox.SelectedIndex <- -1
        )
    |> disposable.Add

// 通用实现
let bindingToggleButton
    (disposable: CompositeDisposable)
    (value: BehaviorSubject<bool>)
    (control: #System.Windows.Controls.Primitives.ToggleButton)
    =
    control.IsThreeState <- false
    let c = (control.Checked :?> IObservable<_>).Select(fun _ -> true)
    let u = (control.Unchecked :?> IObservable<_>).Select(fun _ -> false)
    c.Merge(u).Subscribe(fun ck -> value.OnNext ck)
    |> disposable.Add

    value
        .DistinctUntilChanged()
        .Subscribe(fun x ->
            if
                control.IsChecked.HasValue
                && control.IsChecked.Value <> x
            then
                control.IsChecked <- Nullable(x)
        )
    |> disposable.Add

let bindingCheckBox
    (disposable: CompositeDisposable)
    (value: BehaviorSubject<bool>)
    (checkbox: CheckBox)
    =
    bindingToggleButton disposable value checkbox

let bindingRadioButton
    (disposable: CompositeDisposable)
    (value: BehaviorSubject<bool>)
    (radioButton: RadioButton)
    =
    bindingToggleButton disposable value radioButton

let bindingRun (disposable: CompositeDisposable) (run: Run) (data: IObservable<'t>) =
    data
        .DistinctUntilChanged()
        .Select(sprintf "%A")
        .Subscribe((fun s -> run.Text <- s), (fun (ex: exn) -> run.Text <- ex.Message))
    |> disposable.Add

/// 绑定到Item
let bindingComboBoxItem
    (disposable: CompositeDisposable)
    (item: BehaviorSubject<'t>)
    (comboBox: ComboBox)
    =

    (comboBox.SelectionChanged :?> IObservable<_>)
        .Select(fun _ ->
            match comboBox.SelectedItem with
            | :? 't as i -> Some i
            | _ -> None
        )
        .Where(Option.isSome)
        .Select(Option.get)
        .DistinctUntilChanged()
        .Subscribe(fun i -> item.OnNext i)
    |> disposable.Add

    item
        .DistinctUntilChanged()
        .Subscribe(fun newValue ->
            let currentValue =
                match comboBox.SelectedItem with
                | :? 't as value -> value
                | _ -> Unchecked.defaultof<'t>

            if currentValue <> newValue then
                comboBox.SelectedItem <- newValue
        )
    |> disposable.Add

let bindingRadioButtonGroup
    (disposable: CompositeDisposable)
    (value: BehaviorSubject<int>)
    (radioButtons: RadioButton[])
    =

    let radioObservable =
        radioButtons
        |> Array.mapi(fun i radio -> (radio.Checked :?> IObservable<_>).Select(fun _ -> i))
        |> Observable.Merge

    radioObservable.Subscribe(value) |> disposable.Add

    value
        .DistinctUntilChanged()
        .Subscribe(fun i ->
            if i >= 0 && i < radioButtons.Length then
                let radio = radioButtons.[i]
                if radio.IsChecked <> Nullable(true) then
                    radio.IsChecked <- Nullable(true)
            else
                radioButtons
                |> Array.iter(fun radio ->
                    if radio.IsChecked <> Nullable(false) then
                        radio.IsChecked <- Nullable(false)
                )
        )
    |> disposable.Add
