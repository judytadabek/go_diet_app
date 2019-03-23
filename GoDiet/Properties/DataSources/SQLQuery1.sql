CREATE PROC UserAdd
@Username VARCHAR(50),
@Password VARCHAR(50),
@Name VARCHAR(50),
@Surname VARCHAR(50),
@Height INT,
@Weight FLOAT,
@Email VARCHAR(50)
AS
	INSERT INTO tblUsers(Username, Password, Name, Surname, Height, Weight, Email)
	VALUES (@Username, @Password, @Name, @Surname, @Height, @Weight, @Email)
