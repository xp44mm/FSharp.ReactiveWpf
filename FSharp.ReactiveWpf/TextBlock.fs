module FSharp.ReactiveWpf.TextBlock

open System
open System.Reactive.Linq
open System.Reactive.Disposables

open System.Windows.Controls
open System.Threading
open FSharp.Idioms.Literal

let bind (disposable: CompositeDisposable) (data: IObservable<'t>) (tb: TextBlock) =
    data
        .Select(stringify)
        .ObserveOn(SynchronizationContext.Current)
        .Subscribe((fun s -> tb.Text <- s), (fun (ex: exn) -> tb.Text <- ex.Message))
    |> disposable.Add

let create (disposable: CompositeDisposable) (data: IObservable<'t>) =
    let tb = TextBlock()
    //let disposable = new CompositeDisposable()
    bind disposable data tb
    tb.Unloaded.Add(fun _ -> disposable.Dispose())
    tb
