module LangOne.Result

/// Bind: Chain Result-returning functions
let bind f result =
    match result with
    | Ok x -> f x
    | Error e -> Error e

/// Map: Apply a function to the success value
let map f result =
    match result with
    | Ok x -> Ok (f x)
    | Error e -> Error e

/// MapError: Apply a function to the error value
let mapError f result =
    match result with
    | Ok x -> Ok x
    | Error e -> Error (f e)

/// Bind operator (>>=)
let (>>=) result f = bind f result

/// Map operator (<!>)
let (<!>) f result = map f result

/// Kleisli composition (>=>)
let (>=>) f g x = f x >>= g
