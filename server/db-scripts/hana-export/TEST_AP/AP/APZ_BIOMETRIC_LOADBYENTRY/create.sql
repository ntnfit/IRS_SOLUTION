
CREATE PROCEDURE APZ_Biometric_LoadByEntry(IN AbsEntry int)
AS
BEGIN
	Select a.*,b."EnrollName" 
	From APZ_TBD1 a Join APZ_OTEM b On a."EnrollNo" = b."EnrollNo" And a."MachineNo" = b."MachineNo" 
	Where a."AbsEntry" = :AbsEntry
	Order By a."EnrollNo",a."LineNum";
END




