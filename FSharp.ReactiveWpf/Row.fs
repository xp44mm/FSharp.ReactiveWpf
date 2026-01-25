namespace FSharp.ReactiveWpf

open System.Windows.Controls
open System.Windows

type Row = {
    Root: DockPanel
    Name: Border
    Measure: Border
    Value: Border
    Spec: Border
} with

    static member empty() =
        let panel = Internal.loadXaml "Row.xaml" :?> DockPanel
        {
            Root = panel
            Name = panel.FindName("name") :?> Border
            Measure = panel.FindName("measure") :?> Border
            Value = panel.FindName("value") :?> Border
            Spec = panel.FindName("spec") :?> Border
        }

    static member fill(?name: UIElement, ?measure: UIElement, ?value: UIElement, ?spec: UIElement) =
        let this = Row.empty()

        match name with
        | Some n -> this.Name.Child <- n
        | None -> ()

        match measure with
        | Some m -> this.Measure.Child <- m
        | None -> ()

        match value with
        | Some v -> this.Value.Child <- v
        | None -> ()

        match spec with
        | Some s -> this.Spec.Child <- s
        | None -> ()

        this
