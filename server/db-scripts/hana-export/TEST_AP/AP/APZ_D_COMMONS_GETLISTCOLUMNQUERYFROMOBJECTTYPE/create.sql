CREATE PROCEDURE APZ_D_Commons_GetListColumnQueryFromObjectType(
	in tableData nvarchar(50)
)
AS
BEGIN
	if(:tableData = 'OCRD')
	then
		select '"CardCode" as "Mã KH","CardName" as "Tên KH","CntctPrsn" as "Liên hệ","Cellular" as "SDT","Address" as "Địa chỉ","CardCode","CardName"' as "Result" from dummy;
	else
		select '' from dummy;
	end if;
END



