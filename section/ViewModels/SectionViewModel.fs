namespace section

open System
open System.Reactive.Subjects
open System.Reactive.Linq

///
type SectionViewModel =
    {
        shape: ShapeViewModel
        volume: BehaviorSubject<float>
        velocity: BehaviorSubject<float>

    }

    static member create() =
        let shape = ShapeViewModel.create("circle", 0, 0, 100)
        let volume = new BehaviorSubject<float>(10)

        {
            shape = shape
            volume = volume
            velocity = new BehaviorSubject<float> 0.0
        }
