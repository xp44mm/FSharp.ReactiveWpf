namespace PipeSections

open System
open System.Reactive.Linq
open System.Reactive.Subjects

/// 镜像 System.Windows.Controls.UIElementCollection的变更通知：
/// 命名与参数输入严格对齐 Children 的增删操作（Add/AddRange/Insert/Remove/RemoveAt/RemoveRange/Clear）。
[<RequireQualifiedAccess>]
type UIElementCollectionChange<'T> =
    | Add of item: 'T
    | AddRange of items: 'T seq
    | Insert of index: int * item: 'T
    | RemoveAt of index: int
    | RemoveRange of index: int * count: int
    | Clear

/// 增删操作镜像 UIElementCollection（Panel.Children）的命名与参数输入，
/// 通过 changes 通知订阅者。每个条目与一个 IDisposable 平行存放，
/// 删除时自动释放；Dispose 时释放全部存活条目（数组即聚合释放器）。
type ObservableArray<'T>() =
    let items = ResizeArray<'T>()
    let disposables = ResizeArray<IDisposable>()
    let changes = new Subject<UIElementCollectionChange<'T>>()

    member _.Count = items.Count

    /// 尾部追加一个项目及其释放句柄，并通知订阅者（对应 Children.Add）
    member _.Add(item: 'T, disp: IDisposable) =
        items.Add(item)
        disposables.Add(disp)
        changes.OnNext(UIElementCollectionChange.Add item)

    /// 批量尾部追加（对应 Children.AddRange）
    member _.AddRange(pairs: seq<'T * IDisposable>) =
        let pairs = Seq.toArray pairs
        if pairs.Length > 0 then
            for (item, disp) in pairs do
                items.Add(item)
                disposables.Add(disp)
            changes.OnNext(UIElementCollectionChange.AddRange(pairs |> Array.map fst))

    /// 在指定索引插入一个项目及其释放句柄，并通知订阅者（对应 Children.Insert）
    member _.Insert(index: int, item: 'T, disp: IDisposable) =
        items.Insert(index, item)
        disposables.Insert(index, disp)
        changes.OnNext(UIElementCollectionChange.Insert(index, item))

    /// 删除指定索引的项目，先通知订阅者再自动释放其句柄（对应 Children.RemoveAt）
    member _.RemoveAt(index: int) =
        let item = items.[index]
        let disp = disposables.[index]
        items.RemoveAt(index)
        disposables.RemoveAt(index)
        changes.OnNext(UIElementCollectionChange.RemoveAt index)
        disp.Dispose()

    /// 删除指定索引区间的项目，批量通知后逐一释放句柄（对应 Children.RemoveRange）
    member _.RemoveRange(index: int, count: int) =
        if count > 0 then
            let disps = disposables.GetRange(index, count)
            items.RemoveRange(index, count)
            disposables.RemoveRange(index, count)
            changes.OnNext(UIElementCollectionChange.RemoveRange(index, count))
            for d in disps do d.Dispose()

    /// 清空全部项目，通知后释放全部句柄（对应 Children.Clear）
    member _.Clear() =
        if items.Count > 0 then
            let disps = disposables.ToArray()
            items.Clear()
            disposables.Clear()
            changes.OnNext(UIElementCollectionChange.Clear)
            for d in disps do d.Dispose()

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
            for d in disposables do d.Dispose()
            disposables.Clear()
            changes.OnCompleted()
            changes.Dispose()
