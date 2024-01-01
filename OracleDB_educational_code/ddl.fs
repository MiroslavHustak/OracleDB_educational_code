module DDL

    //A database definition (DDL) statement (CREATE, ALTER, or DROP).
    (*
    CREATE
    ALTER
    DROP
    TRUNCATE
    COMMENT
    RENAME
    CREATE INDEX
    DROP INDEX
    CREATE VIEW
    DROP VIEW
    *)

    (*

    ORACLE SQL DEVELOPER

    //SQL

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

    *)