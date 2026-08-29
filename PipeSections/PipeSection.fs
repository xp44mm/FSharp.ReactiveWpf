module PipeSections.PipeSection

/// 一段管道的截面参数（单位 mm）
type PipeSection =
    { Name: string
      OuterDiameter: float
      WallThickness: float }

/// 常用管道截面规格
let sections =
    [| { Name = "DN50"; OuterDiameter = 60.3; WallThickness = 3.5 }
       { Name = "DN80"; OuterDiameter = 88.9; WallThickness = 4.0 }
       { Name = "DN100"; OuterDiameter = 114.3; WallThickness = 4.5 }
       { Name = "DN150"; OuterDiameter = 168.3; WallThickness = 5.0 }
       { Name = "DN200"; OuterDiameter = 219.1; WallThickness = 6.0 }
       { Name = "DN300"; OuterDiameter = 323.9; WallThickness = 7.0 } |]

/// 绘图画布边长（与 MainWindow.xaml 中 Grid 尺寸保持一致）
let canvasSize = 240.0

/// 把 mm 直径换算为绘图半径的缩放系数
let scale =
    let maxOuter =
        sections
        |> Array.maxBy (fun s -> s.OuterDiameter)

    (canvasSize / 2.0 - 6.0) / (maxOuter.OuterDiameter / 2.0)

/// 根据规格计算外圆、内圆的绘制半径（单位：画布像素）
let radii (s: PipeSection) =
    let outerR = s.OuterDiameter / 2.0 * scale
    let innerR = (s.OuterDiameter - 2.0 * s.WallThickness) / 2.0 * scale
    outerR, innerR

/// 规格的显示文本
let description (s: PipeSection) =
    sprintf "%s    外径 %.1f mm    壁厚 %.1f mm"
        s.Name s.OuterDiameter s.WallThickness
