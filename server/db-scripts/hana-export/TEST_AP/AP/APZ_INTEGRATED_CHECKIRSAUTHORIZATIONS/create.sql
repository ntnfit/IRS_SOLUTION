
CREATE PROCEDURE APZ_Integrated_CheckIRSAuthorizations
as
BEGIN
	declare error nvarchar(254)='';
	declare checkCount int = 0;
	select IFNULL((Select top 1 1 From OPRM Where "menucode"='frm_Integrate_Document'),0) into checkCount from DUMMY;
	
	if :checkCount = 0
	then
		Insert into OPRM (USER_CODE,"menucode","authoriza") Values ('manager','frm_Integrate_Document','F');
	END if;
	
	checkCount = 0;
	select IFNULL((Select top 1 1 From OPRM Where "menucode"='frm_Integrate_BusinessPartner'),0) into checkCount from DUMMY;
	if checkCOunt = 0
	then
		Insert into OPRM (USER_CODE,"menucode","authoriza") Values ('manager','frm_Integrate_BusinessPartner','F');
	end if;
		
	select :error from DUMMY;
END;




