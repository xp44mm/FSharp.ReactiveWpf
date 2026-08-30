namespace PipeSections

open System
open System.Reactive.Subjects
open System.Reactive.Linq
open System.Reactive.Disposables
open FSharp.Idioms.Jsons

/// 主视图模型：管理一组管道截面视图模型
type MainViewModel =
    {
        items: ObservableArray<PipeSectionViewModel>
    }

    static member empty() =
        {
            items = new ObservableArray<PipeSectionViewModel>()

        }

    /// 在数组尾部追加一个管道项目，并返回新项目
    member this.append
        (bind: CompositeDisposable -> PipeSectionViewModel -> unit)
        (diameter: float)
        =
        let item = PipeSectionViewModel.create(diameter)
        let disp = new CompositeDisposable()
        bind disp item
        this.items.Add(item)
        item

    /// 在指定索引前插入一个管道项目，并返回新项目
    member this.insertBefore
        (bind: CompositeDisposable -> PipeSectionViewModel -> unit)
        (index: int)
        (diameter: float)
        =
        let item = PipeSectionViewModel.create(diameter)
        let disp = new CompositeDisposable()
        bind disp item
        this.items.Insert(index, item)
        item

    member this.detach(unbind: PipeSectionViewModel -> unit) =
        this.removeAt unbind (this.items.Count - 1)

    /// 删除指定索引的管道项目
    member this.removeAt (unbind: PipeSectionViewModel -> unit) (index: int) =
        let item = this.items.[index]
        this.items.RemoveAt(index)
        unbind item

    /// 序列化为 JSON 数组
    member this.toJson() =
        FSharp.Idioms.Json.from
            (this.items.ToArray() |> Array.map (fun item -> item.toJson()))

    /// 从 JSON 数组加载：为每个元素创建、绑定并追加管道项目
    member this.fromJson
        (bind: CompositeDisposable -> PipeSectionViewModel -> unit)
        (json: Json)
        =
        for itemJson in json.elements do
            let item = PipeSectionViewModel.create(0.0)
            let disp = new CompositeDisposable()
            bind disp item
            item.fromJson itemJson
            this.items.Add(item)
