module PipeSections.Json

open System.Reactive.Subjects
open FSharp.Idioms.Jsons

let tryNext (json: Json) (subject: ISubject<float>) (prop: string) =
    if json.hasProperty prop then
        subject.OnNext(json.[prop].floatValue)

let tryNextString (json: Json) (subject: ISubject<string>) (prop: string) =
    if json.hasProperty prop then
        subject.OnNext(json.[prop].stringText)

let tryNextInt (json: Json) (subject: ISubject<int>) (prop: string) =
    if json.hasProperty prop then
        subject.OnNext(int json.[prop].floatValue)

let tryNextBool (json: Json) (subject: ISubject<bool>) (prop: string) =
    if json.hasProperty prop then
        subject.OnNext(json.[prop].boolValue)

let tryJson (json: Json) (fromJson: Json -> unit) (prop: string) =
    if json.hasProperty prop then
        fromJson json.[prop]
