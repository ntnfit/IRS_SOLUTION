

CREATE PROCEDURE APZ_MarketingCampaign_GetById(in Id int default 0)
AS
BEGIN
	
	create local temporary table #tempLine
	(
		"AbsEntry" int default 0,
		"ObjType" nvarchar(20) default null,
		"DocEntry" int default 0,
		"DocNum" int default 0,
		"ObjTypeName" nvarchar(200) default null,
		"Total" decimal(21,3) default 0,
		"Remark" nvarchar(254),
		"Currency" nvarchar(3)
	);
	
	select * from APZ_SMC1 where "DocEntry" = :Id;
	select * from APZ_SMC2 where "DocEntry" = :Id;
	select * from APZ_SMC3 where "DocEntry" = :Id;
	select * from APZ_SMC4 where "DocEntry" = :Id;
	select * from APZ_SMC5 where "DocEntry" = :Id;
	select *  from APZ_OSMC where "ParentCampaign"= :Id;
	
	insert into #tempLine("AbsEntry", "ObjType" , "DocEntry", "DocNum", "ObjTypeName", "Total", "Remark", "Currency")
	select :id, '23', "DocEntry", "DocNum", 'Sale Quotations', "DocTotal", "Comments", "DocCur" from OQUT 
	where "U_RelatedTyp" = '1470000145'  and "U_RelatedEnt" = 1;
	
	--select 0 as "DocNum", '' as "ObjTypeName", 0 as "Total", '' as Remark,'' as "Currency" into #tempLine from APZ_PRG5;

	insert into #tempLine("AbsEntry", "ObjType" , "DocEntry", "DocNum", "ObjTypeName", "Total", "Remark", "Currency")
	select :Id,'17', "DocEntry", "DocNum", 'Sale Order', "DocTotal", "Comments", "DocCur" from ORDR where "U_RelatedTyp" = '1470000145' and "U_RelatedEnt" =:Id;
	
	insert into #templine("AbsEntry", "ObjType" , "DocEntry", "DocNum", "ObjTypeName", "Total", "Remark", "Currency")
	select :Id,'1470000113', q1."DocEntry", q1."DocNum",'Purchase Request', q1."DocTotal", q1."Comments", q1."DocCur" from OPRQ q1 where "U_RelatedTyp" = '1470000145' and "U_RelatedEnt" = :Id;

	insert into #tempLine("AbsEntry", "ObjType" , "DocEntry", "DocNum", "ObjTypeName", "Total", "Remark", "Currency")
	select :Id, '20', q1."DocEntry", q1."DocNum", 'Goods Receipt PO', q1."DocTotal", q1."Comments", q1."DocCur" from OPDN q1 where "U_RelatedTyp" = '1470000145' and "U_RelatedEnt" = :Id;


	insert into #tempLine("AbsEntry", "ObjType" , "DocEntry", "DocNum", "ObjTypeName", "Total", "Remark", "Currency")
	select :Id,'22', q1."DocEntry", q1."DocNum", 'Purchase Order', q1."DocTotal", q1."Comments", q1."DocCur" from OPOR q1 where "U_RelatedTyp" = '1470000145'  and "U_RelatedEnt" = :Id;


	insert into #tempLine("AbsEntry", "ObjType" , "DocEntry", "DocNum", "ObjTypeName", "Total", "Remark", "Currency")
	select :Id,'15', q1."DocEntry", q1."DocNum", 'Delivery', q1."DocTotal", q1."Comments", q1."DocCur" from ODLN q1 where "U_RelatedTyp" = '1470000145'  and "U_RelatedEnt" = :Id;


	insert into #tempLine("AbsEntry", "ObjType" , "DocEntry", "DocNum", "ObjTypeName", "Total", "Remark", "Currency")
	select :Id,'18', q1."DocEntry", q1."DocNum", 'A/P Invoice', q1."DocTotal", q1."Comments", q1."DocCur" from OPCH q1 where "U_RelatedTyp" = '1470000145' and "U_RelatedEnt" = :Id;

	insert into #tempLine("AbsEntry", "ObjType" , "DocEntry", "DocNum", "ObjTypeName", "Total", "Remark", "Currency")
	select :Id, '13', q1."DocEntry", q1."DocNum", 'A/R Invoice', q1."DocTotal", q1."Comments", q1."DocCur" from OINV q1 where "U_RelatedTyp" = '1470000145'  and "U_RelatedEnt" = :Id;

	insert into #tempLine("AbsEntry", "ObjType" , "DocEntry", "DocNum", "ObjTypeName", "Total", "Remark", "Currency")
	select :Id,'540000006', q1."DocEntry", q1."DocNum", 'Purchase Quotation', q1."DocTotal", q1."Comments", q1."DocCur" from OPQT q1 where "U_RelatedTyp" = '1470000145' and "U_RelatedEnt" = :Id;

	insert into #tempLine("AbsEntry", "ObjType" , "DocEntry", "DocNum", "ObjTypeName", "Total", "Remark", "Currency")
	select :Id,'203', q1."DocEntry", q1."DocNum" ,'A/R Down Payment Request', q1."DocTotal", q1."Comments", q1."DocCur" from ODPI q1 where "U_RelatedTyp" = '1470000145'  and "U_RelatedEnt" = :Id;

	insert into #tempLine("AbsEntry", "ObjType" , "DocEntry", "DocNum", "ObjTypeName", "Total", "Remark", "Currency")
	select :Id, '204', q1."DocEntry", q1."DocNum", 'A/P Down Payment Request', q1."DocTotal", q1."Comments", q1."DocCur" from ODPO q1 where "U_RelatedTyp" = '1470000145'  and "U_RelatedEnt" = :Id;

	select * from #tempLine where "AbsEntry" = :Id;

END;






