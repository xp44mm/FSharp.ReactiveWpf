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

let create (disposable: CompositeDisposable) (value: ISubject<bool>) (content: obj) =
    let check = CheckBox()
    check.IsThreeState <- false
    check.Content <- content
    bind disposable value check
    //check.Unloaded.Add(fun _ ->
    //    if disposable.IsDisposed then
    //        disposable.Dispose()
    //)

    check

let createLocal (value: ISubject<bool>) (content: obj) =
    let disposable = new CompositeDisposable()
    create disposable value content
