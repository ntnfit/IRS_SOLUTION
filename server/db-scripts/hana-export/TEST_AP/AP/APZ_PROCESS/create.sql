
CREATE  PROCEDURE apz_process(
in objtype NVARCHAR(50) default '',
in transtype NCHAR(1) default '',
in num_of_key INT default 0 ,
in key_columns NVARCHAR(254)default '',
in obj_id NVARCHAR(50000) default ''
)

AS
BEGIN
	
	DECLARE error NVARCHAR(4000) ='';
	DECLARE message NVARCHAR(4000)='';
	DECLARE tbldoc NVARCHAR(100)='';
	DECLARE tblLine NVARCHAR(100)='';
	declare listColumns nvarchar(4000) = '';
	declare sqlstr VARCHAR(4000) = '';
	declare mapCols VARCHAR(4000)='';
	declare mapValue VARCHAR(4000)='';

	
END




