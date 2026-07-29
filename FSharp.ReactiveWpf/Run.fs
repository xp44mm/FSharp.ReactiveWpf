module FSharp.ReactiveWpf.Run

open System
open System.Reactive.Linq
open System.Reactive.Disposables

open FSharp.Idioms.Literal
open System.Threading
open System.Windows.Documents

let bind (disposable: CompositeDisposable) (rn: Run) (format: string) (data: IObservable<'t>) =
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

let formatCreate (disposable: CompositeDisposable) (format: string) (data: IObservable<'t>) =
    let rn = Run()
    bind disposable rn format data
    rn

let create (disposable: CompositeDisposable) (data: IObservable<'t>) =
    formatCreate disposable "" data


