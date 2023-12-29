open System
open System.Data
open Oracle.ManagedDataAccess.Client

open FsToolkit.ErrorHandling

(*
//tabulky vytvoreny pres Oracle SQL Developer

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
*)

[<Struct>]
type private Builder2 = Builder2 with    
    member _.Bind((optionExpr, errDuCase), nextFunc) =
        match optionExpr with
        | Some value -> nextFunc value 
        | _          -> errDuCase  
    member _.Return x : 'a = x

let private pyramidOfDoom = Builder2

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

            let result =
                pyramidOfDoom 
                     {
                         let! cmdDeleteAll = new OracleCommand(queryDeleteAll, connection) |> Option.ofNull, Error "DeleteError"                                    
                         let! cmdInsert = new OracleCommand(queryInsert, connection) |> Option.ofNull, Error "InsertError"   
                         let cmdUpdate productID = new OracleCommand(queryUpdate productID, connection)
                         //let! cmdUpdate = cmdUpdate |> Option.ofNull, Error "UpdateError" //tohle nerobi to, co bych chtel
                         return Ok (cmdDeleteAll, cmdInsert, cmdUpdate)                     
                     }

            match result with            
            | Error err -> 
                         Error err
            | Ok value  -> 
                         let (cmdDeleteAll, cmdInsert, cmdUpdate) = value
                        
                         use cmdDeleteAll = cmdDeleteAll
                         use cmdInsert = cmdInsert
                        
                         let rowsAffected = 
                             cmdDeleteAll.ExecuteNonQuery()
                             ::                
                             (
                             [ productsId1; productsId2; productsId3; productsId4 ]
                             |> List.map
                                 (fun productsId ->
                                                  cmdInsert.Parameters.Clear() // Clear parameters for each iteration
                                                  cmdInsert.Parameters.Add(":ProductID", OracleDbType.Int32).Value <- productsId.productID
                                                  cmdInsert.Parameters.Add(":ProductName", OracleDbType.Varchar2).Value <- productsId.productName
                                                  cmdInsert.Parameters.Add(":Description", OracleDbType.Varchar2).Value <- productsId.description
                                                  cmdInsert.ExecuteNonQuery() 
                                 )
                             )
               
                         match rowsAffected |> List.sum with 
                         | 0 ->
                              Error "InsertOrDeleteError" 
                         | _ -> 
                              let rowsAffectedUpdate =                
                                  ([ 2; 4 ], ["Slab No.16"; "Slab No.32"])
                                  ||> List.map2
                                      (fun productId productName ->
                                                                  let update = (cmdUpdate <| string productId) |> Option.ofNull
                                                                  match update with
                                                                  | Some update -> 
                                                                                 update.Parameters.Clear()                                  
                                                                                 update.Parameters.Add(":ProductName", OracleDbType.Varchar2).Value <- productName
                                                                                 update.ExecuteNonQuery() 
                                                                  | None        ->
                                                                                 0 //pocet ovlivnenych radku
                                                                 
                                      )              
   
                              match rowsAffectedUpdate |> List.sum with 
                              | 0 -> Error "UpdateError" 
                              | _ -> Ok ()  
            
        finally
            closeConnection connection
    with
    | ex ->
          printfn "%s" ex.Message
          Error ex.Message

insertOrUpdateProducts getConnection closeConnection |> ignore

//OPERATORS
let private queryDeleteAll1 = "DELETE FROM Operators"
let private queryInsert1 = "INSERT INTO Operators (OperatorID, FirstName, LastName, JobTitle) VALUES (:OperatorID, :FirstName, :LastName, :JobTitle)"

let private insertOperators getConnection closeConnection =

    try
        let connection: OracleConnection = getConnection()
             
        try
            let result =
                pyramidOfDoom 
                     {
                         let! cmdDeleteAll = new OracleCommand(queryDeleteAll1, connection) |> Option.ofNull, Error "DeleteError"                                    
                         let! cmdInsert = new OracleCommand(queryInsert1, connection) |> Option.ofNull, Error "InsertError"   

                         return Ok (cmdDeleteAll, cmdInsert)                     
                     }

            match result with            
            | Error err -> 
                         Error err
            | Ok value  -> 
                         let (cmdDeleteAll, cmdInsert) = value
                        
                         use cmdDeleteAll = cmdDeleteAll
                         use cmdInsert = cmdInsert
                        
                         let rowsAffected = 
                             cmdDeleteAll.ExecuteNonQuery()
                             ::                
                             (
                             [ operatorsId1; operatorsId2 ]
                             |> List.map
                                 (fun operatorsId ->
                                                   cmdInsert.Parameters.Clear() // Clear parameters for each iteration
                                                   cmdInsert.Parameters.Add(":OperatorID", OracleDbType.Int32).Value <- operatorsId.operatorID
                                                   cmdInsert.Parameters.Add(":FirstName", OracleDbType.Varchar2).Value <- operatorsId.firstName
                                                   cmdInsert.Parameters.Add(":LastName", OracleDbType.Varchar2).Value <- operatorsId.lastName
                                                   cmdInsert.Parameters.Add(":JobTitle", OracleDbType.Varchar2).Value <- operatorsId.jobTitle
                                                   cmdInsert.ExecuteNonQuery() 
                                 )
                             )
               
                         match rowsAffected |> List.sum with 
                         | 0 -> Error "InsertOrDeleteError"                               
                         | _ -> Ok ()                              
            
        finally
            closeConnection connection
    with
    | ex -> 
          printfn "%s" ex.Message
          Error ex.Message

insertOperators getConnection closeConnection |> ignore

//MACHINES
let private queryDeleteAll2 = "DELETE FROM Machines"
let private queryInsert2 = "INSERT INTO Machines (MachineID, MachineName, Location) VALUES (:MachineID, :MachineName, :Location)"

let private insertMachines getConnection closeConnection =

    try
        let connection: OracleConnection = getConnection()
             
        try
            let result =
                pyramidOfDoom 
                     {
                         let! cmdDeleteAll = new OracleCommand(queryDeleteAll2, connection) |> Option.ofNull, Error "DeleteError"                                    
                         let! cmdInsert = new OracleCommand(queryInsert2, connection) |> Option.ofNull, Error "InsertError"   

                         return Ok (cmdDeleteAll, cmdInsert)                     
                     }

            match result with            
            | Error err -> 
                         Error err
            | Ok value  -> 
                         let (cmdDeleteAll, cmdInsert) = value
                        
                         use cmdDeleteAll = cmdDeleteAll
                         use cmdInsert = cmdInsert
                        
                         let rowsAffected = 
                             cmdDeleteAll.ExecuteNonQuery()
                             ::                
                             (
                             [ machinesId1; machinesId2 ]
                             |> List.map
                                 (fun machinesId ->
                                                  cmdInsert.Parameters.Clear() // Clear parameters for each iteration
                                                  cmdInsert.Parameters.Add(":MachineID", OracleDbType.Int32).Value <- machinesId.machineID
                                                  cmdInsert.Parameters.Add(":MachineName", OracleDbType.Varchar2).Value <- machinesId.machineName
                                                  cmdInsert.Parameters.Add(":Location", OracleDbType.Varchar2).Value <- machinesId.location
                                                  cmdInsert.ExecuteNonQuery() 
                                 )
                             )
               
                         match rowsAffected |> List.sum with 
                         | 0 -> Error "InsertOrDeleteError"                               
                         | _ -> Ok ()                              
            
        finally
            closeConnection connection
    with
    | ex ->
          printfn "%s" ex.Message
          Error ex.Message

insertMachines getConnection closeConnection |> ignore

//PRODUCTIONORDER
let private queryDeleteAll3 = "DELETE FROM ProductionOrder"
let private queryInsert3 = 
    "INSERT INTO ProductionOrder (OrderID, ProductID, OperatorID, MachineID, Quantity, StartTime, EndTime, Status) 
    VALUES (:OrderID, :ProductID, :OperatorID, :MachineID, :Quantity, :StartTime, :EndTime, :Status)"

let private insertProductionOrder getConnection closeConnection =

    try
        let connection: OracleConnection = getConnection()
             
        try
            let result =
                pyramidOfDoom 
                     {
                         let! cmdDeleteAll = new OracleCommand(queryDeleteAll3, connection) |> Option.ofNull, Error "DeleteError"                                    
                         let! cmdInsert = new OracleCommand(queryInsert3, connection) |> Option.ofNull, Error "InsertError"   

                         return Ok (cmdDeleteAll, cmdInsert)                     
                     }

            match result with            
            | Error err -> 
                         Error err
            | Ok value  -> 
                         let (cmdDeleteAll, cmdInsert) = value
                        
                         use cmdDeleteAll = cmdDeleteAll
                         use cmdInsert = cmdInsert
                        
                         let rowsAffected = 
                             cmdDeleteAll.ExecuteNonQuery()
                             ::                
                             (
                             [ productionOrder101; productionOrder102 ]
                             |> List.map
                                 (fun productionOrder ->
                                                      cmdInsert.Parameters.Clear() // Clear parameters for each iteration
                                                      cmdInsert.Parameters.Add(":OrderID", OracleDbType.Int32).Value <- productionOrder.orderID
                                                      cmdInsert.Parameters.Add(":ProductID", OracleDbType.Int32).Value <- productionOrder.productID
                                                      cmdInsert.Parameters.Add(":OperatorID", OracleDbType.Int32).Value <- productionOrder.operatorID
                                                      cmdInsert.Parameters.Add(":MachineID", OracleDbType.Int32).Value <- productionOrder.machineID
                                                      cmdInsert.Parameters.Add(":Quantity", OracleDbType.Double).Value <- productionOrder.quantity
                                                      cmdInsert.Parameters.Add(":StartTime", OracleDbType.Date).Value <- productionOrder.startTime
                                                      cmdInsert.Parameters.Add(":EndTime", OracleDbType.Date).Value <- productionOrder.endTime
                                                      cmdInsert.Parameters.Add(":Status", OracleDbType.Varchar2).Value <- productionOrder.status
                                                      cmdInsert.ExecuteNonQuery() 
                                 )
                             )
               
                         match rowsAffected |> List.sum with 
                         | 0 -> Error "InsertOrDeleteError"                               
                         | _ -> Ok ()                             
            
        finally
            closeConnection connection
    with
    | ex ->
          printfn "%s" ex.Message
          Error ex.Message

insertProductionOrder getConnection closeConnection |> ignore