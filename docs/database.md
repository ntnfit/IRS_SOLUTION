# Database

## Nguồn và công cụ

- Catalog HANA gốc: [`server/db-scripts/hana-export`](../server/db-scripts/hana-export/).
- Migration HANA đã giải mã: [`server/db-scripts/migrations`](../server/db-scripts/migrations/).
- SQL Server waves: [`server/db-scripts/sqlserver`](../server/db-scripts/sqlserver/).
- Contract procedure từ C#: [`server/db-scripts/contracts`](../server/db-scripts/contracts/).
- Công cụ chuyển đổi/kiểm tra: [`server/tools`](../server/tools/).

Script SQL Server đang dùng tên company database dev làm mặc định. Trước khi áp dụng lên database khác,
review câu `USE`, chạy trong môi trường thử nghiệm và dùng cơ chế backup của đơn vị vận hành.

## Dev registry

[`server/tools/dev-db/setup-dev-db.ps1`](../server/tools/dev-db/setup-dev-db.ps1) tạo registry `IRS-COMMON`,
login dev ngẫu nhiên và đăng ký company database. Script chỉ in password sinh ngẫu nhiên trong lần chạy đó;
không commit output hay `.dev-site/Web.config`.

## Kiểm tra readiness

Dùng [`server/tools/check-sqlserver-readiness.sql`](../server/tools/check-sqlserver-readiness.sql)
và manifest của từng wave để so sánh object cần thiết. Các smoke test nằm cạnh wave sở hữu contract tương ứng.
