
create procedure APZ_PackingManagement_InsertItem(in CaseCode nvarchar(50), in BundleCode nvarchar(50), in LineNumber int)
as
begin

	INSERT into APZ_DMB ("LineNum", "CaseCode", "BundleCode","LineNumber")
	values
	(
		ifnull((select max("LineNum") from APZ_DMB), 0) + 1,
		:CaseCode,
		:BundleCode,
		:LineNumber
	);

	UPDATE APZ_DMC
	SET "Amount" = (SELECT COUNT("CaseCode") FROM APZ_DMB where "CaseCode" = :CaseCode )
	WHERE "CaseCode" = :CaseCode;
end;




