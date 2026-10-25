
create procedure APZ_ProjectManager_UpdateStatus 
(
in status char(1),
in docEntry int
)
as
begin
UPDATE APZ_OPRG set "PrgStatus" = :status where "AbsEntry"=:docEntry;
select :docEntry,'' from DUMMY;
end;




