open System.Data
open Oracle.ManagedDataAccess.Client

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
        description ="Rolled at Q3"
    }

let private productsId4 = 
    {
        productID = 4
        productName = "Slab No.4"
        description = "Rolled at Q4"
    }

let private queryDeleteAll = "DELETE FROM Products"

let private queryInsert =
    sprintf "INSERT INTO Products (ProductID, ProductName, Description) VALUES (:ProductID, :ProductName, :Description)"
                    
let private queryUpdate productID =
    sprintf "UPDATE Products SET ProductName = :ProductName WHERE ProductID = %s" productID

let private insertOrUpdate getConnection closeConnection =

    try
        let connection: OracleConnection = getConnection()
        use cmdDeleteAll = new OracleCommand(queryDeleteAll, connection)
        use cmdInsert = new OracleCommand(queryInsert, connection)
        let cmdUpdate productID = new OracleCommand(queryUpdate productID, connection)
        
        try
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
                                                     use update = cmdUpdate <| string productId  
                                                     update.Parameters.Clear()                                  
                                                     update.Parameters.Add(":ProductName", OracleDbType.Varchar2).Value <- productName
                                                     update.ExecuteNonQuery() 
                         )              
   
                 match rowsAffectedUpdate |> List.sum with 
                 | 0 -> Error "UpdateError" 
                 | _ -> Ok ()  
            
        finally
            closeConnection connection
    with
    | ex -> Error ex.Message

insertOrUpdate getConnection closeConnection |> ignore

