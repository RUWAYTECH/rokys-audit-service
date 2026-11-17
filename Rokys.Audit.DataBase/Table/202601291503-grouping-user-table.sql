CREATE TABLE GroupingUser
(
    GroupingUserId UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    EnterpriseGroupingId UNIQUEIDENTIFIER NOT NULL,
    UserReferenceId UNIQUEIDENTIFIER NOT NULL,
    RolesCodes NVARCHAR(200) NOT NULL,

    IsActive BIT NOT NULL DEFAULT 1,
    CreatedBy NVARCHAR(100) NOT NULL,
    CreationDate DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    UpdatedBy NVARCHAR(100) NULL,
    UpdateDate DATETIME2 NULL,

    CONSTRAINT PK_GroupingUser PRIMARY KEY (GroupingUserId),

    CONSTRAINT FK_GroupingUser_EnterpriseGrouping
        FOREIGN KEY (EnterpriseGroupingId)
        REFERENCES EnterpriseGrouping (EnterpriseGroupingId),

    CONSTRAINT FK_GroupingUser_UserReference
        FOREIGN KEY (UserReferenceId)
        REFERENCES UserReference (UserReferenceId)
);
