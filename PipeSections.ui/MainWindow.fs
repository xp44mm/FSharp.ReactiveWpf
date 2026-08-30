module PipeSections.MainWindow

open System
open System.IO

open System.Windows
open System.Windows.Controls

open System.Reactive.Disposables

open MahApps.Metro.Controls

open FSharp.Idioms

let jsonFilter = "JSON 文件 (*.json)|*.json|文本文件 (*.txt)|*.txt"

/// 加载并配置主窗口
let createWindow () =
    let window = App.loadXaml "MainWindow.xaml" :?> MetroWindow
    let rows = window.FindName("rows") :?> StackPanel
    let openFileButton =
        window.FindName("openFileButton") :?> Button
    let saveFileButton =
        window.FindName("saveFileButton") :?> Button
    let addTopButton = window.FindName("addTopButton") :?> Button
    let disposable = new CompositeDisposable()

    let vm = MainViewModel.empty()

    // 订阅数组变更：同步更新界面行（rows 面板只包含数据行，索引直接对应数组索引）
    // 行元素以 Tag 挂接其数据项，供按引用删除（Remove）时定位
    vm.items.Changes
    |> Observable.subscribe(fun change ->
        match change with
        | UIElementCollectionChange.Add item ->
            let row = Row.create disposable vm item
            row.Tag <- item
            rows.Children.Add(row) |> ignore

        | UIElementCollectionChange.AddRange items ->
            for item in items do
                let row = Row.create disposable vm item
                row.Tag <- item
                rows.Children.Add(row) |> ignore

        | UIElementCollectionChange.Insert(index, item) ->
            let row = Row.create disposable vm item
            row.Tag <- item
            rows.Children.Insert(index, row)

        | UIElementCollectionChange.RemoveAt(index) -> 
            rows.Children.RemoveAt(index)

        | UIElementCollectionChange.RemoveRange(index, count) ->
            rows.Children.RemoveRange(index, count)

        | UIElementCollectionChange.Clear -> rows.Children.Clear()
    )
    |> disposable.Add

    // 表头“添加”按钮：在 0 位置插入一个管道
    (addTopButton.Click :?> IObservable<_>)
    |> Observable.subscribe(fun _ ->
        let item = PipeSectionViewModel.create(0.0)
        Bindings.Collection.insert vm 0 item
        |> ignore
    )
    |> disposable.Add

    // “打开”按钮：从 JSON 文件加载数据（先清空现有项目）
    (openFileButton.Click :?> IObservable<_>)
        .Subscribe(fun _ ->
            let dialog = Microsoft.Win32.OpenFileDialog()
            dialog.Filter <- jsonFilter
            dialog.DefaultExt <- ".json"

            if dialog.ShowDialog() = Nullable true then
                try
                    let text =
                        File.ReadAllText(dialog.FileName, System.Text.Encoding.UTF8)
                    let json = FSharp.RfcJson.JsonCompiler.compile text

                    // 清空现有项目后加载（Clear 一次性通知并释放全部绑定句柄）
                    vm.items.Clear()

                    let bind
                        (disposable: CompositeDisposable)
                        (item: PipeSectionViewModel)
                        =
                        Bindings.Binding.bindPipeSection disposable item

                    vm.fromJson bind json

                with ex ->
                    printfn $"❌ 打开失败: {ex.Message}"
        )
    |> disposable.Add

    // “保存”按钮：将当前数据写入 JSON 文件
    (saveFileButton.Click :?> IObservable<_>)
        .Subscribe(fun _ ->
            let saveDialog = Microsoft.Win32.SaveFileDialog()
            saveDialog.Filter <- jsonFilter
            saveDialog.DefaultExt <- ".json"

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

    // 初始添加一行，便于查看效果
    let initialItem = PipeSectionViewModel.create(0.0)
    Bindings.Collection.insert vm 0 initialItem
    |> ignore

    window.Closing.Add(fun _ -> disposable.Dispose())
    window
