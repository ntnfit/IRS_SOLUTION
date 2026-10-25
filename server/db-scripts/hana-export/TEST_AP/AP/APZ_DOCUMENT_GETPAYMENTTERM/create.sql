
CREATE  PROCEDURE Apz_Document_GetPaymentTerm(
	in tableName nvarchar(20),
	in DocEntry INT,
	in ObjType INT
)
AS
BEGIN
	DECLARE PaymentEntry INT;
	IF :ObjType=13
	then
		select (SELECT TOP 1 "U_paymentEntry" FROM OINV WHERE "DocEntry"=:DocEntry) into PaymentEntry from DUMMY;
	ELSEIF :ObjType=14
	THEN
		select (SELECT TOP 1 "U_paymentEntry" FROM ORIN WHERE "DocEntry"=:DocEntry) into PaymentEntry FROM DUMMY;
	ELSEIF :ObjType=15
	THEN
		Select (SELECT TOP 1 "U_paymentEntry" FROM ODLN WHERE "DocEntry"=:DocEntry) into PaymentEntry from DUMMY;
	ELSEIF :ObjType=16
	THEN
		select (SELECT TOP 1 "U_paymentEntry" FROM ORDN WHERE "DocEntry"=:DocEntry) into PaymentEntry FROM DUMMY;
	ELSEIF :ObjType=17
	THEN
		select (SELECT TOP 1 "U_paymentEntry" FROM ORDR WHERE "DocEntry"=:DocEntry) into PaymentEntry FROM DUMMY;
	ELSEIF :ObjType=23
	THEN
		select (SELECT TOP 1 "U_paymentEntry" FROM OQUT WHERE "DocEntry"=:DocEntry) into PaymentEntry FROM DUMMY;
	ELSEIF :ObjType=203
	THEN
		select (SELECT TOP 1 "U_paymentEntry" FROM ODPI WHERE "DocEntry"=:DocEntry) into PaymentEntry FROM DUMMY;
	ELSEIF :ObjType=18
	THEN
		select (SELECT TOP 1 "U_paymentEntry" FROM OPCH WHERE "DocEntry"=:DocEntry) into PaymentEntry FROM DUMMY;
	ELSEIF :ObjType=19
	THEN
		select (SELECT TOP 1 "U_paymentEntry" FROM ORPC WHERE "DocEntry"=:DocEntry) into PaymentEntry FROM DUMMY;
	ELSEIF :ObjType=20
	THEN
		select (SELECT TOP 1 "U_paymentEntry" FROM OPDN WHERE "DocEntry"=:DocEntry) into PaymentEntry FROM DUMMY;
	ELSEIF :ObjType=21
	THEN
		select (SELECT TOP 1 "U_paymentEntry" FROM ORPD WHERE "DocEntry"=:DocEntry) into PaymentEntry FROM DUMMY;
	ELSEIF :ObjType=22
	THEN
		select (SELECT TOP 1 "U_paymentEntry" FROM OPOR WHERE "DocEntry"=:DocEntry) into PaymentEntry FROM DUMMY;
	ELSEIF :ObjType=540000006
	THEN
		select (SELECT TOP 1 "U_paymentEntry" FROM OPQT WHERE "DocEntry"=:DocEntry) into PaymentEntry FROM DUMMY;
	ELSEIF :ObjType=112
	THEN
		select (SELECT TOP 1 "U_paymentEntry" FROM ODRF WHERE "DocEntry"=:DocEntry) into PaymentEntry FROM DUMMY;
	END IF;
	--PRINT @PaymentEntry
	SELECT * FROM "@PMT1" WHERE "DocEntry" = :PaymentEntry;

END;




