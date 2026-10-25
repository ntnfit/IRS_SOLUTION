
CREATE PROCEDURE apz_retail_SaveCmdConfig

AS
BEGIN	
create local temporary table #temp_RCMC  ("CmdID" int NOT NULL,
	"CmdName" nvarchar(100) NULL,
	"Text" nvarchar(254) NULL,
	"TextColor" int NULL,
	"BackColor" int NULL,
	"Hotkey" nvarchar(254) NULL,
	"MnGroup" int NULL,
	"VisOrder" int NULL,
	"Visible" char(1) DEFAULT 'Y' NULL);
	
	-- ở đây là lệnh đổ dữ liệu từ table bulkcopy sang table tep vừa tạo chú ý table temp từ bulk copy có dấu nháy "" vì phân biệt hoa thường
	execute immediate ' insert into #temp_RCMC select * from #APZ_RCMC;';

	delete from APZ_RCMC;
	
	UPSERT APZ_RCMC select  Case when ifnull(S."CmdID",0) = 0 then (SELECT IFNULL(MAX("CmdID"), 0) + 1 FROM APZ_RCMC)
else S."CmdID" end,S."CmdName" ,S."Text" ,S."TextColor" ,S."BackColor" ,S."Hotkey",S."MnGroup",S."VisOrder" ,S."Visible" from #temp_RCMC S;
END





