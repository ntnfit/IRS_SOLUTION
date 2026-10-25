CREATE procedure ILD_DIY_SINHSOBATCH_OUTSOURCING
(
	 in U_Type int,
	 in Batch nvarchar(20),
	 in SSCC  nvarchar(20)
)
as
begin
 
	--declare U_Type int   = 1;-- 1: up, 2:down
	--declare Batch nvarchar(20) = '42240023RWF1 ';
	--declare SSCC  nvarchar(20) ='61240002300173';
	declare Rs nvarchar (20);
-- 2:down
	IF (U_Type  = 2) then
	 select  replace  (Batch,right(Batch,2), 'RW'||right(Batch,2)) into Rs default '' from dummy ;
	END IF;
	
-- 1: up
	IF (U_Type  = 1) then
	 If length(Batch) =  8 then   -- Số Batch Rework quá khứ 
		select left(Batch,4) || right(left(SSCC,9),4) ||right (Batch,2) into Rs default ''
		from dummy;
	  Else  --Số Batch Rework Moi
	  	select  replace  (Batch, 'RW','') into Rs default '' from dummy ;
	 End If;
	  	
	END IF;
	
----
	select rs as "BatchRS" from dummy;

end
