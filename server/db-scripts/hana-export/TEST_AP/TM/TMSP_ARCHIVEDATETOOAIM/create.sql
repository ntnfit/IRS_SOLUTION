-- B1 DEPENDS: AFTER:PT:PROCESS_END

CREATE PROCEDURE TmSp_ArchiveDateToOAIM
(
in archiveDate		 DateTime
)
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER
	
	--*With Encryption

AS

BEGIN
insert into OAIM
("MessageID","DocEntry","TransType","DocLineNum","Quantity","EffectQty","LocType","LocCode","TotalLC",
	"TotalFC","BaseAbsEnt","BaseType","Currency","AccumType","ActionType","ExpensesLC","ExpensesFC","ItemCode",
	"DocDate","DocRate","JrnlMemo","BaseLine","CreateTime","CreateDate","OcrCode","OcrCode2","OcrCode3",
	"OcrCode4","OcrCode5","DocPrice","TreeType","ApplObj","AppObjAbs","AppObjType","AppObjLine","TransSeqRf",
	"LayerIDRef","VersionNum","PriceRate","PriceCurr","Price","CIShbQty","SubLineNum","PrjCode","UseDocPric",
	"Location","BSubLineNo","AppSubLine","DocAction")
select "MessageID","DocEntry","TransType","DocLineNum","Quantity","EffectQty","LocType","LocCode","TotalLC",
	"TotalFC","BaseAbsEnt","BaseType","Currency","AccumType","ActionType","ExpensesLC","ExpensesFC","ItemCode",
	"DocDate","DocRate","JrnlMemo","BaseLine","CreateTime","CreateDate","OcrCode","OcrCode2","OcrCode3",
	"OcrCode4","OcrCode5","DocPrice","TreeType","ApplObj","AppObjAbs","AppObjType","AppObjLine","TransSeqRf",
	"LayerIDRef","VersionNum","PriceRate","PriceCurr","Price","CIShbQty","SubLineNum","PrjCode","UseDocPric",
	"Location","BSubLineNo","AppSubLine","DocAction"
from oilm
where "CreateDate" <= :archiveDate and "AccumType" = 1 and "TransType" != 10000046;

END;


