module FSharp.ReactiveWpf.Run

open System
open System.Reactive.Linq
open System.Reactive.Disposables

open FSharp.Idioms.Literal
open System.Threading
open System.Windows.Documents

let bind (format: string) (rn: Run) (data: IObservable<'t>) (disposable: CompositeDisposable) =
    data
        .Select(formatValue format)
        .DistinctUntilChanged()
        .Throttle(TimeSpan.FromMilliseconds(100.0))
        .ObserveOn(SynchronizationContext.Current)
        .Subscribe(
            onNext = (fun s -> rn.Text <- s), 
            onError = (fun (ex: exn) -> rn.Text <- ex.Message)
            )
    |> disposable.Add

let formatCreate (format: string) (data: IObservable<'t>) (disposable: CompositeDisposable) =
    let rn = Run()
    bind format rn data disposable
    rn

let create (data: IObservable<'t>) (disposable: CompositeDisposable) =
    formatCreate "" data disposable

let createLocal (data: IObservable<'t>) =
    let disposable = new CompositeDisposable()
    create data disposable

