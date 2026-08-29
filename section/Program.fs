module section.Program

open System
open System.Threading
open System.Windows.Threading

SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext())

[<STAThread>]
[<EntryPoint>]
let main _ =
    let app = App.app
    let w = MainWindow.createWindow()
    app.Run(w)
