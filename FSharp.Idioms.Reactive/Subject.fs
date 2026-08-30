module FSharp.Idioms.Reactive.Subject

open System.Reactive.Subjects

let OnNext (value: 't) (subject: ISubject<'t>) = subject.OnNext value
