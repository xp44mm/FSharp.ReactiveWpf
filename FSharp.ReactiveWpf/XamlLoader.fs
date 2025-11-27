module FSharp.ReactiveWpf.XamlLoader

open System
open System.IO
open System.Xml
open System.Windows
open System.Windows.Markup
open System.Reflection

open FSharp.Idioms

/// 从嵌入式资源加载 XAML 并解析为对象
let loadXaml (assy: Assembly) (name: string) =
    //use stream = assy.GetManifestResourceStream(name)
    //if isNull stream then
    //    failwith $"Resource '{name}' not found in assembly '{assy.GetName()}'."
    //use sr = new StreamReader(stream)
    //sr.ReadToEnd()

    let str = FSharp.Idioms.String.fromEmbedded assy name
    use sr = new StringReader(str)
    let xr = XmlReader.Create(sr)
    XamlReader.Load(xr)
