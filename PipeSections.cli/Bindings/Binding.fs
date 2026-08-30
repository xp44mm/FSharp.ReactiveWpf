module PipeSections.Bindings.Binding

open PipeSections
open System
open System.Reactive.Linq
open System
open System.Reactive.Disposables
open System.Reactive.Linq
open System.Reactive.Subjects
open System.Diagnostics
open System.Threading

open FSharp.Idioms.Literal

let bindPipeSection (disposable: CompositeDisposable) (vm: PipeSectionViewModel) =
    vm.diameter
    |> Observable.map Calculations.Calculation.circumference
    |> Observable.subscribe(fun c -> vm.circumference.OnNext c)
    |> disposable.Add

    vm.diameter
    |> Observable.map Calculations.Calculation.area
    |> Observable.subscribe(fun a -> vm.area.OnNext a)
    |> disposable.Add

