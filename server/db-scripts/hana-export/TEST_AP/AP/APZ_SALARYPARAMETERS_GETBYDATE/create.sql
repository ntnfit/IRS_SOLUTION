
Create procedure APZ_SalaryParameters_GetByDate
(
 in fromDate timestamp default '',
 in toDate timestamp default ''
)
As
Begin
Select * from APZ_OHSP where 
							(("ValidFrom" BETWEEN IFNULL(:fromDate,'1900-01-01') AND IFNULL(:toDate,'1900-01-01')) OR
							("ValidTo" BETWEEN IFNULL(:fromDate,'1900-01-01') AND IFNULL(:toDate,'1900-01-01')));
End;




