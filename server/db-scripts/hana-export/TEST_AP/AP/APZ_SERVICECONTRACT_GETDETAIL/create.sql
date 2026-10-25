
CREATE  procedure APZ_ServiceContract_GetDetail(in ContractID int)
as
begin
	--Load data ganeral
	SELECT "ContractID", "CntrcType", "CntrcTmplt", "ResponseV", "ResponseU", "ResponsVal", "ResponsUnt", "Status", "Owner", "Renewal", "RemindVal", "RemindUnit", "Remarks1", "Remarks2", "SrvcType" FROM OCTR WHERE "ContractID" = :ContractID;
	--Load data converage
	SELECT "MonEnabled", "MonStart", "MonEnd", "TueEnabled", "TueStart", "TueEnd", "WedEnabled", "WedStart", "WedEnd", "ThuEnabled", "ThuStart", "ThuEnd", "FriStart", "FriEnabled", "FriEnd", "SatEnabled", "SatStart", "SatEnd", "SunEnabled", "SunStrart", "SunEnd", "InclParts", "InclWork", "InclTravel", "InclHldays" FROM OCTR WHERE "ContractID" = :ContractID;
	--Load file 
	SELECT "AbsEntry", "Line", "srcPath", "trgtPath", "FileName", "FileExt", "Date" FROM ATC1 WHERE "AbsEntry" = (SELECT "AtcEntry" FROM octr WHERE "ContractID" = :ContractID);
	--Load DataItems 
	select * from CTR1 where "ContractID"=:ContractID;
	--Load Data Service call	
	SELECT "callID", "createDate", "subject", "itemCode", "manufSN", "custmrName", B."Name" AS "status" FROM OSCL A LEFT OUTER JOIN OSCS B ON A."status" = B."statusID" WHERE "contractID" = :ContractID;
end



