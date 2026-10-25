
insert into oprm(USER_CODE,"menucode","authoriza")
select USER_CODE,'frm_DownPaymentInvoice',a."authoriza"
from oprm a
where a."menucode"='frm_DownPaymentRequest' and a."authoriza"<>'N'
and not exists(select 1 from oprm where USER_CODE=a.USER_CODE and "menucode"='frm_DownPaymentInvoice');

GO

insert into oprm(USER_CODE,"menucode","authoriza")
select USER_CODE,'frm_PurchaseDownPaymentInvoice',a."authoriza"
from oprm a
where a."menucode"='frm_PurchaseDownPaymentRequest' and a."authoriza"<>'N'
and not exists(select 1 from oprm where USER_CODE=a.USER_CODE and "menucode"='frm_PurchaseDownPaymentInvoice');