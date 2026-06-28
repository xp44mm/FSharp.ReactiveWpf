module Tests

open System
open Xunit

open System
open System.Reactive.Linq
open FSharp.ReactiveWpf

// 创建并使用ObservableArray
let demo() =
    use array = new ObservableArray<string>()
        
    // 订阅具体的变更事件
    let subscription2 =
        array.changes
            .Subscribe(function
                | CollectionChange.Added(i, item) ->
                    printfn "添加了项目 [%d]: %s" i item
                | CollectionChange.Removed(i, item) ->
                    printfn "移除了项目 [%d]: %s" i item
                | CollectionChange.Replaced(i, oldItem, newItem) ->
                    printfn "替换了项目 [%d]: %s -> %s" i oldItem newItem
                | CollectionChange.Cleared ->
                    printfn "清空了数组"
                //| _ -> ()
                )
        
    // 执行操作
    array.Add("第一个项目")
    array.Add("第二个项目")
    array.Insert(1, "中间项目")
    array.[0] <- "修改后的第一个项目"
    array.RemoveAt(1)
    array.Clear()
    

// 运行示例
demo()
