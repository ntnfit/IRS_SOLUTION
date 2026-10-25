
create procedure APZ_Javacancy_DeleteDocs
(
in ids nvarchar(5000) default ''
)
as
begin
delete from APZ_HJV1 where "DocEntry" =:ids;
delete from APZ_HJV2 where "DocEntry" =:ids;
delete from APZ_HJV3 where "DocEntry" =:ids;
delete from APZ_HJV4 where "DocEntry" =:ids;
delete from APZ_HJV5 where "DocEntry" =:ids;
delete from APZ_HJV6 where "DocEntry" =:ids;
delete from APZ_HJV7 where "DocEntry" =:ids;
delete from APZ_HJV8 where "DocEntry" =:ids;
delete from APZ_OHJV where "DocEntry" =:ids;
end;




