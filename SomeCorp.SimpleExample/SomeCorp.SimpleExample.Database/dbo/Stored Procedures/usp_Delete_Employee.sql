-- =============================================
-- Author:		Matt Middleton
-- Create date: 05/06/2017
-- =============================================
CREATE PROCEDURE [usp_Delete_Employee] 
	@Id INT
AS
BEGIN

	SET NOCOUNT ON;

	DELETE FROM Employee
	WHERE Id = @Id;

END