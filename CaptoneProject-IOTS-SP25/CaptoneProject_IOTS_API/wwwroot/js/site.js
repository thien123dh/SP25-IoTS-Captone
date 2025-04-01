$(() => {
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/chathub") 
        .build();

    // Khởi tạo kết nối SignalR
    connection.start().then(function () {
        console.log("SignalR Connected");

        connection.on("ReceiveMessage", function (message) {
            console.log("New message received:", message);
            LoadData();
        });

        // Lấy dữ liệu ban đầu
        LoadData();
    }).catch(function (err) {
        return console.error(err.toString());
    });

    function LoadData() {
        var tr = '';
        $.ajax({
            url: '/api/Message/GetMessages?userId=' + userId + '&receiverId=' + receiverId,
            method: 'GET',
            dataType: 'json',  // Đảm bảo kiểu dữ liệu trả về là JSON
            success: (result) => {
                console.log(result);  // Log kết quả để kiểm tra
                // Xử lý và hiển thị tin nhắn nhận được
                $.each(result, (k, v) => {
                    tr += `
                        <tr>
                            <td>${v.senderId === userId ? "You" : "User " + v.senderId}</td>
                            <td>${v.content}</td>
                            <td>${new Date(v.createdDate).toLocaleDateString()}</td>
                        </tr>`;
                });
                // Cập nhật bảng với các dòng dữ liệu mới
                $('#tableBody').html(tr);
            },
            error: (error) => {
                console.log(error);
            }
        });
    }

    $('#sendMessageButton').click(function () {
        var messageContent = $('#messageInput').val();
        if (!messageContent.trim()) return;

        var message = {
            senderId: userId,  
            receiverId: receiverId,  
            content: messageContent 
        };

        $.ajax({
            url: '/api/Message/Chat-RabbitMQ',
            method: 'POST',
            data: JSON.stringify(message),
            contentType: 'application/json',
            success: function (response) {
                console.log("Message sent:", response);
                $('#messageInput').val('');
            },
            error: function (error) {
                console.log("Error sending message:", error);
            }
        });
    });
});
