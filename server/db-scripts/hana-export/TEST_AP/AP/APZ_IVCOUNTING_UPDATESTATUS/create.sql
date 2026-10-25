
create procedure APZ_IVCounting_UpdateStatus
(
in status char(1) default '',
in docEntry int
)
as
begin
UPDATE APZ_OINC set "DocStatus" = :status where "DocEntry"=:docEntry;
select :docEntry,'' from dummy;
end;




