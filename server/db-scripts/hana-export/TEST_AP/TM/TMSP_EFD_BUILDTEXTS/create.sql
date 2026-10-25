create procedure TmSP_EFD_BuildTexts (in PeriodFrom datetime DEFAULT '19700101', in PeriodTo datetime DEFAULT '20371231', in BranchId int DEFAULT '-1')
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER
AS

strIndex int;
lastStrType nchar;
lastStrText nvarchar(4000);
CURSOR StrCursor FOR
	select D0."ObjType", D0."DocEntry", 'F' as "StrType", D0."TaxDate", cast(D0."Footer" as nvarchar (4000)) as "StrText", D0."BPLId" from "OINV" D0
		INNER JOIN "ONFM" T1 ON T1."AbsEntry" = D0."Model"	
		Where D0."Footer" is not null and cast(D0."Footer" as nvarchar (4000)) <> '' and D0."TaxDate" BETWEEN :PeriodFrom AND :PeriodTo
			AND T1."NfmCode" in ('1','1B','1/A','4','55','65',        '6','28','29',         '7','8','8B','9','10','11','26','27','57',  '21','22')
	UNION ALL							 		 
	select D0."ObjType", D0."DocEntry", 'H' as "StrType", D0."TaxDate", cast(D0."Header" as nvarchar (4000)) as "StrText", D0."BPLId" from "OINV" D0
		INNER JOIN "ONFM" T1 ON T1."AbsEntry" = D0."Model"	
		Where D0."Header" is not null and cast(D0."Header" as nvarchar (4000)) <> '' and D0."TaxDate" BETWEEN :PeriodFrom AND :PeriodTo
			AND T1."NfmCode" in ('1','1B','1/A','4','55','65',  '2',  '6','28','29',  '59',  '7','8','8B','9','10','11','26','27','57',  '21','22')
			AND D0."CANCELED" = 'N'
	UNION ALL							 		 
	select D0."ObjType", D0."DocEntry", 'F' as "StrType", D0."TaxDate", cast(D0."Footer" as nvarchar (4000)) as "StrText", D0."BPLId" from "OPCH" D0
		INNER JOIN "ONFM" T1 ON T1."AbsEntry" = D0."Model"
		Where D0."Footer" is not null and cast(D0."Footer" as nvarchar (4000)) <> '' and D0."DocDate" BETWEEN :PeriodFrom AND :PeriodTo
			AND T1."NfmCode" in ('1','1B','1/A','4','55','65',        '6','28','29',         '7','8','8B','9','10','11','26','27','57',  '21','22')
	UNION ALL							 		 
	select D0."ObjType", D0."DocEntry", 'H' as "StrType", D0."TaxDate", cast(D0."Header" as nvarchar (4000)) as "StrText", D0."BPLId" from "OPCH" D0
		INNER JOIN "ONFM" T1 ON T1."AbsEntry" = D0."Model"
		Where D0."Header" is not null and cast(D0."Header" as nvarchar (4000)) <> '' and D0."DocDate" BETWEEN :PeriodFrom AND :PeriodTo
			AND T1."NfmCode" in ('1','1B','1/A','4','55','65',  '2',  '6','28','29',  '59',  '7','8','8B','9','10','11','26','27','57',  '21','22')
			AND D0."CANCELED" = 'N'
	ORDER BY "StrType", "StrText";

begin

if 1=0 then
create local temporary table "#EFDStrings"
(
	"StrNum" int,
	"StrType" nchar,
	"StrText" nvarchar (4000)
);

create local temporary table "#EFDStringMapping"
(
	"DocType" int,
	"DocEntry" int,
	"StrType" nchar,
	"StrNum" int
);

end if;


strIndex := 0;
lastStrType := '/';
lastStrText := '';

FOR cur_row as StrCursor DO
	IF BranchId < 1 or cur_row."BPLId" = BranchId THEN
		IF cur_row."StrText" <> lastStrText or cur_row."StrType" <> lastStrType THEN
			strIndex := strIndex + 1;
			insert into "#EFDStrings" ("StrNum", "StrType", "StrText") values (strIndex, cur_row."StrType", cur_row."StrText");
			lastStrType := cur_row."StrType";
			lastStrText := cur_row."StrText";
		END IF;

		insert into "#EFDStringMapping" ("DocType", "DocEntry", "StrType", "StrNum")
			values (cur_row."ObjType", cur_row."DocEntry", cur_row."StrType", strIndex);
	END IF;
END FOR;

CLOSE StrCursor;

end;


