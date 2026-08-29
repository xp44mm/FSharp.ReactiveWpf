module FSharp.ReactiveWpf.UIElement

open System
open System.Reactive.Linq
open System.Reactive.Disposables

open System.Windows
open System.Windows.Controls

open System.Threading

let bindVisible
    (disposable: CompositeDisposable)
    (ui: #UIElement)
    (visible: IObservable<bool>)
    =
    visible
        .DistinctUntilChanged()
        .ObserveOn(SynchronizationContext.Current)
        .Subscribe(fun visible ->
            ui.Visibility <-
                if visible then
                    Visibility.Visible
                else
                    Visibility.Hidden
        )
    |> disposable.Add

let setVisible
    (disposable: CompositeDisposable)
    (visible: IObservable<bool>)
    (ui: #UIElement)
    =
    bindVisible disposable ui visible
    ui
