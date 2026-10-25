-- B1 DEPENDS: AFTER:PT:PROCESS_END

CREATE PROCEDURE TmSp_UpdateNum
(
	in obj_id int,
	in dockey int,
	in docnum int,
	in tempnum int,
	in docsubtype nvarchar(2)
)
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER
AS
-- Return values
error  int;				-- Result (0 for no error)
error_message nvarchar (200); 		-- Error string to be displayed
begin

error := 0;
error_message := N'Ok';

if (:obj_id = 17) then	-- Sales Orders
UPDATE "ORDR" SET "DocNum"=:docnum, "Ref1"=:docnum WHERE "DocEntry"=:dockey;
UPDATE "OILM" SET "Ref1"=:docnum, "BASE_REF"=:docnum WHERE "DocEntry" = :dockey AND "TransType" = :obj_id;
UPDATE "OITL" SET "DocNum"=:docnum, "AppDocNum"=:docnum WHERE "ApplyType" = :obj_id AND "ApplyEntry" = :dockey;

elseif (:obj_id = 22) then	-- Purchase Orders
UPDATE "OPOR" SET "DocNum"=:docnum WHERE "DocEntry"=:dockey;
UPDATE "RDR1" SET "PoTrgNum"=:docnum WHERE "PoTrgEntry" = :dockey and "PcDocType" = :obj_id;
UPDATE "QUT1" SET "PoTrgNum"=:docnum WHERE "PoTrgEntry" = :dockey and "PcDocType" = :obj_id;
UPDATE "OILM" SET "Ref1"=:docnum, "BASE_REF"=:docnum WHERE "DocEntry" = :dockey AND "TransType" = :obj_id;

elseif (:obj_id = 23) then	-- Quotations
UPDATE "OQUT" SET "DocNum"=:docnum WHERE "DocEntry"=:dockey;

elseif (:obj_id = 540000006) then	-- Purchase Quotations
UPDATE "OPQT" SET "DocNum"=:docnum WHERE "DocEntry"=:dockey;
UPDATE "RDR1" SET "PoTrgNum"=:docnum WHERE "PoTrgEntry" = :dockey and "PcDocType" = :obj_id;
UPDATE "QUT1" SET "PoTrgNum"=:docnum WHERE "PoTrgEntry" = :dockey and "PcDocType" = :obj_id;

elseif (:obj_id = 13) then	-- AR Invoices
UPDATE "OINV" SET "DocNum"=:docnum,"Ref1"=:docnum WHERE "DocEntry"=:dockey;
UPDATE "OILM" SET "Ref1"=:docnum, "BASE_REF"=:docnum WHERE "DocEntry" = :dockey AND "TransType" = :obj_id;
UPDATE "OIVL" SET "BASE_REF"=:docnum WHERE "CreatedBy" = :dockey AND "TransType" = :obj_id;
--UPDATE "OJDT" SET "BaseRef"=:docnum, "Ref1"=:docnum WHERE "CreatedBy"=:dockey AND "TransType"=:obj_id;
--UPDATE "JDT1" SET "BaseRef"=:docnum, "Ref1"=:docnum WHERE "CreatedBy"=:dockey AND "TransType"=:obj_id;
UPDATE "OITL" SET "DocNum"=:docnum, "AppDocNum"=:docnum WHERE "ApplyType" = :obj_id AND "ApplyEntry" = :dockey;
UPDATE "NFN4" SET "DocNumber"=:docnum WHERE "ObjectCode" = :obj_id AND "DocEntry" = :dockey;
UPDATE "ORTI" SET "BaseRef"=:docnum WHERE "CreatedBy"=:dockey AND "TransType"=:obj_id;
UPDATE "OINS" SET "invoiceNum"=:docnum WHERE "invoice" = :dockey;

elseif (:obj_id = 15) then	-- Deliveries
UPDATE "ODLN" SET "DocNum"=:docnum,"Ref1"=:docnum WHERE "DocEntry"=:dockey;
UPDATE "OILM" SET "Ref1"=:docnum, "BASE_REF"=:docnum WHERE "DocEntry" = :dockey AND "TransType" = :obj_id;
UPDATE "OIVL" SET "BASE_REF"=:docnum WHERE "CreatedBy" = :dockey AND "TransType" = :obj_id;
--UPDATE "JDT1" SET "BaseRef"=:docnum, "Ref1"=:docnum WHERE "CreatedBy"=:dockey AND "TransType"=:obj_id;
UPDATE "OITL" SET "DocNum"=:docnum, "AppDocNum"=:docnum WHERE "ApplyType" = :obj_id AND "ApplyEntry" = :dockey;
UPDATE "NFN4" SET "DocNumber"=:docnum WHERE "ObjectCode" = :obj_id AND "DocEntry" = :dockey;
UPDATE "OINS" SET "deliveryNo"=:docnum WHERE "delivery" = :dockey;

elseif (:obj_id = 18) then	-- AP Invoices
UPDATE "OPCH" SET "DocNum"=:docnum,"Ref1"=:docnum WHERE "DocEntry"=:dockey;
UPDATE "OILM" SET "Ref1"=:docnum, "BASE_REF"=:docnum WHERE "DocEntry" = :dockey AND "TransType" = :obj_id;
UPDATE "OIVL" SET "BASE_REF"=:docnum WHERE "CreatedBy" = :dockey AND "TransType" = :obj_id;
--UPDATE "OJDT" SET "BaseRef"=:docnum, "Ref1"=:docnum WHERE "CreatedBy"=:dockey AND "TransType"=:obj_id;
--UPDATE "JDT1" SET "BaseRef"=:docnum, "Ref1"=:docnum WHERE "CreatedBy"=:dockey AND "TransType"=:obj_id;
UPDATE "OITL" SET "DocNum"=:docnum, "AppDocNum"=:docnum WHERE "ApplyType" = :obj_id AND "ApplyEntry" = :dockey;
UPDATE "NFN4" SET "DocNumber"=:docnum WHERE "ObjectCode" = :obj_id AND "DocEntry" = :dockey;
UPDATE "OACQ" SET "BaseRef"=:docnum WHERE "CreatedBy"=:dockey AND "TransType"=:obj_id;

elseif (:obj_id = 20) then	-- Goods Receipts
UPDATE "OPDN" SET "DocNum"=:docnum,"Ref1"=:docnum WHERE "DocEntry"=:dockey;
UPDATE "OILM" SET "Ref1"=:docnum, "BASE_REF"=:docnum WHERE "DocEntry" = :dockey AND "TransType" = :obj_id;
UPDATE "OIVL" SET "BASE_REF"=:docnum WHERE "CreatedBy" = :dockey AND "TransType" = :obj_id;
--UPDATE "OJDT" SET "BaseRef"=:docnum, "Ref1"=:docnum WHERE "CreatedBy"=:dockey AND "TransType"=:obj_id;
--UPDATE "JDT1" SET "BaseRef"=:docnum, "Ref1"=:docnum WHERE "CreatedBy"=:dockey AND "TransType"=:obj_id;
UPDATE "OITL" SET "DocNum"=:docnum, "AppDocNum"=:docnum WHERE "ApplyType" = :obj_id AND "ApplyEntry" = :dockey;
UPDATE "NFN4" SET "DocNumber"=:docnum WHERE "ObjectCode" = :obj_id AND "DocEntry" = :dockey;

elseif (:obj_id = 59) then	-- Goods Receipts
UPDATE "OIGN" SET "DocNum"=:docnum,"Ref1"=:docnum WHERE "DocEntry"=:dockey;
UPDATE "OILM" SET "Ref1"=:docnum, "BASE_REF"=:docnum WHERE "DocEntry" = :dockey AND "TransType" = :obj_id;
UPDATE "OIVL" SET "BASE_REF"=:docnum WHERE "CreatedBy" = :dockey AND "TransType" = :obj_id;
--UPDATE "OJDT" SET "BaseRef"=:docnum, "Ref1"=:docnum WHERE "CreatedBy"=:dockey AND "TransType"=:obj_id;
--UPDATE "JDT1" SET "BaseRef"=:docnum, "Ref1"=:docnum WHERE "CreatedBy"=:dockey AND "TransType"=:obj_id;
UPDATE "OITL" SET "DocNum"=:docnum, "AppDocNum"=:docnum WHERE "ApplyType" = :obj_id AND "ApplyEntry" = :dockey;
UPDATE "NFN4" SET "DocNumber"=:docnum WHERE "ObjectCode" = :obj_id AND "DocEntry" = :dockey;
UPDATE "ORTL" SET "DocNum"=:docnum WHERE "DocEntry" = :dockey AND "DocType" = :obj_id;

elseif (:obj_id = 60) then	-- Goods Receipts
UPDATE "OIGE" SET "DocNum"=:docnum,"Ref1"=:docnum WHERE "DocEntry"=:dockey;
UPDATE "OILM" SET "Ref1"=:docnum, "BASE_REF"=:docnum WHERE "DocEntry" = :dockey AND "TransType" = :obj_id;
UPDATE "OIVL" SET "BASE_REF"=:docnum WHERE "CreatedBy" = :dockey AND "TransType" = :obj_id;
--UPDATE "OJDT" SET "BaseRef"=:docnum, "Ref1"=:docnum WHERE "CreatedBy"=:dockey AND "TransType"=:obj_id;
--UPDATE "JDT1" SET "BaseRef"=:docnum, "Ref1"=:docnum WHERE "CreatedBy"=:dockey AND "TransType"=:obj_id;
UPDATE "OITL" SET "DocNum"=:docnum, "AppDocNum"=:docnum WHERE "ApplyType" = :obj_id AND "ApplyEntry" = :dockey;
UPDATE "NFN4" SET "DocNumber"=:docnum WHERE "ObjectCode" = :obj_id AND "DocEntry" = :dockey;
UPDATE "ORTL" SET "DocNum"=:docnum WHERE "DocEntry" = :dockey AND "DocType" = :obj_id;

elseif (:obj_id = 67) then	-- Goods Receipts
UPDATE "OWTR" SET "DocNum"=:docnum,"Ref1"=:docnum WHERE "DocEntry"=:dockey;
UPDATE "OILM" SET "Ref1"=:docnum, "BASE_REF"=:docnum WHERE "DocEntry" = :dockey AND "TransType" = :obj_id;
UPDATE "OIVL" SET "BASE_REF"=:docnum WHERE "CreatedBy" = :dockey AND "TransType" = :obj_id;
--UPDATE "OJDT" SET "BaseRef"=:docnum, "Ref1"=:docnum WHERE "CreatedBy"=:dockey AND "TransType"=:obj_id;
--UPDATE "JDT1" SET "BaseRef"=:docnum, "Ref1"=:docnum WHERE "CreatedBy"=:dockey AND "TransType"=:obj_id;
UPDATE "OITL" SET "DocNum"=:docnum, "AppDocNum"=:docnum WHERE "ApplyType" = :obj_id AND "ApplyEntry" = :dockey;
UPDATE "NFN4" SET "DocNumber"=:docnum WHERE "ObjectCode" = :obj_id AND "DocEntry" = :dockey;

elseif (:obj_id = 24) then  -- Incoming Payments
UPDATE "ORCT" SET "DocNum"=:docnum, "Ref1"=:docnum WHERE "DocEntry"=:dockey;
UPDATE "ORCT" SET "BoeNum"=:docnum WHERE "DocEntry"=:dockey and ("BoeNum" <= 0 OR "BoeNum" IS NULL);
UPDATE "OBOE" SET "BoeNum"=:docnum WHERE "PmntNum"=:dockey and "BoeType" = 'I' and ("BoeNum" <= 0 OR "BoeNum" IS NULL);
--UPDATE "OJDT" SET "BaseRef"=:docnum, "Ref1"=:docnum WHERE "CreatedBy"=:dockey AND "TransType"=:obj_id;

elseif (:obj_id = 46) then  -- Outgoing Payments
UPDATE "OVPM" SET "DocNum"=:docnum, "Ref1"=:docnum WHERE "DocEntry"=:dockey;
UPDATE "OVPM" SET "BoeNum"=:docnum WHERE "DocEntry"=:dockey and ("BoeNum" <= 0 OR "BoeNum" IS NULL);
UPDATE "OBOE" SET "BoeNum"=:docnum WHERE "PmntNum"=:dockey and "BoeType" = 'O' and ("BoeNum" <= 0 OR "BoeNum" IS NULL);
--UPDATE "OJDT" SET "BaseRef"=:docnum, "Ref1"=:docnum WHERE "CreatedBy"=:dockey AND "TransType"=:obj_id;
UPDATE "OCHO" SET "TransRef"=:docnum WHERE "PmntNum"=:dockey;

end if;

-- Handle reference link
if :obj_id = 13 or :obj_id = 15 or :obj_id = 18 or :obj_id = 20 or :obj_id = 59  or :obj_id = 60  or :obj_id = 67 then
	DECLARE status nchar;
	DECLARE rowidx int;
	DECLARE exe_cmd nvarchar(512);
	DECLARE isBasedOnWOR int;
	
	isBasedOnWOR :=0;
	if :obj_id = 59 then
		select count(*) into isBasedOnWOR from IGN1 where "DocEntry" = :dockey and "BaseType" = 202; 
	end if;
	if :obj_id = 60 then
		select count(*) into isBasedOnWOR from IGE1 where "DocEntry" = :dockey and "BaseType" = 202;
	end if;
	
	
	exe_cmd := 'update JDT1 set ';
	select "Status" into status from ORLD where "ObjectCode" = :obj_id and "DocSubType" = :docsubtype;	
	if status = 'C' and isBasedOnWOR = 0 then 
		DECLARE CURSOR C_L FOR select DISTINCT "TargetFld","TargetType" from RLD1 as t0 join ORLD as t1 on t0."AbsEntry" = t1."AbsEntry" where t0."TargetType" = 'L' and t0."SourceFld" = 'DocNum' and t1."ObjectCode" = :obj_id  and t1."DocSubType" = :docsubtype;
		DECLARE CURSOR C_B FOR select DISTINCT "TargetFld","TargetType" from RLD1 as t0 join ORLD as t1 on t0."AbsEntry" = t1."AbsEntry" where t0."TargetType" = 'B' and t0."SourceFld" = 'DocNum' and t1."ObjectCode" = :obj_id and t1."DocSubType" = :docsubtype;
		
		-- Always Update the BaseRef
		UPDATE "JDT1" SET "BaseRef"=:docnum WHERE "CreatedBy"=:dockey AND "TransType"=:obj_id;
		
		-- Note: because currently HANA doesn't support user defined function, I have to repeat some codes in the follow section. I don't want to encapsulate such small section into a SP either. 
		-- update the account line
		rowidx := 0;
		FOR R as C_L DO 
			if rowidx > 0 then 
				exe_cmd := exe_cmd || ',';
			end if;
			exe_cmd := exe_cmd || '"' || R."TargetFld" || '"' || '=' || :docnum;
			rowidx := rowidx + 1;
		END FOR;
		
		if rowidx > 0 then 
			exe_cmd := exe_cmd || ' where "CreatedBy"=' || dockey || ' and "TransType"=' || obj_id || ' and "Account" = "ShortName"';
			EXECUTE IMMEDIATE exe_cmd;
		end if;
		
		-- update the BP line
		exe_cmd := 'update JDT1 set ';
		rowidx := 0;
		FOR R as C_B DO 
			if rowidx > 0 then 
				exe_cmd := exe_cmd || ',';
			end if;
			exe_cmd := exe_cmd || '"' || R."TargetFld" || '"' || '=' || :docnum;
			rowidx := rowidx + 1;
		END FOR;
		if rowidx > 0 then
			exe_cmd := exe_cmd || ' where "CreatedBy"=' || :dockey || ' and "TransType"=' || :obj_id || ' and "Account" <> "ShortName"';
			EXECUTE IMMEDIATE exe_cmd;
		end if;                                                                          
	else
		UPDATE "JDT1" SET "BaseRef"=:docnum, "Ref1"=:docnum WHERE "CreatedBy"=:dockey AND "TransType"=:obj_id;
	end if;
end if; 

if :obj_id = 24 or :obj_id = 46 then
	UPDATE "JDT1" SET "BaseRef"=:docnum WHERE "CreatedBy"=:dockey AND "TransType"=:obj_id;
	UPDATE "JDT1" SET "Ref1"=:docnum WHERE "CreatedBy"=:dockey AND "TransType"=:obj_id and "Ref1"=TO_NVARCHAR(:tempnum);
	UPDATE "JDT1" SET "Ref3Line"=:docnum WHERE "CreatedBy"=:dockey AND "TransType"=:obj_id and "Ref3Line"=TO_NVARCHAR(:tempnum);
	UPDATE "JDT1" SET "DocNum"=:docnum WHERE "CreatedBy"=:dockey AND "TransType"=:obj_id and "DocArr"=15;
end if;

-- Select the return values
select :error, :error_message FROM DUMMY;

end;


