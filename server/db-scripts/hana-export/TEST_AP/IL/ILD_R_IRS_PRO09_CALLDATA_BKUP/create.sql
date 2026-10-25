create procedure ILD_R_IRS_Pro09_CallData_bkup (
	in FromDate			timestamp,
	in ToDate			timestamp,
	in Resource			nclob,
	in OutSoucing		nclob
)
as
begin

call ILD_R_IRS_LenhSanXuatDaNhapThanhPham (FromDate,ToDate,Resource,OutSoucing,tab_result);
select * from $tab_result;
end;
