-- =============================================
-- Author:		Matt Middleton
-- Create date: 05/06/2017
-- =============================================
CREATE PROCEDURE [usp_Update_Employee] 
	@Id INT,
	@Email NVARCHAR(256),
	@HomeBased BIT
AS
BEGIN

	SET NOCOUNT ON;

	UPDATE Employee
	SET
		Email = ISNULL(Email, @Email),
		HomeBased = ISNULL(HomeBased, @HomeBased)
	WHERE
		Id = @Id;

END