
create procedure APZ_IVCounting_GetLineById
(
in id bigint
)
As
Begin
	Select c.*, case when d."ManBtchNum"='Y' then 'B' when d."ManSerNum"='Y' then 'S' else 'N' end  as "TranType"
	From APZ_INC1 c Left join OITM d on d."ItemCode"=c."ItemCode"  
	Where c."DocEntry"=:Id;

	Select a.*,case when b."ManBtchNum"='Y' then 'Y' when b."ManSerNum"='Y' then 'N' end as "IsBatch"
 	From APZ_INC2 a Left join OITM b on b."ItemCode"=a."ItemCode"
 	Where a."DocEntry"=:Id;
End;




