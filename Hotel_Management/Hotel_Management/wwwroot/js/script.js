/*================================= Hàm xử lí cho customer ===================================*/
$(document).ready(function () {
    // Hàm thêm khách hàng
    $('#addCustomerForm').on('submit', function (event) {
        event.preventDefault();

        // Validate input trước khi gửi
        const nationalId = $('input[name="national_id"]').val();

        // Kiểm tra độ dài CCCD (ví dụ: 9 hoặc 12 chữ số)
        if (!/^\d{12}$/.test(nationalId)) {
            Swal.fire({
                icon: 'error',
                title: 'Error',
                text: 'Invalid ID number. It must be 12 digits.'
            });
            return;
        }

        // Thu thập dữ liệu từ form
        const customerData = {
            FullName: $('input[name="customer_name"]').val(),
            PhoneNumber: $('input[name="phone_number"]').val(),
            Email: $('input[name="email"]').val(),
            CCCD: nationalId
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
                // Xử lý các loại lỗi khác nhau
                if (xhr.status === 409) {
                    // Lỗi trùng CCCD
                    Swal.fire({
                        icon: 'error',
                        title: 'Error',
                        text: 'A customer with this ID number already exists.'
                    });
                } else {
                    // Các lỗi khác
                    Swal.fire({
                        icon: 'error',
                        title: 'Lỗi',
                        text: xhr.responseJSON?.message || 'An error occurred while adding the customer.'
                    });
                }
            }
        });
    });

    // Kiểm tra validate input CCCD khi nhập
    $('input[name="national_id"]').on('input', function () {
        const value = $(this).val();
        const errorSpan = $('#national-id-error');

        if (!/^\d+$/.test(value)) {
            $(this).val(value.replace(/[^\d]/g, '')); // Chỉ cho phép số
        }

        if (value.length > 0 && !/^\d{12}$/.test(value)) {
            errorSpan.text('Invalid ID number. It must be 12 digits.');
            $(this).addClass('is-invalid');
        } else {
            errorSpan.text('');
            $(this).removeClass('is-invalid');
        }
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
                    title: 'Success',
                    text: 'Customer information updated successfully.',
                    confirmButtonText: 'OK'
                }).then(() => {
                    location.reload(); // Làm mới trang
                });
            },
            error: function (xhr) {
                if (xhr.status === 409) { // Trùng CCCD
                    Swal.fire({
                        icon: 'error',
                        title: 'Error!',
                        text: 'A customer with this ID number already exists.',
                        confirmButtonText: 'Close'
                    });
                } else {
                    Swal.fire({
                        icon: 'error',
                        title: 'Error!',
                        text: xhr.responseJSON.message || 'An error occurred while updating the customer information.',
                        confirmButtonText: 'Close'
                    });
                }
            }
        });
    });

});
// Thêm vào file script.js
$(document).ready(function () {
    $('#customerSearch').on('keyup', function () {
        var value = $(this).val().toLowerCase();
        $('.datatable tbody tr').filter(function () {
            $(this).toggle($(this).children('td').eq(1).text().toLowerCase().indexOf(value) > -1)
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
                        text: response.message,
                        confirmButtonText: 'OK'
                    }).then(() => {
                        location.reload(); // Làm mới trang
                    });
                },
                error: function (xhr) {
                    if (xhr.status === 409) {
                        Swal.fire({
                            icon: 'error',
                            title: 'Error',
                            text: 'The customer cannot be deleted because the customer has already booked.'
                        });
                    } else {
                        Swal.fire({
                            icon: 'error',
                            title: 'Error!',
                            text: 'An error occurred while deleting the customer.',
                            confirmButtonText: 'Close'
                        });
                    }
                }
            });
        }
    });
}

/*================================= Hàm xử lí cho Booking ===================================*/


// Add booking
$(document).ready(function () {



    // Validate dates when they change
    $('#checkInDate, #checkOutDate').on('change', function () {
        validateDates();
    });

    function validateDates() {
        const checkInDate = new Date($('#checkInDate').val());
        const checkOutDate = new Date($('#checkOutDate').val());

        // Reset validation state
        $('#checkInDate, #checkOutDate').removeClass('is-invalid');

        if (checkInDate && checkOutDate) {
            if (checkInDate > checkOutDate) {
                $('#checkOutDate').addClass('is-invalid');
                Swal.fire({
                    icon: 'error',
                    title: 'Invalid Dates',
                    text: 'Check-out date must be after check-in date'
                });
                return false;
            }

            // Check if check-in date is in the past
            const today = new Date();
            today.setHours(0, 0, 0, 0);

            if (checkInDate <= today) {
                $('#checkInDate').addClass('is-invalid');
                Swal.fire({
                    icon: 'error',
                    title: 'Invalid Date',
                    text: 'Check-in date cannot be in the past'
                });
                return false;
            }
        }
        return true;
    }

    // Modify your existing form submission
    $('#addBookingForm').on('submit', function (event) {
        event.preventDefault();

        if (!validateDates()) {
            return;
        }

        const bookingData = {
            customerID: parseInt($('select[name="CustomerID"]').val()),
            roomID: parseInt($('select[name="RoomID"]').val()),
            checkInDate: $('#checkInDate').val(),
            checkOutDate: $('#checkOutDate').val(),
            status: "Pending"
        };

        // Validate required fields
        if (!bookingData.customerID || !bookingData.roomID) {
            Swal.fire({
                icon: 'error',
                title: 'Missing Data',
                text: 'Please select both customer and room'
            });
            return;
        }

        $.ajax({
            url: '/Booking/CreateBooking',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(bookingData),
            success: function (response) {
                Swal.fire({
                    icon: 'success',
                    title: 'Success',
                    text: 'Booking added successfully!'
                }).then(() => {
                    location.reload();
                });
            },
            error: function (xhr) {
                let errorMessage = 'An error occurred while adding the booking.';

                if (xhr.responseJSON) {
                    errorMessage = xhr.responseJSON.message;

                    // If there's a booking conflict, show detailed information
                    if (xhr.responseJSON.conflictBooking) {
                        const conflictBooking = xhr.responseJSON.conflictBooking;
                        const checkIn = new Date(conflictBooking.checkIn).toLocaleDateString();
                        const checkOut = new Date(conflictBooking.checkOut).toLocaleDateString();
                        errorMessage += `\nExisting booking: ${checkIn} to ${checkOut}`;
                    }
                }

                Swal.fire({
                    icon: 'error',
                    title: 'Error',
                    text: "Room is booked! Please choose another option"
                });
            }
        });
    });


    // Hàm format ngày để hiển thị trong input date
    function formatDate(date) {
        const year = date.getFullYear();
        const month = String(date.getMonth() + 1).padStart(2, '0');
        const day = String(date.getDate()).padStart(2, '0');
        return `${year}-${month}-${day}`;
    }

    // Sự kiện khi nhấn nút Edit trong dropdown
    $(document).on('click', '.edit-room', function () {
        // Lấy BookingID thực tế từ dòng hiện tại
        const bookingIndex = $(this).closest('tr').find('td:first-child').text() - 1;
        const booking = bookings[bookingIndex];

        if (booking) {
            // Điền thông tin vào modal
            $('#editBookingModal input[name="BookingID"]').val(booking.bookingID);
            $('#editBookingModal select[name="CustomerID"]').val(booking.customerID);
            $('#editBookingModal select[name="RoomID"]').val(booking.roomID);
            $('#editBookingModal input[name="CheckInDate"]').val(formatDate(new Date(booking.checkInDate)));
            $('#editBookingModal input[name="CheckOutDate"]').val(formatDate(new Date(booking.checkOutDate)));
            $('#editBookingModal select[name="Status"]').val(booking.status.toLowerCase());
        }
    });

    // Xử lý sự kiện update booking
    $('#editBookingButton').on('click', function () {
        const bookingData = {
            BookingID: parseInt($('#editBookingModal input[name="BookingID"]').val()),
            CustomerID: parseInt($('#editBookingModal select[name="CustomerID"]').val()),
            RoomID: parseInt($('#editBookingModal select[name="RoomID"]').val()),
            CheckInDate: $('#editBookingModal input[name="CheckInDate"]').val(),
            CheckOutDate: $('#editBookingModal input[name="CheckOutDate"]').val(),
            Status: $('#editBookingModal select[name="Status"]').val()
        };

        console.log('Booking Data to Update:', bookingData);

        // Gọi API update
        $.ajax({
            url: '/Booking/UpdateBooking',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(bookingData),
            success: function (response) {
                Swal.fire({
                    icon: 'success',
                    title: 'Booking Updated',
                    text: response.message || 'Booking updated successfully'
                }).then(() => {
                    location.reload(); // Tải lại trang sau khi cập nhật
                });
            },
            error: function (xhr) {
                console.error('Update Error:', xhr);
                Swal.fire({
                    icon: 'error',
                    title: 'Update Failed',
                    text: xhr.responseJSON ?
                        (xhr.responseJSON.message || 'An error occurred') :
                        'An error occurred while updating the booking'
                });
            }
        });
    });
});

//Delete Booking

function deleteBooking(bookingId) {
    const row = $(`tr[data-booking-id="${bookingId}"]`);
    row.addClass('deleting');

    $.ajax({
        url: '/Booking/DeleteBooking/' + bookingId,
        type: 'DELETE',
        beforeSend: function () {
            row.find('.dropdown-action').addClass('disabled');
        },
        success: function (response) {
            Swal.fire({
                icon: 'success',
                title: 'Deleted!',
                text: 'Booking has been deleted successfully.',
                showConfirmButton: true,  // Hiển thị nút OK
                confirmButtonText: 'OK',  // Text cho nút OK
                timer: null  // Bỏ timer để alert không tự đóng
            }).then((result) => {
                if (result.isConfirmed) {  // Khi người dùng click OK
                    location.reload();  // Load lại danh sách booking
                }
            });
        },
        error: function (xhr) {
            row.removeClass('deleting');
            row.find('.dropdown-action').removeClass('disabled');

            let errorMessage = 'An error occurred while deleting the booking.';
            if (xhr.responseJSON && xhr.responseJSON.message) {
                errorMessage = xhr.responseJSON.message;
            }

            Swal.fire({
                icon: 'error',
                title: 'Error!',
                text: errorMessage,
                confirmButtonText: 'OK'
            });
        }
    });
}

function confirmBookingDelete(bookingId) {
    Swal.fire({
        title: 'Are you sure?',
        text: 'You want to delete this booking? This action cannot be undone!',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#d33',
        cancelButtonColor: '#3085d6',
        confirmButtonText: 'Yes, delete it!',
        cancelButtonText: 'Cancel'
    }).then((result) => {
        if (result.isConfirmed) {
            deleteBooking(bookingId);
        }
    });
}

function loadBookings() {
    $.ajax({
        url: '/Booking/GetBookings', // Đảm bảo API này trả về danh sách booking
        type: 'GET',
        success: function (data) {
            const tbody = $('.datatable tbody');
            tbody.empty();
            data.forEach(booking => {
                tbody.append(`
                    <tr data-booking-id="${booking.BookingID}">
                        <td>${booking.BookingID}</td>
                        <td>${booking.Customer.FullName}</td>
                        <td>${booking.Room.RoomNumber}</td>
                        <td>${new Date(booking.CheckInDate).toLocaleDateString()}</td>
                        <td>${new Date(booking.CheckOutDate).toLocaleDateString()}</td>
                        <td>${booking.Status}</td>
                        <td class="text-right">
                            <div class="dropdown dropdown-action">
                                <a href="#" class="action-icon dropdown-toggle" data-toggle="dropdown" aria-expanded="false">
                                    <i class="fas fa-ellipsis-v ellipse_color"></i>
                                </a>
                                <div class="dropdown-menu dropdown-menu-right">
                                    <a class="dropdown-item" href="javascript:void(0);" onclick="confirmBookingDelete(${booking.BookingID})">
                                        <i class="fas fa-trash-alt m-r-5"></i> Delete
                                    </a>
                                </div>
                            </div>
                        </td>
                    </tr>
                `);
            });
        },
        error: function (xhr) {
            Swal.fire({
                icon: 'error',
                title: 'Error!',
                text: 'An error occurred while loading bookings.',
                confirmButtonText: 'OK'
            });
        }
    });
}

/*================================= Hàm xử lí cho room ===================================*/
$(document).ready(function () {   
    $('#addRoomForm').on('submit', function (event) {
        event.preventDefault();

        // Validate input before sending
        const roomID = 0;
        const roomNumber = $('input[name="RoomNumber"]').val();
        const roomType = $('select[name="room_type"]').val();
        const price = $('input[name="Price"]').val();
        const status = $('select[name="status"]').val();
        const description = $('textarea[name="Description"]').val();

        // Check if required fields are filled
        if (!roomNumber || !roomType || !price) {
            Swal.fire({
                icon: 'error',
                title: 'Error',
                text: 'Room number, room type, and price are required.'
            });
            return;
        }

        // Collect data from form
        const roomData = {
            RoomID: 0,
            RoomNumber: roomNumber,
            RoomType: roomType,
            Price: parseInt(price),
            Status: status,
            Description: description
        };

        // Send data via API
        $.ajax({
            url: '/Rooms/CreateRoom', // API URL
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(roomData),
            success: function (response) {
                Swal.fire({
                    icon: 'success',
                    title: 'Success',
                    text: 'Room added successfully!'
                }).then(() => {
                    location.reload(); // Reload the page
                });
            },
            error: function (xhr) {
                // Handle different types of errors
                if (xhr.status === 409) {
                    // Duplicate room number error
                    Swal.fire({
                        icon: 'error',
                        title: 'Error',
                        text: 'A room with this number already exists.'
                    });
                } else {
                    // Other errors
                    Swal.fire({
                        icon: 'error',
                        title: 'Error',
                        text: xhr.responseJSON?.message || 'An error occurred while adding the room.'
                    });
                }
            }
        });
    });   
});

function confirmRoomDelete(roomId) {
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
                url: '/Rooms/DeleteRoom/' + roomId,
                type: 'DELETE',
                success: function (response) {
                    Swal.fire({
                        icon: 'success',
                        title: 'Deleted',
                        text: response.message,
                        confirmButtonText: 'OK'
                    }).then(() => {
                        location.reload(); // Reload the page
                    });
                },
                error: function (xhr) {
                    if (xhr.status === 409) {
                        Swal.fire({
                            icon: 'error',
                            title: 'Error',
                            text: 'The room cannot be deleted because it is associated with existing bookings.'
                        });
                    } else {
                        Swal.fire({
                            icon: 'error',
                            title: 'Error!',
                            text: 'An error occurred while deleting the room.',
                            confirmButtonText: 'Close'
                        });
                    }
                }
            });
        }
    });
}