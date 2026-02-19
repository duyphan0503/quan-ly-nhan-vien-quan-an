#!/bin/bash
echo "Đang khởi tạo cơ sở dữ liệu..."
docker exec -i mssql_quanlynhanvien /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P 'YourPassword123!' -i /usr/config/CreateDatabase.sql -C
echo "Xong!"
