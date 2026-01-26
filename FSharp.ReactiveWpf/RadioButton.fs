module FSharp.ReactiveWpf.RadioButton

open System
open System.Reactive.Linq
open System.Reactive.Subjects
open System.Reactive.Disposables

open System.Windows.Controls
open FSharp.Idioms
open System.Threading

let bindingRadioButton
    (disposable: CompositeDisposable)
    (value: ISubject<bool>)
    (radioButton: RadioButton)
    =
    ToggleButton.bind disposable value radioButton

let bindingRadioButtonGroup
    (disposable: CompositeDisposable)
    (value: ISubject<int>)
    (radioButtons: RadioButton[])
    =
    let radioObservable =
        radioButtons
        |> Array.mapi(fun i radio -> (radio.Checked :?> IObservable<_>).Select(fun _ -> i))
        |> Observable.Merge

    radioObservable.Subscribe(value) |> disposable.Add

    value
        .DistinctUntilChanged()
        .ObserveOn(SynchronizationContext.Current)
        .Subscribe(fun i ->
            if i >= 0 && i < radioButtons.Length then
                let radio = radioButtons.[i]
                if radio.IsChecked <> Nullable(true) then
                    radio.IsChecked <- Nullable(true)
            else
                radioButtons
                |> Array.iter(fun radio ->
                    if radio.IsChecked <> Nullable(false) then
                        radio.IsChecked <- Nullable(false)
                )
        )
    |> disposable.Add

let bindingRadioButtonGroupUsingContent
    (disposable: CompositeDisposable)
    (content: ISubject<string>)
    (radioButtons: RadioButton[])
    =

    let radioObservable =
        radioButtons
        |> Array.map(fun radio ->
            (radio.Checked :?> IObservable<_>).Select(fun _ ->                 
                radio.Content :?> string)
                //.Do(fun s -> Debug.WriteLine s)
        )
        |> Observable.Merge

    radioObservable.Subscribe(content)
    |> disposable.Add

    content
        .DistinctUntilChanged()
        .ObserveOn(SynchronizationContext.Current)
        .Subscribe(fun sc ->
            match
                radioButtons
                |> Array.tryFind(fun radio ->
                    let c = radio.Content :?> string
                    sc = c
                )
            with
            | Some radio ->
                if radio.IsChecked <> Nullable(true) then
                    radio.IsChecked <- Nullable(true)
            | None -> ()
        )
    |> disposable.Add






