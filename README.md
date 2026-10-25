# IRS Solution

Mã nguồn IRS 2.10.776 được phục hồi thành một repository gồm ba phần rõ ràng:

- [`client/`](client/) — ứng dụng WinForms `Apzon.Forms`.
- [`server/`](server/) — ASP.NET Web API, công cụ cấu hình và script cơ sở dữ liệu.
- [`license/`](license/) — tài liệu vận hành license và script lưu license SQL Server.

Đây là source phục hồi từ build hợp pháp của hệ thống. Không commit credential, license đã cấp,
database backup hoặc DLL thương mại. DevExpress, SAP DI API và Crystal Reports phải được cung cấp
từ bộ cài có license của đơn vị sử dụng.

## Bắt đầu

1. Đọc [hướng dẫn thiết lập](docs/setup.md).
2. Dựng database theo [hướng dẫn database](docs/database.md).
3. Cấu hình/chạy API, sau đó build và chạy client.
4. Khi cần QC license, làm theo [hướng dẫn license](license/README.md).

Toàn bộ tài liệu được điều hướng tại [`docs/README.md`](docs/README.md).

## Trạng thái xác minh

- Client và server build trên .NET Framework 4.6.2 với `0 error`.
- API dev trả HTTP 200 tại port cấu hình sau khi warm-up.
- Các hợp đồng procedure/object trực tiếp của DMS và POS đã được đối chiếu với SQL Server.
- Crystal Report thay thế cho POS đã render thành PDF từ dữ liệu SQL Server thật trong môi trường dev.

Xem giới hạn phục hồi và cách xử lý lỗi tại [troubleshooting](docs/troubleshooting.md).
