module PipeSections.Calculations.Calculation

open System

/// 由直径(mm)计算周长(mm)
let circumference (diameter: float) =
    Math.PI * diameter

/// 由直径(mm)计算截面积(mm²)
let area (diameter: float) =
    Math.PI / 4.0 * diameter ** 2.0

