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
        console.error(err.toString());
    });

    async function LoadData() {
        let tr = '';
        try {
            const response = await $.ajax({
                url: '/api/Message/GetMessages?userId=' + userId + '&receiverId=' + receiverId,
                method: 'GET',
                dataType: 'json',  // Đảm bảo kiểu dữ liệu trả về là JSON
            });
            console.log(response);  // Log kết quả để kiểm tra

            $.each(response, (k, v) => {
                tr += `
                    <tr>
                        <td>${v.senderId === userId ? "You" : "User " + v.senderId}</td>
                        <td>${v.content}</td>
                        <td>${new Date(v.createdDate).toLocaleDateString()}</td>
                    </tr>`;
            });
            // Cập nhật bảng với các dòng dữ liệu mới
            $('#tableBody').html(tr);
        } catch (error) {
            console.log("Error loading data:", error);
        }
    }

    $('#sendMessageButton').click(async function () {
        var messageContent = $('#messageInput').val();
        if (!messageContent.trim()) return;

        var message = {
            senderId: userId,
            receiverId: receiverId,
            content: messageContent
        };

        try {
            const response = await $.ajax({
                url: '/api/Message/Chat-RabbitMQ',
                method: 'POST',
                data: JSON.stringify(message),
                contentType: 'application/json',
            });
            console.log("Message sent:", response);
            $('#messageInput').val(''); // Clear input after sending
        } catch (error) {
            console.log("Error sending message:", error);
        }
    });
});
