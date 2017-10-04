
USE [SBO_CZ_TEST]
GO

/****** Object:  Table [dbo].[AVA_CZ_TASKLIST]    Script Date: 2016/11/8 17:44:25 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[AVA_CZ_TASKLIST](
	[DocEntry] [int] NOT NULL,
	[CreateDate] [datetime] NULL,
	[CreateTime] [smallint] NULL,
	[Status] [nvarchar](1) NULL,
	[BusinessType] [nvarchar](30) NULL,
	[SubType] [nvarchar](30) NULL,
	[Direction] [nvarchar](1) NULL,
	[UniqueKey] [nvarchar](100) NULL,
	[WhsCode] [nvarchar](20) NULL,
	[IsSync] [nvarchar](1) NULL,
	[SyncDate] [datetime] NULL,
	[SyncTime] [smallint] NULL,
	[SyncErrorMsg] [nvarchar](255) NULL,
 CONSTRAINT [KAVA_CZ_TASKLIST] PRIMARY KEY CLUSTERED 
(
	[DocEntry] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[AVA_CZ_TASKLIST] ADD  CONSTRAINT [DF_AVA_CZ_TASKLIST_IsSync]  DEFAULT (N'N') FOR [IsSync]
GO


