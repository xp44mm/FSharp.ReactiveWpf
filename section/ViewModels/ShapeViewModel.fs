namespace section

open System
open System.Reactive.Subjects
open System.Reactive.Linq

type ShapeViewModel =
    {
        kind: BehaviorSubject<string>
        width: BehaviorSubject<float>
        height: BehaviorSubject<float>
        diameter: BehaviorSubject<float>

        area: BehaviorSubject<float>
        peri: BehaviorSubject<float>

    }

    static member create(k, w, h, d) =
        let kind = new BehaviorSubject<string>(k)
        let width = new BehaviorSubject<float>(w)
        let height = new BehaviorSubject<float>(h)
        let diameter = new BehaviorSubject<float>(d)


        //let result =
        //    Observable.CombineLatest(
        //        kind,
        //        width,
        //        height,
        //        diameter,

        //        fun kind width height diameter ->
        //            match kind with
        //            | "rectangle" ->
        //                let area = width * height
        //                let peri = (width + height) * 2.0
        //                area, peri
        //            | "circle" ->
        //                let area = Math.PI / 4.0 * diameter ** 2
        //                let peri = Math.PI * diameter
        //                area, peri
        //            | _ -> failwith "never"

        //    )

        //let area = result.Select(fst)
        //let peri = result.Select(snd)

        {
            kind = kind
            width = width
            height = height
            diameter = diameter
            area = new BehaviorSubject<float> 0.0
            peri = new BehaviorSubject<float> 0.0

        }
