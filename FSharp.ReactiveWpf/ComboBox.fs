/// ComboBox 与 Reactive Extensions 的绑定工具
/// 提供 ComboBox 与 Rx 主题之间的双向绑定功能
module FSharp.ReactiveWpf.ComboBox

open System
open System.Windows.Controls
open System.Reactive.Subjects
open System.Reactive.Linq
open System.Threading
open System.Reactive.Disposables

// ============================================
// 核心绑定函数
// ============================================

/// 绑定到索引（推荐使用，因为与外部数据源解耦）
let bindIndex
    (disposable: CompositeDisposable)
    (index: ISubject<int>)
    (comboBox: ComboBox)
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
    (item: ISubject<'t>)
    (comboBox: ComboBox)
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

// ============================================
// 工厂函数（创建已绑定的 ComboBox）
// ============================================

/// 创建带有预设项目列表并按索引绑定的 ComboBox
let comboBoxIndex 
    (items: seq<string>) 
    (index: ISubject<int>)
    =
    let comboBox = ComboBox()
    
    for item in items do
        comboBox.Items.Add(item) |> ignore

    let disposable = new CompositeDisposable()
    bindIndex disposable index comboBox
    
    comboBox.Unloaded.Add(fun _ -> disposable.Dispose())
    comboBox

/// 创建带有预设项目列表并按项目值绑定的 ComboBox
let comboBoxItem 
    (items: seq<string>) 
    (item: ISubject<string>)
    =
    let comboBox = ComboBox()
    
    for itemText in items do
        comboBox.Items.Add(itemText) |> ignore

    // 简化版绑定（不管理 disposable）
    (comboBox.SelectionChanged :?> IObservable<_>)
        .Select(fun _ -> comboBox.SelectedItem :?> string)
        .DistinctUntilChanged()
        .Subscribe(item)
    |> ignore

    item
        .DistinctUntilChanged()
        .ObserveOn(SynchronizationContext.Current)
        .Subscribe(fun newValue -> 
            comboBox.SelectedItem <- newValue
        )
    |> ignore

    comboBox

