CREATE procedure ILD_F_inTem
(
	DocID int,
	ObjectType nvarchar(10)

)
as
Begin
		-- ObjectType = 20 --> Goods Receipt PO;  ObjectType = 59 -->  Goods Receipt: Receipt from production
		Select :DocID as "DOCENTRY",a."ItemCode",a."ItemName", "MnfSerial" as "Batch", "BatchNum" as  "SSCC" , a."Quantity", 
		--"PrdDate" as "ManufacturingDate",
		  c."MnfDate" as "ManufacturingDate",
		c."InDate" as  "AdmissionDate",
		--"ExpDate" as "ExpirationDate",
		c."ExpDate" as "ExpirationDate",
		b."InvntryUom" as "Unit",
		'' as "NetContent",
		1 as "Qty_Decimal",
		case when :ObjectType = '20' then 'Receiving date' else 'Manufacturing date' end as "Re_Manuf_date_title"
		
				
		From IBT1 a
		inner join OBTN c  on a."BatchNum" =c."DistNumber"
		left join OITM b on a."ItemCode" = b."ItemCode"
		where "BaseType"  = :ObjectType and  "BaseEntry"  = :DocID;
End;

