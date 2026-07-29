module FSharp.ReactiveWpf.NumberParser

open System
open FSharp.Idioms

let tryFloat = JsonNumber.tryParse

let trySingle = tryFloat >> Option.map single

let tryInt64 = Int64.tryParse

let tryInt = tryInt64 >> Option.map int
