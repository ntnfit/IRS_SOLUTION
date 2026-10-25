
Do
Begin
	Declare MaxId Int;
	If Exists(Select 1 From APZ_OHWP Where ID Is Null)
	Then
		Select (Select IFNULL(Max(ID),0) From APZ_OHWP) Into MaxId From Dummy;
		CREATE local temporary table #tblNewID AS (Select *,ROW_NUMBER() OVER () AS "Row_num" From APZ_OHWP Where IFnull("ID",0) =0);
		
		Update a Set ID = "Row_num" + MaxId
		From APZ_OHWP a Join #tblNewID b On a."PeriodCode" = b."PeriodCode"
		Where IFnull(a."ID",0) =0;
		
		Drop Table #tblNewID;
	End If;
End