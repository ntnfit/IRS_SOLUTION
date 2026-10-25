
create procedure APZ_LandedCosts_DeleteDocs
(
in ids nvarchar(5000) default ''
)
as
begin
delete from OIPF where "DocEntry" in(SELECT Data FROM Split(:ids,','));
delete from IPF1 where "DocEntry" in(SELECT Data FROM Split(:ids,','));
delete from IPF2 where "DocEntry" in(SELECT Data FROM Split(:ids,','));
delete from ATC1 where "AbsEntry" in(SELECT Data FROM Split(:ids,','));
end




