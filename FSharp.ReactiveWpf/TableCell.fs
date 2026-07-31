module FSharp.ReactiveWpf.TableCell

open System.Windows
open System.Windows.Documents

/// cell.TextAlignment <- TextAlignment.Right
let TextAlignment_Right (cell : TableCell) =
    cell.TextAlignment <- TextAlignment.Right
    cell


