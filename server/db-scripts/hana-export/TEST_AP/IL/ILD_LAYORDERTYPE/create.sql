CREATE procedure ILD_LayOrderType
as
begin
		
Select "Code","Name","U_Descriptions","U_Notes" 
from "@MAINTENANCE_AC_TYPE"
where left("Code",4) = 'PM03'
 order by "Code";
		 
 

end;

