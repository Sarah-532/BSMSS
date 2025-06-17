
create database ms
USE ms
GO

CREATE TABLE members
(
	memberId INT IDENTITY PRIMARY KEY,
	memberName NVARCHAR(40) NOT NULL,
	dateOfBirth DATE NOT NULL,
	running BIT,
	picture NVARCHAR(100)
)
GO
DROP TABLE members

CREATE TABLE roles
(
	roleId INT PRIMARY KEY IDENTITY,
	role NVARCHAR(50) NOT NULL,
	assignDate DATE NOT NULL,
	deadline DATE NOT NULL,
	remuneration money not null,
	memberId INT NOT NULL REFERENCES members(memberId)
)
CREATE TABLE supervisor
(
	Id INT PRIMARY KEY,
	Name NVARCHAR(50) NOT NULL,
	Contact NVARCHAR(50) NOT NULL,
	Email NVARCHAR(30) NOT NULL,
	picture VARBINARY(MAX) NULL,
	roleId INT REFERENCES roles(roleId)
)
select s.Id, s.Name, s.Email, s.picture, r.role from supervisor s inner join roles r on s.roleId=r.roleId

CREATE TABLE attendee
(
	id INT IDENTITY PRIMARY KEY,
	[name] VARCHAR(50) NOT NULL,
	city VARCHAR(30) NOT NULL,
	institution VARCHAR(30) NOT NULL,
	gender VARCHAR(20) NOT NULL
)
GO

--sp
CREATE PROC SPattendee 
(
	@Id INT=NULL,
	@name VARCHAR(50)=NULL,
	@city  VARCHAR(30)=NULL,
	@institution VARCHAR(30)=NULL,
	@gender VARCHAR(20)=NULL,
	@actionType VARCHAR(25)
)
AS
BEGIN
	IF @actionType='SaveData'
	BEGIN
		IF NOT EXISTS (SELECT * FROM attendee WHERE id=@Id)
			BEGIN
				INSERT INTO attendee(name,city,institution,gender)
				VALUES(@name,@city,@institution,@gender)
			END
		ELSE
			BEGIN
				UPDATE attendee SET name=@name,city=@city,institution=@institution,gender=@gender WHERE id=@Id
			END
	END
	IF @actionType='DeleteData'
		BEGIN
			DELETE attendee WHERE id=@Id
		END
	IF @actionType='ShowAllData'
		BEGIN
			SELECT * FROM attendee
		END
	IF @actionType='ShowAllDataById'
		BEGIN
			SELECT * FROM attendee WHERE id=@Id
		END
END
GO

SELECT * FROM attendee			   

SELECT * FROM supervisor
select * from members
select * from roles