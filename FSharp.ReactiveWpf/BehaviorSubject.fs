module FSharp.ReactiveWpf.BehaviorSubject

open System.Reactive.Subjects
open System

let tryNextWith (equals: 't -> 't -> bool) (newValue: 't) (bs: BehaviorSubject<'t>) =
    if not(equals bs.Value newValue) then
        bs.OnNext newValue

let tryNext (newValue: 't) (bs: BehaviorSubject<'t>) = tryNextWith (=) newValue bs

/// 实际差异大于阈值delta
let tryNextDelta (delta: float) (newValue: float) (bs: BehaviorSubject<float>) =
    let equals x y = abs(x - y) <= delta
    tryNextWith equals newValue bs

/// Math.Round(x, decimals, MidpointRounding.AwayFromZero)
let tryNextRounded (decimals: int) (newValue: float) (bs: BehaviorSubject<float>) =
    let round (x: float) = Math.Round(x, decimals, MidpointRounding.AwayFromZero)
    let equals x y = round x = round y
    tryNextWith equals newValue bs
