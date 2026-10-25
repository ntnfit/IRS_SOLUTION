# Kiến trúc

Luồng runtime là `WinForms client -> ASP.NET Web API -> SAP B1 company database`.
Client và API phải cùng phiên bản `2.10.776`; broker đăng nhập sở hữu kiểm tra tương thích phiên bản.

## Ranh giới mã nguồn

- Client: [`client/ApzonIRS.sln`](../client/ApzonIRS.sln), entry project tại
  [`client/src/Apzon.Forms`](../client/src/Apzon.Forms/).
- API: [`server/ApiIRS.sln`](../server/ApiIRS.sln), controller tại
  [`server/src/Apzon.Api`](../server/src/Apzon.Api/) và repository tại
  [`server/src/Apzon.Services`](../server/src/Apzon.Services/).
- Database: [`server/db-scripts`](../server/db-scripts/) chứa HANA catalog phục hồi,
  migration và các wave SQL Server.
- License: UI import nằm trong client; controller/repository verify và lưu license nằm trong server;
  [`license/`](../license/) là route vận hành tập trung.

SQL Server là backend dev được hỗ trợ bởi các script phục hồi. HANA catalog được giữ làm bằng chứng
để tiếp tục port, không phải tập lệnh SQL Server có thể chạy trực tiếp.
