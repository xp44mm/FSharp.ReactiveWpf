namespace FSharp.ReactiveWpf

open System
open System.Collections
open System.Collections.Generic
open System.Reactive.Linq
open System.Reactive.Subjects

[<RequireQualifiedAccess>]
type CollectionChange<'T> =
    | Added of index: int * item: 'T
    | Removed of index: int * item: 'T
    | Replaced of index: int * oldItem: 'T * newItem: 'T
    | Cleared

type ObservableArray<'T>() =
    let items = ResizeArray<'T>()
    let subject =
        new BehaviorSubject<IReadOnlyList<'T>>(items :> IReadOnlyList<'T>)
    let changesSubject = new Subject<CollectionChange<'T>>()

    member _.latestValue = subject.AsObservable()
    member _.changes = changesSubject.AsObservable()

    member _.Add(item: 'T) =
        let index = items.Count
        items.Add(item)
        changesSubject.OnNext(CollectionChange.Added(index, item))
        subject.OnNext(items :> IReadOnlyList<'T>)

    member _.Insert(index: int, item: 'T) =
        items.Insert(index, item)
        changesSubject.OnNext(CollectionChange.Added(index, item))
        subject.OnNext(items :> IReadOnlyList<'T>)

    member _.Remove(item: 'T) =
        let index = items.IndexOf(item)
        if index >= 0 then
            items.RemoveAt(index)
            changesSubject.OnNext(CollectionChange.Removed(index, item))
            subject.OnNext(items :> IReadOnlyList<'T>)

    member _.RemoveAt(index: int) =
        let item = items.[index]
        items.RemoveAt(index)
        changesSubject.OnNext(CollectionChange.Removed(index, item))
        subject.OnNext(items :> IReadOnlyList<'T>)

    member _.Clear() =
        if items.Count > 0 then
            items.Clear()
            changesSubject.OnNext(CollectionChange.Cleared)
            subject.OnNext(items :> IReadOnlyList<'T>)

    member _.Item
        with get (index) = items.[index]
        and set index value =
            let oldItem = items.[index]
            items.[index] <- value
            changesSubject.OnNext(CollectionChange.Replaced(index, oldItem, value))
            subject.OnNext(items :> IReadOnlyList<'T>)

    interface IDisposable with
        member _.Dispose() =
            changesSubject.OnCompleted()
            subject.OnCompleted()
            changesSubject.Dispose()
            subject.Dispose()
