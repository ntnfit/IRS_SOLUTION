CREATE procedure USP_CFL_KD14_01 (
)
as
begin

	select 'AR' Code, 'AR' Name from dummy
	union all
	select 'ARCRE', 'AR Credit Memo' from dummy;
	
end

