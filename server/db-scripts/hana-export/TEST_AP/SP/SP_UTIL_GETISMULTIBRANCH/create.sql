
CREATE procedure SP_UTIL_GetIsMultiBranch
as
BEGIN
	Select top 1 "MltpBrnchs" From OADM;
END



