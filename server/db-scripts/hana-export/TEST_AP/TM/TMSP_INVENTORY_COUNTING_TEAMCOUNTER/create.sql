Create PROCEDURE TMSP_Inventory_Counting_TeamCounter(
in DocKey  nvarchar(12) ,
in ObjectId NCHAR(20)
)
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER
AS
BEGIN
IF (:ObjectId = '1470000065') then
		select "OINC"."DocEntry", "OINC"."DocNum", "OINC"."CountDate", "OINC"."Time", "OINC"."CountType", "OINC"."TeamCount",
	   "INC1"."ItemCode", "INC1"."ItemDesc", "INC1"."WhsCode", "INC1"."InWhsQty", "INC1"."CountQty", "INC1"."BinEntry", "INC1"."Counted", OBIN."BinCode",
	   "INC41"."CounteName" as "TeamCounter1Name", "INC42"."CounteName" as "TeamCounter2Name", INC43."CounteName" as "TeamCounter3Name", INC44."CounteName" as "TeamCounter4Name", INC45."CounteName" as "TeamCounter5Name",
	   INC46."CounteName" as "TeamCounter6Name", INC47."CounteName" as "TeamCounter7Name", INC48."CounteName" as "TeamCounter8Name", INC49."CounteName" as "TeamCounter9Name", INC410."CounteName" as "TeamCounter10Name",
	   INC51."TotalQty" as "TeamCounter1Qty", INC52."TotalQty" as "TeamCounter2Qty", INC53."TotalQty" as "TeamCounter3Qty", INC54."TotalQty" as "TeamCounter4Qty", INC55."TotalQty" as "TeamCounter5Qty",
	   INC56."TotalQty" as "TeamCounter6Qty", INC57."TotalQty" as "TeamCounter7Qty", INC58."TotalQty" as "TeamCounter8Qty", INC59."TotalQty" as "TeamCounter9Qty", INC510."TotalQty" as "TeamCounter10Qty" from "INC1"
				   left join "OINC" on "OINC"."DocEntry" = "INC1"."DocEntry"
				   left join OBIN on "INC1"."BinEntry" = "OBIN"."AbsEntry"
				   left join "INC4" as "INC41" on ("INC1"."DocEntry" = "INC41"."DocEntry" and "INC41"."VisOrder" = 1)
				   left join "INC4" as "INC42" on ("INC1"."DocEntry" = "INC42"."DocEntry" and "INC42"."VisOrder" = 2)
				   left join "INC4" as "INC43" on ("INC1"."DocEntry" = "INC43"."DocEntry" and "INC43"."VisOrder" = 3)
				   left join "INC4" as "INC44" on ("INC1"."DocEntry" = "INC44"."DocEntry" and "INC44"."VisOrder" = 4)
				   left join "INC4" as "INC45" on ("INC1"."DocEntry" = "INC45"."DocEntry" and "INC45"."VisOrder" = 5)
				   left join "INC4" as "INC46" on ("INC1"."DocEntry" = "INC46"."DocEntry" and "INC46"."VisOrder" = 6)
				   left join "INC4" as "INC47" on ("INC1"."DocEntry" = "INC47"."DocEntry" and "INC47"."VisOrder" = 7)
				   left join "INC4" as "INC48" on ("INC1"."DocEntry" = "INC48"."DocEntry" and "INC48"."VisOrder" = 8)
				   left join "INC4" as "INC49" on ("INC1"."DocEntry" = "INC49"."DocEntry" and "INC49"."VisOrder" = 9)
				   left join "INC4" as "INC410" on ("INC1"."DocEntry" = "INC410"."DocEntry" and "INC410"."VisOrder" = 10)	
				   			   
				   left join "INC5" as "INC51" on ("INC1"."DocEntry" = "INC51"."DocEntry" and "INC1"."LineNum" = "INC51"."LineNum" and "INC41"."CounterNum" = "INC51"."CounterNum" and "INC41"."VisOrder" = 1)
				   left join "INC5" as "INC52" on ("INC1"."DocEntry" = "INC52"."DocEntry" and "INC1"."LineNum" = "INC52"."LineNum" and "INC42"."CounterNum" = "INC52"."CounterNum" and "INC42"."VisOrder" = 2)
				   left join "INC5" as "INC53" on ("INC1"."DocEntry" = "INC53"."DocEntry" and "INC1"."LineNum" = "INC53"."LineNum" and "INC43"."CounterNum" = "INC53"."CounterNum" and "INC43"."VisOrder" = 3)
				   left join "INC5" as "INC54" on ("INC1"."DocEntry" = "INC54"."DocEntry" and "INC1"."LineNum" = "INC54"."LineNum" and "INC44"."CounterNum" = "INC54"."CounterNum" and "INC44"."VisOrder" = 4)			
				   left join "INC5" as "INC55" on ("INC1"."DocEntry" = "INC55"."DocEntry" and "INC1"."LineNum" = "INC55"."LineNum" and "INC45"."CounterNum" = "INC55"."CounterNum" and "INC45"."VisOrder" = 5)
				   left join "INC5" as "INC56" on ("INC1"."DocEntry" = "INC56"."DocEntry" and "INC1"."LineNum" = "INC56"."LineNum" and "INC46"."CounterNum" = "INC56"."CounterNum" and "INC46"."VisOrder" = 6)
				   left join "INC5" as "INC57" on ("INC1"."DocEntry" = "INC57"."DocEntry" and "INC1"."LineNum" = "INC57"."LineNum" and "INC47"."CounterNum" = "INC57"."CounterNum" and "INC47"."VisOrder" = 7)
				   left join "INC5" as "INC58" on ("INC1"."DocEntry" = "INC58"."DocEntry" and "INC1"."LineNum" = "INC58"."LineNum" and "INC48"."CounterNum" = "INC58"."CounterNum" and "INC48"."VisOrder" = 8)
				   left join "INC5" as "INC59" on ("INC1"."DocEntry" = "INC59"."DocEntry" and "INC1"."LineNum" = "INC59"."LineNum" and "INC49"."CounterNum" = "INC59"."CounterNum" and "INC49"."VisOrder" = 9)			
				   left join "INC5" as "INC510" on ("INC1"."DocEntry" = "INC510"."DocEntry" and "INC1"."LineNum" = "INC510"."LineNum" and "INC410"."CounterNum" = "INC510"."CounterNum" and "INC410"."VisOrder" = 10)
		where "OINC"."DocEntry" = :DocKey order by "INC1"."VisOrder";
Else
		select "OICD"."DocEntry", "OICD"."DocNum", "OICD"."CountDate", "OICD"."Time", "OICD"."CountType", "OICD"."TeamCount",
	   "ICD1"."ItemCode", "ICD1"."ItemDesc", "ICD1"."WhsCode", "ICD1"."InWhsQty", "ICD1"."CountQty", "ICD1"."BinEntry", "ICD1"."Counted", OBIN."BinCode",
	   "ICD41"."CounteName" as "TeamCounter1Name", "ICD42"."CounteName" as "TeamCounter2Name", ICD43."CounteName" as "TeamCounter3Name", ICD44."CounteName" as "TeamCounter4Name", ICD45."CounteName" as "TeamCounter5Name",
	   ICD46."CounteName" as "TeamCounter6Name", ICD47."CounteName" as "TeamCounter7Name", ICD48."CounteName" as "TeamCounter8Name", ICD49."CounteName" as "TeamCounter9Name", ICD410."CounteName" as "TeamCounter10Name",
	   ICD51."TotalQty" as "TeamCounter1Qty", ICD52."TotalQty" as "TeamCounter2Qty", ICD53."TotalQty" as "TeamCounter3Qty", ICD54."TotalQty" as "TeamCounter4Qty", ICD55."TotalQty" as "TeamCounter5Qty",
	   ICD56."TotalQty" as "TeamCounter6Qty", ICD57."TotalQty" as "TeamCounter7Qty", ICD58."TotalQty" as "TeamCounter8Qty", ICD59."TotalQty" as "TeamCounter9Qty", ICD510."TotalQty" as "TeamCounter10Qty" from "ICD1"
				   left join "OICD" on "OICD"."DocEntry" = "ICD1"."DocEntry"
				   left join OBIN on "ICD1"."BinEntry" = "OBIN"."AbsEntry"
				   left join "ICD4" as "ICD41" on ("ICD1"."DocEntry" = "ICD41"."DocEntry" and "ICD41"."VisOrder" = 1)
				   left join "ICD4" as "ICD42" on ("ICD1"."DocEntry" = "ICD42"."DocEntry" and "ICD42"."VisOrder" = 2)
				   left join "ICD4" as "ICD43" on ("ICD1"."DocEntry" = "ICD43"."DocEntry" and "ICD43"."VisOrder" = 3)
				   left join "ICD4" as "ICD44" on ("ICD1"."DocEntry" = "ICD44"."DocEntry" and "ICD44"."VisOrder" = 4)
				   left join "ICD4" as "ICD45" on ("ICD1"."DocEntry" = "ICD45"."DocEntry" and "ICD45"."VisOrder" = 5)
				   left join "ICD4" as "ICD46" on ("ICD1"."DocEntry" = "ICD46"."DocEntry" and "ICD46"."VisOrder" = 6)
				   left join "ICD4" as "ICD47" on ("ICD1"."DocEntry" = "ICD47"."DocEntry" and "ICD47"."VisOrder" = 7)
				   left join "ICD4" as "ICD48" on ("ICD1"."DocEntry" = "ICD48"."DocEntry" and "ICD48"."VisOrder" = 8)
				   left join "ICD4" as "ICD49" on ("ICD1"."DocEntry" = "ICD49"."DocEntry" and "ICD49"."VisOrder" = 9)
				   left join "ICD4" as "ICD410" on ("ICD1"."DocEntry" = "ICD410"."DocEntry" and "ICD410"."VisOrder" = 10)	
				   			   
				   left join "ICD5" as "ICD51" on ("ICD1"."DocEntry" = "ICD51"."DocEntry" and "ICD1"."LineNum" = "ICD51"."LineNum" and "ICD41"."CounterNum" = "ICD51"."CounterNum" and "ICD41"."VisOrder" = 1)
				   left join "ICD5" as "ICD52" on ("ICD1"."DocEntry" = "ICD52"."DocEntry" and "ICD1"."LineNum" = "ICD52"."LineNum" and "ICD42"."CounterNum" = "ICD52"."CounterNum" and "ICD42"."VisOrder" = 2)
				   left join "ICD5" as "ICD53" on ("ICD1"."DocEntry" = "ICD53"."DocEntry" and "ICD1"."LineNum" = "ICD53"."LineNum" and "ICD43"."CounterNum" = "ICD53"."CounterNum" and "ICD43"."VisOrder" = 3)
				   left join "ICD5" as "ICD54" on ("ICD1"."DocEntry" = "ICD54"."DocEntry" and "ICD1"."LineNum" = "ICD54"."LineNum" and "ICD44"."CounterNum" = "ICD54"."CounterNum" and "ICD44"."VisOrder" = 4)			
				   left join "ICD5" as "ICD55" on ("ICD1"."DocEntry" = "ICD55"."DocEntry" and "ICD1"."LineNum" = "ICD55"."LineNum" and "ICD45"."CounterNum" = "ICD55"."CounterNum" and "ICD45"."VisOrder" = 5)
				   left join "ICD5" as "ICD56" on ("ICD1"."DocEntry" = "ICD56"."DocEntry" and "ICD1"."LineNum" = "ICD56"."LineNum" and "ICD46"."CounterNum" = "ICD56"."CounterNum" and "ICD46"."VisOrder" = 6)
				   left join "ICD5" as "ICD57" on ("ICD1"."DocEntry" = "ICD57"."DocEntry" and "ICD1"."LineNum" = "ICD57"."LineNum" and "ICD47"."CounterNum" = "ICD57"."CounterNum" and "ICD47"."VisOrder" = 7)
				   left join "ICD5" as "ICD58" on ("ICD1"."DocEntry" = "ICD58"."DocEntry" and "ICD1"."LineNum" = "ICD58"."LineNum" and "ICD48"."CounterNum" = "ICD58"."CounterNum" and "ICD48"."VisOrder" = 8)
				   left join "ICD5" as "ICD59" on ("ICD1"."DocEntry" = "ICD59"."DocEntry" and "ICD1"."LineNum" = "ICD59"."LineNum" and "ICD49"."CounterNum" = "ICD59"."CounterNum" and "ICD49"."VisOrder" = 9)			
				   left join "ICD5" as "ICD510" on ("ICD1"."DocEntry" = "ICD510"."DocEntry" and "ICD1"."LineNum" = "ICD510"."LineNum" and "ICD410"."CounterNum" = "ICD510"."CounterNum" and "ICD410"."VisOrder" = 10)
		where "OICD"."DocEntry" = :DocKey order by "ICD1"."VisOrder";
END if;

End;


