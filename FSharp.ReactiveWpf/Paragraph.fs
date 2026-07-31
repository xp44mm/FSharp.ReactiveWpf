module FSharp.ReactiveWpf.Paragraph

open System.Windows
open System.Windows.Documents

/// p.TextAlignment <- TextAlignment.Right
let TextAlignment_Right (p : Paragraph) =
    p.TextAlignment <- TextAlignment.Right
    p


