CREATE procedure ILD_Lay_BatchQA
as
begin

		select distinct "MnfSerial" as "Batch", TO_VARCHAR("InDate",'DD/MM/YYYY') as "Admission Date",
			   TO_VARCHAR("MnfDate",  'DD/MM/YYYY') as "Manufacturing Date"
		from OBTN
		where  "MnfSerial"  <> ''
		order by "MnfSerial";

end;

