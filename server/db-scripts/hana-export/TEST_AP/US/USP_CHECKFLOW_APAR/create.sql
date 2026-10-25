CREATE procedure USP_CHECKFLOW_APAR
(
	in object_type nvarchar(30), 				-- SBO Object Type
	in list_of_cols_val_tab_del nvarchar(255),
	out error  int,
	out error_message nvarchar (200)
)
as
begin

	--- Chặn không làm độc lập-đảm bảo flow AP-AR
	/*
		Không được làm độc lập mà phải kế thừa theo flow:

		•AP: PR-PO -GRPO-AP Invoices-Outgoing .--Chua thong nhat them A/P Resert Invoice(Chi theo green been.Item group 117)

		•AP có downpayment: PR-PO -AP downpayment request-AP invoices (for downpayment)-Outgoing 1-GRPO-AP-Outgoing 2

		•AR: SO-Delivery note-AR-Incoming
	*/

	declare pObjType	nvarchar(50);
	declare pTable		nvarchar(50);
	declare pBaseRef	nvarchar(50);
	declare pMessage	nvarchar(100);
	declare pCancel 	nvarchar(1) = 'N';
	DECLARE pquery		nvarchar(150);
	
	declare tbflow table
	(
		"Code"		nvarchar(30),
		"TableName" nvarchar(50),
		"Descript"	nvarchar(150),
		"Mesage"	nvarchar(100)
	);
	error := 0;
	error_message := N'Ok';	
	pMessage := N'Phiếu phải được kế thừa';
--	return; 

	insert into :tbflow("Code","TableName","Descript")
	select 22 as "Code",'POR1' as "Name",'SELECT CANCELED from OPOR where "DocEntry" = ' as "Descript" from dummy
	union all
	select 20 as "Code",'PDN1'as "Name",'SELECT CANCELED from OPDN where "DocEntry" = '  as "Descript" from dummy	
	union all
	select 18,'PCH1','SELECT CANCELED from OPCH where "DocEntry" = ' from dummy	
	union all
	select 46,'VPM2','SELECT "Canceled" from OVPM where "DocEntry" = ' from dummy		
	union all
	select 204,'DPO1','SELECT CANCELED from ODPO where "DocEntry" = ' from dummy				
	union all
	select 15,'DLN1','SELECT CANCELED from ODLN where "DocEntry" = ' from dummy			
	union all
	select 13,'INV1','SELECT CANCELED from OINV where "DocEntry" = ' from dummy	
	union all
	select 112,'DRF1','SELECT CANCELED from ODRF where "DocEntry" = ' from dummy ;	
/*	union all
	select 24,'RCT2','SELECT "Canceled" from ORCT where "DocEntry" = ' from dummy	
Khong can kiem soat Incoming Pyament	*/	

	-- Loai tru phieu A/P Inv co Type = Service. Chi kiem tra phieu type = Item.
	if(object_type = '18') then -- A/P chinh thuc
		if exists (select "DocType" from opch where "DocType" = 'S' and "DocEntry" = :list_of_cols_val_tab_del) then
			return;
		end if;
	end if;
	if(object_type = '13') then -- A/R chinh thuc
		if exists (select "DocType" from oinv where "DocType" = 'S' and "DocEntry" = :list_of_cols_val_tab_del) then
			return;
		end if;
	end if;	
	if(object_type = '112') then -- Phieu draft
		if exists (select "DocType" from ODRF where "DocType" = 'S' and "ObjType" in ('13','18') and "DocEntry" = :list_of_cols_val_tab_del) then
			return;
		end if;
	end if;
	
	--- Loai tru phieu Cancel, khong kiem tra.
	select "TableName","Descript" into pTable,pquery default '','' from :tbflow where "Code" = object_type;	
	if (pquery != '') then
		execute immediate pquery || list_of_cols_val_tab_del into pCancel default 'N';
		if (:pCancel != 'N') then	
			error := 0;			
			return;
		end if;
	end if;
	--- end

	if (pTable != '') then
		if(object_type in ('46')) then --'24'
			execute immediate 'select top 1 max("DocEntry") as "BaseRef","InvType" from ' || pTable || ' where "DocNum" = ' || list_of_cols_val_tab_del || ' group by "InvType"' into pBaseRef,pObjType default '','';
			if (pBaseRef = '') then
				error = 1763220;
				error_message= pMessage;
			end if;
			return;
		else
			execute immediate 'select top 1 case when  ifnull("BaseRef",'''') = '''' then ''x'' else "BaseRef" end "BaseRef" ,"ObjType" from ' || pTable || ' where ifnull("BaseRef",'''') = '''' and "DocEntry" = ' || list_of_cols_val_tab_del into pBaseRef,pObjType default '','';
			if (pBaseRef = 'x') then --pBaseRef = '' or 
				if (object_type = 112) then
					if exists(select "TableName" from :tbflow where "Code" = pObjType) then
						error = 176321;
						error_message= pMessage;				
					end if;
				else
					error = 176322;
					error_message= pMessage;
				end if;
				return;
			end if;
		end if;
	end if;
	
end
