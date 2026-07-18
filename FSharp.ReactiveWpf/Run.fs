module FSharp.ReactiveWpf.Run

open System
open System.Reactive.Linq
open System.Reactive.Disposables

open FSharp.Idioms.Literal
open System.Threading
open System.Windows.Documents

let bind (disposable: CompositeDisposable) (data: IObservable<'t>) (run: Run) =
    data
        .DistinctUntilChanged()
        .Select(stringify)
        .ObserveOn(SynchronizationContext.Current)
        .Subscribe((fun s -> run.Text <- s), (fun (ex: exn) -> run.Text <- ex.Message))
    |> disposable.Add

let create (disposable: CompositeDisposable) (data: IObservable<'t>) =
    let rn = Run()
    bind disposable data rn
    //rn.Unloaded.Add(fun _ -> disposable.Dispose())
    rn

let createLocal (data: IObservable<'t>) =
    let disposable = new CompositeDisposable()
    create disposable data
