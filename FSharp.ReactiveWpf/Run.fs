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

let create (data: IObservable<'t>) =
    let r = Run()
    let disposable = new CompositeDisposable()
    bind disposable data r
    r.Unloaded.Add(fun _ -> disposable.Dispose())
    r


