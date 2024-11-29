/*================================= Hàm xử lí cho customer ======================================*/
$(document).ready(function () {
    // Hàm thêm khách hàng
    $('#addCustomerForm').on('submit', function (event) {
        event.preventDefault();
        // Thu thập dữ liệu từ form
        const customerData = {
            FullName: $('input[name="customer_name"]').val(),
            PhoneNumber: $('input[name="phone_number"]').val(),
            Email: $('input[name="email"]').val(),
            CCCD: $('input[name="national_id"]').val()
        };
        // Gửi dữ liệu qua API
        $.ajax({
            url: '/Customer/CreateCustomer', // URL API
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(customerData),
            success: function (response) {
                Swal.fire({
                    icon: 'success',
                    title: 'Success',
                    text: 'Customer added successfully!'
                }).then(() => {
                    location.reload(); // Tải lại trang
                });
            },
            error: function (xhr) {
                Swal.fire({
                    icon: 'error',
                    title: 'Error',
                    text: xhr.responseJSON?.message || 'An error occurred while adding the customer.'
                });
            }
        });
    });

    // Xử lý sự kiện khi nhấn nút Edit
    $('.edit-room').on('click', function () {
        var row = $(this).closest('tr');

        // Điền thông tin vào modal
        $('#editCustomerModal input[name="customer_id"]').val(row.find('td:first').text());
        $('#editCustomerModal input[name="customer_name"]').val(row.find('td:nth-child(2)').text());
        $('#editCustomerModal input[name="phone_number"]').val(row.find('td:nth-child(3)').text());
        $('#editCustomerModal input[name="email"]').val(row.find('td:nth-child(4)').text());
        $('#editCustomerModal input[name="national_id"]').val(row.find('td:nth-child(5)').text());
    });

    // Xử lý form submit để cập nhật khách hàng
    $('#editRoomForm').on('submit', function (e) {
        e.preventDefault();

        var customerId = $('#editCustomerModal input[name="customer_id"]').val();
        var customerData = {
            CustomerID: parseInt(customerId),
            FullName: $('#editCustomerModal input[name="customer_name"]').val(),
            PhoneNumber: $('#editCustomerModal input[name="phone_number"]').val(),
            Email: $('#editCustomerModal input[name="email"]').val(),
            CCCD: $('#editCustomerModal input[name="national_id"]').val()
        };

        $.ajax({
            url: '/Customer/UpdateCustomer/' + customerId,
            type: 'PUT',
            contentType: 'application/json',
            data: JSON.stringify(customerData),
            success: function (response) {
                Swal.fire({
                    icon: 'success',
                    title: 'Thành công!',
                    text: 'Cập nhật thông tin khách hàng thành công.',
                    confirmButtonText: 'OK'
                }).then(() => {
                    location.reload(); // Làm mới trang
                });
            },
            error: function (xhr) {
                Swal.fire({
                    icon: 'error',
                    title: 'Lỗi!',
                    text: xhr.responseJSON.message || 'Có lỗi xảy ra khi cập nhật thông tin khách hàng.',
                    confirmButtonText: 'Đóng'
                });
            }
        });
    });
});

// Hàm xác nhận và xóa khách hàng
function confirmCustomerDelete(customerId) {
    Swal.fire({
        title: 'Are you sure?',
        text: 'You will not be able to undo this action!',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it',
        cancelButtonText: 'Cancel'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: '/Customer/DeleteCustomer/' + customerId,
                type: 'DELETE',
                success: function (response) {
                    Swal.fire({
                        icon: 'success',
                        title: 'Deleted',
                        text: 'The customer has been successfully deleted.',
                        confirmButtonText: 'OK'
                    }).then(() => {
                        location.reload(); // Làm mới trang
                    });
                },
                error: function (xhr) {
                    Swal.fire({
                        icon: 'error',
                        title: 'Error!',
                        text: xhr.responseJSON.message || 'An error occurred while deleting the customer.',
                        confirmButtonText: 'Close'
                    });
                }
            });
        }
    });
}