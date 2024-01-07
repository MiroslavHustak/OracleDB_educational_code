module ITVFs

(*

Inline Table-Valued Functions should always return a result set in T-SQL as well as in Oracle SQL.

MS SQL SERVER MANAGEMENT STUDIO

USE [XlxsToDb.ContextClass];

DROP FUNCTION IF EXISTS dbo.MyFunction;
GO

CREATE FUNCTION dbo.MyFunction
(@cid AS INT) RETURNS TABLE
AS
RETURN
	SELECT
		id,
		[name],
		salary,
		departmentId   
	FROM
		Employee1
	WHERE salary > 70000;

**************************************************************************
ORACLE SQL DEVELOPER

Nepodarilo se mi vytvorit ekvivalent v Oracle SQL (table function).

Inline table-valued functions, by definition, return a result set. In both T-SQL and Oracle SQL, an inline table-valued function is 
a function that returns a table-like structure as its result. This result set can be queried and used in the context of a SELECT statement, similar to querying a table.

In T-SQL, an inline table-valued function is typically created using the RETURNS TABLE syntax, and it returns a table structure.

In Oracle SQL, a similar concept is achieved using a table function that returns a result set (commonly using the SYS_REFCURSOR type).
*)

