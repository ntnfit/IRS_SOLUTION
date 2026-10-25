CREATE procedure ILD_R_IRS_Wh07_CallData (
	in fdate			timestamp,
	in tdate			timestamp,
	in litemcode		nclob, --ILD_LAY_ITEMCODE
	in lwhs				nclob, --ILD_LAY_WAREHOUSE
	in litemgroup		nclob, --ILD_LAY_ITEMGROUP
	in account			nclob --USP_CFL_INVENTORY_ACCOUNT
)
as
begin

call ILD_R_IRS_CHITIETXUATNHAPTONTHEOBATCH (fdate,tdate,litemcode,lwhs,litemgroup,account,result_out);
select * from $result_out;
end;

