create FUNCTION Num2Text (
	number nvarchar(20)
)
RETURNS rtnStr nvarchar(4000)
AS
BEGIN
	declare sNumber, mTemp, mNumText nvarchar(4000);
	declare mLen, i, mDigit, mGroup int;
	
	sNumber = number;
	
	mLen = LENGTH(sNumber);
	
	i = 1;
	
	mTemp = '';
	
	while (i <= mLen) do
		mDigit = substring(sNumber,i,1);
		
		if (mDigit = 0) then
			mNumText = N'không';
		elseif (mDigit = 1) then
			mNumText = N'một';
		elseif (mDigit = 2) then
			mNumText = N'hai';
		elseif (mDigit = 3) then
			mNumText = N'ba';
		elseif (mDigit = 4) then
			mNumText = N'bốn';
		elseif (mDigit = 5) then
			mNumText = N'năm';
		elseif (mDigit = 6) then
			mNumText = N'sáu';
		elseif (mDigit = 7) then
			mNumText = N'bảy';
		elseif (mDigit = 8) then
			mNumText = N'tám';
		elseif (mDigit = 9) then
			mNumText = N'chín';
		end if;
		
		mTemp = mTemp || ' ' || mNumText;
		
		if (mLen = i) then
			break;
		end if;
		
		mGroup = mod((mLen - i),9);
		
		if (mGroup = 0) then
			mTemp = mTemp || N' tỷ';
			
			if (substring(sNumber, i+1, 3) = N'000') then
				i = i + 3;
			end if;
			
			if (substring(sNumber, i+1, 3) = N'000') then
				i = i + 3;
			end if;
			
			if (substring(sNumber, i+1, 3) = N'000') then
				i = i + 3;
			end if;
		elseif (mGroup = 6) then
			mTemp = mTemp || N' triệu';
			
			if (substring(sNumber, i+1, 3) = N'000') then
				i = i + 3;
			end if;
			
			if (substring(sNumber, i+1, 3) = N'000') then
				i = i + 3;
			end if;
		elseif (mGroup = 3) then
			mTemp = mTemp || N' nghìn';
			
			if (substring(sNumber, i+1, 3) = N'000') then
				i = i + 3;
			end if;
		else
			mGroup = mod((mLen - i),3);
			
			if (mGroup = 2) then
				mTemp = mTemp || N' trăm';
			elseif (mGroup = 1) then
				mTemp = mTemp || N' mươi';
			end if;
		end if;
		
		i = i + 1;
	end while;
	
	mTemp = replace(mTemp,N'không mươi không','');
	mTemp = replace(mTemp,N'không mươi ',N'linh ');
	
	mTemp = replace(mTemp, N'mươi không', N'mươi');
	
	mTemp = replace(mTemp, N'một mươi', N'mười');
	
	mTemp = replace(mTemp, N'mươi bốn', N'mươi tư');
	
	mTemp = replace(mTemp, N'linh bốn', N'linh tư');
	
	mTemp = replace(mTemp, N'mươi năm', N'mươi lăm');
	
	mTemp = replace(mTemp, N'mươi một', N'mươi mốt');
	
	mTemp = replace(mTemp, N'mười năm', N'mười lăm');
	
	mTemp = trim(mTemp);
	
	rtnStr = upper(left(mTemp,1)) || substring(mTemp,2,4000);
END



