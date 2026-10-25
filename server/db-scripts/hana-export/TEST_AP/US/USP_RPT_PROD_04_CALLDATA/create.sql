CREATE procedure USP_RPT_PROD_04_CallData (
	in fdate			timestamp,
	in tdate			timestamp,
	in itemcode			nclob,--ILD_LayItemThanhPham
	in warehouse		nclob,
	in batch			nclob, --ILD_LayBatch_BCQAQC
	in DocNumPO			nclob, -- Get_DocNumPO
	in OutSoucing		nclob
)
as
begin
/*
tblitemcode		= 	select cast(Data as nvarchar(50)) as Code from split(itemcode, ',');
tblwarehouse	= 	select cast(Data as nvarchar(50)) as Code from split(warehouse, ',');
tblbatch		= 	select cast(Data as nvarchar(50)) as Code from split(batch, ',');
tblDocNumPO		= 	select cast(Data as nvarchar(50)) as Code from split(DocNumPO, ',');
*/
call USP_RPT_PROD_04 (fdate,tdate,:itemcode,:warehouse,:batch,:DocNumPO,:OutSoucing,tab_result);
select * from $tab_result;
end;

