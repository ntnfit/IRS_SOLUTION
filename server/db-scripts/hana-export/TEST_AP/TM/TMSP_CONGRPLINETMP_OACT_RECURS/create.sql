-- B1 DEPENDS: AFTER:PT:PROCESS_END

CREATE PROCEDURE TmSp_ConGrpLinetmp_OACT_recurs(
in acct1 nvarchar(15),
in line int,
in counter tinyint,
in ErrNum int,
out LineOut int
)
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER
AS

acct2 nvarchar(15);
level smallint;
postable nvarchar (1);
acct3 nvarchar(15);
acct4 nvarchar(15);
acct5 nvarchar(15);
acct6 nvarchar(15);
acct7 nvarchar(15);
acct8 nvarchar(15);
acct9 nvarchar(15);
acct10 nvarchar(15);
-------------------------------------------------
ifcond2 int;
ifcond31 int;
ifcond32 int;
ifcond41 int;
ifcond42 int;
ifcond51 int;
ifcond52 int;
ifcond61 int;
ifcond62 int;
ifcond71 int;
ifcond72 int;
ifcond81 int;
ifcond82 int;
ifcond91 int;
ifcond92 int;
ifcond101 int;
ifcond102 int;
linetmp int;  --Used to solve the issue that "in line" couldn't be assinged value temporarily.
-------------------------------------------------
begin
	linetmp := :line;
	if  :linetmp = 2 then
		level := 2; 
	else
	  select "Levels" + 1 into level
	 	 from OACT
	  where "AcctCode" = :acct1;
	end if;
 -------------------Level 2------------------------------------	
	select count("AcctCode") into ifcond2 from OACT
               where "FatherNum" = :acct1 and "Levels" = :level and      
               "Groups" = :ErrNum;
	while :ifcond2 > 0
    do
    	 select Top 1 "AcctCode" into acct2
         	from OACT
      		where "FatherNum" = :acct1 and 
      		"Levels" = :level and 
      		"Groups" = :ErrNum and
            "GrpLine" = (select MIN("GrpLine") from OACT
                       		 where "FatherNum" = :acct1 and
                          	 "Levels" = :level and 
                          	 "Groups" = :ErrNum);
 
 		  update OACT
		  set "Groups" = :linetmp,
		      "GroupMask" = :counter
		  where "AcctCode" = :acct2;
		  

		  linetmp := :linetmp + 1;

  -------------------Level 3------------------------------------		
 		  select count("AcctCode") into ifcond31
 		  	from OACT
		  where "FatherNum" = :acct2 and "Levels" = 3 and
		                "Groups" = :ErrNum;
		  
		  select count("Postable") into ifcond32 
		  	from OACT 
		  where "AcctCode" = :acct2 and
		  		 "Postable" = 'N';  
		  		 
		  WHILE (:ifcond31 > 0) and  (:ifcond32 > 0)
		  do
			 select Top 1 "AcctCode" into acct3
			  	from OACT
			 where "FatherNum" = :acct2 and "Levels" = 3 and "Groups" = :ErrNum and
					"GrpLine" = (select MIN ("GrpLine") from OACT
							   where "FatherNum" = :acct2 and
							   "Levels" = 3 and "Groups" = :ErrNum);
			 update OACT
			   set "Groups" = :linetmp,
				   "GroupMask" = :counter
			 where "AcctCode" = :acct3;
			 
			 linetmp := :linetmp + 1; 
	
	-------------------For Level 4--------------------------------------
			select count("AcctCode") into ifcond41
				 from OACT
			where "FatherNum" = :acct3 and "Levels" = 4 and
					 "Groups" = :ErrNum;
					 
			select count("Postable") into ifcond42 from OACT 
				where "AcctCode" = :acct3 and "Postable" = 'N';
				
			WHILE :ifcond41 > 0 and :ifcond42 > 0
	-------------------Level 4--------------------------------------
			
			do
				select Top 1 "AcctCode" into acct4
				   from OACT
				   where "FatherNum" = :acct3 and "Levels" = 4 and "Groups" = :ErrNum and
						 "GrpLine" = (select MIN ("GrpLine") from OACT
											where "FatherNum" = :acct3 and
									"Levels" = 4 and "Groups" = :ErrNum);
				update OACT
				set "Groups" = :linetmp,
					"GroupMask" = :counter
				where "AcctCode" = :acct4;
			 
			    linetmp := :linetmp + 1;
			    
			    -------------------For Level 5 while Condition --------------------------------------
				select count("AcctCode") into ifcond51 
					from OACT
				where "FatherNum" = :acct4 and "Levels" = 5 and
							 "Groups" = :ErrNum;
				
				select count("Postable") into ifcond52 from OACT 
					where "AcctCode" = :acct4 and "Postable" = 'N';
					  		    
				WHILE :ifcond51 > 0 and :ifcond52 > 0
				do
				-------------------Level 5--------------------------------------    
					select Top 1 "AcctCode" into acct5
						   from OACT
						   where "FatherNum" = :acct4 and "Levels" = 5 and "Groups" = :ErrNum and
								 "GrpLine" = (select MIN ("GrpLine") from OACT
											where "FatherNum" = :acct4 and
												"Levels" = 5 and "Groups" = :ErrNum);
					update OACT
					set "Groups" = :linetmp,
						"GroupMask" = :counter
					where "AcctCode" = :acct5;
					
					linetmp := :linetmp + 1;
			
				
				-------------------For Level 6 while Condition --------------------------------------		
				select count("AcctCode") into ifcond61 from OACT
					where "FatherNum" = :acct5 and "Levels" = 6 and
							 "Groups" = :ErrNum;
				select count("Postable") into ifcond62 from OACT 
				where "AcctCode" = :acct5 and "Postable" = 'N'; 	
						 
				WHILE :ifcond61 > 0 and :ifcond62 > 0
				-------------------Level 6--------------------------------------    
				do
					select Top 1 "AcctCode" into acct6
						   from OACT
						   where "FatherNum" = :acct5 
						   and "Levels" = 6 
						   and "Groups" = :ErrNum 
						   and "GrpLine" = (select MIN("GrpLine") from OACT
											where "FatherNum" = :acct5 and
											"Levels" = 6 and "Groups" = :ErrNum);
					update OACT
					set "Groups" = :linetmp,
						"GroupMask" = :counter
					where "AcctCode" = :acct6;
					linetmp := :linetmp + 1;
					
				-------------------For Level 7 while Condition --------------------------------------		
					select count("AcctCode") into ifcond71 from OACT
						where "FatherNum" = :acct6 and "Levels" = 7 and
								 "Groups" = :ErrNum;	
					select count("Postable") into ifcond72 from OACT 
						where "AcctCode" = :acct6 and  "Postable"= 'N'; 		
				-------------------Level 7--------------------------------------  
					WHILE :ifcond71 > 0 and :ifcond72 > 0
					do
						select Top 1 "AcctCode" into acct7
							   from OACT
							   where "FatherNum" = :acct6 and 
							   "Levels" = 7 and 
							   "Groups" = :ErrNum and
								"GrpLine" = (select MIN("GrpLine") from OACT
												where "FatherNum" = :acct6 and
												"Levels" = 7 and "Groups" = :ErrNum);
						update OACT
						set "Groups" = :linetmp,
							"GroupMask" = :counter
						where "AcctCode" = :acct7;
						
						linetmp := :linetmp + 1;
					-------------------For Level 8 while Condition --------------------------------------		
					select count("AcctCode") into ifcond81 from OACT
						where "FatherNum" = :acct7 and "Levels" = 8 and
							"Groups" = :ErrNum;
									 
					select count("Postable") into ifcond82 from OACT 
						where "AcctCode" = :acct7 and "Postable" = 'N'; 
					
					-------------------Level 8--------------------------------------    
					WHILE :ifcond81 > 0 and :ifcond82 > 0
					do
							select Top 1 "AcctCode" into acct8
								   from OACT
							where "FatherNum" = :acct7 and "Levels" = 8 
							and "Groups" = :ErrNum and
										 "GrpLine" = (select MIN ("GrpLine") from OACT
															where "FatherNum" = :acct7 and
													 "Levels" = 8 and "Groups" = :ErrNum);
							update OACT
							set "Groups" = :linetmp,
								"GroupMask" = :counter
							where "AcctCode" = :acct8;
							linetmp := :linetmp + 1;
							
					-------------------For Level 9 while Condition --------------------------------------						
							select count("AcctCode") into ifcond91 from OACT
								where "FatherNum" = :acct8 and "Levels" = 9 and
										 "Groups" = :ErrNum;
							select count("Postable") into ifcond92 from OACT 
							where "AcctCode" = :acct8 and "Postable" = 'N';
							-------------------Level 9--------------------------------------   
							WHILE :ifcond91 > 0 and :ifcond92 > 0
							do
								select Top 1 "AcctCode" into acct9
									   from OACT
									   where "FatherNum" = :acct8 and "Levels" = 9 and "Groups" = :ErrNum and
											 "GrpLine" = (select MIN ("GrpLine") from OACT
														where "FatherNum" = :acct8 and
														"Levels" = 9 and "Groups" = :ErrNum);
								update OACT
								set "Groups" = :linetmp,
									"GroupMask" = :counter
								where "AcctCode" = :acct9;
								linetmp := :linetmp + 1;
								
								-------------------For Level 10 while Condition --------------------------------------						
								select count("AcctCode") into ifcond101 from OACT
										where "FatherNum" = :acct9 and "Levels" =10 and
											 "Groups" = :ErrNum;
											 
								select count("Postable") into ifcond102 from OACT 
									where "AcctCode" = :acct9 and "Postable" = 'N';
								-------------------Level 10--------------------------------------    	
								WHILE :ifcond101 > 0 and :ifcond102 > 0
								do 
									select Top 1 "AcctCode" into acct10
										   from OACT
									 where "FatherNum" = :acct9 and 
										   "Levels" = 10 and "Groups" = :ErrNum 
										   and	 "GrpLine" = (select MIN ("GrpLine") from OACT
																where "FatherNum" = :acct9 and
															  "Levels" = 10 and "Groups" = :ErrNum);
									update OACT
									set "Groups" = :linetmp,
										"GroupMask" = :counter
									where "AcctCode" = :acct10;
									linetmp := :linetmp + 1;							
									-------------------For Level 10 while Condition --------------------------------------						
									select count("AcctCode") into ifcond101 from OACT
										where "FatherNum" = :acct9 and "Levels" =10 and
												 "Groups" = :ErrNum;
												 
									select count("Postable") into ifcond102 from OACT 
										where "AcctCode" = :acct9 and "Postable" = 'N';
								end while;
							-------------------For Level 9 while Condition --------------------------------------						
								select count("AcctCode") into ifcond91 from OACT
									where "FatherNum" = :acct8 and "Levels" = 9 and
											 "Groups" = :ErrNum;
								select count("Postable") into ifcond92  from OACT 
								where "AcctCode" = :acct8 and "Postable" = 'N';
							end while;

			-------------------For Level 8 while Condition --------------------------------------					

						select count("AcctCode") into ifcond81 from OACT
							where "FatherNum" = :acct7 and "Levels" = 8 and
							"Groups" = :ErrNum;								
						select count("Postable") into ifcond82 from OACT
							where "AcctCode" = :acct7 and "Postable" = 'N';
					end while;
			-------------------For Level 7 while Condition --------------------------------------	

						select count("AcctCode") into ifcond71 from OACT
							where "FatherNum" = :acct6 and "Levels" = 7 and
									 "Groups" = :ErrNum;
									 
						select "Postable" into ifcond72 from OACT
							where "AcctCode" = :acct6 and  "Postable"= 'N'; 	
					end while;
				-------------------For Level 6 while Condition --------------------------------------	

					select count("AcctCode") into ifcond61 from OACT
						where "FatherNum" = :acct5 and "Levels" = 6 and
								 "Groups" = :ErrNum;

					select count("Postable") into ifcond62 from OACT
						where "AcctCode" = :acct5 and "Postable" = 'N';
				end while;
			-------------------For Level 5 while Condition --------------------------------------	

					select count("AcctCode") into ifcond51
						from OACT
					where "FatherNum" = :acct4 and "Levels" = 5 and
								 "Groups" = :ErrNum;
								 
					select count("Postable") into ifcond52 from OACT
						where "AcctCode" = :acct4 and "Postable" = 'N';
				end while;

				-------------------For Level 4 while Condition -------------------------------------

					select count("AcctCode") into ifcond41
						 from OACT
					where "FatherNum" = :acct3 and "Levels" = 4 and
							 "Groups" = :ErrNum;
					select count("Postable") into ifcond42 from OACT
						where "AcctCode" = :acct3 and "Postable" = 'N';
			end while;
		-------------------For Level 3 while condition-----------------------------------

				select count("AcctCode") into ifcond31
		 		  	from OACT
				  where "FatherNum" = :acct2 and "Levels" = 3 and
				                "Groups" = :ErrNum;
				select count("Postable") into ifcond32
				  	from OACT
				  where "AcctCode" = :acct2 and
					 "Postable" = 'N'; 
		  end while;
 -------------------For Level 2 while condition-----------------------------------

      select count("AcctCode") into ifcond2 from OACT
          where "FatherNum" = :acct1 and "Levels" = :level and     
			 "Groups" = :ErrNum;
    end while;
	
	lineout := :linetmp;

	return;
end;


