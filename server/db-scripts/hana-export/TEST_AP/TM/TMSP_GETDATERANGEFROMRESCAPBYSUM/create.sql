CREATE PROCEDURE TmSp_GetDateRangeFromResCapBySum (
	IN ResCode NVARCHAR (50),
	IN WhsCode NVARCHAR (8),
	IN CalcFromDate TIMESTAMP,
	IN UntilDate TIMESTAMP,
	IN CapacitySum DECIMAL (21,6),
	IN AvailableProportionOfFirstDay DECIMAL (21,6), --in % (range 0..100)
	IN ResAllocType NVARCHAR,
		-- OnStartDate		= "S",
		-- OnEndDate		= "D",
		-- StartDateForwards= "F",
		-- EndDateBackwards	= "B"		
	IN WORAbsEntry INT,
	IN WORLine INT
	)

		
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER	
AS
	freeCapacity DECIMAL (21,6);
	returnDate TIMESTAMP;	
	freeCapacityPartialSum DECIMAL (21,6);
	dayProportion DECIMAL (21,6);
	remainingDayProportion DECIMAL (21,6);	

	CURSOR capacityCursorF FOR   
			SELECT SUM (CASE WHEN "CapType" = 'I' THEN "SngRunCap" ELSE "Capacity" END) As "FreeCapacity", "CapDate"
			FROM ORCJ 
			WHERE	"ResCode" = :ResCode
				And "WhsCode" = :WhsCode				
				And (("CapType" = 'I' AND "SngRunCap" <> 0) 
					OR 
					("CapType" = 'U' AND "BaseObjTyp" = 202 And "BaseAbsEnt" = :WORAbsEntry And "BaseLine" = :WORLine))				
				And "CapDate" >= :CalcFromDate
			GROUP BY "CapDate"
			ORDER BY "CapDate";
				
	CURSOR capacityCursorB FOR   
			SELECT SUM (CASE WHEN "CapType" = 'I' THEN "SngRunCap" ELSE "Capacity" END) As "FreeCapacity", "CapDate"
			FROM ORCJ  
			WHERE	"ResCode" = :ResCode
				And "WhsCode" = :WhsCode
				And (("CapType" = 'I' AND "SngRunCap" <> 0) 
					OR 
					("CapType" = 'U' AND "BaseObjTyp" = 202 And "BaseAbsEnt" = :WORAbsEntry And "BaseLine" = :WORLine))
				And "CapDate" <= :CalcFromDate
				And "CapDate" >= :UntilDate
			GROUP BY "CapDate"
			ORDER BY "CapDate" DESC;

BEGIN

	freeCapacityPartialSum := 0;
	returnDate := :CalcFromDate;
	dayProportion := :AvailableProportionOfFirstDay;
	remainingDayProportion := 0;
	
	IF ResAllocType = 'F' THEN
	
		FOR cur_row AS capacityCursorF DO			
			freeCapacity := cur_row."FreeCapacity";
			returnDate := cur_row."CapDate";
			
			-- this can be true only in first itteration when no capcacity is defined for "CalcFromDate"
			IF :dayProportion < 100 AND :returnDate <> :CalcFromDate THEN
				dayProportion := 100;
			END IF;
			
			freeCapacityPartialSum := :freeCapacityPartialSum + (:freeCapacity * :dayProportion / 100);	--we first multiplicate then divide -> to keep precision of 6 decimal point
			dayProportion := 100;	--100%
			IF :freeCapacityPartialSum >= :CapacitySum THEN		
				remainingDayProportion := ((:freeCapacityPartialSum - :CapacitySum) / :freeCapacity) * 100;
				BREAK;
			END IF;		
		END FOR;
	
	ELSEIF ResAllocType = 'B' THEN
	
		FOR cur_row AS capacityCursorB DO			
			freeCapacity := cur_row."FreeCapacity";
			returnDate := cur_row."CapDate";
			
			-- this can be true only in first itteration when no capcacity is defined for "CalcFromDate"
			IF :dayProportion < 100 AND :returnDate <> :CalcFromDate THEN
				dayProportion := 100;
			END IF;
			
			freeCapacityPartialSum := :freeCapacityPartialSum + (:freeCapacity * :dayProportion / 100);	--we first multiplicate then divide -> to keep precision of 6 decimal point
			dayProportion := 100;	--100%
			IF :freeCapacityPartialSum >= :CapacitySum THEN		
				remainingDayProportion := ((:freeCapacityPartialSum - :CapacitySum) / :freeCapacity) * 100;
				BREAK;
			END IF;		
		END FOR;
	
	END IF;	

	SELECT :returnDate, :remainingDayProportion from DUMMY;

END;


