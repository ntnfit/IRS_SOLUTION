
Create procedure APZ_VehicleType_DeleteDocs
(in ids nvarchar(5000) default '')
As
Begin
delete from APZ_VHCT where "Code" in(SELECT Data FROM Split(:ids,','));
End;




