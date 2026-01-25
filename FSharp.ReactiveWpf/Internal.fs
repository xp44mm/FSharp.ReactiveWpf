module FSharp.ReactiveWpf.Internal

open System
open System.Windows
open System.Reflection

let assy = Assembly.GetExecutingAssembly()

let loadXaml filename =
    "FSharp.ReactiveWpf." + filename
    |> XamlLoader.loadXaml assy


