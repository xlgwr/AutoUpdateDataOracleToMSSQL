CREATE TABLE [dbo].[iLog](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[type] [nvarchar](50) NULL,
	[system_id] [nvarchar](50) NULL,
	[time_start] [datetime] NULL,
	[time_done] [datetime] NULL,
	[sql] [text] NULL,
	[size] [int] NULL,
	[address] [nvarchar](50) NULL,
	[comment] [text] NULL,
 CONSTRAINT [PK_iLog_new] PRIMARY KEY CLUSTERED 
(
	[id] DESC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]