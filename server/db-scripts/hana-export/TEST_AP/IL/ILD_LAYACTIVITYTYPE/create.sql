CREATE procedure ILD_LayActivityType
as
begin
		
Select "Code","Name","U_Descriptions","U_Notes" from "@MAINTENANCE_AC_TYPE" order by "Code";
		 
end;

