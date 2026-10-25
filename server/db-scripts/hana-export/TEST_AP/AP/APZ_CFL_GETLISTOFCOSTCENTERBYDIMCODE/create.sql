
create procedure APZ_CFL_GetListOfCostCenterByDimCode(in dimCode int default 0)
as
begin
	create local temporary table #TBLSTRUCTURE
	(
		"ColumnName" nvarchar(50) not null,
        "Caption" nvarchar(100) null,
        "DataType" varchar(20) default 'NVARCHAR', 
        "Width" int default 75,
        "GroupIndex" int default -1,
        "Visible" int default 1,
        "FormatType" nvarchar(20) null, 
        "DisplayFormat" nvarchar(20) null, 
        "ReadOnly" int default 0,
        "AllowEdit" int default 1,
        "AllowMove" int default 1,
        "AllowSize" int default 1,
        "AllowSort" int default 1,
        "Fixed" varchar(1) null,
        "SummaryType" varchar(10) null,	
        "GroupSummaryType" varchar(10) null	
	);
	
	insert into #TBLSTRUCTURE("ColumnName","Caption","DataType","Width","GroupIndex","Visible","FormatType","DisplayFormat","ReadOnly","AllowEdit","AllowMove","AllowSize","AllowSort","Fixed","SummaryType","GroupSummaryType")
	values('PrcCode','Center Code','NVARCHAR', 75, -1, 1, NULL, NULL, 1, 0, 1, 1, 1, 'L', NULL, NULL);
	insert into #TBLSTRUCTURE("ColumnName","Caption","DataType","Width","GroupIndex","Visible","FormatType","DisplayFormat","ReadOnly","AllowEdit","AllowMove","AllowSize","AllowSort","Fixed","SummaryType","GroupSummaryType")
	values('PrcName','Center Name','NVARCHAR',200,-1,1,NULL,NULL, 1, 0, 1, 1, 1, 'L', NULL,	NULL);
	
	select * from #TBLSTRUCTURE;  
	select "PrcCode","PrcName" from OPRC where "DimCode"=dimCode and "Locked"='N';
end;




