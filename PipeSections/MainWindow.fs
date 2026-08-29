module PipeSections.MainWindow

open System
open System.Collections.Generic

open System.Windows
open System.Windows.Controls
open System.Windows.Media
open System.Windows.Shapes

open System.Reactive.Disposables
open System.Reactive.Linq
open System.Reactive.Subjects

open MahApps.Metro.Controls

open FSharp.Idioms
open FSharp.ReactiveWpf

/// 加载并配置主窗口
let createWindow () =
    let window = App.loadXaml "MainWindow.xaml" :?> MetroWindow
    let tbl = window.FindName("tbl") :?> StackPanel
    let addButton = window.FindName("addButton") :?> Button
    let disposable = new CompositeDisposable()

    let vm = MainViewModel.empty()

    let disposables = Dictionary<PipeSectionViewModel ,CompositeDisposable>()
    let bind (disposable: CompositeDisposable) (item:PipeSectionViewModel) =
        disposables.Add(item,disposable)
        Bindings.Binding.bindPipeSection disposable item

    // 解绑：释放该项目的绑定句柄，并从字典中移除
    let unbind (item:PipeSectionViewModel) =
        match disposables.TryGetValue(item) with
        | true, disp ->
            disp.Dispose()
            disposables.Remove(item) |> ignore
        | _ -> ()

    // 订阅数组变更：同步更新界面行（表头占索引 0，数据行 i 位于 tbl.Children[i+1]）
    vm.items.Changes
        .Subscribe(fun change ->
            match change with
            | PipeSections.CollectionChange.Added(index, item) ->
                let onInsert () =
                    let i = vm.items.IndexOf(item)
                    if i >= 0 then
                        vm.insertBefore bind i 0.0 |> ignore
                let onDelete () =
                    let i = vm.items.IndexOf(item)
                    if i >= 0 then
                        vm.removeAt unbind i
                let row = Row.create disposable onInsert onDelete item
                tbl.Children.Insert(index + 1, row)

            | PipeSections.CollectionChange.Removed(index, _) ->
                tbl.Children.RemoveAt(index + 1)

            | _ -> ()
        )
    |> disposable.Add

    // “添加项目”按钮：在数组尾部追加一个管道
    (addButton.Click :?> IObservable<_>)
        .Subscribe(fun _ ->
            vm.append bind 0.0 |> ignore
        )
    |> disposable.Add

    // 初始添加一行，便于查看效果
    vm.append bind 0.0 |> ignore

    window.Closing.Add(fun _ -> disposable.Dispose())
    window
