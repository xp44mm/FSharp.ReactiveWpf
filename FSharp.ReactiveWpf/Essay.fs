module FSharp.ReactiveWpf.Essay

open System
open System.Diagnostics
open System.IO
open System.Text

open System.Windows
open System.Windows.Controls
open System.Windows.Documents
open System.Windows.Media

/// 计算Run在段落中的字符偏移量（前面所有Run的总长度）
let getRunOffsetInParagraph (paragraph: Paragraph) (run: Run) =
    paragraph.Inlines
    |> Seq.cast<Inline>
    |> Seq.choose(fun inlineObj ->
        match inlineObj with
        | :? Run as run -> Some run
        | _ -> None
    )
    |> Seq.takeWhile(fun u -> not(Object.ReferenceEquals(u, run)))
    |> Seq.sumBy(fun run -> run.Text.Length)

let baseScrollToCurrent (richTextBox: RichTextBox) (targetPosition: TextPointer) =
    let charRect = targetPosition.GetCharacterRect(LogicalDirection.Forward)

    if charRect.Bottom > richTextBox.ViewportHeight then
        let targetOffset =
            richTextBox.VerticalOffset
            + charRect.Height * 2.0
        let maxOffset =
            richTextBox.ExtentHeight
            - richTextBox.ViewportHeight
            |> max 0.0

        if targetOffset >= maxOffset then
            richTextBox.ScrollToEnd()
        else
            richTextBox.ScrollToVerticalOffset(targetOffset)
