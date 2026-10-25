-- B1 DEPENDS: BEFORE:PT:PROCESS_START
create procedure _TmSp_ValidateSpParam(IN passin_parameter_in nvarchar(5000))
LANGUAGE SQLSCRIPT
SQL SECURITY INVOKER
READS SQL DATA
as
	paralist nvarchar(5000);
	--paralistUpper nvarchar(5000);
	first_quote_position int;
	second_quote_position int;
	string_of_condition nvarchar(256);
	previous_string_of_condition nvarchar(256);
	string_of_parameter nvarchar(256);
	previous_string_of_parameter nvarchar(256);
	paralist_after nvarchar(100);
	length_of_paralist int;
	vi int;
	vCout int := 0;
	debugM int := 0; --Debug witch 1-enable
    Timeout_behavior int := 0; --0 return, 1 exception
    startpoint timestamp := current_timestamp;
begin
    DECLARE arraySplict NVARCHAR(20) ARRAY = ARRAY('''', '"');
    DECLARE VaildFailed CONDITION FOR SQL_ERROR_CODE 10000;
    DECLARE VaildException CONDITION FOR SQL_ERROR_CODE 10001;
    
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    begin
        if debugM = 0 then --debug switch
	        if ::SQL_ERROR_CODE = 10000 then
	            SIGNAL VaildFailed SET MESSAGE_TEXT = ::SQL_ERROR_MESSAGE;    
	        else  
	            SIGNAL VaildException SET MESSAGE_TEXT = ::SQL_ERROR_MESSAGE;    
	        end if;
        end if;
    end;
    
    --check if feature has enabled.
    if session_context('ValidateSpParam') <> 'ENABLE' or session_context('ValidateSpParam') is null then
        return;
    end if;
    
    --Setting on Exception of timeout
    if session_context('ValidateTimeout') = 'Exception' then 
        Timeout_behavior := 1;      
    end if;
    
    begin
        DECLARE VaildFailed CONDITION FOR SQL_ERROR_CODE 10000;
         
        paralist := passin_parameter_in;
        if locate(replace(paralist, ' ', ''), 'or1=1') + locate(paralist, ' --') + locate(paralist, '/*') > 0 then
            SIGNAL VaildFailed SET MESSAGE_TEXT ='RiskyBehaviorsDetected:03';
        end if;
        
        for vi in 1.. CARDINALITY(:arraySplict)
        do  
            paralist := passin_parameter_in;
            --paralistUpper := upper(paralist);

	        first_quote_position := locate(paralist, :arraySplict[vi], 0, 1);
	    
		    while :first_quote_position > 0
		    do
			    vCout := vCout +1;
	            if vCout > 5000 then
	                  SIGNAL VaildFailed SET MESSAGE_TEXT ='ValidateStringTooLong';
	            end if;
	            
                --timeout setting 0.5s
		        if NANO100_BETWEEN(startpoint, current_timestamp) > 5000000  then 
		            if Timeout_behavior = 0 then
		                  return;
		            else 
		                  SIGNAL VaildException SET MESSAGE_TEXT ='ValidateTimeout';
		            end if;
		        end if;
            
		        length_of_paralist := length(paralist);
		        second_quote_position := locate(paralist, :arraySplict[vi], 0, 2); 
		        
		        string_of_condition := substring(paralist, 0, first_quote_position - 1);
		        paralist_after := substring(paralist, second_quote_position+1, length_of_paralist - second_quote_position);
		        string_of_parameter := substring(paralist, first_quote_position+1, second_quote_position- first_quote_position-1);
		        
                if locate(string_of_condition, ';') > 0 then
					SIGNAL VaildFailed SET MESSAGE_TEXT ='RiskyBehaviorsDetected:00';
				end if;	           
		        
		        if (locate(previous_string_of_condition, ' or ') > 0 and 
						locate(trim(string_of_condition), '=') > 0  and
						trim(previous_string_of_parameter) = trim(string_of_parameter) and
				        (  :arraySplict[vi] != '"' or 
				           (:arraySplict[vi] = '"' and locate(string_of_condition, replace(previous_string_of_condition, ' or ', '')) > 0)
						))
				then
		            SIGNAL VaildFailed SET MESSAGE_TEXT ='RiskyBehaviorsDetected:01';
		        end if;
		        
                paralist := paralist_after;
		        first_quote_position :=  locate(paralist, :arraySplict[vi], 0, 1);
		        previous_string_of_condition := string_of_condition;
		        previous_string_of_parameter := string_of_parameter;
		          
		    end while;
        end for;
    end;    
end;


