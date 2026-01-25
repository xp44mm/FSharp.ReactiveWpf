module FSharp.ReactiveWpf.ComboBox

open System
open System.Windows.Controls

open System.Reactive.Subjects
open System.Reactive.Linq

open System.Threading

open FSharp.Idioms
open System.Reactive.Disposables




/// 绑定到索引，因为Item和Value过于复杂可以从外部数组查询中解耦
let bindingComboBox
    (disposable: CompositeDisposable)
    (index: ISubject<int>)
    (comboBox: ComboBox)
    =

    (comboBox.SelectionChanged :?> IObservable<_>)
        .Select(fun _ -> comboBox.SelectedIndex)
        .DistinctUntilChanged()
        .Subscribe(index)
    |> disposable.Add

    index
        .DistinctUntilChanged()
        .ObserveOn(SynchronizationContext.Current)
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


/// 绑定到Item
let bindingComboBoxItem
    (disposable: CompositeDisposable)
    (item: ISubject<'t>)
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
        .Subscribe(item)
    |> disposable.Add

    item
        .DistinctUntilChanged()
        .ObserveOn(SynchronizationContext.Current)
        .Subscribe(fun newValue ->
            let currentValue =
                match comboBox.SelectedItem with
                | :? 't as value -> value
                | _ -> Unchecked.defaultof<'t>

            if currentValue <> newValue then
                comboBox.SelectedItem <- newValue
        )
    |> disposable.Add


/// 
let comboBoxIndex (items: _) (index: ISubject<int>) =
    let comboBox = ComboBox()
    for (item: string) in items do
        comboBox.Items.Add(item) |> ignore

    let disposable = new CompositeDisposable()
    WpfSubscriber.bindingComboBox disposable index comboBox
    comboBox.Unloaded.Add(fun _ -> disposable.Dispose())
    comboBox

let comboBoxItem (items: _) (item: ISubject<string>) =
    let comboBox = ComboBox()
    for (item: string) in items do
        comboBox.Items.Add(item) |> ignore

    (comboBox.SelectionChanged :?> IObservable<_>)
        .Select(fun _ -> comboBox.SelectedItem :?> string)
        .DistinctUntilChanged()
        .Subscribe(item)
    |> ignore

    item
        .DistinctUntilChanged()
        .ObserveOn(SynchronizationContext.Current)
        .Subscribe(fun k -> comboBox.SelectedItem <- k)
    |> ignore

    comboBox
