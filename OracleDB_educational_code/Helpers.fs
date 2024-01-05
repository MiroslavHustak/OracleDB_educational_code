namespace Helpers

open FsToolkit.ErrorHandling

open System
open System.Drawing.Printing

module Casting =

    //for educational purposes only
    let inline internal downCast (x: obj) = //The downCast function does not handle null values explicitly and may raise a runtime exception if x is null regardless of using srtp or generics. 
        match x with
        | :? ^a as value -> Some value 
        | _              -> None

    let internal castAs<'a> (o: obj) : 'a option =    //srtp nefunguje pro tento zpusob type casting 
        match Option.ofNull o with
        | Some (:? 'a as result) -> Some result
        | _                      -> None
