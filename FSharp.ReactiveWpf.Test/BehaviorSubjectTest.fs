namespace FSharp.Idioms.Reactive

open Xunit
open FSharp.xUnit
open System
open System.Reactive.Subjects

// 为了方便，使用类型别名引用模块中的函数
// 模块名为 FSharp.Idioms.Reactive.BehaviorSubject，已在代码中定义
// 下面直接通过模块名调用

type BehaviorSubjectTest(output: ITestOutputHelper) =

    // ---------- tryNext 测试 ----------
    [<Theory>]
    [<InlineData(123.45, 123.45)>]        // 相等 → 不更新
    [<InlineData(123.45, 123.46)>]        // 不等 → 更新
    member this.``tryNext 仅在值不同时更新``(old: float, next: float) =
        let bs = new BehaviorSubject<_>(old)
        BehaviorSubject.tryNext next bs
        let expected = if old = next then old else next
        Should.equal expected bs.Value

    // ---------- tryNextDelta 测试 ----------
    // delta = 0.01
    [<Theory>]
    [<InlineData(1.234, 1.244)>]  // 差值 0.010，等于阈值 → 不更新（因为 <= delta）
    [<InlineData(1.234, 1.245)>]  // 差值 0.011，大于阈值 → 更新
    [<InlineData(1.234, 1.233)>]  // 差值 0.001，小于阈值 → 不更新
    member this.``tryNextDelta 当差值 > delta 时更新``(old: float, next: float) =
        let delta = 0.01
        let bs = new BehaviorSubject<_>(old)
        BehaviorSubject.tryNextDelta delta next bs
        let shouldUpdate = abs (old - next) > delta
        let expected = if shouldUpdate then next else old
        Should.equal expected bs.Value

    // ---------- tryNextRounded 测试 ----------
    // decimals = 2，四舍五入到两位小数
    [<Theory>]
    [<InlineData(1.2345, 1.2346)>]  // 舍入后同为 1.23 → 不更新
    [<InlineData(1.2345, 1.2350)>]  // 舍入后 1.23 vs 1.24 → 更新
    [<InlineData(1.2349, 1.2351)>]  // 舍入后 1.23 vs 1.24 → 更新
    [<InlineData(1.2349, 1.2341)>]  // 舍入后同为 1.23 → 不更新
    member this.``tryNextRounded 仅当四舍五入后值不同时更新``(old: float, next: float) =
        let decimals = 2
        let bs = new BehaviorSubject<_>(old)
        BehaviorSubject.tryNextRound decimals next bs
        let round (x:float) = Math.Round(x, decimals, MidpointRounding.AwayFromZero)
        let shouldUpdate = round old <> round next
        let expected = if shouldUpdate then next else old
        Should.equal expected bs.Value

    // ---------- 额外测试：验证负数小数位会抛出异常 ----------
    [<Fact>]
    member this.``tryNextRounded 负数 decimals 抛出异常``() =
        let bs = new BehaviorSubject<_>(1.0)
        let act = fun () -> BehaviorSubject.tryNextRound -1 2.0 bs |> ignore
        Assert.Throws<ArgumentOutOfRangeException>(act) |> ignore
