CREATE TABLE [dbo].[iLog](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[lName] [nvarchar](50) NULL,
	[ltype] [nvarchar](50) NULL,
	[lvalue] [bigint] NULL,
	[lDesc] [text] NULL,
	[ldate] [datetime] NULL,
	[remark] [text] NULL,
 CONSTRAINT [PK_iLog] PRIMARY KEY CLUSTERED 
(
	[id] DESC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]