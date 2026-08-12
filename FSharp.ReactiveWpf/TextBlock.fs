module FSharp.ReactiveWpf.TextBlock

open System
open System.Reactive.Linq
open System.Reactive.Disposables
open System.Windows.Controls
open System.Threading

let bind 
    (disposable: CompositeDisposable) 
    (tb: TextBlock) 
    (data: IObservable<string>)
    =
    data
        .DistinctUntilChanged()
        .Throttle(TimeSpan.FromMilliseconds(50L))
        .ObserveOn(SynchronizationContext.Current)
        .Subscribe(
            onNext = (fun s -> tb.Text <- s), 
            onError = (fun (ex: exn) -> tb.Text <- ex.Message)
            )
    |> disposable.Add

let create (disposable: CompositeDisposable) (data: IObservable<string>) =
    let tb = TextBlock()
    bind disposable tb data
    tb
