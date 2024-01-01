open DML

open System
open System.Data
open FsToolkit.ErrorHandling
open Oracle.ManagedDataAccess.Client

open Triggers
open StoredProcedures

module Program = 

    [<Literal>]
    let private connectionString = 
        "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=10.0.0.2)(PORT=1521)))(CONNECT_DATA=(SERVICE_NAME=XEPDB1)));User Id=Test_User;Password=Test_User;"
    
    let private getConnection () =
        let connection = new OracleConnection(connectionString)
        connection.Open()
        connection
        
    let private closeConnection (connection: OracleConnection) =
        connection.Close()
        connection.Dispose()
    
    //uncomment what needed

    //insertOrUpdateProductionOrder getConnection closeConnection |> ignore

    //insertOrUpdateProducts getConnection closeConnection |> ignore

    //insertOperators getConnection closeConnection |> ignore

    //insertMachines getConnection closeConnection |> ignore

    //createStoredProcedure getConnection closeConnection |> ignore
    
    //createTrigger getConnection closeConnection |> ignore


    