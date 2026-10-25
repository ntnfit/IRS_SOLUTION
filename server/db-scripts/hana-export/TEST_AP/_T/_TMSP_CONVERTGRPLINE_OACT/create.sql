-- B1 DEPENDS: BEFORE:PT:PROCESS_START

CREATE PROCEDURE _TmSp_ConvertGrpLine_OACT (in v_RandomNum nvarchar(1))
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER

AS

acct1 nvarchar(15);
acct2 nvarchar(15);
acct3 nvarchar(15);
acct4 nvarchar(15);
counter tinyint;
maxlevel tinyint;
line int;
ErrNum int;

level2 int;
level3 int;
postable3 nvarchar(15);
level4 int;
postable4 nvarchar(15);
maxGrpLine int;
exists1 int;

begin
IF :v_RandomNum <> '' THEN

-----------------------------------------------------------
	maxlevel := 10;
	counter := 1;
	ErrNum := 10000;

	update OACT set "GrpLine" = 0 where "GrpLine" is NULL;

	update OACT set "Groups" = :ErrNum;

	while :counter <= :maxlevel do
-------------------Level 1------------------------------------
		if :counter <= 9 then
		  acct1 := cast(counter as nchar(1)) || '00000000000000';
		else 
		  acct1 := 'A' || '00000000000000';
		end if;

		update OACT
		set "Groups" = 1,
			"GroupMask" = :counter,
			"Levels" = 1,
			"FatherNum" = NULL, 
			"Postable" = 'N'
		where "AcctCode" = :acct1;

		line := 2;
		
		select count("AcctCode") into level2 from OACT
		where "FatherNum" = :acct1 and "Levels" = 2 and      
			  "Groups" = :ErrNum;
					    
		while (:level2 > 0)  do
		-------------------Level 2-------------------------------------	   
			select Top 1 "AcctCode" into acct2
			from OACT
			where "FatherNum" = :acct1 and "Levels" = 2 and "Groups" = :ErrNum and
				  "GrpLine" = (select MIN ("GrpLine") from OACT
							   where "FatherNum" = :acct1 and
									 "Levels" = 2 and "Groups" = :ErrNum);
			update OACT
			set "Groups" = :line,
				"GroupMask" = :counter
			where "AcctCode" = :acct2;
			
			line := :line + 1;

			select count("AcctCode") into level3 from OACT
			where "FatherNum" = :acct2 and "Levels" = 3 and "Groups" = :ErrNum;
			
			select "Postable" into postable3 from OACT where "AcctCode" = :acct2;			  
			
			while (:level3 > 0) and (:postable3 = 'N') do
			-------------------Level 3------------------------------------
				select Top 1 "AcctCode" into acct3
				from OACT
				where "FatherNum" = :acct2 and "Levels" = 3 and "Groups" = :ErrNum and
					  "GrpLine" = (select MIN ("GrpLine") from OACT
								   where "FatherNum" = :acct2 and
									     "Levels" = 3 and "Groups" = :ErrNum);
				update OACT
				set "Groups" = :line,
				    "GroupMask" = :counter
				where "AcctCode" = :acct3;
				
				line := :line + 1 ;

				select count("AcctCode") into level4 from OACT
				where "FatherNum" = :acct3 and "Levels" = 4 and "Groups" = :ErrNum;
				
				select "Postable" into postable4 from OACT where "AcctCode" = :acct3;
				
				while (:level4 > 0) and (:postable4 = 'N') do
				-------------------Level 4--------------------------------------
					select Top 1 "AcctCode" into acct4
						   from OACT
						   where "FatherNum" = :acct3 and "Levels" = 4 and "Groups" = :ErrNum and
								 "GrpLine" = (select MIN ("GrpLine") from OACT
											where "FatherNum" = :acct3 and
											"Levels" = 4 and "Groups" = :ErrNum);
					update OACT
					set "Groups" = :line,
						"GroupMask" = :counter
					where "AcctCode" = :acct4;
					
					line := :line + 1;
					
					select count("AcctCode") into level4 from OACT
					where "FatherNum" = :acct3 and "Levels" = 4 and "Groups" = :ErrNum;
				
					select "Postable" into postable4 from OACT where "AcctCode" = :acct3;
					
				end while;--End Level 4 

			select count("AcctCode") into level3 from OACT
			where "FatherNum" = :acct2 and "Levels" = 3 and "Groups" = :ErrNum;
			
			select "Postable" into postable3 from OACT where "AcctCode" = :acct2;
			
			end while;--End Level 3
			
			select count("AcctCode") into level2 from OACT
			where "FatherNum" = :acct1 and "Levels" = 2 and      
			      "Groups" = :ErrNum;
		end while;--End Level 2

		counter := :counter + 1 ;
	end while;--End Level 1

	update OACT set "GrpLine" = "Groups","Groups" = NULL;

	counter := 1;

	while :counter <= :maxlevel do
-------------------Error Accounts Manage----------------------
		select MAX ("GrpLine") into maxGrpLine from OACT
		where "GroupMask" = :counter and "GrpLine" <> :ErrNum;
		
		line := :maxGrpLine + 1 ;
				 
		if :counter <= 9 then
		  acct1 := cast(:counter as nchar(1)) || '00000000000000';
		else 
		  acct1 := 'A' || '00000000000000';
		end if;

		select count(1) into exists1 from OACT
		where "GrpLine" = :ErrNum and "GroupMask" = :counter;
		
		while (:exists1 > 0) do
			
			select Top 1 "AcctCode" into acct2
			from OACT
			where "GrpLine" = :ErrNum and "GroupMask" = :counter;
			
			update OACT
			set "Levels" = 2,
				"FatherNum" = :acct1,
				"GrpLine" = :line
			where "AcctCode" = :acct2;
			
			line := :line + 1;
			
			select count(1) into exists1 from OACT
			where "GrpLine" = :ErrNum and "GroupMask" = :counter;
		
		end while;
		counter := :counter + 1 ;
	end while;
END IF;
----------------------------------------------------------------
end;


