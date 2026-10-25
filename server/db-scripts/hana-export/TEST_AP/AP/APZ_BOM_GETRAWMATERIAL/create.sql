
Create  Procedure apz_bom_getRawMaterial
(in ItemCode NVARCHAR(20))
As
Begin
create local temporary table #tbl (
"Type" nvarchar(10) NULL,
 "ItemCode" nvarchar(20) NOT NULL,
  "Dscription" nvarchar(100) NULL, 
  "Qty" decimal(19, 6) NOT NULL, 
  "UoM" nvarchar(30) NULL,
   "WhsCode" nvarchar(8) NOT NULL,
    "IssueMethod" nvarchar(20) NULL,
     "PriceList" nvarchar(20) NULL);
 INSERT INTO #tbl VALUES(N'Item','CFBB',N'Blue (144.6kg/cuộn)',1,'kg','PRD','Manual','PriceList 01');

INSERT INTO #tbl VALUES(N'Item','CFBG',N'Gren (73.5kg/cuộn)',1,'kg','PRD','Manual','PriceList 01');

SELECT * FROM #tbl;
End;




