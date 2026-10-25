-- B1 DEPENDS: AFTER:PT:CREATE_TABLES_TO_VIEWS AFTER:SP:_TmSp_RestoreIsCommited_CursorInOIBT

CREATE PROCEDURE _TmSp_RestoreIsCommited_OIBT
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER

AS
-------------------------------------------------------------
ItemCode	nvarchar (50);
BatchNum	nvarchar (32);
WhsCode		nvarchar (8);

hasNext 	int := 1;
IsCommited 	DECIMAL(21,6);

--SET NOCOUNT ON

CURSOR Currs for 
	select "ItemCode","BatchNum","WhsCode"
	from OIBT;

begin

	Open Currs;

	Fetch Currs Into ItemCode,BatchNum,WhsCode;
	if not Currs::NOTFOUND then
		hasNext := -1;
	end if;
	
	While (:hasNext > 0) do
	
		IsCommited := 0;
		-----------------------------------------------------
		call _TmSp_RestoreIsCommited_CursorInOIBT(ItemCode,BatchNum,WhsCode,IsCommited);
		--------------------------------------------------------------
		Update OBTQ Set "CommitQty" = :IsCommited;

		Fetch Currs Into ItemCode,BatchNum,WhsCode;
		if not Currs::NOTFOUND then
			hasNext := -1;
		end if;
		
	end while;
	
	Close Currs;
	
	--SET NOCOUNT OFF
end;
