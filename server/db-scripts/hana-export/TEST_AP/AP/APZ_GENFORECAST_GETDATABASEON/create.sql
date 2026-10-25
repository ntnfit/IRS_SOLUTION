
CREATE PROCEDURE APZ_GenForecast_GetDataBaseOn
AS
BEGIN

	DECLARE tbl table
	(
	  "Code" nvarchar(4), 
	  "Name" Nvarchar(100)
	);
	
	INSERT INTO :tbl values ('17', 'Sales Order');
	INSERT INTO :tbl values ('15','Delivery');
	INSERT INTO :tbl values ('13','AR Invoice');
	INSERT INTO :tbl values ('22','Purchase Order');
	INSERT INTO :tbl values ('20','Goods Receipt PO');
	INSERT INTO :tbl values ('18','AP Invoice');
	INSERT INTO :tbl values ('60','Issue for Production');
	INSERT INTO :tbl values ('59','Receipt from Production');
	INSERT INTO :tbl values ('2021','Receipt Plan from Production');
	INSERT INTO :tbl values ('2022','Issue Plan for from Production');
	
	Select * from :tbl;
	
END;




