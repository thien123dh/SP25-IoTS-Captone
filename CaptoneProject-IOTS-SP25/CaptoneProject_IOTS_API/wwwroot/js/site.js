﻿$(() => {
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
                url: '/api/Message/GetMessages?receiverId=' + receiverId,
                method: 'GET',
                dataType: 'json', 
            });
            console.log(response);

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


async function revokeMessage(messageId) {
    const token = localStorage.getItem("jwtToken");
    if (!token) {
        alert("Bạn chưa đăng nhập!");
        return;
    }

    try {
        const response = await fetch(`/api/messages/revoke/${messageId}`, {
            method: "PUT",
            headers: {
                "Content-Type": "application/json",
                "Authorization": `Bearer ${token}`
            }
        });

        const data = await response.json();
        if (response.ok) {
            alert("Tin nhắn đã được thu hồi!");
            // Cập nhật UI
        } else {
            alert(data.message || "Không thể thu hồi tin nhắn!");
        }
    } catch (error) {
        console.error("Error revoking message:", error);
    }
}
