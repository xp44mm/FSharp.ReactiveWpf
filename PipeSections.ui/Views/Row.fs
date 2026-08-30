module PipeSections.Row

open System
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


/// 创建一行：直径、周长、截面积、操作（插入/删除）
/// 展开原 onInsert/onDelete 逻辑，改为接收主视图模型 mainvm 后在本行内直接处理集合操作
let create
    (disposable: CompositeDisposable)
    (mainvm: MainViewModel)
    (itemvm: PipeSectionViewModel)
    =
    // 四列单元格，与表头等宽
    let cell (width: float) (child: UIElement) =
        let border =
            Border(
                Width = width,
                Padding = Thickness(5.0),
                BorderBrush = Brushes.Gray,
                BorderThickness = Thickness(1.0)
            )
        border.Child <- child
        border

    let numberBox = NumberBox.createFloat disposable
    let textBlock = TextBlock.create disposable

    // 操作列：插入、删除按钮
    let insertButton =
        Button(
            Content = "添加",
            Padding = Thickness(4.0, 1.0, 4.0, 1.0),
            Margin = Thickness(0.0, 0.0, 3.0, 0.0)
        )

    // 添加：在本行后插入一个新管道项目
    (insertButton.Click :?> IObservable<_>)
    |> Observable.subscribe(fun _ ->
        let i = mainvm.items.IndexOf(itemvm)
        if i >= 0 then
            let next = i + 1
            let newItem = PipeSectionViewModel.create(0.0)
            Bindings.Collection.insert mainvm next newItem
            |> ignore
    )
    |> disposable.Add

    // 删除：移除本行对应的管道项目
    let deleteButton =
        Button(Content = "删除", Padding = Thickness(4.0, 1.0, 4.0, 1.0))

    (deleteButton.Click :?> IObservable<_>)
    |> Observable.subscribe(fun _ ->
        let i = mainvm.items.IndexOf(itemvm)
        if i >= 0 then
            mainvm.items.RemoveAt i
    )
    |> disposable.Add

    let op =
        StackPanel(
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Center
        )
    op.Children.Add(insertButton) |> ignore
    op.Children.Add(deleteButton) |> ignore

    // 行号单元格：与表头同色（深底白字），由 renumber 按显示顺序刷新（从 1 开始）
    let numberTb =
        TextBlock(
            TextAlignment = TextAlignment.Center,
            Foreground = Brushes.White
        )
    let numberBorder =
        Border(
            Width = 40.0,
            Padding = Thickness(5.0),
            Background = SolidColorBrush(Color.FromRgb(0x4Cuy, 0x4Cuy, 0x4Cuy)),
            BorderBrush = Brushes.Gray,
            BorderThickness = Thickness(1.0)
        )
    numberBorder.Child <- numberTb

    // 直径列可编辑，周长、截面积自动计算显示
    let row = StackPanel(Orientation = Orientation.Horizontal)

    row.Children.Add(numberBorder)
    |> ignore

    row.Children.Add(cell 150.0 (numberBox itemvm.diameter))
    |> ignore

    row.Children.Add(
        cell 80.0 (textBlock(itemvm.circumference.Select(fun x -> x.ToString("0.##"))))
    )
    |> ignore

    row.Children.Add(
        cell 230.0 (textBlock(itemvm.area.Select(fun x -> x.ToString("0.##"))))
    )
    |> ignore

    row.Children.Add(cell 160.0 op)
    |> ignore
    row

/// 按显示顺序刷新每行的行号（行首单元格为行号，从 1 开始）
let renumber (rows: StackPanel) =
    rows.Children
    |> Seq.cast<UIElement>
    |> Seq.iteri(fun i child ->
        match child with
        | :? StackPanel as row when row.Children.Count > 0 ->
            match row.Children.[0] with
            | :? Border as border ->
                match border.Child with
                | :? TextBlock as tb -> tb.Text <- string (i + 1)
                | _ -> ()
            | _ -> ()
        | _ -> ()
    )
