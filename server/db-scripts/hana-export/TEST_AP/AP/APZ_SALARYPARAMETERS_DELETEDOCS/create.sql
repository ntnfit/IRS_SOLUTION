
Create procedure APZ_SalaryParameters_DeleteDocs(in ids nvarchar(5000) default '')
As
Begin
Delete from APZ_OHSP where "Code" in(SELECT Data FROM Split(:ids,','));
End;




