-- B1 DEPENDS: AFTER:PT:CREATE_TABLES_TO_VIEWS

CREATE PROCEDURE _TmSp_RestoreIsCommited_CursorInOIBT(IN ItemCode nvarchar (50),
													  IN BatchNum nvarchar (32),
													  IN WhsCode  nvarchar (8),
													  OUT IsCommited DECIMAL(21,6))
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER

AS
-------------------------------------------------------------
Quantity	DECIMAL(21,6);
BaseType	smallint;
BsDocType	smallint;
BsDocEntry	int;
BsDocLine	int;

exists1 int;
BaseQuantity DECIMAL(21,6);

CURSOR Currs for 
	 select "Quantity","BaseType","BsDocType","BsDocEntry","BsDocLine"
	 from IBT1
	 where "ItemCode" = :ItemCode and
		   "BatchNum" = :BatchNum and
		   "WhsCode" = :WhsCode and
		   ("BaseType" = 17 or "BsDocType" = 17);
begin

	
		Open Currs;
		 
		Fetch Currs 
		Into Quantity,BaseType,BsDocType,BsDocEntry,BsDocLine;
		While NOT Currs::NOTFOUND do

			If :BaseType = 17 then
				IsCommited := :IsCommited + :Quantity;
			Else
				select count(*) into exists1 from IBT1
				where "ItemCode"  = :ItemCode   and "BatchNum"   = :BatchNum  and
					  "WhsCode"   = :WhsCode    and "BaseType"   = :BsDocType and
					  "BaseEntry" = :BsDocEntry and "BaseLinNum" = :BsDocLine;
				if exists1 <> 0 then 	  
					select "Quantity" into BaseQuantity 
					from IBT1
					where "ItemCode"  = :ItemCode   and "BatchNum"   = :BatchNum  and
						  "WhsCode"   = :WhsCode    and "BaseType"   = :BsDocType and
						  "BaseEntry" = :BsDocEntry and "BaseLinNum" = :BsDocLine;		   
					if :BaseQuantity is not null then
						If :Quantity <= :BaseQuantity then
							IsCommited := :IsCommited - :Quantity;
						Else
							IsCommited := :IsCommited - :BaseQuantity;
						End if;
					end if;	
				end if;	
			End if;
			
			Fetch Currs
			Into Quantity,BaseType,BsDocType,BsDocEntry,BsDocLine;
		end while;
		
		Close Currs;
	
end;


