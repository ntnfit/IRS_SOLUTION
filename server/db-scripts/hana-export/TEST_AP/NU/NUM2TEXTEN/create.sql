CREATE  FUNCTION Num2TextEN (
	number nvarchar(20)
)
RETURNS rtnStr nvarchar(4000)
AS
BEGIN
	declare sNumber, mTemp, mNumText, m1 nvarchar(4000);
	declare mLen, i, mDigit, mGroup int;
	declare l1 boolean ;
	
	sNumber = number;
	
	mLen = LENGTH(sNumber);
	
	i = 1;
	
	mTemp = '';
	l1 = false ;
	
	while (i <= mLen) do
		mDigit = substring(sNumber,i,1);
		
		mGroup = mod((mLen - i +1),3);
		
		
		
		if mGroup = 2 then
			m1 = substring(sNumber,i+1,1);
			
			
					
					if (mDigit = 0) then
						mNumText = N'';
					elseif (mDigit = 1) then
						if m1 = 0 then
								mNumText = N'Ten';
						elseif m1 = 1 then
								mNumText = N'Eleven';
						elseif m1 = 2 then
								mNumText = N'Twelve';
						elseif m1 = 3 then
								mNumText = N'Thirteen';				
						elseif m1 = 4 then
								mNumText = N'Fourteen';
						elseif m1 = 5 then
								mNumText = N'Fifteen';
						elseif m1 = 6 then
								mNumText = N'Sixteen';
						elseif m1 = 7 then
								mNumText = N'Seventeen';				
						elseif m1 = 8 then
								mNumText = N'Eighteen';
						elseif m1 = 9 then
								mNumText = N'Nineteen';																
						end if ;
					
							
					elseif (mDigit = 2) then
						mNumText = N'Twenty';
					elseif (mDigit = 3) then
						mNumText = N'Thirty';
					elseif (mDigit = 4) then
						mNumText = N'Forty';
					elseif (mDigit = 5) then
						mNumText = N'Fifty';
					elseif (mDigit = 6) then
						mNumText = N'Sixty';
					elseif (mDigit = 7) then
						mNumText = N'Seventy';
					elseif (mDigit = 8) then
						mNumText = N'Eightty';
					elseif (mDigit = 9) then
						mNumText = N'Ninety';
					end if;
		
				if m1 = 0 or mDigit = 1  then
						i = i + 1 ;	
				end if ;
		else
		
					if (mDigit = 0) then
						 mNumText = N'';
						l1 = true ;
					elseif (mDigit = 1) then
						mNumText = N'One';
					elseif (mDigit = 2) then
						mNumText = N'Two';
					elseif (mDigit = 3) then
						mNumText = N'Three';
					elseif (mDigit = 4) then
						mNumText = N'Four';
					elseif (mDigit = 5) then
						mNumText = N'Five';
					elseif (mDigit = 6) then
						mNumText = N'Six';
					elseif (mDigit = 7) then
						mNumText = N'Seven';
					elseif (mDigit = 8) then
						mNumText = N'Eight';
					elseif (mDigit = 9) then
						mNumText = N'Nine';
					end if;
					
				
		
		end if ;
		
		mTemp = mTemp || ' ' || mNumText;
		
		if (mLen = i) then
			break;
		end if;
		
		mGroup = mod((mLen - i),9);
		
		if (mGroup = 0) then
			mTemp = mTemp || N' Billion';
			
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
			mTemp = mTemp || N' Million';
			
			if (substring(sNumber, i+1, 3) = N'000') then
				i = i + 3;
			end if;
			
			if (substring(sNumber, i+1, 3) = N'000') then
				i = i + 3;
			end if;
		elseif (mGroup = 3) then
			mTemp = mTemp || N' Thousand';
			
			if (substring(sNumber, i+1, 3) = N'000') then
				i = i + 3;
			end if;
		else
			mGroup = mod((mLen - i),3);
			
			if (mGroup = 2) and l1 = false then					
					mTemp = mTemp || N' Hundred';													
			elseif (mGroup = 1) then
				mTemp = mTemp || N' ';
			end if;
		end if;
		
		l1 = false ;
		i = i + 1;
	end while;
	
		
	mTemp = trim(mTemp);
	
	mTemp = Lcase(mTemp) ;
	
	rtnStr = upper(left(mTemp,1)) || substring(mTemp,2,4000);
END


