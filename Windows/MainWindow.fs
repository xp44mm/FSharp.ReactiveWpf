module Windows.MainWindow

open System
open System.IO

open System.Windows
open System.Windows.Controls
open System.Windows.Documents
open System.Windows.Input
open System.Windows.Media
open System.Windows.Threading

open System.Reactive.Subjects
open System.Reactive.Disposables
open System.Reactive.Linq
open System.Threading
open System.Reflection

open MahApps.Metro.Controls

open FSharp.Idioms
open FSharp.ReactiveWpf

/// 初始化主窗口并绑定事件
let createWindow () =
    let window = App.loadXaml "MainWindow.xaml" :?> MetroWindow

    let openFileButton = window.FindName("openFileButton") :?> Button
    let saveFileButton = window.FindName("saveFileButton") :?> Button

    let disposable = new CompositeDisposable()


    (openFileButton.Click :?> IObservable<_>)
        .Subscribe(fun _ ->
            let dialog = Microsoft.Win32.OpenFileDialog()
            dialog.Filter <- "JSON 文件 (*.json)|*.json|文本文件 (*.txt)|*.txt"
            dialog.DefaultExt <- ".json"

        )
    |> disposable.Add

    (saveFileButton.Click :?> IObservable<_>)
        .Subscribe(fun _ ->
            let saveDialog = Microsoft.Win32.SaveFileDialog()
            saveDialog.Filter <- "JSON 文件 (*.json)|*.json|文本文件 (*.txt)|*.txt"
            saveDialog.DefaultExt <- ".json"

        )
    |> disposable.Add

    window.Closing.Add(fun _ -> disposable.Dispose())
    window
