module FSharp.ReactiveWpf.ToggleButton

open System
open System.Reactive.Linq
open System.Reactive.Subjects
open System.Reactive.Disposables

open System.Windows.Controls
open FSharp.Idioms
open System.Threading
open System.Windows.Controls.Primitives

let bind
    (disposable: CompositeDisposable)
    (value: ISubject<bool>)
    (control: ToggleButton)
    =

    control.IsThreeState <- false
    let c = (control.Checked :?> IObservable<_>).Select(fun _ -> true)
    let u = (control.Unchecked :?> IObservable<_>).Select(fun _ -> false)
    c.Merge(u).Subscribe(value)
    |> disposable.Add

    value
        .DistinctUntilChanged()
        .ObserveOn(SynchronizationContext.Current)
        .Subscribe(fun x ->
            if
                control.IsChecked.HasValue
                && control.IsChecked.Value <> x
            then
                control.IsChecked <- Nullable(x)
        )
    |> disposable.Add

/// 创建并绑定 Observable 到新的 ToggleButton 控件
let create (value: ISubject<bool>) =
    let control = ToggleButton()
    let disposable = new CompositeDisposable()
    bind disposable value control
    
    // 添加清理逻辑
    //control.Unloaded.Add(fun _ -> 
    //    if not disposable.IsDisposed then
    //        disposable.Dispose())
    
    control
