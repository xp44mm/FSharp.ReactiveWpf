module FSharp.ReactiveWpf.Run

open System
open System.Reactive.Linq
open System.Reactive.Disposables

open System.Windows.Documents
open System.Threading

let bind 
    (disposable: CompositeDisposable) 
    (rn: Run) 
    (data: IObservable<string>) =
    data
        .DistinctUntilChanged()
        .Throttle(TimeSpan.FromMilliseconds(100.0))
        .ObserveOn(SynchronizationContext.Current)
        .Subscribe(
            onNext = (fun s -> rn.Text <- s), 
            onError = (fun (ex: exn) -> rn.Text <- ex.Message)
            )
    |> disposable.Add

let create (disposable: CompositeDisposable) (data: IObservable<string>) =
    let rn = Run()
    bind disposable rn data
    rn


