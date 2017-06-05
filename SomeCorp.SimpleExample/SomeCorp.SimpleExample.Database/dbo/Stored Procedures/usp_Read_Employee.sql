-- =============================================
-- Author:		Matt Middleton
-- Create date: 05/06/2017
-- =============================================
CREATE PROCEDURE [usp_Read_Employee] 
	@Id INT,
	@ManagerId INT,
	@FirstName NVARCHAR(128),
	@LastName NVARCHAR(128)
AS
BEGIN

	SET NOCOUNT ON;

	SELECT
		e.Id,
		e.ManagerId,
		e.HireDate,
		e.FirstName,
		e.LastName,
		e.Email,
		e.HomeBased
	FROM Employee AS e
	WHERE
		(
			(@Id IS NULL)
				OR
			(e.Id = @Id)
		)
			AND
		(
			(@ManagerId IS NULL)
				OR
			(e.ManagerId = @ManagerId)
		)
			AND
		(
			(@FirstName IS NULL)
				OR
			(e.FirstName = @FirstName)
		)
			AND
		(
			(@LastName IS NULL)
				OR
			(e.LastName = @LastName)
		);

END