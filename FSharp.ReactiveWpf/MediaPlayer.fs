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
            (mediaPlayer.MediaEnded :?> IObservable<_>)
                .Select(fun _ -> 1)
                .StartWith(0)
                .Zip(ls.ToObservable(), fun _ path -> path)
                .Where(File.Exists)
                .Do(fun path ->
                    mediaPlayer.Open(Uri(path))
                    mediaPlayer.Play()
                )
                .IgnoreElements()
        )
        .Switch()
