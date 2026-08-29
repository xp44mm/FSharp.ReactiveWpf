module section.Calculations.Calculation

open System

let shape kind width height diameter =
    match kind with
    | "rectangle" ->
        let area = width * height
        let peri = (width + height) * 2.0
        area, peri
    | "circle" ->
        let area = Math.PI / 4.0 * diameter ** 2.0
        let peri = Math.PI * diameter
        area, peri
    | _ -> failwith "never"

let velocity v a = v / 3600.0 / a *1e6
