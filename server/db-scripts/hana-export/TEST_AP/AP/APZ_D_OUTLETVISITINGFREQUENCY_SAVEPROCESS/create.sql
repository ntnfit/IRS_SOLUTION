

CREATE PROCEDURE Apz_D_OutletVisitingFrequency_SaveProcess
AS
BEGIN

	create local temporary table #APZ_D_OOVFLocal as (select top 0 * from APZ_D_OOVF);
	exec 'drop table #APZ_D_OOVFLocal';
	exec 'create local temporary table #APZ_D_OOVFLocal as (select * from "#APZ_D_OOVF")';

	if (select count(1) from #APZ_D_OOVFLocal) <= 0
	then
		select 404, ' Temp data is not exist' from dummy;
		return;
	end if;
	
	MERGE INTO APZ_D_OOVF AS T
    USING #APZ_D_OOVFLocal AS S
    ON T."Code" = S."Code"

	 WHEN MATCHED THEN update set 
	 T."Name" = S."Name",
	T."Address" = S."Address", 
      T."Description" = S."Description",
      T."StartDate" = S."StartDate",
      T."Finished" =  S."Finished",
      T."Recurring" = S."Recurring",
      T."RecurNum" = S."RecurNum",
      T."RecurNote" = S."RecurNote",
      T."EveryDay" = S."EveryDay",
      T."DayOfWeek" = S."DayOfWeek",
      T."Mon" = S."Mon",
      T."Tue" = S."Tue",
      T."Wed" = S."Wed",
	  T."Thu" = S."Thu",
      T."Fri" = S."Fri",
      T."Sat" = S."Sat",
      T."Sun" = S."Sun",
      T."DayOfMonth" = S."DayOfMonth",
      T."ListOfDate" = S."ListOfDate",
	    T."UserSign"=S."UserSign",
	  T."UpdateDate"=S."UpdateDate",
	  T."LogInstanc"=S."LogInstanc",
	  T."NumberOfVisits"=S."NumberOfVisits"
	
 WHEN NOT MATCHED THEN  insert 
	values
	(
		S."Code"
      , S."Name"
      , S."Address" 
      , S."Description"
      , S."StartDate"
      , S."Finished"
      , S."Recurring"
      , S."RecurNum"
      , S."RecurNote"
      , S."EveryDay"
      , S."DayOfWeek"
      , S."Mon"
      , S."Tue"
      , S."Wed"
      , S."Thu"
      , S."Fri"
      , S."Sat"
      , S."Sun"
      , S."DayOfMonth"
      , S."ListOfDate",
	   S."UserSign",
	  S."UpdateDate",
	  S."LogInstanc",
	  S."NumberOfVisits"
	);
	
	--select 0, '' from dummy;
	
END

