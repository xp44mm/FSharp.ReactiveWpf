module PipeSections.MainWindow

open System
open System.IO
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
    let openFileButton =
        window.FindName("openFileButton") :?> Button
    let saveFileButton =
        window.FindName("saveFileButton") :?> Button
    let addButton = window.FindName("addButton") :?> Button
    let disposable = new CompositeDisposable()

    let vm = MainViewModel.empty()

    let disposables =
        Dictionary<PipeSectionViewModel, CompositeDisposable>()
    let bind (disposable: CompositeDisposable) (item: PipeSectionViewModel) =
        disposables.Add(item, disposable)
        Bindings.Binding.bindPipeSection disposable item

    // 解绑：释放该项目的绑定句柄，并从字典中移除
    let unbind (item: PipeSectionViewModel) =
        match disposables.TryGetValue(item) with
        | true, disp ->
            disp.Dispose()
            disposables.Remove(item) |> ignore
        | _ -> ()

    // 订阅数组变更：同步更新界面行（表头占索引 0，数据行 i 位于 tbl.Children[i+1]）
    vm.items.Changes.Subscribe(fun change ->
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

    let dialog_Filter = "JSON 文件 (*.json)|*.json|文本文件 (*.txt)|*.txt"
    let dialog_DefaultExt = ".json"

    // “打开”按钮：从 JSON 文件加载数据（先清空现有项目）
    (openFileButton.Click :?> IObservable<_>)
        .Subscribe(fun _ ->
            let dialog = Microsoft.Win32.OpenFileDialog()
            dialog.Filter <- dialog_Filter
            dialog.DefaultExt <- dialog_DefaultExt

            if dialog.ShowDialog() = Nullable true then
                try
                    let text =
                        File.ReadAllText(dialog.FileName, System.Text.Encoding.UTF8)
                    let json = FSharp.RfcJson.JsonCompiler.compile text

                    // 清空现有项目后加载
                    while vm.items.Count > 0 do
                        vm.removeAt unbind 0
                    vm.fromJson bind json

                with ex ->
                    printfn $"❌ 打开失败: {ex.Message}"
        )
    |> disposable.Add

    // “保存”按钮：将当前数据写入 JSON 文件
    (saveFileButton.Click :?> IObservable<_>)
        .Subscribe(fun _ ->
            let saveDialog = Microsoft.Win32.SaveFileDialog()
            saveDialog.Filter <- dialog_Filter
            saveDialog.DefaultExt <- dialog_DefaultExt

            if saveDialog.ShowDialog() = Nullable true then
                try
                    let json = vm.toJson()
                    let text = Json.print json

                    use writer =
                        new StreamWriter(
                            saveDialog.FileName,
                            false,
                            System.Text.Encoding.UTF8
                        )
                    writer.Write(text)
                    writer.Flush()

                    printfn $"✅ 文件已保存: {saveDialog.FileName}"

                with ex ->
                    printfn $"❌ 保存失败: {ex.Message}"
        )
    |> disposable.Add

    // “添加项目”按钮：在数组尾部追加一个管道
    (addButton.Click :?> IObservable<_>).Subscribe(fun _ -> vm.append bind 0.0 |> ignore)
    |> disposable.Add

    // 初始添加一行，便于查看效果
    vm.append bind 0.0 |> ignore

    window.Closing.Add(fun _ -> disposable.Dispose())
    window
