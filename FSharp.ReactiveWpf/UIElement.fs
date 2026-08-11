module FSharp.ReactiveWpf.UIElement

open System
open System.Reactive.Linq
open System.Reactive.Disposables

open System.Windows
open System.Windows.Controls

open System.Threading

let bindVisible (disposable: CompositeDisposable) (visible: IObservable<bool>) (ui: UIElement) =
    visible
        .ObserveOn(SynchronizationContext.Current)
        .Subscribe(fun visible ->
        let vis =
            if visible then
                Visibility.Visible
            else
                Visibility.Hidden
        ui.Visibility <- vis
    )
    |> disposable.Add
