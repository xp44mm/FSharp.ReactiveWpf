namespace FSharp.ReactiveWpf.Test

open System
open Xunit
open FSharp.xUnit
open FSharp.ReactiveWpf.NumberParser

type NumberParserTest(output: ITestOutputHelper) =
    
    [<Theory>]
    [<InlineData("123.45", 123.45)>]
    [<InlineData("-123.45", -123.45)>]
    [<InlineData("0", 0.0)>]
    [<InlineData("0.0", 0.0)>]
    [<InlineData(".5", 0.5)>]
    [<InlineData("-5", -5.0)>]
    [<InlineData(" 123 ", 123.0)>]
    [<InlineData("+123", 123.0)>]
    member this.``tryFloat - 有效数字字符串测试``(str: string, expected: float) =
        let result = tryFloat str
        match result with
        | Some actual -> 
            let diff = Math.Abs(actual - expected)
            Assert.True(diff < 0.0001, $"期望 {expected}，实际 {actual}，差异 {diff}")
        | None -> 
            Assert.Fail($"解析 '{str}' 失败，但应该成功")

    [<Theory>]
    [<InlineData("abc")>]
    [<InlineData("123.45.67")>]
    [<InlineData("1.2.3")>]
    [<InlineData("  ")>]
    [<InlineData("")>]
    [<InlineData("null")>]
    [<InlineData("NaN")>]
    [<InlineData("Infinity")>]
    [<InlineData("12e")>]
    [<InlineData("e10")>]
    member this.``tryFloat - 无效数字字符串测试``(str: string) =
        let result = tryFloat str
        Assert.Equal<float option>(None, result)

    [<Fact>]
    member this.``tryFloat - 边界值测试``() =
        let resultMax = tryFloat (Double.MaxValue.ToString())
        match resultMax with
        | Some value -> Assert.Equal(Double.MaxValue, value)
        | None -> Assert.Fail("解析 Double.MaxValue 失败")
        
        let resultMin = tryFloat (Double.MinValue.ToString())
        match resultMin with
        | Some value -> Assert.Equal(Double.MinValue, value)
        | None -> Assert.Fail("解析 Double.MinValue 失败")

    [<Theory>]
    [<InlineData("123.45", 123.45f)>]
    [<InlineData("-123.45", -123.45f)>]
    [<InlineData("0", 0.0f)>]
    [<InlineData(".5", 0.5f)>]
    [<InlineData(" 456 ", 456.0f)>]
    member this.``trySingle - 有效数字字符串测试``(str: string, expected: float32) =
        let result = trySingle str
        match result with
        | Some actual -> 
            let diff = Math.Abs(actual - expected)
            Assert.True(diff < 0.0001f, $"期望 {expected}，实际 {actual}，差异 {diff}")
        | None -> 
            Assert.Fail($"解析 '{str}' 失败，但应该成功")

    [<Theory>]
    [<InlineData("abc")>]
    [<InlineData("123.45.67")>]
    [<InlineData("  ")>]
    [<InlineData("")>]
    [<InlineData("ABC")>]
    member this.``trySingle - 无效数字字符串测试``(str: string) =
        let result = trySingle str
        Assert.Equal(None, result)

    [<Fact>]
    member this.``trySingle - 范围溢出测试``() =
        let tooLarge = (float Single.MaxValue) * 2.0
        let result = trySingle (tooLarge.ToString())
        Assert.Equal(None, result)

    [<Theory>]
    [<InlineData("0", 0L)>]
    [<InlineData("123", 123L)>]
    [<InlineData("-456", -456L)>]
    [<InlineData("  789  ", 789L)>]
    [<InlineData("+100", 100L)>]
    member this.``tryInt64 - 有效整数测试``(str: string, expected: int64) =
        let result = tryInt64 str
        match result with
        | Some actual -> Assert.Equal(expected, actual)
        | None -> Assert.Fail($"解析 '{str}' 失败，但应该成功")

    [<Fact>]
    member this.``tryInt64 - 有效整数边界值测试``() =
        let resultMax = tryInt64 (Int64.MaxValue.ToString())
        match resultMax with
        | Some actual -> Assert.Equal(Int64.MaxValue, actual)
        | None -> Assert.Fail("解析 Int64.MaxValue 失败")
        
        let resultMin = tryInt64 (Int64.MinValue.ToString())
        match resultMin with
        | Some actual -> Assert.Equal(Int64.MinValue, actual)
        | None -> Assert.Fail("解析 Int64.MinValue 失败")

    [<Theory>]
    [<InlineData("abc")>]
    [<InlineData("123.45")>]
    [<InlineData("123,456")>]
    [<InlineData("  ")>]
    [<InlineData("")>]
    [<InlineData("12.0")>]
    [<InlineData("0x123")>]
    [<InlineData("123L")>]
    member this.``tryInt64 - 无效整数测试``(str: string) =
        let result = tryInt64 str
        Assert.Equal(None, result)

    [<Theory>]
    [<InlineData("1.7976931348623157e+308")>]
    [<InlineData("-9.223372036854776e+18")>]
    member this.``tryInt64 - 溢出测试``(str: string) =
        let result = tryInt64 str
        Assert.Equal(None, result)

    [<Theory>]
    [<InlineData("0", 0)>]
    [<InlineData("123", 123)>]
    [<InlineData("-456", -456)>]
    [<InlineData("  789  ", 789)>]
    [<InlineData("+100", 100)>]
    member this.``tryInt - 有效整数测试``(str: string, expected: int) =
        let result = tryInt str
        match result with
        | Some actual -> Assert.Equal(expected, actual)
        | None -> Assert.Fail($"解析 '{str}' 失败，但应该成功")

    [<Fact>]
    member this.``tryInt - 有效整数边界值测试``() =
        let resultMax = tryInt (Int32.MaxValue.ToString())
        match resultMax with
        | Some actual -> Assert.Equal(Int32.MaxValue, actual)
        | None -> Assert.Fail("解析 Int32.MaxValue 失败")
        
        let resultMin = tryInt (Int32.MinValue.ToString())
        match resultMin with
        | Some actual -> Assert.Equal(Int32.MinValue, actual)
        | None -> Assert.Fail("解析 Int32.MinValue 失败")

    [<Theory>]
    [<InlineData("abc")>]
    [<InlineData("123.45")>]
    [<InlineData("123,456")>]
    [<InlineData("  ")>]
    [<InlineData("")>]
    [<InlineData("12.0")>]
    [<InlineData("0x123")>]
    [<InlineData("123L")>]
    member this.``tryInt - 无效整数测试``(str: string) =
        let result = tryInt str
        Assert.Equal(None, result)

    [<Theory>]
    [<InlineData("2147483648")>]
    [<InlineData("-2147483649")>]
    member this.``tryInt - 溢出测试``(str: string) =
        let result = tryInt str
        Assert.Equal(None, result)

    [<Fact>]
    member this.``tryInt - 类型转换一致性测试``() =
        let testValue = "456"
        let int64Result = tryInt64 testValue
        let intResult = tryInt testValue
        
        match int64Result, intResult with
        | Some i64, Some i -> 
            Assert.Equal(int i64, i)
        | None, None -> 
            ()
        | _ -> 
            Assert.Fail("tryInt64 和 tryInt 的结果应该一致")

    [<Theory>]
    [<InlineData("")>]
    [<InlineData("   ")>]
    [<InlineData("\t")>]
    [<InlineData("\n")>]
    member this.``所有函数 - 空字符串和空白字符串测试``(str: string) =
        Assert.Equal(None, tryFloat str)
        Assert.Equal(None, trySingle str)
        Assert.Equal(None, tryInt64 str)
        Assert.Equal(None, tryInt str)

    [<Theory>]
    [<InlineData(null)>]
    member this.``所有函数 - null 输入测试``(str: string) =
        Assert.Equal(None, tryFloat str)
        Assert.Equal(None, trySingle str)
        Assert.Equal(None, tryInt64 str)
        Assert.Equal(None, tryInt str)
