CREATE procedure APZ_CTA_TaoPhieuBaoTriBaoDuongHangLoat
as
begin
	create local temporary table #temp -- Bang nay chua du lieu tu UI (giao dien)
	(
		TASKLISTNO nvarchar(50),
		TASKLISTNAME nvarchar(250),
		FIXASSETCODE nvarchar(50),
		FIXASSETNAME nvarchar(250),
		FLOC nvarchar(100),
		TOTALNUMBEROFITEMS smallint,
		ACTIVITYTYPE nvarchar(50),
		CREATEDBYCODE nvarchar(50),
		CREATEDBYNAME nvarchar(100),
		PWO nvarchar(50),
		STARTDATE nvarchar(50),
		ENDDATE nvarchar(50),
		NEXTDATE nvarchar(50),
		ROOM nvarchar(50),
		WORKCENTER nvarchar(100),
		INTERVAL smallint,
		NOOFPEOPLE smallint,
		ROOMNAME nvarchar(100),
		DOCTYPE nvarchar(20)
		
	);
	execute immediate 'drop table #temp';
	execute immediate 'create local temporary table #temp  as  (select  * from #TEMPUI);';
	-- temp 2
	create local temporary table #temp2 -- Bang nay chua du lieu tu UI (giao dien)
	(
		Tasklistno Nvarchar(50),
		Tasklistname Nvarchar(250),
		Fixassetcode Nvarchar(50),
		Fixassetname Nvarchar(250),
		Floc Nvarchar(100),
		Activitytype Nvarchar(50),
		Createdbycode Nvarchar(50),
		Createdbyname Nvarchar(100),
		Pwo Nvarchar(50),
		Startdate Nvarchar(50),
		Enddate Nvarchar(50),
		Nextdate Nvarchar(50),
		Room Nvarchar(50),
		Workcenter Nvarchar(100),
		Interval Smallint,
		Noofpeople Smallint,
		Roomname Nvarchar(100),
		--Chi Tiet Bao Tri Bao Duong
		Itemcode Nvarchar(50),
		Itemname Nvarchar(100),
		Quantity Decimal(19,6),
		Uom Nvarchar(20),
		Price Decimal(21,6),
		Amount Decimal(21,6),
		Doctype nvarchar(20)

	);

Insert into  #temp2 (Tasklistno,
						Tasklistname,
						Fixassetcode,
						Fixassetname,
						Floc,
						Activitytype,
						Createdbycode,
						Createdbyname,
						Pwo,
						Startdate,
						Enddate,
						Nextdate,
						Room,
						Workcenter,
						Interval,
						Noofpeople,
						Roomname,
						--Chi Tiet Bao Tri Bao Duong
						Itemcode,
						Itemname,
						Quantity,
						Uom,
						Price,
						Amount,
						Doctype)
select  a.TASKLISTNO,
		a.TASKLISTNAME,
		a.FIXASSETCODE,
		a.FIXASSETNAME,
		a.FLOC,
		a.ACTIVITYTYPE,
		a.CREATEDBYCODE,
		a.CREATEDBYNAME,
		a.PWO,
		cast(a.STARTDATE as date),
		cast(a.ENDDATE as date),
		cast(a.NEXTDATE as date),
		a.ROOM,
		a.WORKCENTER,
		a.INTERVAL,
		a.NOOFPEOPLE,
		a.ROOMNAME,
		--Chi Tiet Bao Tri Bao Duong
		bom."U_ItemCode",
		bom."U_ItemName",
		bom."U_Quantity",
		bom."U_UOM",
		0 as "Price",
		0 as "Amount",
		a.DOCTYPE

from #temp a
inner join (select a."U_ItemCodeF",b.*
			from "@BOMAINTENANCE_H" a
			inner join  "@BOMAINTENANCE_L" b on a."DocEntry" = b."DocEntry") bom 
				   on a.FIXASSETCODE = bom."U_ItemCodeF" and  a.FLOC = bom."U_Floc"
left join  OITM b on bom."U_ItemCode"		=  b."ItemCode"   
where  a.TOTALNUMBEROFITEMS  >0 and   a.INTERVAL  >0 and
(b."U_IsNeedCheckM" = case  ifnull(a.DOCTYPE,'')    when 'Maintainance' then 'Y' end)
	 or  (b."U_IsNeedCheckHRE" = case  ifnull(a.DOCTYPE,'')    when 'HighRiskEquipment' then 'Y' end);
--
select * from #temp2 order by FIXASSETCODE;
--
drop table #temp2;
drop table #temp;
exec 'drop table #TEMPUI';

end;

