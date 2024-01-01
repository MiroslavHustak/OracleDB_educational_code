module Triggers

open FsToolkit.ErrorHandling
open Oracle.ManagedDataAccess.Client

(*
ORACLE SQL DEVELOPER

SYS!!!

GRANT CREATE TRIGGER, ALTER ANY TRIGGER TO Test_User;
/

COMMIT;


ORACLE SQL DEVELOPER

Test_User

Triggers

CREATE OR REPLACE TRIGGER my_test_trigger 
BEFORE UPDATE 
OF Quantity
ON ProductionOrder 
FOR EACH ROW
BEGIN 
   IF (:NEW.Quantity + :OLD.Quantity) >= 1.0 THEN
      :NEW.Quantity := 44.0;
   ELSE
      :NEW.Quantity := 68.0;
   END IF;
END;

*)

let private queryCreateTrigger =  //jen zkusebni trigger
    "   
    CREATE OR REPLACE TRIGGER my_test_trigger2 
    BEFORE UPDATE 
    OF Quantity
    ON ProductionOrder 
    FOR EACH ROW
    BEGIN 
       IF (:NEW.Quantity + :OLD.Quantity) >= 1.0 THEN
            :NEW.Quantity := :OLD.Quantity;
       ELSE
            :NEW.Quantity := :OLD.Quantity;
       END IF;
    END;
    " 
    
let internal createTrigger getConnection closeConnection =
    
    try
        let connection: OracleConnection = getConnection()
                 
        try            
            match new OracleCommand(queryCreateTrigger, connection) |> Option.ofNull with
            | None       -> 
                          Error "CreateTriggerError"
            | Some value -> 
                          use cmdCreateStoredProcedure = value
                          
                          cmdCreateStoredProcedure.ExecuteNonQuery() //rowsAffected      
                          |> function                                                         
                              | value when value > 0 -> Ok ()      
                              | _                    -> Error "CreateTriggerError"
        finally
            closeConnection connection
    with
    | ex ->
          printfn "%s" ex.Message
          Error ex.Message


