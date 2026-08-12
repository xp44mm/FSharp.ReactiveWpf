module FSharp.ReactiveWpf.TableRow

open System.Windows.Documents

/// 添加单元格
let addCell (cell: TableCell) (row: TableRow) =
    row.Cells.Add(cell)
    row

/// 添加多个单元格
let addCells (cells: TableCell seq) (row: TableRow) =
    for c in cells do
        row.Cells.Add c
    row

let create (cells: TableCell seq) =
    let row = TableRow()
    for c in cells do
        row.Cells.Add c
    row
