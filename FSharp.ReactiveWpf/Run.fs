module FSharp.ReactiveWpf.Run

open System

open System.Reactive.Linq
open System.Reactive.Disposables

open System.Windows
open System.Windows.Controls
open System.Windows.Documents
open System.Windows.Media

open System.Threading

open FSharp.Idioms.Literal

let bindText (disposable: CompositeDisposable) (rn: Run) (data: IObservable<string>) =
    data
        .DistinctUntilChanged()
        .Throttle(TimeSpan.FromMilliseconds(50L))
        .ObserveOn(SynchronizationContext.Current)
        .Subscribe(
            onNext = (fun s -> rn.Text <- s), 
            onError = (fun (ex: exn) -> rn.Text <- ex.Message)
            )
    |> disposable.Add

let create (disposable: CompositeDisposable) (data: IObservable<string>) =
    let rn = Run()
    bindText disposable rn data
    rn

let defaultStyle =
    match Application.Current.TryFindResource(typeof<Run>) with
    | :? Style as style -> style
    | null -> Style(typeof<Run>)
    | x -> failwith $"never: {x.GetType()}"

let TransparentStyle =
    let st = Style(typeof<Run>, defaultStyle)
    Setter(TextElement.ForegroundProperty, Brushes.Transparent)
    |> st.Setters.Add
    st

let DangerStyle =
    let st = Style(typeof<Run>, defaultStyle)
    Setter(TextElement.ForegroundProperty, Brushes.Danger)
    |> st.Setters.Add
    st

let SuccessStyle =
    let st = Style(typeof<Run>, defaultStyle)
    Setter(TextElement.ForegroundProperty, Brushes.Success)
    |> st.Setters.Add
    st

let visibleRunStyle (visible: bool) =
    if visible then
        defaultStyle
    else
        TransparentStyle

let normalDangerRunStyle (normal: bool) =
    if normal then
        defaultStyle
    else
        DangerStyle

let successDangerRunStyle (success: bool) =
    if success then
        SuccessStyle
    else
        DangerStyle

let bindVisible (disposable: CompositeDisposable) (run: Run) (visible: IObservable<bool>) =
    visible
        .DistinctUntilChanged()
        .ObserveOn(SynchronizationContext.Current)

        .Subscribe(fun v -> run.Style <- visibleRunStyle v)
    |> disposable.Add

let bindSuccess (disposable: CompositeDisposable) (run: Run) (success: IObservable<bool>) =
    success
        .DistinctUntilChanged()
        .ObserveOn(SynchronizationContext.Current)
        .Subscribe(fun v -> run.Style <- successDangerRunStyle v)
    |> disposable.Add

let setVisible (disposable: CompositeDisposable) (visible: IObservable<bool>) (run: Run) =
    bindVisible disposable run visible
    run

let setSuccess (disposable: CompositeDisposable) (success: IObservable<bool>) (run: Run) =
    bindSuccess disposable run success
    run
