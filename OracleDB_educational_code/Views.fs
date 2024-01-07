﻿module Views

    (*
    -- Oracle SQL Developer

    -- Standard SQL

    CREATE VIEW ProductionOrderView AS
    SELECT
        po.OrderID,
        po.ProductID,
        pr.ProductName,
        pr.Description AS ProductDescription,
        po.OperatorID,
        op.FirstName || ' ' || op.LastName AS OperatorName,
        op.JobTitle AS OperatorJobTitle,
        po.MachineID,
        ma.MachineName,
        ma.Location AS MachineLocation,
        po.Quantity,
        po.StartTime,
        po.EndTime,
        po.Status
    FROM
        ProductionOrder po
        JOIN Products pr ON po.ProductID = pr.ProductID
        JOIN Operators op ON po.OperatorID = op.OperatorID
        JOIN Machines ma ON po.MachineID = ma.MachineID;
    *)

    (*
    Views:    
    Views are explicitly designed to represent a virtual table based on the result of a SELECT query. 
    Therefore, they are often given a dedicated folder or section in database management tools for easy identification and management.
    
    Window Functions, Derived Tables, CTEs, and Inline TVFs:    
    These constructs are often considered as parts of queries rather than standalone database objects.
    Window functions, derived tables, CTEs, and inline TVFs are elements within a SELECT statement rather than separately stored entities in the database. 
    They are used within the context of a query but do not persist as separate objects.
    The lack of a dedicated folder for these constructs might be because they are transient and exist only for the duration of the query execution.
    *)
