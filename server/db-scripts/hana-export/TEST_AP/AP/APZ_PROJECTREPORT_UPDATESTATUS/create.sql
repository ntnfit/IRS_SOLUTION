
create procedure APZ_ProjectReport_UpdateStatus 
(
in status char(1),
in docEntry int
)
as
begin
UPDATE APZ_ORTC set "Status" = :status where "DocEntry"=:docEntry;
select :docEntry,'' from DUMMY;
end;




