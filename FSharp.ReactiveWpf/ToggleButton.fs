module FSharp.ReactiveWpf.ToggleButton
open FSharp.Idioms

open System
open System.Reactive.Linq
open System.Reactive.Subjects
open System.Reactive.Disposables

open System.Windows.Controls
open System.Windows.Controls.Primitives

open System.Threading

let bind
    (disposable: CompositeDisposable)
    (control: ToggleButton)
    (value: ISubject<bool>)
    =

    control.IsThreeState <- false
    let c = (control.Checked :?> IObservable<_>).Select(fun _ -> true)
    let u = (control.Unchecked :?> IObservable<_>).Select(fun _ -> false)
    c.Merge(u).Subscribe(value)
    |> disposable.Add

    value
        .DistinctUntilChanged()
        .ObserveOn(SynchronizationContext.Current)
        .Subscribe(fun x ->
            if
                control.IsChecked.HasValue
                && control.IsChecked.Value <> x
            then
                control.IsChecked <- Nullable(x)
        )
    |> disposable.Add

let create 
    (disposable: CompositeDisposable)
    (value: ISubject<bool>) =
    let control = ToggleButton()
    bind disposable control value
    control
