-- =============================================
-- Author:		Matt Middleton
-- Create date: 05/06/2017
-- =============================================
CREATE PROCEDURE [usp_Create_Employee] 
	@ManagerId INT, 
	@HireDate DATETIME,
	@FirstName NVARCHAR(128),
	@LastName NVARCHAR(128),
	@Email NVARCHAR(256),
	@HomeBased BIT 
AS
BEGIN

	SET NOCOUNT ON;

	INSERT INTO Employee
	(
		ManagerId,
		HireDate,
		FirstName,
		LastName,
		Email,
		HomeBased
	)
	OUTPUT inserted.Id
	VALUES
	(
		@ManagerId,
		@HireDate,
		@FirstName,
		@LastName,
		@Email,
		@HomeBased
	);

END