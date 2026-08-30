namespace PipeSections

open System
open System.Reactive.Subjects
open System.Reactive.Linq
open System.Reactive.Disposables
open FSharp.Idioms.Reactive
open FSharp.Idioms.Jsons

/// 主视图模型：管理一组管道截面视图模型
/// 集合操作（append/insertBefore/detach/removeAt）已移至 Bindings.Collection
type MainViewModel =
    {
        items: ObservableArray<PipeSectionViewModel>
    }

    static member empty() =
        {
            items = new ObservableArray<PipeSectionViewModel>()
        }

    /// 序列化为 JSON 数组
    member this.toJson() =
        FSharp.Idioms.Json.from
            (this.items.ToArray() |> Array.map (fun item -> item.toJson()))

    /// 从 JSON 数组加载：为每个元素创建、绑定并批量追加管道项目
    member this.fromJson
        (bind: CompositeDisposable -> PipeSectionViewModel -> unit)
        (json: Json)
        =
        let pairs =
            [ for itemJson in json.elements do
                let item = PipeSectionViewModel.create(0.0)
                let disp = new CompositeDisposable()
                bind disp item
                item.fromJson itemJson
                yield (item, (disp :> IDisposable)) ]
        this.items.AddRange(pairs)
