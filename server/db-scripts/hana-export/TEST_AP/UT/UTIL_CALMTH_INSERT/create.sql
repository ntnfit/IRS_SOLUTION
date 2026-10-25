
CREATE PROCEDURE Util_CalMth_Insert
as
BEGIN
	insert into APZ_CalMth ("CalMthCode", "CalMthName", "CalMthDetail")
	select '01',N'PS Nợ', N'Tổng PS Nợ trong kỳ' FROM DUMMY
		UNION ALL
	select '02',N'PS Có', N'Tổng PS Có trong kỳ' from DUMMY
		UNION ALL
	select '03',N'PS (Nợ-Có)',N'Tổng PS Nợ trong kỳ - Tổng PS Có trong kỳ' FROM DUMMY
		UNION ALL
	select '04',N'PS (Nợ-Có) +',N'Tổng PS Nợ trong kỳ - Tổng PS Có trong kỳ: Nếu<0 thì bằng 0' from DUMMY
		UNION ALL
	select '05',N'PS (Có-Nợ)',N'Tổng PS Có trong kỳ - Tổng PS Nợ trong kỳ' FROM DUMMY
		UNION ALL
	SELECT '06',N'PS (Có-Nợ) +',N'Tổng PS Có trong kỳ - Tổng PS Nợ trong kỳ: Nếu<0 thì bằng 0' FROM DUMMY
		UNION ALL
	SELECT '07',N'Dư (Nợ-Có)',N'Số dư cuối kỳ = Nợ - Có (Lũy kế)' FROM DUMMY
		UNION ALL
	SELECT '08',N'Dư (Nợ-Có) +',N'Số dư cuối kỳ = Nợ - Có (Lũy kế). Nếu <0 thì bằng 0' FROM DUMMY
		UNION ALL
	select '09',N'Dư (Có-Nợ)',N'Số dư cuối kỳ = Có - Nợ (Lũy kế)' from dummy
		union all
	select '10',N'Dư (Có-Nợ) +',N'Số dư cuối kỳ = Có - Nợ (Lũy kế). Nếu <0 thì bằng 0' from dummy
		union all
	select '11',N'Tổng Nợ lũy kế',N'Tổng phát sinh Nợ lũy kế đến cuối kỳ' from dummy
		union all
	select '12',N'Tổng Có lũy kế',N'Tổng phát sinh Có lũy kế đến cuối kỳ' from dummy;
END



