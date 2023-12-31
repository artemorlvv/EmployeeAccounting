CREATE TABLE JobTitle (
	id INT IDENTITY PRIMARY KEY,
	name NVARCHAR(100) NOT NULL UNIQUE,
	description NVARCHAR(255),
	salary INT NOT NULL
);

CREATE TABLE Employee (
	id INT IDENTITY PRIMARY KEY,
	job_title_id INT REFERENCES JobTitle(id) NOT NULL,
	full_name NVARCHAR(100) NOT NULL,
	phone_number NVARCHAR(30) NOT NULL UNIQUE CHECK (phone_number LIKE '+7([0-9][0-9][0-9])[0-9][0-9][0-9]-[0-9][0-9]-[0-9][0-9]')
);

CREATE TABLE JobHistory (
	id INT IDENTITY PRIMARY KEY,
	job_title_id INT REFERENCES JobTitle(id) NOT NULL,
	employee_id INT NOT NULL,
	start_date DATE NOT NULL,
	end_date DATE,
	CONSTRAINT fk_jh_employee_id
	FOREIGN KEY (employee_id) REFERENCES Employee(id) ON DELETE CASCADE
);

DROP TABLE JobHistory;
DROP TABLE Employee;
DROP TABLE JobTitle;

INSERT INTO JobTitle (name, description, salary)
VALUES
	(N'Junior Frontend Android', N'Junior Разработчик мобильного ПО под ОС Android', 35000),
	(N'Junior Frontend', N'Junior Frontend разработчик web-сайтов', 33000),
	(N'Middle Frontend Android', N' MiddleРазработчик мобильного ПО под ОС Android', 75000),
	(N'Middle Frontend', N'Middle Frontend разработчик web-сайтов', 72500);

INSERT INTO Employee (job_title_id, full_name, phone_number)
VALUES 
	((SELECT id FROM JobTitle WHERE name = N'Junior Frontend Android'), N'Петров П.П.', N'+7(923)284-33-42'),
	((SELECT id FROM JobTitle WHERE name = N'Junior Frontend'), N'Васильев В.В.', N'+7(963)784-63-46'),
	((SELECT id FROM JobTitle WHERE name = N'Junior Frontend'), N'Сергеев С.С.', N'+7(977)211-33-43');

DELETE FROM Employee WHERE phone_number =  N'+7(977)211-33-43';

UPDATE Employee
SET job_title_id = (SELECT id FROM JobTitle WHERE name = N'Junior Frontend Android')
WHERE phone_number = N'+7(963)784-63-46'
	
SELECT * FROM JobTitle;
SELECT * FROM Employee;
SELECT * FROM JobHistory;