module FSharp.ReactiveWpf.Table

open System.Windows
open System.Windows.Documents
open System.Windows.Media

/// 添加列
let addColumn (col:TableColumn) (tbl: Table) =
    tbl.Columns.Add(col)
    tbl

/// 添加行组（显式分组）
let addRowGroup (rowGroup: TableRowGroup) (tbl: Table) =
    tbl.RowGroups.Add(rowGroup)
    tbl

/// 添加行（作为 TableRowGroup）
let addRow (row: TableRow) (tbl: Table) =
    if tbl.RowGroups.Count = 0 then
        tbl.RowGroups.Add(TableRowGroup())
    tbl.RowGroups.[tbl.RowGroups.Count - 1].Rows.Add(row)
    tbl

/// 设置单元格间距
let cellSpacing (value: double) (tbl: Table) =
    tbl.CellSpacing <- value
    tbl

/// 设置表格边框
let borderThickness (thickness: Thickness) (tbl: Table) =
    tbl.BorderThickness <- thickness
    tbl

let borderBrush (brush: Brush) (tbl: Table) =
    tbl.BorderBrush <- brush
    tbl
