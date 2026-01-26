module FSharp.ReactiveWpf.CheckBox

open System
open System.Reactive.Linq
open System.Reactive.Subjects
open System.Reactive.Disposables

open System.Windows.Controls
open FSharp.Idioms
open System.Threading

let bind
    (disposable: CompositeDisposable)
    (value: ISubject<bool>)
    (checkbox: CheckBox)
    =
    ToggleButton.bind disposable value checkbox

let checkBox (value: ISubject<bool>) =
    let cb = CheckBox()
    cb.IsThreeState <- false

    let disposable = new CompositeDisposable()
    bind disposable value cb
    cb.Unloaded.Add(fun _ ->
        if disposable.IsDisposed then
            disposable.Dispose()
    )

    //let c = (cb.Checked :?> IObservable<_>).Select(fun _ -> true)
    //let u = (cb.Unchecked :?> IObservable<_>).Select(fun _ -> false)
    //let sub1 = c.Merge(u).Subscribe(value)
    //let sub2 =
    //    value
    //        .DistinctUntilChanged()
    //        .ObserveOn(SynchronizationContext.Current)
    //        .Subscribe(fun x ->
    //            if cb.IsChecked.HasValue && cb.IsChecked.Value <> x then
    //                cb.IsChecked <- Nullable(x)
    //        )
    //cb.Unloaded.Add(fun _ ->
    //    sub1.Dispose()
    //    sub2.Dispose()
    //)

    cb
