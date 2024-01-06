module WindowFunctions

    (*
        -- ORACLE SQL DEVELOPER
    
        //Aggregate Window Function
        SELECT
            OperatorID,
            FirstName,
            LastName,
            JobTitle,
        AVG(LENGTH(FirstName)) OVER (PARTITION BY JobTitle) AS AvgFirstNameLength
        FROM
          Operators;

        OPERATORID FIRSTNAME                                          LASTNAME                                           JOBTITLE                                           AVGFIRSTNAMELENGTH
        ---------- -------------------------------------------------- -------------------------------------------------- -------------------------------------------------- ------------------
                 4 John                                               Doe                                                Engineer                                                            4
                 3 Bob                                                Johnson                                            Manager                                                             3
                 6 Bob                                                Johnson                                            Valcíř                                                            4.5
                 1 Jakub                                              Zválcovaný                                         Valcíř                                                            4.5
                 5 Jane                                               Smith                                              Valcíř                                                            4.5
                 2 Donald                                             Válcempřejetý                                      Valcíř                                                            4.5
    

        //Value Window Function
        SELECT
            OperatorID,
            FirstName,
            LastName,
            JobTitle,
        LAG(JobTitle) OVER (ORDER BY OperatorID) AS PreviousJobTitle
        FROM
          Operators;

    
        OPERATORID FIRSTNAME                                          LASTNAME                                           JOBTITLE                                           PREVIOUSJOBTITLE                                  
        ---------- -------------------------------------------------- -------------------------------------------------- -------------------------------------------------- --------------------------------------------------
                 1 Jakub                                              Zválcovaný                                         Valcíř                                                                                               
                 2 Donald                                             Válcempřejetý                                      Valcíř                                             Valcíř                                            
                 3 Bob                                                Johnson                                            Manager                                            Valcíř                                            
                 4 John                                               Doe                                                Engineer                                           Manager                                           
                 5 Jane                                               Smith                                              Valcíř                                             Engineer                                          
                 6 Bob                                                Johnson                                            Valcíř                                             Valcíř                                            
    
      
        //Ranking Window Function
        SELECT
            OperatorID,
            FirstName,
            LastName,
            JobTitle,
        DENSE_RANK() OVER (PARTITION BY JobTitle ORDER BY OperatorID) AS JobTitleRank
        FROM
          Operators;

          OPERATORID FIRSTNAME                                          LASTNAME                                           JOBTITLE                                           JOBTITLERANK
          ---------- -------------------------------------------------- -------------------------------------------------- -------------------------------------------------- ------------
                   4 John                                               Doe                                                Engineer                                                      1
                   3 Bob                                                Johnson                                            Manager                                                       1
                   1 Jakub                                              Zválcovaný                                         Valcíř                                                        1
                   2 Donald                                             Válcempřejetý                                      Valcíř                                                        2
                   5 Jane                                               Smith                                              Valcíř                                                        3
                   6 Bob                                                Johnson                                            Valcíř                                                        4


    Window functions in SQL typically operate over a window of rows defined by the OVER clause, and they produce a result for each row within that window.
    The result is not a single scalar value but is associated with each row in the result set. The window functions are applied to a set of rows related to 


    Aggregate Window Function:
    The AVG function calculates the average length of first names for each job title.
    The result is associated with each row, and you get a result for every row in the output.

    Value Window Function:
    The LAG function retrieves the previous job title for each operator. 
    The result is associated with each row, and you get the previous job title for every operator in the result set.

    Ranking Window Function:
    The DENSE_RANK function assigns a rank to each operator within their job title partition. 
    The result is associated with each row, and you get a rank for every operator in the output.

    *)

