CREATE function getTransactionSC (
	fdate		timestamp,
	tdate		timestamp,		-- Kỳ lấy dữ liệu
	acc			nvarchar(4000),	-- Tài khoản: Cách nhau bằng dấu (,);			Nhập * hoặc % là lấy tất cả		Không cho để trống
	bp			nvarchar(4000),	-- Đối tượng công nợ: Cách nhau bằng dấu (,);	Nhập * hoặc % là lất tất cả		Để trống là lấy tất cả
	dim1			nvarchar(4000),	-- Dimension 1: Cách nhau bằng dấu (,);			Nhập * hoặc % là lất tất cả		Để trống là lấy những nghiệp vụ không nhập Dim1
	dim2			nvarchar(4000),	-- Dimension 2: Cách nhau bằng dấu (,);			Nhập * hoặc % là lất tất cả		Để trống là lấy những nghiệp vụ không nhập Dim2
	dim3			nvarchar(4000),	-- Dimension 3: Cách nhau bằng dấu (,);			Nhập * hoặc % là lất tất cả		Để trống là lấy những nghiệp vụ không nhập Dim3
	dim4			nvarchar(4000),	-- Dimension 4: Cách nhau bằng dấu (,);			Nhập * hoặc % là lất tất cả		Để trống là lấy những nghiệp vụ không nhập Dim4
	dim5			nvarchar(4000),	-- Dimension 5: Cách nhau bằng dấu (,);			Nhập * hoặc % là lất tất cả		Để trống là lấy những nghiệp vụ không nhập Dim5
	prj			nvarchar(4000),	-- Project: Cách nhau bằng dấu (,);				Nhập * hoặc % là lất tất cả		Để trống là lấy những nghiệp vụ không nhập Project
	-- Đối ứng
	ofsacc		nvarchar(4000),	-- Tài khoản: Cách nhau bằng dấu (,);			Nhập * hoặc % là lấy tất cả		Không cho để trống
	ofsbp		nvarchar(4000),	-- Đối tượng công nợ: Cách nhau bằng dấu (,);	Nhập * hoặc % là lất tất cả		Để trống là lấy tất cả
	ofsdim1		nvarchar(4000),	-- Dimension 1: Cách nhau bằng dấu (,);			Nhập * hoặc % là lất tất cả		Để trống là lấy những nghiệp vụ không nhập Dim1
	ofsdim2		nvarchar(4000),	-- Dimension 2: Cách nhau bằng dấu (,);			Nhập * hoặc % là lất tất cả		Để trống là lấy những nghiệp vụ không nhập Dim2
	ofsdim3		nvarchar(4000),	-- Dimension 3: Cách nhau bằng dấu (,);			Nhập * hoặc % là lất tất cả		Để trống là lấy những nghiệp vụ không nhập Dim3
	ofsdim4		nvarchar(4000),	-- Dimension 4: Cách nhau bằng dấu (,);			Nhập * hoặc % là lất tất cả		Để trống là lấy những nghiệp vụ không nhập Dim4
	ofsdim5		nvarchar(4000),	-- Dimension 5: Cách nhau bằng dấu (,);			Nhập * hoặc % là lất tất cả		Để trống là lấy những nghiệp vụ không nhập Dim5
	ofsprj		nvarchar(4000),	-- Project: Cách nhau bằng dấu (,);				Nhập * hoặc % là lất tất cả		Để trống là lấy những nghiệp vụ không nhập Project
	-- Phương pháp tính
	method		nvarchar(5),	-- Phương pháp lấy dữ liệu trên Transaction. Danh sách các phương pháp tính được liệt kê bên dưới
	ObjTypelist	nvarchar(100),	-- Số dư Nợ/Có theo loại đối tượng nào
	ttype		char(2),		-- Y: Lấy cả nghiệp vụ Closing Period; N: Không lấy nghiệp vụ Closing Period
	includeJV	char(1),		-- C/K: Có bao gồm các nghiệp vụ Journal Voucher (Có/Không)
	curr			nvarchar(2),	-- LC/SC: Local/System Currency,
	OBorEB		nvarchar(10),	-- Đầu kỳ/Cuối kỳ
	signCal		int default 1,
	branch		int
	/*
		DANH SÁCH CÁC PHƯƠNG PHÁP TÍNH
	||================================================================================================||
	||	Mã		Tên						Mô tả cách tính
	||	--		------------			----------------------------------------------------------
	||	01		PS Nợ					Tổng PS Nợ trong kỳ
	||	02		PS Có					Tổng PS Có trong kỳ
	||	03		PS (Nợ-Có)				Tổng PS Nợ trong kỳ - Tổng PS Có trong kỳ
	||	04		PS (Nợ-Có) +			Tổng PS Nợ trong kỳ - Tổng PS Có trong kỳ: Nếu<0 thì bằng 0
	||	05		PS (Có-Nợ)				Tổng PS Có trong kỳ - Tổng PS Nợ trong kỳ
	||	06		PS (Có-Nợ) +			Tổng PS Có trong kỳ - Tổng PS Nợ trong kỳ: Nếu<0 thì bằng 0
	||	07		Dư (Nợ-Có)				Số dư cuối kỳ = Nợ - Có (Lũy kế)
	||	08		Dư (Nợ-Có) +			Số dư cuối kỳ = Nợ - Có (Lũy kế). Nếu <0 thì bằng 0
	||	09		Dư (Có-Nợ)				Số dư cuối kỳ = Có - Nợ (Lũy kế)
	||	10		Dư (Có-Nợ) +			Số dư cuối kỳ = Có - Nợ (Lũy kế). Nếu <0 thì bằng 0
	||	11		Tổng Nợ lũy kế			Tổng phát sinh Nợ lũy kế đến cuối kỳ
	||	12		Tổng Có lũy kế			Tổng phát sinh Có lũy kế đến cuối kỳ
	||	13		Dư Nợ theo đối tượng	Dư Nợ theo đối tượng (Acc,BP,D1,D2,D3,D4,D5,PO)
	||	14		Dư Có theo đối tượng	Dư Có theo đối tượng (Acc,BP,D1,D2,D3,D4,D5,PO)
	||================================================================================================||
	*/
)
returns rtnVal numeric(21,6)
as
begin
	declare ret, deb, cred, totdeb, totcred, KQ, KQSC, Debit, Credit, DebitSC, CreditSC numeric(21,6);
	
	if (OBorEB = N'ĐK') then
		declare tmpToDate, tmpFromDate timestamp;
		tmpToDate	= tdate;
		tmpFromDate	= fdate;
		tdate		= ADD_DAYS(tmpFromDate,-1);
		fdate		= ADD_MONTHS(tmpFromDate,-MONTHS_BETWEEN(tmpFromDate,tmpToDate)-1);
	end if;
	--------------------
	acc = REPLACE(acc,' ','');
	acc = REPLACE(acc,';',',');

	ofsacc = REPLACE(ofsacc,' ','');
	ofsacc = REPLACE(ofsacc,';',',');
	if (ttype = 'Y') then
		ttype = '';
	elseif (ttype <> 'Y' and ttype <> '') then
		ttype = '-3';
	end if;
	
--- Journal Entry ---------------------------------------------------------------------------------------------------------------------------------------

	KQ = 0;
	KQSC = 0;
	
	if (method in ('01','02','03','04','05','06')) then
		select sum(Debit), sum(DebitSC), sum(Credit), sum(CreditSC)
		into Debit, DebitSC, Credit, CreditSC
		from ALL_JE a
		where a.RefDate between fdate and tdate and (a.BPLId=branch or branch in (-1,0))
		and a.TransType not like ttype
		and a.Account in (select Data from getlistChildAccount(father=>:acc))
		
		and (:bp = '' or :bp = '*' or a.BPCode in (select "DATA" from Split(:bp,',')))
		and (:dim1 = '' or :dim1 = '*' or a.Dim1 in (select "DATA" from Split(:dim1,',')))
		and (:dim2 = '' or :dim2 = '*' or a.Dim2 in (select "DATA" from Split(:dim2,',')))
		and (:dim3 = '' or :dim3 = '*' or a.Dim3 in (select "DATA" from Split(:dim3,',')))
		and (:dim4 = '' or :dim4 = '*' or a.Dim4 in (select "DATA" from Split(:dim4,',')))
		and (:dim5 = '' or :dim5 = '*' or a.Dim5 in (select "DATA" from Split(:dim5,',')))
		and (:prj = '' or :prj= '*' or a.PrjCode in (select "DATA" from Split(:prj,',')))

		and (:ofsacc = '' or :ofsacc = '*' or a.ContraAct in (select Data from getListChildAccount(father=>:ofsacc)))
		and (:ofsbp = '' or :ofsbp = '*' or a.BPCodeCor	in (select "DATA" from Split(:ofsbp,',')))
		and (:ofsdim1 = '' or :ofsdim1 = '*' or a.Dim1Cor in (select "DATA" from Split(:ofsdim1,',')))
		and (:ofsdim2 = '' or :ofsdim2 = '*' or a.Dim2Cor in (select "DATA" from Split(:ofsdim2,',')))
		and (:ofsdim3 = '' or :ofsdim3 = '*' or a.Dim3Cor in (select "DATA" from Split(:ofsdim3,',')))
		and (:ofsdim4 = '' or :ofsdim4 = '*' or a.Dim4Cor in (select "DATA" from Split(:ofsdim4,',')))
		and (:ofsdim5 = '' or :ofsdim5 = '*' or a.Dim5Cor in (select "DATA" from Split(:ofsdim5,',')))
		and (:ofsprj = '' or :ofsprj = '*' or a.PrjCodeCor in (select "DATA" from Split(:ofsprj,',')));
		
		if (method = '01') then
			KQ = Debit;
			KQSC = DebitSC;
		elseif (method = '02') then
			KQ = Credit;
			KQSC = DebitSC;
		elseif (method = '03') then
			KQ = IFNULL(Debit,0) - IFNULL(Credit,0);
			KQSC = IFNULL(DebitSC,0) - IFNULL(CreditSC,0);
		elseif (method = '04') then
			KQ = CASE when IFNULL(Debit,0) - IFNULL(Credit,0)>0 then IFNULL(Debit,0) - IFNULL(Credit,0) else 0 end;
			KQSC = CASE when IFNULL(DebitSC,0) - IFNULL(CreditSC,0)>0 then IFNULL(DebitSC,0) - IFNULL(CreditSC,0) else 0 end;
		elseif (method = '05') then
			KQ = IFNULL(Credit,0) - IFNULL(Debit,0);
			KQSC = IFNULL(CreditSC,0) - IFNULL(DebitSC,0);
		elseif (method = '06') then
			KQ = CASE when IFNULL(Credit,0) - IFNULL(Debit,0)>0 then IFNULL(Credit,0) - IFNULL(Debit,0) else 0 end;
			KQSC = CASE when IFNULL(CreditSC,0) - IFNULL(DebitSC,0)>0 then IFNULL(CreditSC,0) - IFNULL(DebitSC,0) else 0 end;
		end if;
	elseif (method in ('07','08','09','10','11','12')) then
		select sum(Debit), sum(DebitSC), sum(Credit), sum(CreditSC)
		into Debit, DebitSC, Credit, CreditSC
		from ALL_JE a
		where a.RefDate <= tdate and (a.BPLId=branch or branch in (-1,0))
		and a.TransType not like ttype
		and a.Account in (select Data from getlistChildAccount(father=>:acc))

		and (:bp = '' or :bp = '*' or a.BPCode in (select "DATA" from Split(:bp,',')))
		and (:dim1 = '' or :dim1 = '*' or a.Dim1 in (select "DATA" from Split(:dim1,',')))
		and (:dim2 = '' or :dim2 = '*' or a.Dim2 in (select "DATA" from Split(:dim2,',')))
		and (:dim3 = '' or :dim3 = '*' or a.Dim3 in (select "DATA" from Split(:dim3,',')))
		and (:dim4 = '' or :dim4 = '*' or a.Dim4 in (select "DATA" from Split(:dim4,',')))
		and (:dim5 = '' or :dim5 = '*' or a.Dim5 in (select "DATA" from Split(:dim5,',')))
		and (:prj = '' or :prj= '*' or a.PrjCode in (select "DATA" from Split(:prj,',')))

		and (:ofsacc = '' or :ofsacc = '*' or a.ContraAct in (select Data from getListChildAccount(father=>:ofsacc)))
		and (:ofsbp = '' or :ofsbp = '*' or a.BPCodeCor	in (select "DATA" from Split(:ofsbp,',')))
		and (:ofsdim1 = '' or :ofsdim1 = '*' or a.Dim1Cor in (select "DATA" from Split(:ofsdim1,',')))
		and (:ofsdim2 = '' or :ofsdim2 = '*' or a.Dim2Cor in (select "DATA" from Split(:ofsdim2,',')))
		and (:ofsdim3 = '' or :ofsdim3 = '*' or a.Dim3Cor in (select "DATA" from Split(:ofsdim3,',')))
		and (:ofsdim4 = '' or :ofsdim4 = '*' or a.Dim4Cor in (select "DATA" from Split(:ofsdim4,',')))
		and (:ofsdim5 = '' or :ofsdim5 = '*' or a.Dim5Cor in (select "DATA" from Split(:ofsdim5,',')))
		and (:ofsprj = '' or :ofsprj = '*' or a.PrjCodeCor in (select "DATA" from Split(:ofsprj,',')));

		if (method = '07') then
			KQ = IFNULL(Debit,0) - IFNULL(Credit,0);
			KQSC = IFNULL(DebitSC,0) - IFNULL(CreditSC,0);
		elseif (method = '08') then
			KQ = CASE when IFNULL(Debit,0) - IFNULL(Credit,0)>0 then IFNULL(Debit,0) - IFNULL(Credit,0) else 0 end;
			KQSC = CASE when IFNULL(DebitSC,0) - IFNULL(CreditSC,0)>0 then IFNULL(DebitSC,0) - IFNULL(CreditSC,0) else 0 end;
		elseif (method = '09') then
			KQ = IFNULL(Credit,0) - IFNULL(Debit,0);
			KQSC = IFNULL(CreditSC,0) - IFNULL(DebitSC,0);
		elseif (method = '10') then
			KQ = CASE when IFNULL(Credit,0) - IFNULL(Debit,0)>0 then IFNULL(Credit,0) - IFNULL(Debit,0) else 0 end;
			KQSC = CASE when IFNULL(CreditSC,0) - IFNULL(DebitSC,0)>0 then IFNULL(CreditSC,0) - IFNULL(DebitSC,0) else 0 end;
		elseif (method = '11') then
			KQ = IFNULL(Debit,0);
			KQSC = IFNULL(DebitSC,0);
		elseif (method = '12') then
			KQ = IFNULL(Credit,0);
			KQSC = IFNULL(CreditSC,0);
		end if;
	elseif (method = '13') then
		select sum(T0.Amnt), sum(T0.AmntSC)
		into KQ, KQSC
		from
			(select 
				sum(Debit-Credit) as Amnt,
				sum(DebitSC-CreditSC) as AmntSC
			from ALL_JE a
			where a.RefDate<=tdate and (a.BPLId=branch or branch in (-1,0))
			and a.TransType not like ttype
			and a.Account in (select Data from getlistChildAccount(father=>:acc))
	
			and (:bp = '' or :bp = '*' or a.BPCode in (select "DATA" from Split(:bp,',')))
			and (:dim1 = '' or :dim1 = '*' or a.Dim1 in (select "DATA" from Split(:dim1,',')))
			and (:dim2 = '' or :dim2 = '*' or a.Dim2 in (select "DATA" from Split(:dim2,',')))
			and (:dim3 = '' or :dim3 = '*' or a.Dim3 in (select "DATA" from Split(:dim3,',')))
			and (:dim4 = '' or :dim4 = '*' or a.Dim4 in (select "DATA" from Split(:dim4,',')))
			and (:dim5 = '' or :dim5 = '*' or a.Dim5 in (select "DATA" from Split(:dim5,',')))
			and (:prj = '' or :prj= '*' or a.PrjCode in (select "DATA" from Split(:prj,',')))
	
			and (:ofsacc = '' or :ofsacc = '*' or a.ContraAct in (select Data from getListChildAccount(father=>:ofsacc)))
			and (:ofsbp = '' or :ofsbp = '*' or a.BPCodeCor	in (select "DATA" from Split(:ofsbp,',')))
			and (:ofsdim1 = '' or :ofsdim1 = '*' or a.Dim1Cor in (select "DATA" from Split(:ofsdim1,',')))
			and (:ofsdim2 = '' or :ofsdim2 = '*' or a.Dim2Cor in (select "DATA" from Split(:ofsdim2,',')))
			and (:ofsdim3 = '' or :ofsdim3 = '*' or a.Dim3Cor in (select "DATA" from Split(:ofsdim3,',')))
			and (:ofsdim4 = '' or :ofsdim4 = '*' or a.Dim4Cor in (select "DATA" from Split(:ofsdim4,',')))
			and (:ofsdim5 = '' or :ofsdim5 = '*' or a.Dim5Cor in (select "DATA" from Split(:ofsdim5,',')))
			and (:ofsprj = '' or :ofsprj = '*' or a.PrjCodeCor in (select "DATA" from Split(:ofsprj,',')))

			group by 
				Account,
				CASE when LOCATE(ObjTypelist,'SN')>0 then BPCode else '' end,
				CASE when LOCATE(ObjTypelist,'Dim1')>0 then Dim1 else '' end,
				CASE when LOCATE(ObjTypelist,'Dim2')>0 then Dim2 else '' end,
				CASE when LOCATE(ObjTypelist,'Dim3')>0 then Dim3 else '' end,
				CASE when LOCATE(ObjTypelist,'Dim4')>0 then Dim4 else '' end,
				CASE when LOCATE(ObjTypelist,'Dim5')>0 then Dim5 else '' end,
				CASE when LOCATE(ObjTypelist,'Project')>0 then PrjCode else '' end
			having sum(Debit-Credit)>0) T0;
	elseif (method = '14') then
		select sum(T0.Amnt), sum(T0.AmntSC)
		into KQ, KQSC
		from
			(select 
				sum(Credit-Debit) as Amnt,
				sum(CreditSC-DebitSC) as AmntSC
			from ALL_JE a
			where a.RefDate<=tdate and (a.BPLId=branch or branch in (-1,0))
			and a.TransType not like ttype
			and a.Account in (select Data from getlistChildAccount(father=>:acc))
	
			and (bp='' or bp='*' or a.BPCode	in (select "DATA" from Split(:bp,',')))
			and (:bp = '' or :bp = '*' or a.BPCode in (select "DATA" from Split(:bp,',')))
			and (:dim1 = '' or :dim1 = '*' or a.Dim1 in (select "DATA" from Split(:dim1,',')))
			and (:dim2 = '' or :dim2 = '*' or a.Dim2 in (select "DATA" from Split(:dim2,',')))
			and (:dim3 = '' or :dim3 = '*' or a.Dim3 in (select "DATA" from Split(:dim3,',')))
			and (:dim4 = '' or :dim4 = '*' or a.Dim4 in (select "DATA" from Split(:dim4,',')))
			and (:dim5 = '' or :dim5 = '*' or a.Dim5 in (select "DATA" from Split(:dim5,',')))
			and (:prj = '' or :prj= '*' or a.PrjCode in (select "DATA" from Split(:prj,',')))
	
			and (:ofsacc = '' or :ofsacc = '*' or a.ContraAct in (select Data from getListChildAccount(father=>:ofsacc)))
			and (:ofsbp = '' or :ofsbp = '*' or a.BPCodeCor	in (select "DATA" from Split(:ofsbp,',')))
			and (:ofsdim1 = '' or :ofsdim1 = '*' or a.Dim1Cor in (select "DATA" from Split(:ofsdim1,',')))
			and (:ofsdim2 = '' or :ofsdim2 = '*' or a.Dim2Cor in (select "DATA" from Split(:ofsdim2,',')))
			and (:ofsdim3 = '' or :ofsdim3 = '*' or a.Dim3Cor in (select "DATA" from Split(:ofsdim3,',')))
			and (:ofsdim4 = '' or :ofsdim4 = '*' or a.Dim4Cor in (select "DATA" from Split(:ofsdim4,',')))
			and (:ofsdim5 = '' or :ofsdim5 = '*' or a.Dim5Cor in (select "DATA" from Split(:ofsdim5,',')))
			and (:ofsprj = '' or :ofsprj = '*' or a.PrjCodeCor in (select "DATA" from Split(:ofsprj,',')))
		
			group by 
				Account,
				CASE when LOCATE(ObjTypelist,'SN')>0 then BPCode else '' end,
				CASE when LOCATE(ObjTypelist,'Dim1')>0 then Dim1 else '' end,
				CASE when LOCATE(ObjTypelist,'Dim2')>0 then Dim2 else '' end,
				CASE when LOCATE(ObjTypelist,'Dim3')>0 then Dim3 else '' end,
				CASE when LOCATE(ObjTypelist,'Dim4')>0 then Dim4 else '' end,
				CASE when LOCATE(ObjTypelist,'Dim5')>0 then Dim5 else '' end,
				CASE when LOCATE(ObjTypelist,'Project')>0 then PrjCode else '' end
			having sum(CreditSC-DebitSC)>0) T0;
	end if;

	-------------------------------------------------------------------------------------------
	--------------- TRẢ VỀ KẾT QUẢ THEO TỪNG PHƯƠNG PHÁP---------------------------------------
	ret = KQ;
	if (curr = 'SC') then
		ret = KQSC;
	end if;
	if signCal = -1 then
		ret = -ret;
	end if;
	-------------------------------------------------------------------------------------------
	rtnVal = IFNULL(ret,0);
end

