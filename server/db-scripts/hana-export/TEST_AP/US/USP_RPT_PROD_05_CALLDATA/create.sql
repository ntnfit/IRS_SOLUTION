CREATE procedure USP_RPT_PROD_05_CallData (
	in fdate			timestamp,
	in tdate			timestamp,
	in warehouse		nclob, --USP_CFL_WAREHOUSE
	in batch			nclob, --ILD_LayBatch_BCQAQC
	in DocNumPO			nclob, --GET_DOCNUMPO
 	in itemcode			nclob,--ILD_LayItemThanhPham
	in Resource 		nclob -- GET_ITEMCODE
)
as
begin
call USP_RPT_PROD_05 (fdate,tdate,:warehouse,:batch,:DocNumPO,:itemcode,:Resource,tab_result);
select  
	GrpCode "Group Code",
	GrpName "Group",
	ItemCode "Item Code",
	ItemName "Item Name",
	ItemUoM "UoM",
	WOID "W/OID",
	WONo "W/O No.",
	WOStatus "W/O Status",
	WOType "W/O Type",
	WOItemCode "W/O Item Code",
	WOItemName "W/O Item Name",
	WOItemGroup "W/O Item Group",
	DocType "DocType",
	DocNote "Document Note",
	DocID "DocID",
	DocNum "DocNum",
	DocDate "Document Date",
	DocTime  "DocTime",
	QtyTotal "QtyTotal",
	Qty "Quantity",
	UnitPrice  "UnitPrice",
	--round(Amount,2) "Amount",
	 Amount  "Amount",
	Note "Note",
	SSCC_BatchNum "SSCC",
	Batch_MnfSerial "Batch",
	MnfDate "MnfDate",
	ExpDate "ExpDate",
	AdmissionDate "Admission Date",
	SSCCStatus_LotNumber "LotNumber",
	DISID "DisID",
	DISNO "DisNo",
	RFPID,
	RFPNo "RFPNo",
	
	FATHER1 "Father1",
	FATHER1_Date "Father1_Date"
	
	from $tab_result a 
	where (ifnull(:Batch, '') = '' or cast (a.Batch_MnfSerial as nvarchar) in (select "DATA" from split(:Batch ,',')))
	order by GrpName,ItemCode;
 
end;

