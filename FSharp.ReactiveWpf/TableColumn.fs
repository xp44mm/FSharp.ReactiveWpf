module FSharp.ReactiveWpf.TableColumn

open System.Windows
open System.Windows.Documents
open System.Windows.Media

/// col.Width <- value
let width (value: GridLength) (col: TableColumn) =
    col.Width <- value
    col

/// col.Width <- GridLength(value)
let widthPixels (value: double) (col: TableColumn) =
    col.Width <- GridLength(value)
    col

/// col.Width <- GridLength(value, GridUnitType.Star)
let widthStar (value: double) (col: TableColumn) =
    col.Width <- GridLength(value, GridUnitType.Star)
    col

/// col.Width <- GridLength.Auto
let widthAuto (col: TableColumn) =
    col.Width <- GridLength.Auto
    col

/// col.Background <- brush
let background (brush: Brush) (col: TableColumn) =
    col.Background <- brush
    col
/// tbl.Columns.Add(col)
let appendTo (tbl: Table) (col: TableColumn) = tbl.Columns.Add(col)
