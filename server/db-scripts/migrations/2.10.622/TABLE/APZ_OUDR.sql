
CREATE COLUMN TABLE APZ_OUDR
(
		 "Code" nvarchar(20) NOT NULL PRIMARY KEY,
		 "Name" nvarchar(100) NULL,
		 "TableName" nvarchar(19) NOT NULL,
		 "StoreName" nvarchar(500) NOT NULL,
		 "LogTable" nvarchar(19) NULL,
		 "ReportFile" nvarchar(500) NULL,
		 TYPE char(1) NULL default '1',
		 "CanDelete" char(1) NULL DEFAULT 'N',
		 "CanClose" char(1) NULL DEFAULT 'N',
		 "CanCancel" char(1) NULL DEFAULT 'N',
		 "CanFind" char(1) NULL DEFAULT 'N',
		 "CanLog" char(1) NULL DEFAULT 'N',
		 "MenuItem" char(1) NULL DEFAULT 'N',
		 "MenuCapt" nvarchar(254) NULL,
		 "FatherMenu" nvarchar(254) NULL,
		 "Position" smallint NULL,
		 "MenuUid" nvarchar(32) NULL,
		 "LstUpdDate" timestamp NULL,
		 "LstUpdTime" integer NULL,
		 "QueryType" char(1) DEFAULT 'S',	-- S: store/ U: sap user query
		 "SystemReport" char(1) DEFAULT 'N',
		 "AllRowsInGrp" varchar(1) DEFAULT 'N',
		 "ShChkBSelect" char(1) DEFAULT 'Y',
		 "EnbExlHeader" char(1) DEFAULT 'N',
		 "ExcelHeader" nvarchar(4000)
);