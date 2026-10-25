-- B1 DEPENDS: AFTER:PT:PROCESS_END

CREATE procedure TmSp_InsertIntoUITMandUITW

LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER
as
 str nvarchar (2048);
begin
	str :='Please use TmSp_InsertIntoTargetITMandTargetITW instead';
	select str from dummy;
----------------------------------------------------------------------
----------------------------------------------------------------------
----------------------------------------------------------------------
-- FOR MERGE, please merge the changes into TmSp_InsertIntoTargetITMandTargetITW.txt !!!!
----------------------------------------------------------------------
-- this SP is not used in 881 anymore,
-- TmSp_InsertIntoTargetITMandTargetITW.txt is used instead
----------------------------------------------------------------------
----------------------------------------------------------------------
----------------------------------------------------------------------

end;


