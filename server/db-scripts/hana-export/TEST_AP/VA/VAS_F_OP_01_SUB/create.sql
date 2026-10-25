CREATE PROCEDURE VAS_F_OP_01_SUB (
	in objtype	nvarchar(20),
	in docKey	int
)
AS
BEGIN
	declare JEID int;
	if :objtype='30' then
		JEID :=docKey;
	else
		select "TransId" into JEID from OJDT where "TransType"=:objtype and "CreatedBy"=:docKey;
	end if;
	create local temporary table #tblJE (
		NOTE				nvarchar(200)
		,ACCOUNT			nvarchar(254)
		,AMNT				numeric(19,6)
		,DECIAMT			int default 0
		,DecSep				nvarchar(5) default '.'
		,ThousSep			nvarchar(5) default ','
	);
	insert into #tblJE (NOTE,ACCOUNT,AMNT)
	select 
		case when "DebCred"='D' then 'Nợ/Debit' else 'Có/Credit' end,
		"Account","Debit"+"Credit"
	from JDT1 
	where "TransId"=:JEID and ("Debit"<>0 or "Credit"<>0);

	select * from #tblJE;
	drop table #tblJE;
END;

