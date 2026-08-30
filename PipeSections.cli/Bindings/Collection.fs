module PipeSections.Bindings.Collection

open PipeSections
open FSharp.Idioms.Reactive
open System
open System.Reactive.Disposables

/// 镜像 ObservableArray 的接口：
/// 每个函数封装「创建释放句柄 + 建立绑定」的样板，再委托给 vm.items 的对应操作。
/// 命名与参数输入与 ObservableArray 的成员一一对应。

/// 在数组尾部追加一个管道项目，并返回该项目（对应 ObservableArray.Add）
let add (vm: MainViewModel) (item: PipeSectionViewModel) =
    let disp = new CompositeDisposable()
    Binding.bindPipeSection disp item
    vm.items.Add(item, disp)
    item

/// 批量在数组尾部追加管道项目（对应 ObservableArray.AddRange）
let addRange (vm: MainViewModel) (items: PipeSectionViewModel seq) =
    let pairs =
        items
        |> Seq.map (fun item ->
            let disp = new CompositeDisposable()
            Binding.bindPipeSection disp item
            item, (disp :> IDisposable))
    vm.items.AddRange(pairs)

/// 在指定索引前插入一个管道项目，并返回该项目（对应 ObservableArray.Insert）
let insert (vm: MainViewModel) (index: int) (item: PipeSectionViewModel) =
    let disp = new CompositeDisposable()
    Binding.bindPipeSection disp item
    vm.items.Insert(index, item, disp)
    item

