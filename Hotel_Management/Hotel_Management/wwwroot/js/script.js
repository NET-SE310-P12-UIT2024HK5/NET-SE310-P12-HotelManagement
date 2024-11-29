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

    // Hàm xoá khách hàng
    $('.deleteCustomerBtn').on('click', function () {
        const customerId = $(this).data('id'); // Lấy ID khách hàng từ button

        Swal.fire({
            title: 'Are you sure?',
            text: "You won't be able to revert this!",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Yes, delete it!'
        }).then((result) => {
            if (result.isConfirmed) {
                $.ajax({
                    url: `/Customer/${customerId}`,
                    type: 'DELETE',
                    success: function (response) {
                        Swal.fire({
                            icon: 'success',
                            title: 'Deleted!',
                            text: 'Customer has been deleted.'
                        }).then(() => {
                            location.reload(); // Tải lại trang
                        });
                    },
                    error: function () {
                        Swal.fire({
                            icon: 'error',
                            title: 'Error',
                            text: 'An error occurred while deleting the customer.'
                        });
                    }
                });
            }
        });
    });

    // Hàm chỉnh sửa khách hàng
    $('#editCustomerForm').on('submit', function (event) {
        event.preventDefault();

        const customerId = $('input[name="customer_id"]').val();
        const updatedCustomerData = {
            FullName: $('input[name="edit_customer_name"]').val(),
            PhoneNumber: $('input[name="edit_phone_number"]').val(),
            Email: $('input[name="edit_email"]').val(),
            CCCD: $('input[name="edit_national_id"]').val()
        };

        $.ajax({
            url: `/Customer/UpdateCustomer/${customerId}`,
            type: 'PUT',
            contentType: 'application/json',
            data: JSON.stringify(updatedCustomerData),
            success: function () {
                Swal.fire({
                    icon: 'success',
                    title: 'Updated!',
                    text: 'Customer updated successfully.'
                }).then(() => {
                    location.reload();
                });
            },
            error: function () {
                Swal.fire({
                    icon: 'error',
                    title: 'Error',
                    text: 'An error occurred while updating the customer.'
                });
            }
        });
    });
});
