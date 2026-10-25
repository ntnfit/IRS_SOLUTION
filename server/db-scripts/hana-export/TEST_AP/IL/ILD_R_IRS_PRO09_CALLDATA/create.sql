CREATE procedure ILD_R_IRS_Pro09_CallData (
	in FromDate			timestamp,
	in ToDate			timestamp,
	in Resource			nclob,
	in OutSoucing		nclob,
	in WODocNums		nclob,
	in ItemCodes		nclob
)
as
begin

call ILD_R_IRS_LenhSanXuatDaNhapThanhPham (FromDate,ToDate,Resource,OutSoucing,WODocNums,ItemCodes,tab_result);
select * from $tab_result;
end;
