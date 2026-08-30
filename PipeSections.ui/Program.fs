module PipeSections.Program

open System
open System.Threading
open System.Windows.Threading

SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext())

[<STAThread>]
[<EntryPoint>]
let main _ =
    let app = App.app
    let window = MainWindow.createWindow()
    app.Run(window)
