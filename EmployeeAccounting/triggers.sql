CREATE TRIGGER trg_Employee_AfterInsert
ON Employee
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO JobHistory (job_title_id, employee_id, start_date)
    SELECT i.job_title_id, i.id, CONVERT(DATE, GETDATE())
    FROM inserted i;
END;
GO

DROP TRIGGER trg_Employee_AfterUpdate;
GO

CREATE TRIGGER trg_Employee_AfterUpdate
ON Employee
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (
        SELECT 1
        FROM Inserted i
        INNER JOIN Deleted d
        ON i.job_title_id <> d.job_title_id
    )
    BEGIN
        UPDATE JobHistory
        SET end_date = CONVERT(DATE, GETDATE())
        WHERE 
            employee_id = (SELECT id FROM Inserted)
            AND end_date IS NULL

        INSERT INTO JobHistory (job_title_id, employee_id, start_date)
        SELECT i.job_title_id, i.id, CONVERT(DATE, GETDATE())
        FROM Inserted i
        INNER JOIN Deleted d
        ON i.job_title_id <> d.job_title_id;
    END;
END;
GO