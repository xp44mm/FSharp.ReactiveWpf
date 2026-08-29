module section.MainWindow

open System
open System.Windows.Input
open System.Windows.Controls
open System.Windows.Threading

open System.Reactive.Subjects
open System.Reactive.Disposables
open System.Reactive.Linq
open System.Threading
open System.Reflection

open MahApps.Metro.Controls

open FSharp.Idioms
open FSharp.ReactiveWpf
open System.Windows

open section

let comboBox (kind: ISubject<string>) =
    let comboBox = ComboBox()
    comboBox.Items.Add("rectangle") |> ignore
    comboBox.Items.Add("circle") |> ignore

    (comboBox.SelectionChanged :?> IObservable<_>)
        .Select(fun _ -> comboBox.SelectedItem :?> string)
        .DistinctUntilChanged()
        .Subscribe(kind)
    |> ignore

    kind
        .DistinctUntilChanged()
        .ObserveOn(SynchronizationContext.Current)
        .Subscribe(fun k -> comboBox.SelectedItem <- k)
    |> ignore

    comboBox

let shape_rows (disposable: CompositeDisposable) (model: ShapeViewModel) =
    let numberBox = NumberBox.createFloat disposable
    let textBlock = TextBlock.create disposable

    let rect_rows = [
        Row.fill(TextBlock(Text = "宽度"), 
            TextBlock(Text = "mm"),
            value = numberBox model.width)

        Row.fill(TextBlock(Text = "高度"), 
            TextBlock(Text = "mm"),
            value = numberBox model.height)
    ]

    let diameter_row =
        Row.fill(TextBlock(Text = "直径"), 
            TextBlock(Text = "mm"),
            value = numberBox model.diameter)

    model.kind
        .DistinctUntilChanged()
        .Subscribe(fun i ->
            match i with
            | "rectangle" ->
                for row in rect_rows do
                    row.Root.Visibility <- Visibility.Visible
                diameter_row.Root.Visibility <- Visibility.Collapsed

            | "circle" ->
                for row in rect_rows do
                    row.Root.Visibility <- Visibility.Collapsed
                diameter_row.Root.Visibility <- Visibility.Visible

            | _ -> failwith "never"

        )
    |> ignore

    [
        Row.fill(TextBlock(Text = "截面形状"),value = comboBox model.kind)
        yield! rect_rows
        diameter_row
    ]

/// 初始化主窗口并绑定事件
let createWindow () =
    let window = App.loadXaml "MainWindow.xaml" :?> MetroWindow

    let panel = window.FindName("tbl") :?> StackPanel
    let disposable = new CompositeDisposable()
    let section = SectionViewModel.create()
    Bindings.Binding.bind disposable section

    let textBlock = TextBlock.create disposable
    let textBlockForFloat (src:IObservable<float>) =
            src
            |> Observable.map(fun x -> x.ToString("0.##"))
            |> TextBlock.create disposable 

    let rows = [
        Row.fill(
            name = TextBlock(Text = "名称"),
            measure = TextBlock(Text = "单位"),
            value = TextBlock(Text = "数值"),
            spec = TextBlock(Text = "备注")
        )

        yield! shape_rows disposable section.shape
        
        Row.fill(
            TextBlock(Text = "截面积"),
            TextBlock(Text = "mm2"),
            textBlockForFloat section.shape.area
            |> TextBlock.textAlignment TextAlignment.Right
        )

        Row.fill(
            TextBlock(Text = "体积流量"),
            TextBlock(Text = "m3/hr"),
            textBlockForFloat section.volume
            |> TextBlock.textAlignment TextAlignment.Right

        )

        Row.fill(
            TextBlock(Text = "流速"),
            TextBlock(Text = "m/s"),
            textBlockForFloat section.velocity
            |> TextBlock.textAlignment TextAlignment.Right

        )

    ]

    for row in rows do
        panel.Children.Add(row.Root) |> ignore

    window
