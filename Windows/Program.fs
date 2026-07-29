module Windows.Program

open System

[<STAThread>]
[<EntryPoint>]
let main _ =
    let app = App.app
    let w = MainWindow.createWindow()
    app.Run(w)
