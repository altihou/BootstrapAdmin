﻿CREATE TABLE Users (
    [ID]             INTEGER       PRIMARY KEY,
    [UserName]       NVARCHAR (50) NOT NULL,
    [Password]       VARCHAR (50)  NOT NULL,
    [PassSalt]       VARCHAR (50)  NOT NULL,
    [DisplayName]    VARCHAR (50)  NOT NULL,
    [RegisterTime]   DATETIME      NOT NULL,
    [ApprovedTime]   DATETIME,
    [ApprovedBy]     VARCHAR (50),
    [Description]    VARCHAR (500) NOT NULL,
    [RejectedBy]     VARCHAR (50),
    [RejectedTime]   DATETIME,
    [RejectedReason] VARCHAR (50),
    [Icon]           VARCHAR (50),
    [Css]            VARCHAR (50) 
);

CREATE TABLE UserRole (
	[ID] INTEGER PRIMARY KEY,
	[UserID] INT NOT NULL,
	[RoleID] INT NOT NULL
);

CREATE TABLE UserGroup(
	[ID] INTEGER PRIMARY KEY,
	[UserID] INT NOT NULL,
	[GroupID] INT NOT NULL
);

CREATE TABLE Roles(
	[ID] INTEGER PRIMARY KEY,
	[RoleName] VARCHAR (50) NULL,
	[Description] VARCHAR (500) NULL
);

CREATE TABLE RoleGroup(
	[ID] INTEGER PRIMARY KEY,
	[RoleID] INT NOT NULL,
	[GroupID] INT NOT NULL
);

CREATE TABLE Notifications(
	[ID] INTEGER PRIMARY KEY,
	[Category] VARCHAR (50) NOT NULL,
	[Title] VARCHAR (50) NOT NULL,
	[Content] VARCHAR (50) NOT NULL,
	[RegisterTime] DATETIME NOT NULL,
	[ProcessTime] DATETIME NULL,
	[ProcessBy] VARCHAR (50) NULL,
	[ProcessResult] VARCHAR (50) NULL,
	[Status] VARCHAR (50) DEFAULT (0)
);

CREATE TABLE Navigations(
	[ID] INTEGER PRIMARY KEY,
	[ParentId] INT DEFAULT (0),
	[Name] VARCHAR (50) NOT NULL,
	[Order] INT NOT NULL DEFAULT (0),
	[Icon] VARCHAR (50) DEFAULT none,
	[Url] VARCHAR (4000) NULL,
	[Category] VARCHAR (50) DEFAULT 0,
	[Target] VARCHAR (10) DEFAULT _self,
	[IsResource] INT DEFAULT (0),
	[Application] VARCHAR (200) DEFAULT (0)
);

CREATE TABLE NavigationRole(
	[ID] INTEGER PRIMARY KEY,
	[NavigationID] INT NOT NULL,
	[RoleID] INT NOT NULL
);

CREATE TABLE Logs(
	[ID] INTEGER PRIMARY KEY,
	[CRUD] VARCHAR (50) NOT NULL,
	[UserName] VARCHAR (50) NOT NULL,
	[LogTime] DATETIME NOT NULL,
	[ClientIp] VARCHAR (15) NOT NULL,
	[ClientAgent] VARCHAR (500) NOT NULL,
	[RequestUrl] VARCHAR (500) NOT NULL
);

CREATE TABLE Groups(
	[ID] INTEGER PRIMARY KEY,
	[GroupName] VARCHAR (50) NULL,
	[Description] VARCHAR (500) NULL
);

CREATE TABLE Exceptions(
	[ID] INTEGER PRIMARY KEY,
	[AppDomainName] VARCHAR (50) NOT NULL,
	[ErrorPage] VARCHAR (50) NOT NULL,
	[UserID] VARCHAR (50) NULL,
	[UserIp] VARCHAR (15) NULL,
	[ExceptionType] TEXT NOT NULL,
	[Message] TEXT NOT NULL,
	[StackTrace] TEXT NULL,
	[LogTime] DATETIME NOT NULL
);

CREATE TABLE Dicts(
	[ID] INTEGER PRIMARY KEY,
	[Category] VARCHAR (50) NOT NULL,
	[Name] VARCHAR (50) NOT NULL,
	[Code] VARCHAR (500) NOT NULL,
	[Define] INT NOT NULL DEFAULT (1)
);

CREATE TABLE Messages(
	[ID] INTEGER PRIMARY KEY,
	[Title] VARCHAR (50) NOT NULL,
	[Content] VARCHAR (500) NOT NULL,
	[From] VARCHAR (50) NOT NULL,
	[To] VARCHAR (50) NOT NULL,
	[SendTime] DATETIME NOT NULL,
	[Status] VARCHAR (50) NOT NULL,
	[Flag] INT DEFAULT (0),
	[IsDelete] INT DEFAULT (0),
	[Label] VARCHAR (50)
);

CREATE TABLE Tasks(
	[ID] INTEGER PRIMARY KEY,
	[TaskName] VARCHAR (500) NOT NULL,
	[AssignName] VARCHAR (50) NOT NULL,
	[UserName] VARCHAR (50) NOT NULL,
	[TaskTime] INT NOT NULL,
	[TaskProgress] INT NOT NULL,
	[AssignTime] DATETIME NOT NULL
);

CREATE TABLE RejectUsers(
	[ID] INTEGER PRIMARY KEY,
	[UserName] VARCHAR (50) NOT NULL,
	[DisplayName] VARCHAR (50) NOT NULL,
	[RegisterTime] DATETIME NOT NULL,
	[RejectedBy] VARCHAR (50) NULL,
	[RejectedTime] DATETIME NULL,
	[RejectedReason] VARCHAR (50) NULL
);