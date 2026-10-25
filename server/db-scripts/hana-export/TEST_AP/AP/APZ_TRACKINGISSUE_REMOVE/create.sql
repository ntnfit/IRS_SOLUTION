
Create procedure APZ_TrackingIssue_Remove
(
in Id int default 0
)
as
Begin
 declare atcEntry int=0;
--select "AttachedFiled" into atcEntry from APZ_TGIE where "Id"=:Id;
--Delete from ATC1 where "AbsEntry"=:atcEntry;
--Delete from OATC where "AbsEntry"=:atcEntry;
--Delete from APZ_TGIE1 where "Id"=:Id;
--Delete from APZ_TGIE where "Id"=:Id;
Update APZ_TGIE set "IssueDeleted"='Y' where  "Id"=:Id;
End;




