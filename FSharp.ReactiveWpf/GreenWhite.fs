module FSharp.ReactiveWpf.GreenWhite

open System
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

/// 显示带有换行格式的文本
let initialArticle (flowDoc: FlowDocument) (content: string) =
    flowDoc.Blocks.Clear()
    //Margin = Thickness(0.0, 2.0, 0.0, 12.0),
    let paragraph = Paragraph(TextAlignment = TextAlignment.Left)
    paragraph.Inlines.Add((Run(content)))
    flowDoc.Blocks.Add(paragraph)

/// 根据字符索引c生成绿白段落
let updateParagraph (paragraph: Paragraph) (text: string) (charIndex: int) =
    let pastText = text.Substring(0, charIndex)
    let futureText = text.Substring(charIndex)

    paragraph.Inlines.Clear()
    paragraph.Inlines.Add(Run(Foreground = Brushes.Lime, Text = pastText))
    paragraph.Inlines.Add(Run(futureText))


///白绿交界处的位置
let getPos (paragraph: Paragraph) (charIdx: int) =
    if
        paragraph.Inlines.Count = 1
        && charIdx = 0
    then
        paragraph.ContentStart
    elif
        paragraph.Inlines.Count = 1
        && charIdx > 0
    then
        paragraph.ContentEnd
    else
        let whiteRun = paragraph.Inlines |> Seq.item 1 :?> Run
        whiteRun.ContentStart


let scrollToCurrent (richTextBox: RichTextBox) (charIdx: int) =
    try
        let targetPosition =
            let paragraph = richTextBox.Document.Blocks |> Seq.head :?> Paragraph
            getPos paragraph charIdx
        baseScrollToCurrent richTextBox targetPosition
    with _ ->
        ()
