module FSharp.ReactiveWpf.TableRowGroup

open System.Windows
open System.Windows.Documents
open System.Windows.Media

/// rg.Rows.Add(row)
let addRow (row: TableRow) (rg: TableRowGroup) =
    rg.Rows.Add(row)
    rg

/// 添加多行
let addRows (rows: TableRow list) (rg: TableRowGroup) =
    rows |> List.iter (rg.Rows.Add)
    rg

/// rg.Background <- brush
let background (brush: Brush) (rg: TableRowGroup) =
    rg.Background <- brush
    rg

/// rg.FontSize <- value
let fontSize (value: double) (rg: TableRowGroup) =
    rg.FontSize <- value
    rg

/// rg.FontFamily <- value
let fontFamily (value: FontFamily) (rg: TableRowGroup) =
    rg.FontFamily <- value
    rg

/// rg.FontWeight <- value
let fontWeight (value: FontWeight) (rg: TableRowGroup) =
    rg.FontWeight <- value
    rg

/// rg.FontStyle <- value
let fontStyle (value: FontStyle) (rg: TableRowGroup) =
    rg.FontStyle <- value
    rg

/// rg.Foreground <- brush
let foreground (brush: Brush) (rg: TableRowGroup) =
    rg.Foreground <- brush
    rg
///// tbl.RowGroups.Add(rg)
//let appendTo (tbl: Table) (rg: TableRowGroup) = tbl.RowGroups.Add(rg)
