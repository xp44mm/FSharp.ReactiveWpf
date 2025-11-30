module FSharp.ReactiveWpf.XamlLoader

open System.IO
open System.Reflection
open System.Windows.Markup
open System.Xml


/// 从嵌入式资源加载 XAML 并解析为对象
let loadXaml (assy: Assembly) (name: string) =
    let str = FSharp.Idioms.String.fromEmbedded assy name
    use sr = new StringReader(str)
    let xr = XmlReader.Create(sr)
    XamlReader.Load(xr)
