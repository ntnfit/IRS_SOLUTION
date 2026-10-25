
Create procedure APZ_Jobvacancy_GetMaxDocEntry

as
Begin
SELECT MAX("DocNum") FROM APZ_OHJV;
End;




