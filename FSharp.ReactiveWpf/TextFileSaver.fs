namespace FSharp.ReactiveWpf

open System
open System.IO
open System.Text
open Microsoft.Win32
open System.Windows

/// 文件保存器
type TextFileSaver(defaultExt: string, filter: string, encoding: Encoding) =
    let saveDialog = SaveFileDialog()
    
    do
        saveDialog.Filter <- filter
        saveDialog.DefaultExt <- defaultExt

    member _.Save(text: string) =
        if saveDialog.ShowDialog() = Nullable true then
            try
                use writer =
                    new StreamWriter(saveDialog.FileName, false, encoding)
                writer.Write(text)
                writer.Flush()

                MessageBox.Show(
                    sprintf "保存成功: %s" saveDialog.FileName,
                    "提示",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                )
                |> ignore

            with ex ->
                MessageBox.Show(
                    sprintf "保存失败: %s" ex.Message,
                    "提示",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                )
                |> ignore

    static member jsonSaver() =
        TextFileSaver(
            ".json",
            "JSON 文件 (*.json)|*.json|文本文件 (*.txt)|*.txt",
            UTF8Encoding(true)
        )

    new(defaultExt, filter) = TextFileSaver(defaultExt, filter, UTF8Encoding(true))

