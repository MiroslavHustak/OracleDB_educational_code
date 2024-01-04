
module CTEs

(*

-- ORACLE SQL DEVELOPER

-- INSERT INTO Operators (OperatorID, FirstName, LastName, JobTitle) VALUES (4, 'John', 'Doe', 'Engineer');
-- INSERT INTO Operators (OperatorID, FirstName, LastName, JobTitle) VALUES (5, 'Jane', 'Smith', 'Technician');
-- INSERT INTO Operators (OperatorID, FirstName, LastName, JobTitle) VALUES (6, 'Bob', 'Johnson', 'Manager');

UPDATE Operators
SET JobTitle = 'Engineer'
WHERE OperatorID = 4;

UPDATE Operators
SET JobTitle = 'Valcíř'
WHERE OperatorID = 5;

UPDATE Operators
SET JobTitle = 'Valcíř'
WHERE OperatorID = 6;

-- Common Table Expression 
WITH OperatorCTE AS (
    SELECT OperatorID, FirstName, LastName, JobTitle
    FROM Operators
    WHERE JobTitle = 'Valcíř'
)
SELECT * FROM OperatorCTE;
/

COMMIT;

*)
open Helpers
open FsToolkit.ErrorHandling
open Oracle.ManagedDataAccess.Client
open System

[<Struct>]
type private Builder2 = Builder2 with    
    member _.Bind((optionExpr, errDuCase), nextFunc) =
        match optionExpr with
        | Some value -> nextFunc value 
        | _          -> errDuCase  
    member _.Return x : 'a = x

let private pyramidOfDoom = Builder2

let queryExists = @"SELECT 1 FROM user_tables WHERE table_name = 'Operators';"
let querySelect = 
    @"
     WITH OperatorCTE AS
     (
         SELECT OperatorID, FirstName, LastName, JobTitle
         FROM Operators
         WHERE JobTitle = 'Valcíř'
     )
     SELECT * FROM OperatorCTE
    "

let internal selectValues getConnection closeConnection =

    try
        let connection: OracleConnection = getConnection()
             
        try
        
            pyramidOfDoom 
                {
                    let! cmdExists = new OracleCommand(queryExists, connection) |> Option.ofNull, Error "ExistsError"                                    
                    let! cmdSelect = new OracleCommand(querySelect, connection) |> Option.ofNull, Error "SelectError"   

                    return Ok (cmdExists, cmdSelect)                     
                }

            |> function            
                | Error err -> 
                             Error err
                | Ok value  -> 
                             let (cmdExists, cmdSelect) = value
                        
                             use cmdExists = cmdExists
                             use cmdSelect = cmdSelect
                                                      
                             let reader =
                                 //match cmdExists.ExecuteScalar() |> Option.ofNull with //TODO
                                 match Some 1 with //TODO                                 
                                 | Some _ ->
                                           match cmdSelect.ExecuteReader() |> Option.ofNull with
                                           | Some reader -> Ok reader
                                           | None        -> Error "ReaderError"
                                          
                                 | None   ->
                                           Error "Operators table not existing"     
                             
                             match reader with
                             | Ok reader ->
                                         let getValues =                                                 
                                             Seq.initInfinite (fun _ -> reader.Read() && reader.HasRows = true)
                                             |> Seq.takeWhile ((=) true) 
                                             |> Seq.collect
                                                 (fun _ ->
                                                         seq 
                                                             {                                                 
                                                                  //Casting.castAs<Int32> reader.["OperatorID"] |> Option.map string  //TODO      
                                                                  Casting.downCast reader.["OperatorID"] |> Option.map string  //TODO  
                                                                  //Some (string <| reader.["OperatorID"])      //TODO
                                                                  Casting.castAs<string> reader.["FirstName"]                                                                               
                                                                  Casting.castAs<string> reader.["LastName"]
                                                                  Casting.castAs<string> reader.["JobTitle"]   
                                                             } 
                                                 ) 
                                         
                                         getValues |> Seq.iter (fun item -> printfn "%A" item) //TODO
                                         
                                         let getValues = 
                                             match getValues |> Seq.forall (fun item -> item.IsSome) with
                                             | true  -> Ok (getValues |> Seq.choose (fun item -> item))                                       
                                             | false -> Error "ReadingDbError"  
                                             
                                         reader.Close() 
                                         reader.Dispose() 

                                         getValues 
                                        
                             | Error err ->
                                         Error err                            
        finally
            closeConnection connection
    with
    | ex ->
          printfn "%s" ex.Message
          Error ex.Message
    
    