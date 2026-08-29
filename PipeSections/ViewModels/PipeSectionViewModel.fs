namespace PipeSections

open System
open System.Reactive.Disposables
open System.Reactive.Linq
open System.Reactive.Subjects

/// 管道截面的直径、周长、截面积视图模型
/// 数组元素自身维护内部数据绑定（直径 → 周长、截面积），实现 IDisposable 以便解绑
type PipeSectionViewModel =
    {
        /// 直径(mm)
        diameter: BehaviorSubject<float>
        /// 周长(mm)
        circumference: BehaviorSubject<float>
        /// 截面积(mm²)
        area: BehaviorSubject<float>
    }

    /// 创建视图模型并建立内部绑定：直径变化自动联动周长、截面积
    static member create (diameter: float) =
        {
            diameter = new BehaviorSubject<float>(diameter)
            circumference = new BehaviorSubject<float>(0.0)
            area = new BehaviorSubject<float>(0.0)
        }
