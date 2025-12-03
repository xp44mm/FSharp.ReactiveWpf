module FSharp.ReactiveWpf.MediaPlayer

open System
open System.IO
open System.Windows.Media
open System.Reactive.Linq

let playMany (mediaPlayer: MediaPlayer) (ls: string[]) : IDisposable =
    (mediaPlayer.MediaEnded :?> IObservable<_>)
        .Select(fun _ -> 1)
        .StartWith(0)
        .Zip(ls.ToObservable(), fun _ e -> e)
        .Where(File.Exists)
        .Subscribe(fun path ->
            mediaPlayer.Open(Uri(path))
            mediaPlayer.Play()
        )

let createPlaylistObservable (mediaPlayer: MediaPlayer) (subject: IObservable<#seq<string>>) =
    subject
        .Select(fun ls ->
            let ls = ls |> Seq.filter(File.Exists)

            if Seq.isEmpty ls then
                Observable.Empty()
            else
                Observable
                    .Merge(
                        (mediaPlayer.MediaEnded :?> IObservable<_>).Select(fun _ -> 1),
                        (mediaPlayer.MediaFailed :?> IObservable<_>).Select(fun _ -> 1)
                    )
                    .StartWith(0)
                    .Zip(ls.ToObservable(), fun _ path -> path)
                    .Do(fun path ->
                        mediaPlayer.Open(Uri(path))
                        mediaPlayer.Play()
                    )
                    .IgnoreElements()
        )
        .Switch()

let createPlaylistObservable2
    (writeLine: string -> unit)
    (mediaPlayer: MediaPlayer)
    (subject: IObservable<#seq<string>>)
    =
    subject
        .Do(fun ls -> sprintf "收到播放列表: %A" ls |> writeLine)
        .Select(fun ls ->
            let ls = ls |> Seq.filter(File.Exists)
            sprintf "过滤后文件: %A" ls |> writeLine

            if Seq.isEmpty ls then
                writeLine "文件列表为空"
                Observable.Empty()
            else
                Observable
                    .Merge(
                        (mediaPlayer.MediaEnded :?> IObservable<_>)
                            .Select(fun _ -> 1)
                            .Do(fun _ -> writeLine "MediaEnded 触发"),
                        (mediaPlayer.MediaFailed :?> IObservable<_>)
                            .Select(fun _ -> 1)
                            .Do(fun _ -> writeLine "MediaFailed 触发")
                    )
                    .StartWith(0)
                    .Do(fun x -> sprintf "Zip 索引: %d" x |> writeLine)
                    .Zip(ls.ToObservable(), fun _ path -> path)
                    .Do(fun path ->
                        sprintf "准备播放: %s" path |> writeLine
                        try
                            mediaPlayer.Open(Uri(path))
                            mediaPlayer.Play()
                            sprintf "开始播放: %s" path |> writeLine
                        with ex ->
                            sprintf "播放异常: %s" ex.Message |> writeLine
                    )
                    .IgnoreElements()
        )
        .Switch()
