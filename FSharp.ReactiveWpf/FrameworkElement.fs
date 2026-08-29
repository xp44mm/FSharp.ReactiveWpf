module FSharp.ReactiveWpf.FrameworkElement

open System
open System.Reactive.Linq
open System.Reactive.Disposables

open System.Windows

open System.Threading

let bindStyle
    (disposable: CompositeDisposable)
    (fe: #FrameworkElement)
    (style: IObservable<Style>)
    =
    style
        .DistinctUntilChanged()
        .ObserveOn(SynchronizationContext.Current)
        .Subscribe(fun style -> fe.Style <- style)
    |> disposable.Add

let setStyle
    (disposable: CompositeDisposable)
    (style: IObservable<Style>)
    (fe: #FrameworkElement)
    =
    bindStyle disposable fe style
    fe
