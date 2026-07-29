/// ComboBox 与 Reactive Extensions 的绑定工具
/// 提供 ComboBox 与 Rx 主题之间的双向绑定功能
module FSharp.ReactiveWpf.ComboBox

open System
open System.Windows.Controls
open System.Reactive.Subjects
open System.Reactive.Linq
open System.Threading
open System.Reactive.Disposables

/// 绑定到索引
let bindIndex
    (disposable: CompositeDisposable)
    (comboBox: ComboBox)
    (index: ISubject<int>)
    =
    // ComboBox -> Subject
    (comboBox.SelectionChanged :?> IObservable<_>)
        .Select(fun _ -> comboBox.SelectedIndex)
        .DistinctUntilChanged()
        .Subscribe(index)
    |> disposable.Add

    // Subject -> ComboBox
    index
        .DistinctUntilChanged()
        .ObserveOn(SynchronizationContext.Current)
        .Subscribe(fun i ->
            if i >= 0 && i < comboBox.Items.Count && comboBox.SelectedIndex <> i then
                comboBox.SelectedIndex <- i
            elif i = -1 && comboBox.SelectedIndex <> -1 then
                comboBox.SelectedIndex <- -1
        )
    |> disposable.Add

/// 绑定到具体的项目值
let bindItem
    (disposable: CompositeDisposable)
    (comboBox: ComboBox)
    (item: ISubject<'t>)
    =
    // ComboBox -> Subject
    (comboBox.SelectionChanged :?> IObservable<_>)
        .Select(fun _ ->
            match comboBox.SelectedItem with
            | :? 't as selectedItem -> Some selectedItem
            | _ -> None
        )
        .Where(Option.isSome)
        .Select(Option.get)
        .DistinctUntilChanged()
        .Subscribe(item)
    |> disposable.Add

    // Subject -> ComboBox
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

/// 创建带有预设项目列表并按索引绑定的 ComboBox
let indexCreate
    (disposable: CompositeDisposable)
    (items: #seq<string>) 
    (index: ISubject<int>)
    =
    let comboBox = ComboBox()
    for item in items do
        comboBox.Items.Add(item) |> ignore
    bindIndex disposable comboBox index
    comboBox

/// 创建带有预设项目列表并按项目值绑定的 ComboBox
let itemCreate
    (disposable: CompositeDisposable)
    (items: #seq<string>) 
    (item: ISubject<string>)
    =
    let comboBox = ComboBox()
    for itemText in items do
        comboBox.Items.Add(itemText) |> ignore
    bindItem disposable comboBox item
    comboBox

