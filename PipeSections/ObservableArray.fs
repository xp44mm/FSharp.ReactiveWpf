namespace PipeSections

open System
open System.Collections.Generic
open System.Reactive
open System.Reactive.Linq
open System.Reactive.Subjects

/// 集合变更的类型
[<RequireQualifiedAccess>]
type CollectionChange<'T> =
    | Added of index: int * item: 'T
    | Removed of index: int * item: 'T
    | Cleared

/// 在 PipeSections 内重新实现的可观察数组：
/// 添加、插入、删除项目时，通过 changes 通知订阅者。
type ObservableArray<'T>() =
    let items = ResizeArray<'T>()
    let changes = new Subject<CollectionChange<'T>>()

    member _.Count = items.Count

    /// 在数组末尾追加一个项目，并通知订阅者
    member _.Add(item: 'T) =
        let index = items.Count
        items.Add(item)
        changes.OnNext(CollectionChange.Added(index, item))

    /// 在指定索引插入一个项目，并通知订阅者
    member _.Insert(index: int, item: 'T) =
        items.Insert(index, item)
        changes.OnNext(CollectionChange.Added(index, item))

    /// 删除指定索引的项目，并通知订阅者
    member _.RemoveAt(index: int) =
        let item = items.[index]
        items.RemoveAt(index)
        changes.OnNext(CollectionChange.Removed(index, item))

    /// 返回指定项目的当前索引，找不到返回 -1
    member _.IndexOf(item: 'T) =
        items.IndexOf(item)

    member _.Item
        with get (index: int) = items.[index]

    /// 变更通知流
    member _.Changes = changes.AsObservable()

    /// 当前数组快照
    member _.ToArray() = items.ToArray()

    interface IDisposable with
        member _.Dispose() =
            changes.OnCompleted()
            changes.Dispose()
