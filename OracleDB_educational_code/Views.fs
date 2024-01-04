module Views

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
