CREATE TABLE [dbo].[Employee] (
    [Id]        INT            IDENTITY (1, 1) NOT NULL,
    [ManagerId] INT            NULL,
    [HireDate]  DATETIME       NOT NULL,
    [FirstName] NVARCHAR (128) NOT NULL,
    [LastName]  NVARCHAR (128) NOT NULL,
    [Email]     NVARCHAR (256) NOT NULL,
    [HomeBased] BIT            NOT NULL,
    CONSTRAINT [PK_Employee] PRIMARY KEY CLUSTERED ([Id] ASC)
);