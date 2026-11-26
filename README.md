# Hệ Thống Xử Lý Đơn Hàng Phân Tán (OrderSystem)

Dự án bài tập lớn môn Phát triển ứng dụng phân tán.
Xây dựng hệ thống xử lý đơn hàng theo kiến trúc Microservices sử dụng C# (.NET 10).

## 🏗️ Kiến trúc hệ thống (Mục tiêu)
`Client -> OrderApi -> RabbitMQ -> OrderWorker -> RabbitMQ -> NotificationService -> Client`

## 🛠️ Công nghệ sử dụng
* **Ngôn ngữ:** C# (.NET 10)
* **API:** ASP.NET Core Minimal API
* **Cơ sở dữ liệu:** PostgreSQL (Chạy trong Docker)
* **Message Broker:** RabbitMQ (Sẽ tích hợp ở bước tiếp theo)
* **Real-time:** SignalR (Sẽ tích hợp ở bước tiếp theo)
* **Hạ tầng:** Docker & Docker Compose, WSL2 (Ubuntu)

---

## 📈 Tiến độ thực hiện (Cập nhật ngày: 20/11/2026)

### ✅ Đã hoàn thành
- [x] Thiết lập môi trường phát triển: Windows 10 + WSL2 Ubuntu (Docker Engine).
- [x] Khởi tạo cấu trúc Solution .NET 10 gồm các project: `OrderApi`, `OrderWorker`, `NotificationService`, `OrderClient`.
- [x] Cấu hình file `docker-compose.yml` chạy 2 service hạ tầng: `postgres` (port 5432) và `rabbitmq` (port 5672, 15672).
- [x] Cấu hình Git (`.gitignore`, commit lịch sử).
- [x] **OrderApi:**
  - Tạo model `Order` và `AppDbContext`.
  - Kết nối thành công tới Database PostgreSQL (đã chạy migration tạo bảng `Orders`).
  - Viết thành công API `GET /orders` (lấy danh sách) và `POST /orders` (tạo đơn hàng mới với trạng thái mặc định là `Pending`).

### ⏳ Đang thực hiện / Chưa làm
- [ ] **OrderApi:** Tích hợp RabbitMQ để bắn message (chứa OrderId) vào queue `order_queue` khi có đơn hàng mới.
- [ ] **OrderWorker:** Viết BackgroundService lắng nghe queue, cập nhật trạng thái đơn hàng (`Pending -> Processing -> Completed`) và phát sự kiện.
- [ ] **NotificationService:** Tích hợp SignalR và lắng nghe RabbitMQ để gửi thông báo real-time về Client.
- [ ] **Docker hóa:** Viết `Dockerfile` cho 3 service .NET và cấu hình mạng nội bộ trong `docker-compose.yml`.
- [ ] Viết tài liệu hướng dẫn chạy hệ thống chi tiết.

---
