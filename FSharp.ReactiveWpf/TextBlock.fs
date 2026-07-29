module FSharp.ReactiveWpf.TextBlock

open System
open System.Reactive.Linq
open System.Reactive.Disposables
open System.Windows.Controls
open System.Threading

open FSharp.Idioms.Literal

let bind (disposable: CompositeDisposable) (tb: TextBlock) (format: string) (data: IObservable<'t>) =
    data
        .Select(formatValue format)
        .DistinctUntilChanged()
        .Throttle(TimeSpan.FromMilliseconds(100.0))
        .ObserveOn(SynchronizationContext.Current)
        .Subscribe(
            onNext = (fun s -> tb.Text <- s), 
            onError = (fun (ex: exn) -> tb.Text <- ex.Message)
            )
    |> disposable.Add

let formatCreate (disposable: CompositeDisposable) (format: string) (data: IObservable<'t>) =
    let tb = TextBlock()
    bind disposable tb format data
    tb

let create (disposable: CompositeDisposable) (data: IObservable<'t>) =
    formatCreate disposable "" data
