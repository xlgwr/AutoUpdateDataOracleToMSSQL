USE [nec]
GO
/****** Object:  Table [dbo].[iLog]    Script Date: 12/02/2015 13:58:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
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
GO
/****** Object:  Table [dbo].[M_PARTS_LIST_DD]    Script Date: 12/02/2015 13:58:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[M_PARTS_LIST_DD](
	[N_IF_SEQ] [int] NOT NULL,
	[CONTRACT] [nvarchar](255) NULL,
	[WORK_CENTER_NO] [nvarchar](255) NULL,
	[PART_NO] [nvarchar](255) NULL,
	[REVISION_NO] [nvarchar](50) NULL,
	[PROCESS_CODE] [nvarchar](255) NULL,
	[POSITION] [nvarchar](255) NULL,
	[FEEDER] [nvarchar](50) NULL,
	[COMPONENT_PART] [nvarchar](255) NULL,
	[USAGE] [nvarchar](255) NULL,
	[WIDTH] [nvarchar](255) NULL,
	[ARRAY_GRP_ID] [nvarchar](255) NULL,
	[KIT_SEQ] [float] NULL,
	[LOSS_FLG] [nvarchar](255) NULL,
	[LOSS_COEFFICIENT] [nvarchar](255) NULL,
	[ACTIVE_FLG] [nvarchar](255) NULL,
	[CRT_DATE] [nvarchar](255) NULL,
	[CRT_OPERATOR] [nvarchar](255) NULL,
	[UPD_DATE] [nvarchar](255) NULL,
	[UPD_OPERATOR] [nvarchar](255) NULL,
	[UPD_PC] [nvarchar](255) NULL,
	[OPERATION_NO] [numeric](18, 0) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[N_TRANSPORT_ORDER_TAB]    Script Date: 12/02/2015 13:58:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[N_TRANSPORT_ORDER_TAB](
	[CONTRACT] [nvarchar](max) NULL,
	[N_TRANSPORT_ORDER_NO] [nvarchar](max) NULL,
	[N_TRANSPORT_TYPE] [nvarchar](max) NULL,
	[N_TRANSPORT_DATE] [datetime] NULL,
	[N_TRANSPORT_STATUS] [nvarchar](max) NULL,
	[N_ISSUE_LOCATION_NO] [nvarchar](max) NULL,
	[N_TO_LOCATION_NO] [nvarchar](max) NULL,
	[N_TO_LOCATION] [nvarchar](max) NULL,
	[PART_NO] [nvarchar](max) NULL,
	[N_QTY_REQUIRED] [nvarchar](max) NULL,
	[N_QTY_ORIGINAL_ISSUE] [decimal](28, 8) NULL,
	[N_QTY_ISSUE] [decimal](28, 8) NULL,
	[N_QTY_RESERVED] [decimal](28, 8) NULL,
	[N_QTY_ISSUED] [decimal](28, 8) NULL,
	[N_QTY_RECEIVED] [decimal](28, 8) NULL,
	[N_SHORTAGE_FLAG] [nvarchar](max) NULL,
	[N_TRANSPORT_PRINT_FLAG] [nvarchar](max) NULL,
	[N_TRANSPORT_PRINT_DATE] [nvarchar](max) NULL,
	[CARD_NO] [nvarchar](max) NULL,
	[N_KANBAN_TRANSPORT_DATE] [nvarchar](max) NULL,
	[N_RECEIVED_DATE] [nvarchar](max) NULL,
	[N_SHIP_BARCODE] [nvarchar](max) NULL,
	[ORDER_NO] [nvarchar](max) NULL,
	[LINE_NO] [nvarchar](max) NULL,
	[REL_NO] [nvarchar](max) NULL,
	[LINE_ITEM_NO] [nvarchar](max) NULL,
	[PROJECT_ID] [nvarchar](max) NULL,
	[ACTIVITY_SEQ] [nvarchar](max) NULL,
	[BUYER_CODE] [nvarchar](max) NULL,
	[VENDOR_NO] [nvarchar](max) NULL,
	[N_CREATED_DATE] [datetime] NULL,
	[N_CREATED_USER] [nvarchar](max) NULL,
	[N_UPDATED_DATE] [datetime] NULL,
	[N_UPDATED_USER] [nvarchar](max) NULL,
	[ROWVERSION] [datetime] NULL,
	[ROWKEY] [nvarchar](max) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[NAIS_F_RECORD]    Script Date: 12/02/2015 13:58:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[NAIS_F_RECORD](
	[CONTRACT] [nvarchar](5) NULL,
	[N_REPORTED_DATE] [datetime] NULL,
	[N_LABOR_ID] [nvarchar](max) NULL,
	[PRODUCTION_LINE] [nvarchar](12) NULL,
	[ORDER_NO] [nvarchar](12) NULL,
	[RELEASE_NO] [nvarchar](4) NULL,
	[SEQUENCE_NO] [nvarchar](4) NULL,
	[OPERATION_NO] [decimal](18, 0) NULL,
	[N_Z_AXIS] [nvarchar](max) NULL,
	[N_POSITION] [nvarchar](max) NULL,
	[N_MATERIAL_PART_NO] [nvarchar](max) NULL,
	[N_MATERIAL_LOT] [nvarchar](max) NULL,
	[N_MATERIAL_TRACE_SEQ] [nvarchar](max) NULL,
	[N_ACTUAL_USED] [decimal](18, 0) NULL,
	[FLG] [int] NULL,
	[CH] [int] NULL,
	[N_MATERIAL_NOTE1] [nvarchar](10) NULL,
	[N_MATERIAL_NOTE2] [nvarchar](10) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SHOP_MATERIAL_ALLOC_TAB]    Script Date: 12/02/2015 13:58:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SHOP_MATERIAL_ALLOC_TAB](
	[ORDER_NO] [nvarchar](50) NULL,
	[RELEASE_NO] [nvarchar](max) NULL,
	[SEQUENCE_NO] [nvarchar](max) NULL,
	[LINE_ITEM_NO] [nvarchar](max) NULL,
	[QTY_PER_ASSEMBLY] [nvarchar](max) NULL,
	[COMPONENT_SCRAP] [nvarchar](max) NULL,
	[SHRINKAGE_FACTOR] [nvarchar](max) NULL,
	[OPERATION_NO] [nvarchar](max) NULL,
	[LEADTIME_OFFSET] [nvarchar](max) NULL,
	[DRAW_POS_NO] [nvarchar](max) NULL,
	[STRUCTURE_LINE_NO] [nvarchar](max) NULL,
	[DOP_ORDER_ID] [nvarchar](max) NULL,
	[DATE_REQUIRED] [nvarchar](max) NULL,
	[LAST_ISSUE_DATE] [nvarchar](max) NULL,
	[QTY_REQUIRED] [nvarchar](max) NULL,
	[QTY_ISSUED] [nvarchar](max) NULL,
	[QTY_SHORT] [nvarchar](max) NULL,
	[QTY_ASSIGNED] [nvarchar](max) NULL,
	[QTY_ON_ORDER] [nvarchar](max) NULL,
	[GENERATE_DEMAND_QTY] [nvarchar](max) NULL,
	[PRIORITY_NO] [nvarchar](max) NULL,
	[NOTE_ID] [nvarchar](max) NULL,
	[NOTE_TEXT] [nvarchar](max) NULL,
	[CREATE_DATE] [nvarchar](max) NULL,
	[LAST_ACTIVITY_DATE] [nvarchar](max) NULL,
	[ACTIVITY_SEQ] [nvarchar](max) NULL,
	[ORDER_CODE] [nvarchar](max) NULL,
	[SUPPLY_CODE] [nvarchar](max) NULL,
	[CONSUMPTION_ITEM] [nvarchar](max) NULL,
	[CONTRACT] [nvarchar](max) NULL,
	[PART_NO] [nvarchar](max) NULL,
	[ISSUE_TO_LOC] [nvarchar](max) NULL,
	[PRINT_UNIT] [nvarchar](max) NULL,
	[CONFIGURATION_ID] [nvarchar](max) NULL,
	[CONDITION_CODE] [nvarchar](max) NULL,
	[PART_OWNERSHIP] [nvarchar](max) NULL,
	[OWNING_CUSTOMER_NO] [nvarchar](max) NULL,
	[OWNING_VENDOR_NO] [nvarchar](max) NULL,
	[VIM_STRUCTURE_SOURCE] [nvarchar](max) NULL,
	[PARTIAL_PART_REQUIRED] [nvarchar](max) NULL,
	[PROJECT_ID] [nvarchar](max) NULL,
	[CATCH_QTY_ISSUED] [nvarchar](max) NULL,
	[REPLACED_QTY] [nvarchar](max) NULL,
	[REPLACES_QPA_FACTOR] [nvarchar](max) NULL,
	[REPLACES_LINE_ITEM_NO] [nvarchar](max) NULL,
	[QTY_SCRAPPED_COMPONENT] [nvarchar](max) NULL,
	[POSITION_PART_NO] [nvarchar](max) NULL,
	[STD_PLANNED_ITEM] [nvarchar](max) NULL,
	[CRO_NO] [nvarchar](max) NULL,
	[CRO_LINE_NO] [nvarchar](max) NULL,
	[SERVICE_TYPE] [nvarchar](max) NULL,
	[EXCLUDE_FROM_AS_BUILT] [nvarchar](max) NULL,
	[PROCEDURE_STEP] [nvarchar](max) NULL,
	[ROWVERSION] [nvarchar](max) NULL,
	[ROWSTATE] [nvarchar](max) NULL,
	[ROWKEY] [nvarchar](max) NULL,
	[N_MATERIAL_ISSUE_DATE] [nvarchar](max) NULL,
	[N_DEPLOY_METHOD] [nvarchar](max) NULL,
	[N_SELF_SUPLY_FLAG] [nvarchar](max) NULL,
	[N_LOGICAL_PAGE] [nvarchar](max) NULL,
	[N_COMPONENT_ID_SYMBOL] [nvarchar](max) NULL,
	[N_COMPONENT_ID_NUMBER] [nvarchar](max) NULL,
	[N_CREATED_DATE] [nvarchar](max) NULL,
	[N_CREATED_USER] [nvarchar](max) NULL,
	[N_UPDATED_DATE] [nvarchar](max) NULL,
	[N_UPDATED_USER] [nvarchar](max) NULL,
	[N_PROCESS_CODE] [nvarchar](50) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SHOP_ORD_TAB]    Script Date: 12/02/2015 13:58:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SHOP_ORD_TAB](
	[ORDER_NO] [nvarchar](50) NULL,
	[RELEASE_NO] [nvarchar](max) NULL,
	[SEQUENCE_NO] [nvarchar](max) NULL,
	[SERIAL_BEGIN] [nvarchar](max) NULL,
	[SERIAL_END] [nvarchar](max) NULL,
	[SHOP_ORD_ROUT_SERIAL] [nvarchar](max) NULL,
	[ORG_START_DATE] [nvarchar](max) NULL,
	[REVISED_START_DATE] [nvarchar](max) NULL,
	[ORG_DUE_DATE] [nvarchar](max) NULL,
	[REVISED_DUE_DATE] [nvarchar](max) NULL,
	[NEED_DATE] [nvarchar](max) NULL,
	[EARLIEST_START_DATE] [nvarchar](max) NULL,
	[CLOSE_DATE] [nvarchar](max) NULL,
	[CLOSE_TOLERANCE] [nvarchar](max) NULL,
	[COMPLETE_DATE] [nvarchar](max) NULL,
	[ORG_QTY_DUE] [nvarchar](max) NULL,
	[REVISED_QTY_DUE] [nvarchar](max) NULL,
	[QTY_COMPLETE] [nvarchar](max) NULL,
	[QTY_ON_ORDER] [nvarchar](max) NULL,
	[QTY_RELEASED] [nvarchar](max) NULL,
	[OPERATION_SCRAPPED] [nvarchar](max) NULL,
	[QTY_DIFF] [nvarchar](max) NULL,
	[PARTIAL_DIRECTION] [nvarchar](max) NULL,
	[PARTIAL_OPERATION] [nvarchar](max) NULL,
	[PRIORITY_NO] [nvarchar](max) NULL,
	[REJECT_REASON] [nvarchar](max) NULL,
	[PRE_ACCOUNTING_ID] [nvarchar](max) NULL,
	[SECURITY_CLASS] [nvarchar](max) NULL,
	[NOTE_ID] [nvarchar](max) NULL,
	[NOTE_TEXT] [nvarchar](max) NULL,
	[SOURCE] [nvarchar](max) NULL,
	[DATE_ENTERED] [nvarchar](max) NULL,
	[LAST_ACTIVITY_DATE] [nvarchar](max) NULL,
	[PROJECT_ID] [nvarchar](max) NULL,
	[ACTIVITY_SEQ] [nvarchar](max) NULL,
	[SOURCE_ORDER_NO] [nvarchar](max) NULL,
	[SOURCE_RELEASE_NO] [nvarchar](max) NULL,
	[SOURCE_SEQUENCE_NO] [nvarchar](max) NULL,
	[SPLIT_REASON] [nvarchar](max) NULL,
	[MRB_NUMBER] [nvarchar](max) NULL,
	[BALANCED_COST_DIFF] [nvarchar](max) NULL,
	[JOB_ID] [nvarchar](max) NULL,
	[SCHED_CAPACITY] [nvarchar](max) NULL,
	[ORDER_CODE] [nvarchar](max) NULL,
	[ASSIGN_FLAG] [nvarchar](max) NULL,
	[RESCHED_FLAG] [nvarchar](max) NULL,
	[CLOSE_CODE] [nvarchar](max) NULL,
	[SCHED_DIRECTION] [nvarchar](max) NULL,
	[PICK_LIST] [nvarchar](max) NULL,
	[WORK_INSTRUCT] [nvarchar](max) NULL,
	[DEMAND_CODE] [nvarchar](max) NULL,
	[RESCHED_CODE] [nvarchar](max) NULL,
	[PICKLIST_TYPE] [nvarchar](max) NULL,
	[CONTRACT] [nvarchar](max) NULL,
	[PART_NO] [nvarchar](max) NULL,
	[ENG_CHG_LEVEL] [nvarchar](max) NULL,
	[STRUCTURE_ALTERNATIVE] [nvarchar](max) NULL,
	[ROUTING_REVISION] [nvarchar](max) NULL,
	[ROUTING_ALTERNATIVE] [nvarchar](max) NULL,
	[PROPOSED_LOCATION] [nvarchar](max) NULL,
	[PROCESS_TYPE] [nvarchar](max) NULL,
	[CUSTOMER_ORDER_NO] [nvarchar](max) NULL,
	[CUSTOMER_LINE_NO] [nvarchar](max) NULL,
	[CUSTOMER_REL_NO] [nvarchar](max) NULL,
	[CUSTOMER_LINE_ITEM_NO] [nvarchar](max) NULL,
	[CUSTOMER_NO] [nvarchar](max) NULL,
	[PRIORITY_CATEGORY] [nvarchar](max) NULL,
	[CONFIGURATION_ID] [nvarchar](max) NULL,
	[CONDITION_CODE] [nvarchar](max) NULL,
	[LOT_BATCH_STRING] [nvarchar](max) NULL,
	[MAINT_LEVEL_STRUCT] [nvarchar](max) NULL,
	[MAINT_LEVEL_ROUT] [nvarchar](max) NULL,
	[CLOSED_IN_CBS] [nvarchar](max) NULL,
	[PART_OWNERSHIP] [nvarchar](max) NULL,
	[OWNING_CUSTOMER_NO] [nvarchar](max) NULL,
	[MRO_VISIT_ID] [nvarchar](max) NULL,
	[MRO_INT_ORD_HEADER] [nvarchar](max) NULL,
	[MRO_INT_ORDER] [nvarchar](max) NULL,
	[DISPO_ORDER_NO] [nvarchar](max) NULL,
	[DISPO_RELEASE_NO] [nvarchar](max) NULL,
	[DISPO_SEQUENCE_NO] [nvarchar](max) NULL,
	[DISPO_LINE_ITEM] [nvarchar](max) NULL,
	[PARTIAL_PART_REQUIRED] [nvarchar](max) NULL,
	[MODS_DEFINED] [nvarchar](max) NULL,
	[REPAIRS_DEFINED] [nvarchar](max) NULL,
	[ALTERNATE_COMPONENT_USED] [nvarchar](max) NULL,
	[USE_COST_DISTRIBUTION] [nvarchar](max) NULL,
	[CASE_ID] [nvarchar](max) NULL,
	[TASK_ID] [nvarchar](max) NULL,
	[ESO_SUPPLIER] [nvarchar](max) NULL,
	[ESO_SERVICE_TYPE] [nvarchar](max) NULL,
	[CRO_NO] [nvarchar](max) NULL,
	[CRO_LINE] [nvarchar](max) NULL,
	[LAST_AVAIL_RUN_DATE] [nvarchar](max) NULL,
	[SHRINKAGE_FACTOR] [nvarchar](max) NULL,
	[PLANNED_OP_SCRAP] [nvarchar](max) NULL,
	[TEXT_ID$] [nvarchar](max) NULL,
	[USED_STRUC_BOM_TYPE] [nvarchar](max) NULL,
	[USED_ROUT_BOM_TYPE] [nvarchar](max) NULL,
	[MULTILEVEL_REPAIR] [nvarchar](max) NULL,
	[ROWVERSION] [nvarchar](max) NULL,
	[ROWSTATE] [nvarchar](max) NULL,
	[ROWKEY] [nvarchar](max) NULL,
	[BALANCE_ID] [nvarchar](max) NULL,
	[N_SERIAL_START] [nvarchar](max) NULL,
	[N_SERIAL_END] [nvarchar](max) NULL,
	[PRODUCTION_LINE] [nvarchar](max) NULL,
	[N_PLAN_NO] [nvarchar](max) NULL,
	[N_OUTER_ORDER_NO] [nvarchar](max) NULL,
	[N_MS_FLAG] [nvarchar](max) NULL,
	[N_SUPPLY_DEST_ID] [nvarchar](max) NULL,
	[N_CREATE_ORDER_TYPE] [nvarchar](max) NULL,
	[N_PRIORITY] [nvarchar](max) NULL,
	[N_TEMP_ORDER_NO] [nvarchar](max) NULL,
	[N_TP_PP_STEP] [nvarchar](max) NULL,
	[N_INTEGRATED_PRODUCT_FLAG] [nvarchar](max) NULL,
	[N_UNORDERING_FLAG] [nvarchar](max) NULL,
	[N_TOP_ORDER_NO] [nvarchar](max) NULL,
	[N_PARENT_ORDER_NO] [nvarchar](max) NULL,
	[N_SYNPLA_SEND_DATE] [nvarchar](max) NULL,
	[N_PROD_ORDER_OUTPUT_FLAG] [nvarchar](max) NULL,
	[N_PROD_ORDER_OUTPUT_DATE] [nvarchar](max) NULL,
	[N_PROJECT_NO] [nvarchar](max) NULL,
	[N_TRACE_PROCESSED_FLAG] [nvarchar](max) NULL,
	[N_CREATED_DATE] [nvarchar](max) NULL,
	[N_CREATED_USER] [nvarchar](max) NULL,
	[N_UPDATED_DATE] [nvarchar](max) NULL,
	[N_UPDATED_USER] [nvarchar](max) NULL,
	[N_DIRECT_DEPT_EXPENSE] [nvarchar](max) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[W_CRT_HAIZEN_SEL]    Script Date: 12/02/2015 13:58:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[W_CRT_HAIZEN_SEL](
	[IDX] [int] NOT NULL,
	[CHK] [int] NULL,
	[STARTDATE] [nvarchar](10) NULL,
	[LINE] [nvarchar](50) NULL,
	[KIT] [nvarchar](50) NULL,
	[PROCESS] [nvarchar](50) NULL,
	[REV] [nvarchar](50) NULL,
	[QTY] [int] NULL,
	[FINISHDATE] [nvarchar](50) NULL,
	[ORDER_NO] [nvarchar](50) NULL,
	[CONTRACT] [nvarchar](50) NULL,
	[OPERATION_NO] [nvarchar](10) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[W_ORDER_SEL]    Script Date: 12/02/2015 13:58:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[W_ORDER_SEL](
	[IDX] [int] NOT NULL,
	[CHK] [int] NULL,
	[STARTDATE] [nvarchar](50) NULL,
	[LINE] [nvarchar](50) NULL,
	[KIT] [nvarchar](50) NULL,
	[PROCESS] [nvarchar](50) NULL,
	[REV] [nvarchar](50) NULL,
	[QTY] [int] NULL,
	[FINISHDATE] [nvarchar](50) NULL,
	[ORDER_NO] [nvarchar](50) NULL,
	[CONTRACT] [nvarchar](50) NULL,
	[OPERATION_NO] [nvarchar](10) NULL,
	[HAIZEN_ID] [nvarchar](50) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[M_OPERATOR]    Script Date: 12/02/2015 13:58:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[M_OPERATOR](
	[N_IF_SEQ] [numeric](18, 0) NOT NULL,
	[CONTRACT] [nchar](5) NULL,
	[OPERATOR_ID] [nvarchar](30) NOT NULL,
	[OPERATOR_NM] [nvarchar](100) NULL,
	[PG_LEVEL] [nvarchar](10) NULL,
	[STRING1] [nvarchar](50) NULL,
	[STRING2] [nvarchar](50) NULL,
	[STRING3] [nvarchar](50) NULL,
	[STRING4] [nvarchar](50) NULL,
	[STRING5] [nvarchar](50) NULL,
	[CRT_DATE] [datetime] NULL,
	[CRT_OPERATOR] [nvarchar](30) NULL,
	[UPD_DATE] [datetime] NULL,
	[UPD_OPERATOR] [nvarchar](30) NULL,
	[UPD_PC] [nvarchar](30) NULL,
 CONSTRAINT [PK_M_OPERATOR] PRIMARY KEY CLUSTERED 
(
	[N_IF_SEQ] ASC,
	[OPERATOR_ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[M_HAIZENNO_CTL]    Script Date: 12/02/2015 13:58:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[M_HAIZENNO_CTL](
	[KEY-ID] [nvarchar](8) NULL,
	[HAIZEN_SEQ] [int] NULL,
	[UPD_DATE] [datetime] NULL,
	[UPD_OPERATOR] [nvarchar](30) NULL,
	[UPD_PC] [nvarchar](30) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[M_PARTS]    Script Date: 12/02/2015 13:58:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[M_PARTS](
	[PARTS] [nvarchar](20) NULL,
	[PARTSNAM] [nvarchar](40) NULL,
	[PARTSTYP] [nvarchar](2) NULL,
	[STRING1] [nvarchar](20) NULL,
	[STRING2] [nvarchar](20) NULL,
	[STRING3] [nvarchar](20) NULL,
	[STRING4] [nvarchar](20) NULL,
	[STRING5] [nvarchar](20) NULL,
	[STRING6] [nvarchar](20) NULL,
	[STRING7] [nvarchar](20) NULL,
	[STRING8] [nvarchar](20) NULL,
	[STRING9] [nvarchar](20) NULL,
	[STRING10] [nvarchar](20) NULL,
	[STRING11] [nvarchar](20) NULL,
	[STRING12] [nvarchar](20) NULL,
	[STRING13] [nvarchar](20) NULL,
	[STRING14] [nvarchar](20) NULL,
	[NUMERIC1] [int] NULL,
	[NUMERIC2] [int] NULL,
	[NUMERIC3] [int] NULL,
	[NUMERIC4] [int] NULL,
	[REAL1] [float] NULL,
	[REAL2] [float] NULL,
	[REAL3] [float] NULL,
	[REAL4] [float] NULL,
	[FLG] [nvarchar](1) NULL,
	[CRT_DATE] [datetime] NULL,
	[CRT_OPERATOR] [nvarchar](30) NULL,
	[UPD_DATE] [datetime] NULL,
	[UPD_OPERATOR] [nvarchar](30) NULL,
	[UPD_PC] [nvarchar](30) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SHOP_ORDER_OPERATION_TAB]    Script Date: 12/02/2015 13:58:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SHOP_ORDER_OPERATION_TAB](
	[ORDER_NO] [nvarchar](50) NULL,
	[RELEASE_NO] [nvarchar](max) NULL,
	[SEQUENCE_NO] [nvarchar](max) NULL,
	[OPERATION_NO] [nvarchar](max) NULL,
	[OPERATION_DESCRIPTION] [nvarchar](max) NULL,
	[WORK_CENTER_NO] [nvarchar](max) NULL,
	[CONTRACT] [nvarchar](max) NULL,
	[PART_NO] [nvarchar](max) NULL,
	[OPER_STATUS_CODE] [nvarchar](max) NULL,
	[RUN_TIME_CODE] [nvarchar](max) NULL,
	[EFFICIENCY_FACTOR] [nvarchar](max) NULL,
	[MACH_RUN_FACTOR] [nvarchar](max) NULL,
	[MACH_SETUP_TIME] [nvarchar](max) NULL,
	[MOVE_TIME] [nvarchar](max) NULL,
	[QUEUE_TIME] [nvarchar](max) NULL,
	[LABOR_CAPACITY] [nvarchar](max) NULL,
	[LABOR_RUN_FACTOR] [nvarchar](max) NULL,
	[LABOR_SETUP_TIME] [nvarchar](max) NULL,
	[LABOR_CLASS_NO] [nvarchar](max) NULL,
	[SETUP_LABOR_CLASS_NO] [nvarchar](max) NULL,
	[CREW_SIZE] [nvarchar](max) NULL,
	[SETUP_CREW_SIZE] [nvarchar](max) NULL,
	[OUTSIDE_OP_ITEM] [nvarchar](max) NULL,
	[PARALLEL_OPERATION] [nvarchar](max) NULL,
	[SCHED_DIRECTION] [nvarchar](max) NULL,
	[CAPACITY_FLAG] [nvarchar](max) NULL,
	[OP_START_DATE] [datetime] NULL,
	[OP_START_TIME] [nvarchar](max) NULL,
	[OP_FINISH_DATE] [datetime] NULL,
	[OP_FINISH_TIME] [nvarchar](max) NULL,
	[REVISED_QTY_DUE] [nvarchar](max) NULL,
	[QTY_COMPLETE] [nvarchar](max) NULL,
	[QTY_SCRAPPED] [nvarchar](max) NULL,
	[OP_ID] [nvarchar](max) NULL,
	[MACHINE_NO] [nvarchar](max) NULL,
	[NOTE_ID] [nvarchar](max) NULL,
	[NOTE_TEXT] [nvarchar](max) NULL,
	[LAST_ACTIVITY_DATE] [nvarchar](max) NULL,
	[OUTSIDE_QTY_SHIPPED] [nvarchar](max) NULL,
	[BUFFER_TIME] [nvarchar](max) NULL,
	[OPERATION_SCHED_STATUS] [nvarchar](max) NULL,
	[SCHEDULED_SETUP_TIME] [nvarchar](max) NULL,
	[BARCODE_ID] [nvarchar](max) NULL,
	[OUTSIDE_OP_COMPLETE] [nvarchar](max) NULL,
	[OUTSIDE_OP_NOTES] [nvarchar](max) NULL,
	[OUTSIDE_OP_BACKFLUSH] [nvarchar](max) NULL,
	[OUTSIDE_OP_SUPPLY_TYPE] [nvarchar](max) NULL,
	[OVERLAP] [nvarchar](max) NULL,
	[OVERLAP_UNIT] [nvarchar](max) NULL,
	[OPERATION_PRIORITY] [nvarchar](max) NULL,
	[CBS_QUEUE_TIME] [nvarchar](max) NULL,
	[MILESTONE_OPERATION] [nvarchar](max) NULL,
	[INCREASE_LOTSIZE_ON_SCRAP] [nvarchar](max) NULL,
	[GROUP_BY_NOTE] [nvarchar](max) NULL,
	[CRO_NO] [nvarchar](max) NULL,
	[CRO_LINE_NO] [nvarchar](max) NULL,
	[SERVICE_TYPE] [nvarchar](max) NULL,
	[INCLUDE_SETUP_FOR_OVERLAP] [nvarchar](max) NULL,
	[ACTUAL_SETUP_TIME] [nvarchar](max) NULL,
	[ROWVERSION] [nvarchar](max) NULL,
	[ROWKEY] [nvarchar](max) NULL,
	[N_QTY_DEFECT] [nvarchar](max) NULL,
	[N_INSPECT_OPERATION_FLAG] [nvarchar](max) NULL,
	[N_SYNPLA_RESULT_DATE] [nvarchar](max) NULL,
	[N_SYNPLA_PLANNED_NO] [nvarchar](max) NULL,
	[N_TODAY_SHIP_FLAG] [nvarchar](max) NULL,
	[N_CT] [nvarchar](max) NULL,
	[N_PROD_INSTR_PRINT_FLAG] [nvarchar](50) NULL,
	[N_PROCESS_CODE] [nvarchar](50) NULL,
	[N_TYPE_OUT_FLAG] [nvarchar](max) NULL,
	[N_SYNPLA_FIX_FLAG] [nvarchar](max) NULL,
	[N_CREATED_DATE] [nvarchar](max) NULL,
	[N_CREATED_USER] [nvarchar](max) NULL,
	[N_UPDATED_DATE] [nvarchar](max) NULL,
	[N_UPDATED_USER] [nvarchar](max) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[NAIS_F_HAIZEN_DT]    Script Date: 12/02/2015 13:58:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[NAIS_F_HAIZEN_DT](
	[N_IF_SEQ] [numeric](18, 0) NULL,
	[CONTRACT] [nchar](5) NULL,
	[N_REPORTED_DATE] [datetime] NULL,
	[N_LABOR_ID] [nvarchar](300) NULL,
	[N_SHOP_LIST_ID] [nvarchar](14) NULL,
	[PART_NO] [nvarchar](25) NULL,
	[ORDER_NO] [nvarchar](12) NULL,
	[RELEASE_NO] [nchar](4) NULL,
	[SEQUENCE_NO] [nchar](4) NULL,
	[LINE_ITEM_NO] [numeric](18, 0) NULL,
	[OPERATION_NO] [numeric](18, 0) NULL,
	[PRODUCTION_LINE] [nvarchar](50) NULL,
	[COMPONENT_PART] [nvarchar](50) NULL,
	[ENG_CHG_LEVEL] [nchar](6) NULL,
	[CONFIGURATION_ID] [nvarchar](50) NULL,
	[LOCATION_NO] [nvarchar](35) NULL,
	[LOT_BATCH_NO] [nvarchar](20) NULL,
	[SERIAL_NO] [nvarchar](50) NULL,
	[WAIV_DEV_REJ_NO] [nvarchar](15) NULL,
	[ACTIVITY_SEQ] [numeric](18, 0) NULL,
	[N_QTY_RESERVED] [numeric](18, 0) NULL,
	[N_QTY_PICKED] [numeric](18, 0) NULL,
	[N_OP_CLOCKINGS] [datetime] NULL,
	[N_Z_AXIS] [nchar](10) NULL,
	[N_POSITION] [nchar](10) NULL,
	[SEND_FLG] [int] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[NAIS_F_HAIZEN_HD]    Script Date: 12/02/2015 13:58:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[NAIS_F_HAIZEN_HD](
	[N_IF_SEQ] [numeric](18, 0) NULL,
	[CONTRACT] [nchar](5) NULL,
	[N_REPORTED_DATE] [datetime] NULL,
	[N_LABOR_ID] [nvarchar](300) NULL,
	[N_SHOP_LIST_ID] [nvarchar](14) NULL,
	[PART_NO] [nvarchar](25) NULL,
	[RELEASE_NO] [nchar](4) NULL,
	[SEQUENCE_NO] [nchar](4) NULL,
	[LINE_ITEM_NO] [numeric](18, 0) NULL,
	[OPERATION_NO] [numeric](18, 0) NULL,
	[PRODUCTION_LINE] [nvarchar](50) NULL,
	[COMPONENT_PART] [nvarchar](50) NULL,
	[ENG_CHG_LEVEL] [nchar](6) NULL,
	[CONFIGURATION_ID] [nvarchar](50) NULL,
	[LOCATION_NO] [nvarchar](35) NULL,
	[LOT_BATCH_NO] [nvarchar](20) NULL,
	[SERIAL_NO] [nvarchar](50) NULL,
	[WAIV_DEV_REJ_NO] [nvarchar](15) NULL,
	[ACTIVITY_SEQ] [numeric](18, 0) NULL,
	[N_QTY_RESERVED] [numeric](18, 0) NULL,
	[N_QTY_PICKED] [numeric](18, 0) NULL,
	[N_OP_CLOCKINGS] [datetime] NULL,
	[N_Z_AXIS] [nvarchar](10) NULL,
	[N_POSITION] [nvarchar](10) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[F_ORDER_STS]    Script Date: 12/02/2015 13:58:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[F_ORDER_STS](
	[IDX] [int] IDENTITY(1,1) NOT NULL,
	[ORDER_NO] [nvarchar](50) NULL,
	[N_SHOP_LIST_ID] [nvarchar](50) NULL,
	[STS] [nvarchar](2) NULL,
	[N_PROCESS_CODE] [nvarchar](20) NULL,
	[UPD_DATE] [datetime] NULL,
	[UPD_OPERATOR] [nvarchar](30) NULL,
	[UPD_PC] [nvarchar](30) NULL
) ON [PRIMARY]
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'プロセス' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'F_ORDER_STS', @level2type=N'COLUMN',@level2name=N'N_PROCESS_CODE'
GO
/****** Object:  Table [dbo].[M_LINE_PARTS_STOCK]    Script Date: 12/02/2015 13:58:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[M_LINE_PARTS_STOCK](
	[N_IF_SEQ] [numeric](18, 0) NOT NULL,
	[CONTRACT] [nchar](5) NULL,
	[WORK_CENTER_NO] [nvarchar](12) NULL,
	[POSITION] [nvarchar](20) NULL,
	[FEEDER] [nvarchar](7) NULL,
	[COMPONENT_PART] [nvarchar](25) NULL,
	[STOCK_QTY] [numeric](18, 0) NULL,
	[RESERVE_QTY] [numeric](18, 0) NULL,
	[FREE_QTY] [numeric](18, 0) NULL,
	[ARRAY_GRP_ID] [nvarchar](20) NULL,
	[UPD_OPERATOR] [nvarchar](30) NULL,
	[UPD_PC] [nvarchar](30) NULL,
 CONSTRAINT [PK_M_LINE_PARTS_STOCK] PRIMARY KEY CLUSTERED 
(
	[N_IF_SEQ] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[M_CONTOROL]    Script Date: 12/02/2015 13:58:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[M_CONTOROL](
	[KEY-ID] [nvarchar](2) NOT NULL,
	[CONTRACT01] [nchar](5) NULL,
	[CONTRACT02] [nchar](5) NULL,
	[CONTRACT03] [nchar](5) NULL,
	[CONTRACT04] [nchar](5) NULL,
	[CONTRACT05] [nchar](5) NULL,
	[CONTRACT06] [nchar](5) NULL,
	[CONTRACT07] [nchar](5) NULL,
	[CONTRACT08] [nchar](5) NULL,
	[CONTRACT09] [nchar](5) NULL,
	[CONTRACT10] [nchar](5) NULL,
	[AISPICK_UPD_DATE] [datetime] NULL,
	[AISDTA_UPD_DATE] [datetime] NULL,
	[PDRSEL_COL_01] [nchar](10) NULL,
	[PDRSEL_COL_02] [nchar](10) NULL,
	[PDRSEL_COL_03] [nchar](10) NULL,
	[PDRSEL_COL_04] [nchar](10) NULL,
	[PDRSEL_COL_05] [nchar](10) NULL,
	[HAIZEN_COL_01] [nchar](10) NULL,
	[HAIZEN_COL_02] [nchar](10) NULL,
	[HAIZEN_COL_03] [nchar](10) NULL,
	[HAIZEN_COL_04] [nchar](10) NULL,
	[HAIZEN_COL_05] [nchar](10) NULL,
	[CRT_DATE] [datetime] NULL,
	[CRT_OPERATOR] [nvarchar](30) NULL,
	[UPD_DATE] [datetime] NULL,
	[UPD_OPERATOR] [nvarchar](30) NULL,
	[UPD_PC] [nvarchar](30) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[M_PARTS_STOCK]    Script Date: 12/02/2015 13:58:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[M_PARTS_STOCK](
	[N_IF_SEQ] [numeric](18, 0) NOT NULL,
	[COMPONENT_PART] [nvarchar](25) NULL,
	[N_MATERIAL_LOT] [nvarchar](20) NULL,
	[N_MATERIAL_TRACE_SEQ] [nvarchar](30) NULL,
	[N_ACTUAL_USED] [numeric](18, 0) NULL,
	[CONTRACT] [nchar](5) NULL,
	[WORK_CENTER_NO] [nvarchar](12) NULL,
	[POSITION] [nvarchar](20) NULL,
	[FEEDER] [nvarchar](7) NULL,
	[ARRAY_GRP_ID] [nvarchar](20) NULL,
	[RESERVE_QTY] [numeric](18, 0) NULL,
	[FREE_QTY] [numeric](18, 0) NULL,
	[UPD_OPERATOR] [nvarchar](30) NULL,
	[UPD_PC] [nvarchar](30) NULL,
 CONSTRAINT [PK_M_PARTS_STOCK] PRIMARY KEY CLUSTERED 
(
	[N_IF_SEQ] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[INVENTORY_PART_IN_STOCK_TAB]    Script Date: 12/02/2015 13:58:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[INVENTORY_PART_IN_STOCK_TAB](
	[PART_NO] [nvarchar](max) NULL,
	[CONTRACT] [nvarchar](max) NULL,
	[CONFIGURATION_ID] [nvarchar](max) NULL,
	[LOCATION_NO] [nvarchar](max) NULL,
	[LOT_BATCH_NO] [nvarchar](max) NULL,
	[SERIAL_NO] [nvarchar](max) NULL,
	[ENG_CHG_LEVEL] [nvarchar](max) NULL,
	[WAIV_DEV_REJ_NO] [nvarchar](max) NULL,
	[ACTIVITY_SEQ] [nvarchar](max) NULL,
	[AVG_UNIT_TRANSIT_COST] [nvarchar](max) NULL,
	[COUNT_VARIANCE] [nvarchar](max) NULL,
	[DEPARTMENT] [nvarchar](max) NULL,
	[EXPIRATION_DATE] [nvarchar](max) NULL,
	[DEL_TYPE] [nvarchar](max) NULL,
	[FREEZE_FLAG] [nvarchar](max) NULL,
	[LAST_ACTIVITY_DATE] [nvarchar](max) NULL,
	[LAST_COUNT_DATE] [nvarchar](max) NULL,
	[LOCATION_CLASS] [nvarchar](max) NULL,
	[LOCATION_TYPE] [nvarchar](max) NULL,
	[LOW_LEVEL_CODE] [nvarchar](max) NULL,
	[QTY_IN_TRANSIT] [nvarchar](max) NULL,
	[QTY_ONHAND] [nvarchar](max) NULL,
	[QTY_RESERVED] [nvarchar](max) NULL,
	[RECEIPT_DATE] [nvarchar](max) NULL,
	[SOURCE] [nvarchar](max) NULL,
	[WAREHOUSE] [nvarchar](max) NULL,
	[BAY_NO] [nvarchar](max) NULL,
	[ROW_NO] [nvarchar](max) NULL,
	[TIER_NO] [nvarchar](max) NULL,
	[BIN_NO] [nvarchar](max) NULL,
	[AVAILABILITY_CONTROL_ID] [nvarchar](max) NULL,
	[ROTABLE_PART_POOL_ID] [nvarchar](max) NULL,
	[CREATE_DATE] [nvarchar](max) NULL,
	[PROJECT_ID] [nvarchar](max) NULL,
	[CATCH_QTY_IN_TRANSIT] [nvarchar](max) NULL,
	[CATCH_QTY_ONHAND] [nvarchar](max) NULL,
	[PART_OWNERSHIP] [nvarchar](max) NULL,
	[OWNING_CUSTOMER_NO] [nvarchar](max) NULL,
	[OWNING_VENDOR_NO] [nvarchar](max) NULL,
	[ROWVERSION] [nvarchar](max) NULL,
	[ROWKEY] [nvarchar](max) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[INVENTORY_PART_TAB]    Script Date: 12/02/2015 13:58:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[INVENTORY_PART_TAB](
	[PART_NO] [nvarchar](max) NULL,
	[CONTRACT] [nvarchar](max) NULL,
	[ASSET_CLASS] [nvarchar](max) NULL,
	[ACCOUNTING_GROUP] [nvarchar](max) NULL,
	[COUNTRY_OF_ORIGIN] [nvarchar](max) NULL,
	[HAZARD_CODE] [nvarchar](max) NULL,
	[NOTE_ID] [nvarchar](max) NULL,
	[PART_STATUS] [nvarchar](max) NULL,
	[PLANNER_BUYER] [nvarchar](max) NULL,
	[PRIME_COMMODITY] [nvarchar](max) NULL,
	[PART_PRODUCT_CODE] [nvarchar](max) NULL,
	[PART_PRODUCT_FAMILY] [nvarchar](max) NULL,
	[SECOND_COMMODITY] [nvarchar](max) NULL,
	[UNIT_MEAS] [nvarchar](max) NULL,
	[CATCH_UNIT_MEAS] [nvarchar](max) NULL,
	[DESCRIPTION] [nvarchar](max) NULL,
	[ABC_CLASS] [nvarchar](max) NULL,
	[AVAIL_ACTIVITY_STATUS] [nvarchar](max) NULL,
	[COUNT_VARIANCE] [nvarchar](max) NULL,
	[CREATE_DATE] [datetime] NULL,
	[CYCLE_CODE] [nvarchar](max) NULL,
	[CYCLE_PERIOD] [nvarchar](max) NULL,
	[DIM_QUALITY] [nvarchar](max) NULL,
	[CUSTOMS_STAT_NO] [nvarchar](max) NULL,
	[LAST_ACTIVITY_DATE] [nvarchar](max) NULL,
	[DURABILITY_DAY] [nvarchar](max) NULL,
	[LEAD_TIME_CODE] [nvarchar](max) NULL,
	[EXPECTED_LEADTIME] [nvarchar](max) NULL,
	[MANUF_LEADTIME] [nvarchar](max) NULL,
	[OE_ALLOC_ASSIGN_FLAG] [nvarchar](max) NULL,
	[ONHAND_ANALYSIS_FLAG] [nvarchar](max) NULL,
	[PURCH_LEADTIME] [nvarchar](max) NULL,
	[EARLIEST_ULTD_SUPPLY_DATE] [nvarchar](max) NULL,
	[NOTE_TEXT] [nvarchar](max) NULL,
	[SUPERSEDES] [nvarchar](max) NULL,
	[TYPE_CODE] [nvarchar](max) NULL,
	[TYPE_DESIGNATION] [nvarchar](max) NULL,
	[ZERO_COST_FLAG] [nvarchar](max) NULL,
	[SUPPLY_CODE] [nvarchar](max) NULL,
	[ENG_ATTRIBUTE] [nvarchar](max) NULL,
	[SHORTAGE_FLAG] [nvarchar](max) NULL,
	[FORECAST_CONSUMPTION_FLAG] [nvarchar](max) NULL,
	[STOCK_MANAGEMENT] [nvarchar](max) NULL,
	[INTRASTAT_CONV_FACTOR] [nvarchar](max) NULL,
	[PART_COST_GROUP_ID] [nvarchar](max) NULL,
	[DOP_CONNECTION] [nvarchar](max) NULL,
	[STD_NAME_ID] [nvarchar](max) NULL,
	[INVENTORY_VALUATION_METHOD] [nvarchar](max) NULL,
	[NEGATIVE_ON_HAND] [nvarchar](max) NULL,
	[TECHNICAL_COORDINATOR_ID] [nvarchar](max) NULL,
	[ACTUAL_COST_ACTIVATED] [nvarchar](max) NULL,
	[MAX_ACTUAL_COST_UPDATE] [nvarchar](max) NULL,
	[CUST_WARRANTY_ID] [nvarchar](max) NULL,
	[SUP_WARRANTY_ID] [nvarchar](max) NULL,
	[REGION_OF_ORIGIN] [nvarchar](max) NULL,
	[INVENTORY_PART_COST_LEVEL] [nvarchar](max) NULL,
	[EXT_SERVICE_COST_METHOD] [nvarchar](max) NULL,
	[SUPPLY_CHAIN_PART_GROUP] [nvarchar](max) NULL,
	[INVOICE_CONSIDERATION] [nvarchar](max) NULL,
	[INPUT_UNIT_MEAS_GROUP_ID] [nvarchar](max) NULL,
	[AUTOMATIC_CAPABILITY_CHECK] [nvarchar](max) NULL,
	[DOP_NETTING] [nvarchar](max) NULL,
	[CO_RESERVE_ONH_ANALYS_FLAG] [nvarchar](max) NULL,
	[QTY_CALC_ROUNDING] [nvarchar](max) NULL,
	[TEXT_ID$] [nvarchar](max) NULL,
	[LIFECYCLE_STAGE] [nvarchar](max) NULL,
	[FREQUENCY_CLASS] [nvarchar](max) NULL,
	[FIRST_STAT_ISSUE_DATE] [nvarchar](max) NULL,
	[LATEST_STAT_ISSUE_DATE] [nvarchar](max) NULL,
	[MIN_DURAB_DAYS_CO_DELIV] [nvarchar](max) NULL,
	[MIN_DURAB_DAYS_PLANNING] [nvarchar](max) NULL,
	[STORAGE_WIDTH_REQUIREMENT] [nvarchar](max) NULL,
	[STORAGE_HEIGHT_REQUIREMENT] [nvarchar](max) NULL,
	[STORAGE_DEPTH_REQUIREMENT] [nvarchar](max) NULL,
	[STORAGE_VOLUME_REQUIREMENT] [nvarchar](max) NULL,
	[STORAGE_WEIGHT_REQUIREMENT] [nvarchar](max) NULL,
	[MIN_STORAGE_TEMPERATURE] [nvarchar](max) NULL,
	[MAX_STORAGE_TEMPERATURE] [nvarchar](max) NULL,
	[MIN_STORAGE_HUMIDITY] [nvarchar](max) NULL,
	[MAX_STORAGE_HUMIDITY] [nvarchar](max) NULL,
	[STANDARD_PUTAWAY_QTY] [nvarchar](max) NULL,
	[PALLET_HANDLED] [nvarchar](max) NULL,
	[PUTAWAY_ZONE_REFILL_OPTION] [nvarchar](max) NULL,
	[ROWVERSION] [nvarchar](max) NULL,
	[ROWKEY] [nvarchar](max) NULL,
	[N_PART_GROUP] [nvarchar](max) NULL,
	[N_SEMI_PRODUCT_FLAG] [nvarchar](max) NULL,
	[N_PRODUCT_CATEGORY] [nvarchar](max) NULL,
	[N_COST_DELIVER] [nvarchar](max) NULL,
	[N_COST_PRODUCTION_FORM] [nvarchar](max) NULL,
	[N_COST_OEM] [nvarchar](max) NULL,
	[N_OBL_PART_TYPE] [nvarchar](max) NULL,
	[N_DISCONTINUED_DATE] [nvarchar](max) NULL,
	[N_DISCONTINUED_TYPE] [nvarchar](max) NULL,
	[N_PRODUCTION_MODEL_NO] [nvarchar](max) NULL,
	[N_APO_MODEL_NO] [nvarchar](max) NULL,
	[N_CUSTOMER_MODEL_NO] [nvarchar](max) NULL,
	[N_PROJECT_MODEL_NO] [nvarchar](max) NULL,
	[N_OBL_PART_NO] [nvarchar](max) NULL,
	[N_RELAY_MODEL_NO] [nvarchar](max) NULL,
	[N_IFS_SUFFIX] [nvarchar](max) NULL,
	[N_CUSTOMER_SUFFIX] [nvarchar](max) NULL,
	[N_SUFFIX] [nvarchar](max) NULL,
	[N_PP_DATE] [nvarchar](max) NULL,
	[N_CAR_MODEL] [nvarchar](max) NULL,
	[N_SALES_REMAIN_NUMBER] [nvarchar](max) NULL,
	[N_COST_STRUCTURE_TABLE] [nvarchar](max) NULL,
	[N_COST_EXP_DOM] [nvarchar](max) NULL,
	[N_COST_MODULE_TYPE] [nvarchar](max) NULL,
	[N_COST_SEPARATE_SETTING] [nvarchar](max) NULL,
	[N_RATE_DIVISION] [nvarchar](max) NULL,
	[N_MOLD_DIE] [nvarchar](max) NULL,
	[N_KD_MATERIAL] [nvarchar](max) NULL,
	[N_CHARGED_SUPPLY] [nvarchar](max) NULL,
	[N_SALES_CHANNEL] [nvarchar](max) NULL,
	[N_PRODUCTION_AREA] [nvarchar](max) NULL,
	[N_PART_CLASS1] [nvarchar](max) NULL,
	[N_PART_CLASS2] [nvarchar](max) NULL,
	[N_SUB_MATERIAL_INFONO] [nvarchar](max) NULL,
	[N_INVENTORY_PART_TYPE] [nvarchar](max) NULL,
	[N_PART_ORIGIN_SYSTEM] [nvarchar](max) NULL,
	[N_RETURNABLE_CONTAINER_QTY] [nvarchar](max) NULL,
	[N_OEM_MARK] [nvarchar](max) NULL,
	[N_TOOL_TYPE] [nvarchar](max) NULL,
	[N_STOP_STOR_AND_RETR] [nvarchar](max) NULL,
	[N_CORE_UNIT] [nvarchar](max) NULL,
	[N_CONTROL_TYPE] [nvarchar](max) NULL,
	[N_IC_TYPE] [nvarchar](max) NULL,
	[N_MAINT_ABOL_TOTAL_NO] [nvarchar](max) NULL,
	[N_LIFE_CYCLE_FLAG] [nvarchar](max) NULL,
	[N_RETURN_BOX_OUTER_PROP] [nvarchar](max) NULL,
	[N_OPERATION_CODE] [nvarchar](max) NULL,
	[N_SYNPLA_LINKED] [nvarchar](max) NULL,
	[N_TECHN_INTEG_CODE] [nvarchar](max) NULL,
	[N_MANUF_INTEG_CODE] [nvarchar](max) NULL,
	[N_SP_FIX_LT] [nvarchar](max) NULL,
	[N_FIRST_STORAGE_DATE] [nvarchar](max) NULL,
	[N_PROD_SYNPIX_TRAN_DATE] [nvarchar](max) NULL,
	[N_SUPP_SYNPIX_TRAN_DATE] [nvarchar](max) NULL,
	[N_UN_ANNOUNCEMENT_LT] [nvarchar](max) NULL,
	[N_JIG_PARALLEL] [nvarchar](max) NULL,
	[N_DIR_DEL_FLAG] [nvarchar](max) NULL,
	[N_CUST_CHANGE_NO] [nvarchar](max) NULL,
	[N_CUST_CHANGE_DATE] [nvarchar](max) NULL,
	[N_SP_CAT_SUM_CODE] [nvarchar](max) NULL,
	[N_RESOURCE] [nvarchar](max) NULL,
	[N_INTERNAL_MACHINE_FLAG] [nvarchar](max) NULL,
	[N_GRILLE_FLAG] [nvarchar](max) NULL,
	[N_IF_LATEST_DATE] [nvarchar](max) NULL,
	[N_RET_CONT_BOX_FACTORY] [nvarchar](max) NULL,
	[N_RET_CONT_BOX_NO] [nvarchar](max) NULL,
	[N_RET_CONT_BOX_CARD_COLOR] [nvarchar](max) NULL,
	[N_ID_SYMBOL] [nvarchar](max) NULL,
	[N_ID_BASE_COLOR] [nvarchar](max) NULL,
	[N_ID_TEXT_COLOR] [nvarchar](max) NULL,
	[N_CUST_PROD_CATEGORY_ID] [nvarchar](max) NULL,
	[N_CUSTOMER_SERIES_DESC] [nvarchar](max) NULL,
	[N_CUSTOMER_PROJECT_CODE] [nvarchar](max) NULL,
	[N_USE_PART_TYPE_HOME] [nvarchar](max) NULL,
	[N_USE_PART_TYPE_CAR] [nvarchar](max) NULL,
	[N_CONTROL_CODE] [nvarchar](max) NULL,
	[N_OBL_EXPORT_CONTROL] [nvarchar](max) NULL,
	[N_SERVICE_FLAG] [nvarchar](max) NULL,
	[N_PRODUCED_QUANTITY] [nvarchar](max) NULL,
	[N_CREATED_DATE] [datetime] NULL,
	[N_CREATED_USER] [nvarchar](max) NULL,
	[N_UPDATED_DATE] [datetime] NULL,
	[N_UPDATED_USER] [nvarchar](max) NULL,
	[N_MOLD_PART_NO] [nvarchar](max) NULL,
	[N_GROUP_NO] [nvarchar](max) NULL,
	[N_PARTIALLY_SHARE_MOLD_NO] [nvarchar](max) NULL,
	[N_BEFORE_SYNTHESIS_PART_NO] [nvarchar](max) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[INVENTORY_TRANSACTION_HIST_TAB]    Script Date: 12/02/2015 13:58:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[INVENTORY_TRANSACTION_HIST_TAB](
	[TRANSACTION_ID] [nvarchar](max) NULL,
	[ACCOUNTING_ID] [nvarchar](max) NULL,
	[PART_NO] [nvarchar](max) NULL,
	[CONTRACT] [nvarchar](max) NULL,
	[CONFIGURATION_ID] [nvarchar](max) NULL,
	[LOCATION_NO] [nvarchar](max) NULL,
	[LOT_BATCH_NO] [nvarchar](max) NULL,
	[SERIAL_NO] [nvarchar](max) NULL,
	[WAIV_DEV_REJ_NO] [nvarchar](max) NULL,
	[ENG_CHG_LEVEL] [nvarchar](max) NULL,
	[ACTIVITY_SEQ] [nvarchar](max) NULL,
	[ORDER_NO] [nvarchar](max) NULL,
	[RELEASE_NO] [nvarchar](max) NULL,
	[SEQUENCE_NO] [nvarchar](max) NULL,
	[LINE_ITEM_NO] [nvarchar](max) NULL,
	[REJECT_CODE] [nvarchar](max) NULL,
	[TRANSACTION_CODE] [nvarchar](max) NULL,
	[PRE_ACCOUNTING_ID] [decimal](18, 0) NULL,
	[DATE_CREATED] [datetime] NULL,
	[DATE_TIME_CREATED] [datetime] NULL,
	[DATE_APPLIED] [datetime] NULL,
	[DIRECTION] [nvarchar](max) NULL,
	[ORDER_TYPE] [nvarchar](max) NULL,
	[PARTSTAT_FLAG] [nvarchar](max) NULL,
	[QTY_REVERSED] [decimal](28, 8) NULL,
	[DEL_TYPE] [nvarchar](max) NULL,
	[QUANTITY] [decimal](28, 8) NULL,
	[SOURCE] [nvarchar](max) NULL,
	[USERID] [nvarchar](max) NULL,
	[VALUESTAT_FLAG] [nvarchar](max) NULL,
	[OWNING_VENDOR_NO] [nvarchar](max) NULL,
	[OWNING_CUSTOMER_NO] [nvarchar](max) NULL,
	[PART_OWNERSHIP] [nvarchar](max) NULL,
	[ORIGINAL_TRANSACTION_ID] [nvarchar](max) NULL,
	[LOCATION_GROUP] [nvarchar](max) NULL,
	[CONDITION_CODE] [nvarchar](max) NULL,
	[PREVIOUS_PART_OWNERSHIP] [nvarchar](max) NULL,
	[PREVIOUS_OWNING_CUSTOMER_NO] [nvarchar](max) NULL,
	[PREVIOUS_OWNING_VENDOR_NO] [nvarchar](max) NULL,
	[DELIVERY_OVERHEAD] [nvarchar](max) NULL,
	[EXPIRATION_DATE] [nvarchar](max) NULL,
	[RECEIPT_DATE] [nvarchar](max) NULL,
	[PRE_TRANS_LEVEL_QTY_IN_STOCK] [nvarchar](max) NULL,
	[PRE_TRANS_LEVEL_QTY_IN_TRANSIT] [nvarchar](max) NULL,
	[PROJECT_ID] [nvarchar](max) NULL,
	[CATCH_QUANTITY] [nvarchar](max) NULL,
	[CATCH_DIRECTION] [nvarchar](max) NULL,
	[ALT_SOURCE_REF1] [nvarchar](max) NULL,
	[ALT_SOURCE_REF2] [nvarchar](max) NULL,
	[ALT_SOURCE_REF3] [nvarchar](max) NULL,
	[ALT_SOURCE_REF4] [nvarchar](max) NULL,
	[ALT_SOURCE_REF_TYPE] [nvarchar](max) NULL,
	[INVENTORY_PART_COST_LEVEL] [nvarchar](max) NULL,
	[INVENTORY_VALUATION_METHOD] [nvarchar](max) NULL,
	[PALLET_ID] [nvarchar](max) NULL,
	[TRANSACTION_REPORT_ID] [nvarchar](max) NULL,
	[MODIFY_DATE_APPLIED_DATE] [nvarchar](max) NULL,
	[MODIFY_DATE_APPLIED_USER] [nvarchar](max) NULL,
	[TRANSIT_LOCATION_GROUP] [nvarchar](max) NULL,
	[ABNORMAL_DEMAND] [nvarchar](max) NULL,
	[REPORT_EARNED_VALUE] [nvarchar](max) NULL,
	[ROWVERSION] [datetime] NULL,
	[ROWKEY] [nvarchar](max) NULL,
	[N_VOUCHER_NO] [nvarchar](max) NULL,
	[N_VOUCHER_REASON_CD] [nvarchar](max) NULL,
	[N_GDW_SEND_FLAG] [nvarchar](max) NULL,
	[N_PROJECT_NO] [nvarchar](max) NULL,
	[N_PRODUCT_GROUP] [nvarchar](max) NULL,
	[N_SENSOR_SEND_DATE] [nvarchar](max) NULL,
	[N_CREATED_DATE] [datetime] NULL,
	[N_CREATED_USER] [nvarchar](max) NULL,
	[N_UPDATED_DATE] [datetime] NULL,
	[N_UPDATED_USER] [nvarchar](max) NULL,
	[N_TPC_OS_SEND_DATE] [nvarchar](max) NULL,
	[N_TPC_NA_SEND_DATE] [nvarchar](max) NULL,
	[N_PROC_SUPPLIER_CLASS] [nvarchar](max) NULL,
	[N_SUPPLIER_PRODLINE] [nvarchar](max) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[N_AIS_SHOP_LIST_PICKED_ACT_TAB]    Script Date: 12/02/2015 13:58:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[N_AIS_SHOP_LIST_PICKED_ACT_TAB](
	[N_SEQ_NO] [bigint] NOT NULL,
	[CONTRACT] [varchar](5) NULL,
	[N_SHOP_LIST_ID] [varchar](14) NULL,
	[PART_NO] [varchar](25) NULL,
	[LOT_BATCH_NO] [varchar](20) NULL,
	[WORK_CENTER_NO] [varchar](5) NULL,
	[ENG_CHG_LEVEL] [varchar](6) NULL,
	[LOCATION_NO] [varchar](35) NULL,
	[N_PICKED_DATE] [datetime] NULL,
	[N_QTY_PICKED] [int] NULL,
	[N_Z_AXIS] [varchar](10) NULL,
	[N_POSITION] [varchar](10) NULL,
	[N_CREATED_DATE] [datetime] NULL,
	[N_CREATED_USER] [varchar](30) NULL,
	[N_UPDATED_DATE] [datetime] NULL,
	[N_UPDATED_USER] [varchar](30) NULL,
	[ROWVERSION] [datetime] NULL,
	[ROWKEY] [varchar](50) NULL,
 CONSTRAINT [PK_N_AIS_SHOP_LIST_PICKED_ACT_TAB] PRIMARY KEY CLUSTERED 
(
	[N_SEQ_NO] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
