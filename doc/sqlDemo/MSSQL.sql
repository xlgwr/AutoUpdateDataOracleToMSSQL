CREATE TABLE INVENTORY_PART_TAB
(
	PART_NO VARCHAR(25),
	CONTRACT VARCHAR(5),
	ASSET_CLASS VARCHAR(2),
	ACCOUNTING_GROUP VARCHAR(5),
	PRIME_COMMODITY VARCHAR(5),
	N_OBL_PART_TYPE VARCHAR(32),
	LAST_ACTIVITY_DATE datetime
);
CREATE TABLE INVENTORY_TRANSACTION_HIST_TAB
(
	TRANSACTION_ID int primary key,
	ACCOUNTING_ID int,
	PART_NO VARCHAR(25),
	CONTRACT VARCHAR(5),
	DATE_APPLIED datetime,
	EXPIRATION_DATE datetime
);
CREATE TABLE N_AIS_SHOP_LIST_PICKED_ACT_TAB
(
	CONTRACT VARCHAR(5),
	N_SHOP_LIST_ID VARCHAR(14),
	PART_NO VARCHAR(25),
	LOT_BATCH_NO VARCHAR(20),

	N_CREATED_DATE datetime,
	ROWVERSION  datetime,
	ROWKEY VARCHAR(50)
);
CREATE TABLE N_TRANSPORT_ORDER_TAB
(
	CONTRACT VARCHAR(5),
	N_TRANSPORT_ORDER_NO VARCHAR(19),
	N_TRANSPORT_TYPE int,
	N_TRANSPORT_DATE datetime
);
CREATE TABLE SHOP_ORD_TAB
(
	ORDER_NO VARCHAR(12),
	RELEASE_NO VARCHAR(4),
	SEQUENCE_NO VARCHAR(4),

	ORG_START_DATE datetime,
	CONTRACT VARCHAR(5)
);
CREATE TABLE SHOP_ORDER_OPERATION_TAB
(
	ORDER_NO VARCHAR(12),
	RELEASE_NO VARCHAR(4),
	SEQUENCE_NO VARCHAR(4),
	OPERATION_NO int,
	
	ROWVERSION  datetime,
	ROWKEY VARCHAR(50)
);
CREATE TABLE SHOP_MATERIAL_ALLOC_TAB
(
	ORDER_NO VARCHAR(12),
	RELEASE_NO VARCHAR(4),
	SEQUENCE_NO VARCHAR(4),
	LINE_ITEM_NO int,
	
	ROWVERSION  datetime,
	ROWKEY VARCHAR(50)
);
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