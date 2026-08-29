module section.Bindings.Binding
open section

open System
open System.Reactive.Linq
open System
open System.Reactive.Disposables
open System.Reactive.Linq
open System.Reactive.Subjects
open System.Diagnostics
open System.Threading
open MathNet.Numerics.LinearAlgebra

open FSharp.Idioms.Literal

let bind (disposable: CompositeDisposable) (section: SectionViewModel) =
        //let result =
        Observable.CombineLatest(
            section.shape.kind,
            section.shape.width,
            section.shape.height,
            section.shape.diameter,
            Calculations.Calculation.shape
            //fun kind width height diameter ->
            //    match kind with
            //    | "rectangle" ->
            //        let area = width * height
            //        let peri = (width + height) * 2.0
            //        area, peri
            //    | "circle" ->
            //        let area = Math.PI / 4.0 * diameter ** 2
            //        let peri = Math.PI * diameter
            //        area, peri
            //    | _ -> failwith "never"

        )
        |> Observable.subscribe(fun (area, peri) ->
            section.shape.area.OnNext area
            section.shape.peri.OnNext peri
        )
        |> disposable.Add

        //let area = result.Select(fst)
        //let peri = result.Select(snd)

        //let velocity =
        Observable.CombineLatest(
            section.volume,
            section.shape.area, //mm2 -> m2.Select((*) 1e-6)
            Calculations.Calculation.velocity
        )
        |> Observable.subscribe(fun vel ->
            section.velocity.OnNext vel
        )
        |> disposable.Add
