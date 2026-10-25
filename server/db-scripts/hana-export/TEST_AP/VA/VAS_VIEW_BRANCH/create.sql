CREATE VIEW "TEST_AP"."VAS_VIEW_BRANCH" ( "BPLId", "BPLName" ) AS ((select
	 a."BPLId",
	 a."BPLName" 
		from "OBPL" a 
		where a."Disabled" = 'N') UNION ALL (select
	 0 ,
	 'All branch' 
		from "OADM" 
		where (select
	 count(1) 
			from "OBPL")=0)) WITH READ ONLY