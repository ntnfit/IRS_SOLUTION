
create procedure APZ_ActivitiesMeetingLocation_GetUserSignByUserCodeOUSR(in UserCode nvarchar (255))
As
Begin
SELECT DISTINCT A."UserSign" FROM OCLO A LEFT OUTER JOIN OUSR B ON A."UserSign" = B.USERID WHERE B.USER_CODE = :UserCode;

End;





