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
    (checkbox: CheckBox)
    (value: ISubject<bool>)
    =
    ToggleButton.bind disposable checkbox value

let create (disposable: CompositeDisposable) (value: ISubject<bool>) (content: obj) =
    let check = CheckBox()
    check.IsThreeState <- false
    check.Content <- content
    bind disposable check value
    check

