module CTEs

(*
--INSERT INTO Operators (OperatorID, FirstName, LastName, JobTitle) VALUES (4, 'John', 'Doe', 'Engineer');
--INSERT INTO Operators (OperatorID, FirstName, LastName, JobTitle) VALUES (5, 'Jane', 'Smith', 'Technician');
--INSERT INTO Operators (OperatorID, FirstName, LastName, JobTitle) VALUES (6, 'Bob', 'Johnson', 'Manager');

UPDATE Operators
SET JobTitle = 'Engineer'
WHERE OperatorID = 4;

UPDATE Operators
SET JobTitle = 'Valcíř'
WHERE OperatorID = 5;

UPDATE Operators
SET JobTitle = 'Valcíř'
WHERE OperatorID = 6;

-- Common Table Expression 
WITH OperatorCTE AS (
    SELECT OperatorID, FirstName, LastName, JobTitle
    FROM Operators
    WHERE JobTitle = 'Valcíř'
)
SELECT * FROM OperatorCTE;
/

COMMIT;

*)

