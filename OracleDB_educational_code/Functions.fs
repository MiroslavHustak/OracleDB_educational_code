module Functions

open FsToolkit.ErrorHandling
open Oracle.ManagedDataAccess.Client

    (*
        -- Oracle SQL Developer
        --PL/SQL
        
        CREATE OR REPLACE FUNCTION TESTINGFUNCTION 
        RETURN NUMBER AS --standalone (AS), vraci hodnotu
           total NUMBER := 100; 
        BEGIN
           RETURN total; 
        END TESTINGFUNCTION;

        -- Running the function in a worksheet (as a standalone function)
        SELECT TESTINGFUNCTION() FROM DUAL;

        -- Running the function in PL/SQL

        CREATE OR REPLACE FUNCTION TESTINGFUNCTION 
        RETURN NUMBER IS --from a PL/SQL block (IS) , vraci hodnotu
           total NUMBER := 100; 
        BEGIN
           RETURN total; 
        END TESTINGFUNCTION;
          
    *)

let private queryCreateFunction =  //jen zkusebni function
    "   
    CREATE OR REPLACE FUNCTION TESTINGFUNCTION 
    RETURN NUMBER IS --from a PL/SQL block (IS) , vraci hodnotu
       total NUMBER := 100; 
    BEGIN
       RETURN total; 
    END TESTINGFUNCTION;
    " 
    
let internal createFunction getConnection closeConnection =
    
    try
        let connection: OracleConnection = getConnection()
                 
        try            
            match new OracleCommand(queryCreateFunction, connection) |> Option.ofNull with
            | None       -> 
                          Error "FunctionError"
            | Some value -> 
                          use cmdCreateFunction = value                            
                         
                          cmdCreateFunction.ExecuteNonQuery()  //rowsAffected 
                          |> function                                                              
                              | value when value > 0 -> Ok ()      
                              | _                    -> Error "FunctionError"    
        finally
            closeConnection connection
    with
    | ex ->
          printfn "%s" ex.Message
          Error ex.Message