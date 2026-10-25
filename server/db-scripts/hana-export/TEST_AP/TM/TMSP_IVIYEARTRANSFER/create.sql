-- B1 DEPENDS: AFTER:PT:PROCESS_END

Create procedure TmSp_IVIYearTransfer(out retCode int)

LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER
AS


	isexist1 int;
	isexist2 int;
	isexist3 int;
	isexist4 int;
	isexist5 int;
	isexist6 int;
	isexist7 int;
	isexist8 int;
	isexist9 int;
	isexist10 int;
	isexist11 int;
	isexist12 int;
BEGIN

	

	
	select count("DocEntry") into isexist1 from  "ODLN" where "Instance" >0; 
    select count("DocEntry") into isexist2 from  "ORDN" where "Instance" >0;  
    select count("DocEntry") into isexist3 from  "OINV" where "Instance" >0;  
    select count("DocEntry") into isexist4 from  "ORIN" where "Instance" >0;  
    select count("DocEntry") into isexist5 from  "OCSI" where "Instance" >0;  
    select count("DocEntry") into isexist6 from  "OCSV" where "Instance" >0; 
    select count("DocEntry") into isexist7 from  "OPDN" where "Instance" >0;  
    select count("DocEntry") into isexist8 from  "ORPD" where "Instance" >0;  
    select count("DocEntry") into isexist9 from  "OPCH" where "Instance" >0;  
    select count("DocEntry") into isexist10 from  "ORPC" where "Instance" >0;  
    select count("DocEntry") into isexist11 from  "OCPI" where "Instance" >0;  
    select count("DocEntry") into isexist12 from  "OCPV" where "Instance" >0; 
	
	isexist1 := :isexist1 + :isexist2 + :isexist3 + :isexist4 + :isexist5 + :isexist6 + :isexist7 + :isexist8 + :isexist9 + :isexist10 + :isexist11 + :isexist12;	
	
	
	
--return @Count;
retCode := :isexist1;
END;


