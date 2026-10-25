
Create procedure APZ_Vehicle_DeleteDocs
(in ids nvarchar(5000) default '')
As
Begin
delete from APZ_VHCE where "Code" in(SELECT Data FROM Split(:ids,','));
End;






