module FSharp.ReactiveWpf.Table

open System.Windows
open System.Windows.Documents
open System.Windows.Media

/// 添加列
let addColumn (col:TableColumn) (p: Table) =
    p.Columns.Add(col)
    p

/// 添加行组（显式分组）
let addRowGroup (rowGroup: TableRowGroup) (p: Table) =
    p.RowGroups.Add(rowGroup)
    p

/// 添加行（作为 TableRowGroup）
let addRow (row: TableRow) (p: Table) =
    if p.RowGroups.Count = 0 then
        p.RowGroups.Add(TableRowGroup())
    p.RowGroups.[p.RowGroups.Count - 1].Rows.Add(row)
    p

/// 设置单元格间距
let cellSpacing (value: double) (p: Table) =
    p.CellSpacing <- value
    p

/// 设置表格边框
let borderThickness (thickness: Thickness) (p: Table) =
    p.BorderThickness <- thickness
    p

let borderBrush (brush: Brush) (p: Table) =
    p.BorderBrush <- brush
    p
