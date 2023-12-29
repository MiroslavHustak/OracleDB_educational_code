open System.Data
open Oracle.ManagedDataAccess.Client

[<Literal>]
let internal connectionString = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=10.0.0.2)(PORT=1521)))(CONNECT_DATA=(SERVICE_NAME=XEPDB1)));User Id=Test_User;Password=Test_User;";

let internal getConnection () =
    let connection = new OracleConnection(connectionString)
    connection.Open()
    connection
    
let internal closeConnection (connection: OracleConnection) =
    connection.Close()
    connection.Dispose()
              
type Products = 
    {
        productID: int
        productName: string
        description: string
    }

let productsId1 = 
    {
        productID = 1
        productName = "Slab1"
        description = "Rolled at Q1"
    }

let productsId2 = 
    {
        productID = 2
        productName = "Slab2"
        description ="Rolled at Q2"
    }

let productsId3 = 
    {
        productID = 3
        productName = "Slab3"
        description ="Rolled at Q3"
    }

let productsId4 = 
    {
        productID = 4
        productName = "Slab4"
        description = "Rolled at Q4"
    }

let internal queryDeleteAll = "DELETE FROM Products"

let internal queryInsert =
    sprintf "INSERT INTO Products (ProductID, ProductName, Description) VALUES (:ProductID, :ProductName, :Description)"
                    
let internal queryUpdate id =
    sprintf "UPDATE Products SET ProductName = :ProductName, Description = :Description WHERE Id = %s" id

let internal insertOrUpdate getConnection closeConnection =
    try
        let connection: OracleConnection = getConnection()
        use cmdDeleteAll = new OracleCommand(queryDeleteAll, connection)
        use cmdInsert = new OracleCommand(queryInsert, connection)
        
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
            | 0 -> Error "InsertOrDeleteError" 
            | _ -> Ok ()  
            
        finally
            closeConnection connection
    with
    | ex -> Error ex.Message

insertOrUpdate getConnection closeConnection |> ignore

