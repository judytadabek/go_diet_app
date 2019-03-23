CREATE TABLE [dbo].[tblUsers] (
    [Username] VARCHAR (50) NOT NULL,
    [Password] VARCHAR (50) NOT NULL,
    [Gender]     VARCHAR (50) NULL,
    [Vegetarian]  VARCHAR (50) NULL,
    PRIMARY KEY CLUSTERED ([Username] ASC)
);

