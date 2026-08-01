module FSharp.ReactiveWpf.TableCell

open System.Windows
open System.Windows.Documents
open System.Windows.Media

/// cell.TextAlignment <- value
let alignment (value:TextAlignment) (cell : TableCell) =
    cell.TextAlignment <- value
    cell

/// cell.Blocks.Add item
let addBlock (item: Block) (cell: TableCell) =
    cell.Blocks.Add item
    cell

/// cell.ColumnSpan <- value
let columnSpan (value: int) (cell: TableCell) =
    cell.ColumnSpan <- value
    cell

/// cell.RowSpan <- value
let rowSpan (value: int) (cell: TableCell) =
    cell.RowSpan <- value
    cell

/// cell.Background <- brush
let background (brush: Brush) (cell: TableCell) =
    cell.Background <- brush
    cell

let borderThickness (thickness: Thickness) (cell: TableCell) =
    cell.BorderThickness <- thickness
    cell

let borderBrush (brush: Brush) (cell: TableCell) =
    cell.BorderBrush <- brush
    cell
