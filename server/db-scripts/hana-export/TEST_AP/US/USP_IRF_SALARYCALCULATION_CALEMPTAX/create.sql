CREATE procedure USP_IRF_SalaryCalculation_CalEmpTax (
	in TaxableAmt			numeric(19,6)
)
as
begin

select  case when  TaxableAmt  >=0  and    TaxableAmt  <= 5000000 then 5
			 when  TaxableAmt  >=5000001  and    TaxableAmt  <= 10000000 then 10
			 when  TaxableAmt  >=10000001  and    TaxableAmt  <= 18000000 then 15
			 
			 when  TaxableAmt  >=18000001  and    TaxableAmt  <= 32000000 then 20
			 
			 when  TaxableAmt  >=32000001  and    TaxableAmt  <= 52000000 then 25
			 
			 when  TaxableAmt  >=52000001  and    TaxableAmt  <= 80000000 then 30
			 
			 when  TaxableAmt  >= 80000001 then 35 end

from dummy;

end;

