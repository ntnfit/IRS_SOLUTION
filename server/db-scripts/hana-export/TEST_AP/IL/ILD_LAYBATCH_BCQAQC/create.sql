CREATE procedure ILD_LayBatch_BCQAQC
as
begin
	select distinct "MnfSerial" as "Batch", "LotNumber" as "Status",
		"MnfDate", "ExpDate"
		from  OBTN
		where left("MnfSerial",2) <> 'GC' ;
end;

