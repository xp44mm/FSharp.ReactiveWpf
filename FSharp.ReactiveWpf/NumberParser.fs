module FSharp.ReactiveWpf.NumberParser

open System
open FSharp.Idioms

let tryFloat (inp: string) =
    if String.IsNullOrWhiteSpace inp then
        None
    else
        Decimal.tryFloat inp

let trySingle = tryFloat >> Option.map single

let tryInt64 (inp: string) =
    if String.IsNullOrWhiteSpace inp then
        None
    else
        Decimal.tryInt inp

let tryInt = Decimal.tryInt >> Option.map int
