CREATE FUNCTION ILD_FT_SubStringNominalInput_HieuChuan (
	ItemCode	nvarchar(50),
	NominalInput	nvarchar(100)
)
RETURNS TABLE (
		ID int,
		NominalInput nvarchar(20),
		Unit nvarchar(20),
		ItemCode nvarchar(100)
)
AS
BEGIN

	declare rstbl table 
 	(
		ID int,
		NominalInput nvarchar(20),
		Unit nvarchar(20),
		ItemCode nvarchar(100)
 	);
 
 	declare tbl1 table 
 	(
 		id int,
		part	nvarchar(20)
 	);
 	Insert into :tbl1 
 	Select  "id", replace(replace("part", SUBSTR_AFTER("part",'_'),''),'_','')
 	From SplitString (:NominalInput,';') ;
 
 
  	Insert into :rstbl 
 	Select  a.*,SUBSTR_AFTER (NominalInput,'_') as "Unit", :ItemCode as "ItemCode"
 	From :tbl1 a ;
  
  	return :rstbl;
  	

END;

