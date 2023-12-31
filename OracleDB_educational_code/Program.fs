﻿
//SCROLL DOWN FOR F# CODE

(*

//PL/SQL

//tabulky vytvorene pres Oracle SQL Developer

CREATE TABLE Products (
    ProductID INT NOT NULL PRIMARY KEY,
    ProductName VARCHAR2(50),
    Description VARCHAR2(200)
);

CREATE TABLE Operators (
    OperatorID INT NOT NULL PRIMARY KEY,
    FirstName VARCHAR2(50),
    LastName VARCHAR2(50),
    JobTitle VARCHAR2(50)
);

CREATE TABLE Machines (
    MachineID INT NOT NULL PRIMARY KEY,
    MachineName VARCHAR2(50),
    Location VARCHAR2(100)
);

DROP TABLE ProductionOrder;

CREATE TABLE ProductionOrder (
    OrderID INT NOT NULL PRIMARY KEY,
    ProductID INT NOT NULL,
    OperatorID INT NOT NULL,
    MachineID INT NOT NULL,
    Quantity NUMBER,
    StartTime TIMESTAMP,
    EndTime TIMESTAMP,
    Status VARCHAR2(20),    
    CONSTRAINT fk_Product FOREIGN KEY (ProductID) REFERENCES Products(ProductID),
    CONSTRAINT fk_Operator FOREIGN KEY (OperatorID) REFERENCES Operators(OperatorID),
    CONSTRAINT fk_Machine FOREIGN KEY (MachineID) REFERENCES Machines(MachineID)
);

SYS

GRANT DEBUG CONNECT SESSION TO Test_User;
GRANT DEBUG ANY PROCEDURE TO Test_User;

Test_User

Stored Procedure

-- Enable DBMS_OUTPUT
SET SERVEROUTPUT ON;

-- Create or replace the procedure
CREATE OR REPLACE PROCEDURE testingProcedure 
(
    text_param IN VARCHAR2,
    number_param IN NUMBER
)
AS
    i NUMBER := 10;
BEGIN
    DBMS_OUTPUT.PUT_LINE('Ahoj');
    IF i = 10 THEN
        DBMS_OUTPUT.PUT_LINE(text_param);
    ELSIF i != 10 THEN
        DBMS_OUTPUT.PUT_LINE(TO_CHAR(number_param)); -- Converting NUMBER to VARCHAR2
    ELSE
        DBMS_OUTPUT.PUT_LINE('NULL1 (UNKNOWN1)');
    END IF;
END testingProcedure;
/

-- Running the stored procedure in a worksheet
BEGIN
    testingProcedure(text_param => 'Bleep', number_param => 42);
END;
/

-- Create or replace the procedure
CREATE OR REPLACE PROCEDURE quantityAdapter 
(
    old_quantity IN NUMBER,
    new_quantity IN NUMBER
)
AS
    limit NUMBER := 100;
BEGIN    
    IF limit >= old_quantity THEN
       UPDATE ProductionOrder SET Quantity = new_quantity WHERE OrderID = 103;
    ELSE
       UPDATE ProductionOrder SET Quantity = 6 WHERE OrderID = 103;
    END IF;
END quantityAdapter;

-- Running the stored procedure in a worksheet
DECLARE
    quantity_sum NUMBER;
    addValue NUMBER;
BEGIN
    SELECT SUM(quantity) INTO quantity_sum
    FROM ProductionOrder
    WHERE quantity IS NOT NULL;

    addValue := quantity_sum + 20.0;

    quantityAdapter(old_quantity => quantity_sum, new_quantity => addValue);

    COMMIT;
END;
/
*)

open System
open System.Data
open FsToolkit.ErrorHandling
open Oracle.ManagedDataAccess.Client

[<Struct>]
type private Builder2 = Builder2 with    
    member _.Bind((optionExpr, errDuCase), nextFunc) =
        match optionExpr with
        | Some value -> nextFunc value 
        | _          -> errDuCase  
    member _.Return x : 'a = x

let private pyramidOfDoom = Builder2

//zatim nevyuzito
type private Builder(errDuCase) =     
    member _.Bind(condition, nextFunc) =
        match condition with
        | Some value -> nextFunc value 
        | _          -> errDuCase  
    member _.Return x : 'a = x

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
              
type private Products = 
    {
        productID: int
        productName: string
        description: string
    }

let private productsId1 = 
    {
        productID = 1
        productName = "Slab No.1"
        description = "Rolled at Q1"
    }

let private productsId2 = 
    {
        productID = 2
        productName = "Slab No.2"
        description ="Rolled at Q2"
    }

let private productsId3 = 
    {
        productID = 3
        productName = "Slab No.3"
        description ="Rolled at Q1"
    }

let private productsId4 = 
    {
        productID = 4
        productName = "Slab No.4"
        description = "Rolled at Q1"
    }

type private Operators = 
    {
        operatorID: int
        firstName: string
        lastName: string
        jobTitle: string
    }

let private operatorsId1 = 
    {
        operatorID = 1
        firstName = "Jakub"
        lastName = "Zválcovaný"
        jobTitle = "Valcíř"
    }

let private operatorsId2 = 
    {
        operatorID = 2
        firstName = "Donald"
        lastName = "Válcempřejetý"
        jobTitle = "Valcíř"
    }

type private Machines = 
    {
        machineID: int
        machineName: string
        location: string
    }

let private machinesId1 = 
    {
        machineID = 1
        machineName = "Q1"
        location = "240"
    }

let private machinesId2 = 
    {
        machineID = 2
        machineName = "Q2"
        location = "240"
    }

type private ProductionOrder =
    {
        orderID: int
        productID: int  //REFERENCES Products(ProductID),
        operatorID: int //REFERENCES Operators(OperatorID),
        machineID: int  //RFERENCES Machines(MachineID)
        quantity: float
        startTime: DateTime
        endTime: DateTime
        status: string   
    }

let private productionOrder101 =
    {
        orderID = 101
        productID = 1
        operatorID = 1
        machineID = 1
        quantity = 2.0
        startTime = new DateTime(2008, 5, 1, 8, 30, 52)
        endTime = new DateTime(2009, 5, 1, 8, 30, 52)
        status = "OK"
    }

let private productionOrder102 =
    {
        orderID = 102
        productID = 2
        operatorID = 2
        machineID = 2
        quantity = 8.0
        startTime = new DateTime(2008, 5, 1, 8, 30, 52)
        endTime = new DateTime(2009, 5, 1, 8, 30, 52)
        status = "OK"
    }

let private productionOrder103 =
    {
        orderID = 103
        productID = 2
        operatorID = 2
        machineID = 2
        quantity = 0.0
        startTime = new DateTime(2008, 5, 1, 8, 30, 52)
        endTime = new DateTime(2009, 5, 1, 8, 30, 52)
        status = "Open"
    }

//PRODUCTIONORDER
//You cannot delete records from the parent tables (Products, Operators, Machines) if there are corresponding child records in the ProductionOrder table.
//To resolve this issue, delete ProductionOrder records first (ale potom do te tabulky nestrkej znovu data, jako v mem pripade)

let private queryDeleteAll3 = "DELETE FROM ProductionOrder"

let private queryInsert3 = 
    "
    INSERT INTO ProductionOrder (OrderID, ProductID, OperatorID, MachineID, Quantity, StartTime, EndTime, Status) 
    VALUES (:OrderID, :ProductID, :OperatorID, :MachineID, :Quantity, :StartTime, :EndTime, :Status)
    "

let private queryUpdate3 = 
    "
    DECLARE
        quantity_sum NUMBER;
        addValue NUMBER;
    BEGIN
        SELECT SUM(quantity) AS quantity_sum -- AS pro nazev sloupce
        INTO quantity_sum -- INTO pro vlozeni vysledku do promenne
        FROM ProductionOrder
        WHERE quantity IS NOT NULL;

        -- Stored Procedure obsahuje UPDATE
        addValue := quantity_sum + 1.0;
        quantityAdapter(old_quantity => quantity_sum, new_quantity => addValue);
    END;
    " 
    
let private insertOrUpdateProductionOrder getConnection closeConnection =
    
    try
        let connection: OracleConnection = getConnection()
                 
        try
            
            pyramidOfDoom 
                {
                    let! cmdDeleteAll = new OracleCommand(queryDeleteAll3, connection) |> Option.ofNull, Error "DeleteError"                                    
                    let! cmdInsert = new OracleCommand(queryInsert3, connection) |> Option.ofNull, Error "InsertError"   
                    let! cmdUpdate = new OracleCommand(queryUpdate3, connection) |> Option.ofNull, Error "UpdateError"   
    
                    return Ok (cmdDeleteAll, cmdInsert, cmdUpdate)                     
                }
    
            |> function            
                | Error err -> 
                             Error err
                | Ok value  -> 
                             let (cmdDeleteAll, cmdInsert, cmdUpdate) = value
                            
                             use cmdDeleteAll = cmdDeleteAll
                             use cmdInsert = cmdInsert
                             use cmdUpdate = cmdUpdate
                            
                             cmdDeleteAll.ExecuteNonQuery()
                             ::                
                             (
                                 [ productionOrder101; productionOrder102; productionOrder103 ]
                                 |> List.map
                                     (fun p_Order ->
                                                   cmdInsert.Parameters.Clear() // Clear parameters for each iteration
                                                   cmdInsert.Parameters.Add(":OrderID", OracleDbType.Int32).Value <- p_Order.orderID
                                                   cmdInsert.Parameters.Add(":ProductID", OracleDbType.Int32).Value <- p_Order.productID
                                                   cmdInsert.Parameters.Add(":OperatorID", OracleDbType.Int32).Value <- p_Order.operatorID
                                                   cmdInsert.Parameters.Add(":MachineID", OracleDbType.Int32).Value <- p_Order.machineID
                                                   cmdInsert.Parameters.Add(":Quantity", OracleDbType.Double).Value <- p_Order.quantity
                                                   cmdInsert.Parameters.Add(":StartTime", OracleDbType.Date).Value <- p_Order.startTime
                                                   cmdInsert.Parameters.Add(":EndTime", OracleDbType.Date).Value <- p_Order.endTime
                                                   cmdInsert.Parameters.Add(":Status", OracleDbType.Varchar2).Value <- p_Order.status
                                                   cmdInsert.ExecuteNonQuery() 
                                     )
                             ) 
                             |> List.sum
                             |> function   //rowsAffected 
                                 | 0 -> 
                                      Error "InsertOrDeleteError"                               
                                 | _ -> 
                                      match cmdUpdate.ExecuteNonQuery() with  //rowsAffectedUpdate 
                                      | 0 -> Error "UpdateError"                               
                                      | _ -> Ok ()      
        finally
            closeConnection connection
    with
    | ex ->
          printfn "%s" ex.Message
          Error ex.Message
    
insertOrUpdateProductionOrder getConnection closeConnection |> ignore

//PRODUCTS
let private queryDeleteAll = "DELETE FROM Products"
let private queryInsert = "INSERT INTO Products (ProductID, ProductName, Description) VALUES (:ProductID, :ProductName, :Description)"                    
let private queryUpdate productID = sprintf "UPDATE Products SET ProductName = :ProductName WHERE ProductID = %s" productID

let private insertOrUpdateProducts getConnection closeConnection =

    try
        let connection: OracleConnection = getConnection()
             
        try
            //use cmdDeleteAll = new OracleCommand(queryDeleteAll, connection)
            //use cmdInsert = new OracleCommand(queryInsert, connection)            
                       
            pyramidOfDoom 
                {
                    let! cmdDeleteAll = new OracleCommand(queryDeleteAll, connection) |> Option.ofNull, Error "DeleteError"                                    
                    let! cmdInsert = new OracleCommand(queryInsert, connection) |> Option.ofNull, Error "InsertError"   
                    let cmdUpdate productID = new OracleCommand(queryUpdate productID, connection)
                    //let! cmdUpdate = cmdUpdate |> Option.ofNull, Error "UpdateError" //tohle nerobi to, co ocekavam (partially applied fn nelze zachytit)
                    return Ok (cmdDeleteAll, cmdInsert, cmdUpdate)                     
                }

            |> function          
                | Error err -> 
                             Error err
                | Ok value  -> 
                             let (cmdDeleteAll, cmdInsert, cmdUpdate) = value
                        
                             use cmdDeleteAll = cmdDeleteAll
                             use cmdInsert = cmdInsert
                                                    
                             cmdDeleteAll.ExecuteNonQuery()
                             ::                
                             (
                                 [ productsId1; productsId2; productsId3; productsId4 ]
                                 |> List.map
                                     (fun p_Id ->
                                                cmdInsert.Parameters.Clear() // Clear parameters for each iteration
                                                cmdInsert.Parameters.Add(":ProductID", OracleDbType.Int32).Value <- p_Id.productID
                                                cmdInsert.Parameters.Add(":ProductName", OracleDbType.Varchar2).Value <- p_Id.productName
                                                cmdInsert.Parameters.Add(":Description", OracleDbType.Varchar2).Value <- p_Id.description
                                                cmdInsert.ExecuteNonQuery() 
                                     )
                             ) 
                             |> List.sum               
                             |> function   //rowsAffected
                                 | 0 ->
                                      Error "InsertOrDeleteError" 
                                 | _ ->
                                      ([ productsId2.productID; productsId4.productID ], ["Slab No.16"; "Slab No.32"])
                                      ||> List.map2
                                          (fun p_Id p_Name ->
                                                            let update = cmdUpdate >> Option.ofNull
                                                            match update <| string p_Id with
                                                            | Some update -> 
                                                                            use update = update  
                                                                            update.Parameters.Clear()                                  
                                                                            update.Parameters.Add(":ProductName", OracleDbType.Varchar2).Value <- p_Name
                                                                            update.ExecuteNonQuery() 
                                                            | None        ->
                                                                            0 //pocet ovlivnenych radku
                                                                 
                                          ) 
                                          |> List.sum    
                                          |> function   //rowsAffectedUpdate
                                              | 0 -> Error "UpdateError" 
                                              | _ -> Ok () 
        finally
            closeConnection connection
    with
    | ex ->
          printfn "%s" ex.Message
          Error ex.Message

//insertOrUpdateProducts getConnection closeConnection |> ignore

//OPERATORS
let private queryDeleteAll1 = "DELETE FROM Operators"
let private queryInsert1 = "INSERT INTO Operators (OperatorID, FirstName, LastName, JobTitle) VALUES (:OperatorID, :FirstName, :LastName, :JobTitle)"

let private insertOperators getConnection closeConnection =

    try
        let connection: OracleConnection = getConnection()
             
        try
            
            pyramidOfDoom 
                {
                    let! cmdDeleteAll = new OracleCommand(queryDeleteAll1, connection) |> Option.ofNull, Error "DeleteError"                                    
                    let! cmdInsert = new OracleCommand(queryInsert1, connection) |> Option.ofNull, Error "InsertError"   

                    return Ok (cmdDeleteAll, cmdInsert)                     
                }

            |> function           
                | Error err -> 
                             Error err
                | Ok value  -> 
                             let (cmdDeleteAll, cmdInsert) = value
                        
                             use cmdDeleteAll = cmdDeleteAll
                             use cmdInsert = cmdInsert                        
                              
                             cmdDeleteAll.ExecuteNonQuery()
                             ::                
                             (
                                 [ operatorsId1; operatorsId2 ]
                                 |> List.map
                                     (fun o_Id ->
                                                cmdInsert.Parameters.Clear() // Clear parameters for each iteration
                                                cmdInsert.Parameters.Add(":OperatorID", OracleDbType.Int32).Value <- o_Id.operatorID
                                                cmdInsert.Parameters.Add(":FirstName", OracleDbType.Varchar2).Value <- o_Id.firstName
                                                cmdInsert.Parameters.Add(":LastName", OracleDbType.Varchar2).Value <- o_Id.lastName
                                                cmdInsert.Parameters.Add(":JobTitle", OracleDbType.Varchar2).Value <- o_Id.jobTitle
                                                cmdInsert.ExecuteNonQuery() 
                                     ) 
                             ) 
                             |> List.sum
                             |> function   //rowsAffected 
                                 | 0 -> Error "InsertOrDeleteError"                               
                                 | _ -> Ok ()       
        finally
            closeConnection connection
    with
    | ex -> 
          printfn "%s" ex.Message
          Error ex.Message

//insertOperators getConnection closeConnection |> ignore

//MACHINES
let private queryDeleteAll2 = "DELETE FROM Machines"
let private queryInsert2 = "INSERT INTO Machines (MachineID, MachineName, Location) VALUES (:MachineID, :MachineName, :Location)"

let private insertMachines getConnection closeConnection =

    try
        let connection: OracleConnection = getConnection()
             
        try
            
            pyramidOfDoom 
                {
                    let! cmdDeleteAll = new OracleCommand(queryDeleteAll2, connection) |> Option.ofNull, Error "DeleteError"                                    
                    let! cmdInsert = new OracleCommand(queryInsert2, connection) |> Option.ofNull, Error "InsertError"   

                    return Ok (cmdDeleteAll, cmdInsert)                     
                }

            |> function        
                | Error err -> 
                             Error err
                | Ok value  -> 
                             let (cmdDeleteAll, cmdInsert) = value
                        
                             use cmdDeleteAll = cmdDeleteAll
                             use cmdInsert = cmdInsert
                        
                             cmdDeleteAll.ExecuteNonQuery()
                             ::                
                             (
                                 [ machinesId1; machinesId2 ]
                                 |> List.map
                                     (fun m_Id ->
                                                cmdInsert.Parameters.Clear() // Clear parameters for each iteration
                                                cmdInsert.Parameters.Add(":MachineID", OracleDbType.Int32).Value <- m_Id.machineID
                                                cmdInsert.Parameters.Add(":MachineName", OracleDbType.Varchar2).Value <- m_Id.machineName
                                                cmdInsert.Parameters.Add(":Location", OracleDbType.Varchar2).Value <- m_Id.location
                                                cmdInsert.ExecuteNonQuery() 
                                     )
                             ) 
                             |> List.sum
                             |> function   //rowsAffected                             
                                 | 0 -> Error "InsertOrDeleteError"                               
                                 | _ -> Ok ()  
        finally
            closeConnection connection
    with
    | ex ->
          printfn "%s" ex.Message
          Error ex.Message

//insertMachines getConnection closeConnection |> ignore

