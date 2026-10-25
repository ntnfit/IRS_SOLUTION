
CREATE PROCEDURE  APZ_BP_ExistLinkedContact(
cntctCode INT default 0)
AS
BEGIN
DECLARE returnMsg NVARCHAR(100)='';

SELECT IFNULL((SELECT 'A/P Correction Invoice' FROM OCPI WHERE "CntctCode"=:cntctCode),'') into returnMsg from DUMMY;
IF(:returnMsg <> '')then
begin
SELECT returnMsg from DUMMY;
RETURN;
end;
end if;

SELECT IFNULL((SELECT 'A/P Correction Invoice Reversal' FROM OCPV WHERE "CntctCode"=:cntctCode),'') into returnMsg from DUMMY;
IF(:returnMsg <> '')then
begin
SELECT returnMsg from DUMMY;
RETURN;
end;
end if;


SELECT IFNULL((SELECT 'A/P Credit Memo' FROM ORPC WHERE "CntctCode"=:cntctCode),'') into returnMsg from DUMMY;
IF(:returnMsg <> '')then
begin
SELECT returnMsg from DUMMY;
RETURN;
end;
end if;

SELECT IFNULL((SELECT 'A/P Down Payment' FROM ODPO WHERE "CntctCode"=:cntctCode),'') into returnMsg from DUMMY;
IF(:returnMsg <> '')then
begin
SELECT returnMsg from DUMMY;
RETURN;
end;
end if;


SELECT IFNULL((SELECT 'A/P Invoice' FROM OPCH WHERE "CntctCode"=:cntctCode),'') into returnMsg from DUMMY;
IF(:returnMsg <> '')then
begin
SELECT returnMsg from DUMMY;
RETURN;
end;
end if;

SELECT IFNULL((SELECT 'A/R Correction Invoice' FROM OCIN WHERE "CntctCode"=:cntctCode),'') into returnMsg from DUMMY;
IF(:returnMsg <> '')then
begin
SELECT returnMsg from DUMMY;
RETURN;
end;
end if;

SELECT IFNULL((SELECT 'A/R Correction Invoice' FROM OCSI WHERE "CntctCode"=:cntctCode),'') into returnMsg from DUMMY;
IF(:returnMsg <> '')then
begin
SELECT returnMsg from DUMMY;
RETURN;
end;
end if;

SELECT IFNULL((SELECT 'A/R Correction Invoice Reversal' FROM OCSV WHERE "CntctCode"=:cntctCode),'') into returnMsg from DUMMY;
IF(:returnMsg <> '')then
begin
SELECT returnMsg from DUMMY;
RETURN;
end;
end if;

SELECT IFNULL((SELECT 'A/R Credit Memo' FROM ORIN WHERE "CntctCode"=:cntctCode),'') into returnMsg from DUMMY;
IF(:returnMsg <> '')then
begin
SELECT returnMsg from DUMMY;
RETURN;
end;
end if;

SELECT IFNULL((SELECT 'A/R Down Payment' FROM ODPI WHERE "CntctCode"=:cntctCode),'') into returnMsg from DUMMY;
IF(:returnMsg <> '')then
begin
SELECT returnMsg from DUMMY;
RETURN;
end;
end if;

SELECT IFNULL((SELECT 'A/R Invoice' FROM OINV WHERE "CntctCode"=:cntctCode),'') into returnMsg from DUMMY;
IF(:returnMsg <> '')then
begin
SELECT returnMsg from DUMMY;
RETURN;
end;
end if;

SELECT IFNULL((SELECT 'Activities' FROM OCLG WHERE "CntctCode"=:cntctCode),'') into returnMsg from DUMMY;
IF(:returnMsg <> '')then
begin
SELECT returnMsg from DUMMY;
RETURN;
end;
end if;

SELECT IFNULL((SELECT 'Blanket Agreement' FROM OOAT WHERE "CntctCode"=:cntctCode),'') into returnMsg from DUMMY;
IF(:returnMsg <> '')then
begin
SELECT returnMsg from DUMMY;
RETURN;
end;
end if;

SELECT IFNULL((SELECT 'Blanket Agreement' FROM AOAT WHERE "CntctCode"=:cntctCode),'') into returnMsg from DUMMY;
IF(:returnMsg <> '')then
begin
SELECT returnMsg from DUMMY;
RETURN;
end;
end if;

SELECT IFNULL((SELECT 'Customer Equipment Card' FROM OINS WHERE "contactCod"=:cntctCode),'') into returnMsg from DUMMY;
IF(:returnMsg <> '')then
begin
SELECT returnMsg from DUMMY;
RETURN;
end;
end if;

SELECT IFNULL((SELECT 'Customer Equipment Card - History' FROM AINS WHERE "contactCod"=:cntctCode),'') into returnMsg from DUMMY;
IF(:returnMsg <> '')then
begin
SELECT returnMsg from DUMMY;
RETURN;
end;
end if;

SELECT IFNULL((SELECT 'Delivery' FROM ODLN WHERE "CntctCode"=:cntctCode),'') into returnMsg from DUMMY;
IF(:returnMsg <> '')then
begin
SELECT returnMsg from DUMMY;
RETURN;
end;
end if;

SELECT IFNULL((SELECT 'Drafts' FROM ODRF WHERE "CntctCode"=:cntctCode),'') into returnMsg from DUMMY;
IF(:returnMsg <> '')then
begin
SELECT returnMsg from DUMMY;
RETURN;
end;
end if;

SELECT IFNULL((SELECT 'Goods Issue' FROM OIGE WHERE "CntctCode"=:cntctCode),'') into returnMsg from DUMMY;
IF(:returnMsg <> '')then
begin
SELECT returnMsg from DUMMY;
RETURN;
end;
end if;

SELECT IFNULL((SELECT 'Goods Receipt' FROM OIGN WHERE "CntctCode"=:cntctCode),'') into returnMsg from DUMMY;
IF(:returnMsg <> '')then
begin
SELECT returnMsg from DUMMY;
RETURN;
end;
end if;

SELECT IFNULL((SELECT 'Goods Receipt PO' FROM OPDN WHERE "CntctCode"=:cntctCode),'') into returnMsg from DUMMY;
IF(:returnMsg <> '')then
begin
SELECT returnMsg from DUMMY;
RETURN;
end;
end if;

SELECT IFNULL((SELECT 'Goods Return' FROM ORPD WHERE "CntctCode"=:cntctCode),'') into returnMsg from DUMMY;
IF(:returnMsg <> '')then
begin
SELECT returnMsg from DUMMY;
RETURN;
end;
end if;

SELECT IFNULL((SELECT 'History' FROM ASCL WHERE "contctCode"=:cntctCode),'') into returnMsg from DUMMY;
IF(:returnMsg <> '')then
begin
SELECT returnMsg from DUMMY;
RETURN;
end;
end if;

SELECT IFNULL((SELECT 'Incoming Excise Invoice' FROM OIEI WHERE "CntctCode"=:cntctCode),'') into returnMsg from DUMMY;
IF(:returnMsg <> '')then
begin
SELECT returnMsg from DUMMY;
RETURN;
end;
end if;

SELECT IFNULL((SELECT 'Incoming Payment' FROM ORCT WHERE "CntctCode"=:cntctCode),'') into returnMsg from DUMMY;
IF(:returnMsg <> '')then
begin
SELECT returnMsg from DUMMY;
RETURN;
end;
end if;

SELECT IFNULL((SELECT 'Incoming Payment - History' FROM ARCT WHERE "CntctCode"=:cntctCode),'') into returnMsg from DUMMY;
IF(:returnMsg <> '')then
begin
SELECT returnMsg from DUMMY;
RETURN;
end;
end if;

SELECT IFNULL((SELECT 'Inventory Transfer' FROM OWTR WHERE "CntctCode"=:cntctCode),'') into returnMsg from DUMMY;
IF(:returnMsg <> '')then
begin
SELECT returnMsg from DUMMY;
RETURN;
end;
end if;

SELECT IFNULL((SELECT 'Inventory Transfer Request' FROM OWTQ WHERE "CntctCode"=:cntctCode),'') into returnMsg from DUMMY;
IF(:returnMsg <> '')then
begin
SELECT returnMsg from DUMMY;
RETURN;
end;
end if;

SELECT IFNULL((SELECT 'Invoice - History' FROM ADOC WHERE "CntctCode"=:cntctCode),'') into returnMsg from DUMMY;
IF(:returnMsg <> '')then
begin
SELECT returnMsg from DUMMY;
RETURN;
end;
end if;

SELECT IFNULL((SELECT 'Outgoing Excise Invoice' FROM OOEI WHERE "CntctCode"=:cntctCode),'') into returnMsg from DUMMY;
IF(:returnMsg <> '')then
begin
SELECT returnMsg from DUMMY;
RETURN;
end;
end if;

SELECT IFNULL((SELECT 'Outgoing Payments' FROM OVPM WHERE "CntctCode"=:cntctCode),'') into returnMsg from DUMMY;
IF(:returnMsg <> '')then
begin
SELECT returnMsg from DUMMY;
RETURN;
end;
end if;

SELECT IFNULL((SELECT 'Payment Draft' FROM OPDF WHERE "CntctCode"=:cntctCode),'') into returnMsg from DUMMY;
IF(:returnMsg <> '')then
begin
SELECT returnMsg from DUMMY;
RETURN;
end;
end if;

SELECT IFNULL((SELECT 'Production Instructions' FROM OWKO WHERE "CntctCode"=:cntctCode),'') into returnMsg from DUMMY;
IF(:returnMsg <> '')then
begin
SELECT returnMsg from DUMMY;
RETURN;
end;
end if;

SELECT IFNULL((SELECT 'Purchase Order' FROM OPOR WHERE "CntctCode"=:cntctCode),'') into returnMsg from DUMMY;
IF(:returnMsg <> '')then
begin
SELECT returnMsg from DUMMY;
RETURN;
end;
end if;

SELECT IFNULL((SELECT 'Purchase Quotation' FROM OPQT WHERE "CntctCode"=:cntctCode),'') into returnMsg from DUMMY;
IF(:returnMsg <> '')then
begin
SELECT returnMsg from DUMMY;
RETURN;
end;
end if;

SELECT IFNULL((SELECT 'Purchase Request' FROM OPRQ WHERE "CntctCode"=:cntctCode),'') into returnMsg from DUMMY;
IF(:returnMsg <> '')then
begin
SELECT returnMsg from DUMMY;
RETURN;
end;
end if;

SELECT IFNULL((SELECT 'Purchase Tax Invoice' FROM OTPI WHERE "CntctCode"=:cntctCode),'') into returnMsg from DUMMY;
IF(:returnMsg <> '')then
begin
SELECT returnMsg from DUMMY;
RETURN;
end;
end if;

SELECT IFNULL((SELECT 'Returns' FROM ORDN WHERE "CntctCode"=:cntctCode),'') into returnMsg from DUMMY;
IF(:returnMsg <> '')then
begin
SELECT returnMsg from DUMMY;
RETURN;
end;
end if;

SELECT IFNULL((SELECT 'Sales Opportunity' FROM ORDN WHERE "SlpCode"=:cntctCode),'') into returnMsg from DUMMY;
IF(:returnMsg <> '')then
begin
SELECT returnMsg from DUMMY;
RETURN;
end;
end if;

SELECT IFNULL((SELECT 'Sales Opportunity' FROM AOPR WHERE "SlpCode"=:cntctCode),'') into returnMsg from DUMMY;
IF(:returnMsg <> '')then
begin
SELECT returnMsg from DUMMY;
RETURN;
end;
end if;


SELECT IFNULL((SELECT 'Sales Opportunity - Rows' FROM OPR1 WHERE "CntctCode"=:cntctCode),'') into returnMsg from DUMMY;
IF(:returnMsg <> '')then
begin
SELECT returnMsg from DUMMY;
RETURN;
end;
end if;

SELECT IFNULL((SELECT 'Sales Order' FROM ORDR WHERE "CntctCode"=:cntctCode),'') into returnMsg from DUMMY;
IF(:returnMsg <> '')then
begin
SELECT returnMsg from DUMMY;
RETURN;
end;
end if;

SELECT IFNULL((SELECT 'Sales Quotation' FROM OQUT WHERE "CntctCode"=:cntctCode),'') into returnMsg from DUMMY;
IF(:returnMsg <> '')then
begin
SELECT returnMsg from DUMMY;
RETURN;
end;
end if;

SELECT IFNULL((SELECT 'Sales Tax Invoice' FROM OTSI WHERE "CntctCode"=:cntctCode),'') into returnMsg from DUMMY;
IF(:returnMsg <> '')then
begin
SELECT returnMsg from DUMMY;
RETURN;
end;
end if;

SELECT IFNULL((SELECT 'Service Calls' FROM OSCL WHERE "contctCode"=:cntctCode),'') into returnMsg from DUMMY;
IF(:returnMsg <> '')then
begin
SELECT returnMsg from DUMMY;
RETURN;
end;
end if;

SELECT IFNULL((SELECT 'Service Contracts' FROM OCTR WHERE "CntctCode"=:cntctCode),'') into returnMsg from DUMMY;
IF(:returnMsg <> '')then
begin
SELECT returnMsg from DUMMY;
RETURN;
end;
end if;

SELECT IFNULL((SELECT 'Target Group Details' FROM TGG1 WHERE "CntctPrsn"=:cntctCode),'') into returnMsg from DUMMY;
IF(:returnMsg <> '')then
begin
SELECT returnMsg from DUMMY;
RETURN;
end;
end if;

SELECT IFNULL((SELECT 'Tax Invoice - History' FROM ATXI WHERE "CntctCode"=:cntctCode),'') into returnMsg from DUMMY;
IF(:returnMsg <> '')then
begin
SELECT returnMsg from DUMMY;
RETURN;
end;
end if;

SELECT IFNULL((SELECT 'Tax Invoice Draft' FROM OTXD WHERE "CntctCode"=:cntctCode),'') into returnMsg from DUMMY;
IF(:returnMsg <> '')then
begin
SELECT returnMsg from DUMMY;
RETURN;
end;
end if;
END




