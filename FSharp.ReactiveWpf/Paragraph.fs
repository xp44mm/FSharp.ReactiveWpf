module FSharp.ReactiveWpf.Paragraph

open System.Windows
open System.Windows.Documents

/// p.TextAlignment <- value
let alignment (value: TextAlignment) (p: Paragraph) =
    p.TextAlignment <- value
    p

/// p.Inlines.Add item
let add (item: Inline) (p: Paragraph) =
    p.Inlines.Add item
    p
