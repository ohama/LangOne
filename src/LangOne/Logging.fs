module LangOne.Logging

open Serilog

/// Initialize Serilog logger
let initialize () =
    Log.Logger <- LoggerConfiguration()
        .MinimumLevel.Debug()
        .WriteTo.Console()
        .CreateLogger()

/// Get logger for a specific context
let forContext<'T> () = Log.ForContext<'T>()
