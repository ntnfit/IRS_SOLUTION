
Create procedure apz_JobVacancy_GetDetail
(
in docId int
)
as
Begin
select *from APZ_HJV7 where "DocEntry"=:docId;
select * from APZ_HJV1 where "DocEntry"=:docId;
select * from APZ_HJV2 where "DocEntry"=:docId;
select * from APZ_HJV3 where "DocEntry"=:docId;
select * from APZ_HJV4 where "DocEntry"=:docId;
select * from APZ_HJV5 where "DocEntry"=:docId;
select * from APZ_HJV6 where "DocEntry"=:docId;
select a.*, b."DoB" as "DoB",b."Address" as "Address",b."Email" as "Email", b."Gender" as "Gender",b."Qualifications" as "Qualifications",b."Phone" as "Phone"
from APZ_HJV8 a left join APZ_HJV3 b on a."ApplicantNo"=b."ApplicantNo" AND a."DocEntry"= B."DocEntry" where a."DocEntry"=:docId;
End;




