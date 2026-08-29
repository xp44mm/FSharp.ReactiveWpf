module section.App

open System
open System.Windows
open System.Reflection

open FSharp.ReactiveWpf

let assy = Assembly.GetExecutingAssembly()

let loadXaml filename =
    "section." + filename
    |> XamlLoader.loadXaml assy

let app = loadXaml "App.xaml" :?> Application

