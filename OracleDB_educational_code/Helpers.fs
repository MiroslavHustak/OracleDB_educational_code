namespace Helpers

open FsToolkit.ErrorHandling

module Casting =

    //for educational purposes only
    let inline internal downCast (x: obj) = //The downCast function does not handle null values explicitly and may raise a runtime exception if x is null regardless of using srtp or generics. 
        match x with
        | :? ^a as value -> Some value 
        | _              -> None

    let inline castAs<'a> (o: obj) : 'a option =
        match Option.ofNull o with
        | Some result when obj.ReferenceEquals(result, null) -> None
        | Some result -> 
            match result with
            | :? 'a as casted -> Some casted
            | _ -> None
        | _ -> None

