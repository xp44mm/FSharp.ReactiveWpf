module FSharp.ReactiveWpf.TableColumn

open System.Windows
open System.Windows.Documents
open System.Windows.Media

/// 设置列宽
let width (value: GridLength) (col: TableColumn) =
    col.Width <- value
    col

/// 设置列宽（像素值）
let widthPixels (value: double) (col: TableColumn) =
    col.Width <- GridLength(value)
    col

/// 设置列宽（星号比例）
let widthStar (value: double) (col: TableColumn) =
    col.Width <- GridLength(value, GridUnitType.Star)
    col

/// 设置列宽（自动）
let widthAuto (col: TableColumn) =
    col.Width <- GridLength.Auto
    col

/// 设置列背景
let background (brush: Brush) (col: TableColumn) =
    col.Background <- brush
    col
