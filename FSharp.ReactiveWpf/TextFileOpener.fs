namespace FSharp.ReactiveWpf

open System
open System.IO
open System.Text
open Microsoft.Win32
open System.Windows

/// 文件打开器（用于读取文本文件）
type TextFileOpener(defaultExt: string, filter: string, encoding: Encoding) =
    let openDialog = OpenFileDialog()
    do
        openDialog.Filter <- filter
        openDialog.DefaultExt <- defaultExt

    /// 打开文件对话框，读取用户选择的文件内容。
    member _.Open() =
        if openDialog.ShowDialog() = Nullable true then
            File.ReadAllText(openDialog.FileName, encoding)
            |> Some
        else
            None

    /// 默认 JSON/文本文件配置的静态工厂
    static member openJson() =
        TextFileOpener(
            ".json",
            "JSON 文件 (*.json)|*.json|文本文件 (*.txt)|*.txt",
            UTF8Encoding(true) // 带 BOM 读取，无影响（会自动跳过 BOM）
        )

    /// 便捷构造（使用默认 UTF8 编码）
    new(defaultExt, filter) = TextFileOpener(defaultExt, filter, UTF8Encoding(true))
