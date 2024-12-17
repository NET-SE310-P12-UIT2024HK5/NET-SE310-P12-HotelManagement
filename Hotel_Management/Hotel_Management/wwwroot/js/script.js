const { error } = require("jquery");

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
// Customer search by name
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

            if (checkInDate < today) {
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

        console.log('Booking Data:', bookingData); // Log the booking data

        $.ajax({
            url: '/Booking/CreateBooking',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(bookingData),
            success: function (response) {
                console.log('Success Response:', response); // Log the success response
                Swal.fire({
                    icon: 'success',
                    title: 'Success',
                    text: 'Booking added successfully!'
                }).then(() => {
                    location.reload();
                });
            },
            error: function (xhr) {
                console.error('Error Response:', xhr); // Log the error response
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
                    text: "Rooms is unavailable! Please choose another option"
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
            $('#editBookingModal select[name="Status"]').val(booking.status);
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
            if (xhr.status === 409) {
                errorMessage = 'Reservations cannot be deleted because the reservation is in use with the service.';
            }
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
    $('#editRoomInformationForm').on('submit', function (e) {
        e.preventDefault();
       
        var roomId = $('#editRoomModal input[name="room_id"]').val();
        var roomData = {
            RoomID: parseInt(roomId),
            RoomNumber: $('#editRoomModal input[name="room_number"]').val(),
            RoomType: $('#editRoomModal select[name="room_type"]').val(),
            Price: parseInt($('#editRoomModal input[name="HourlyRateEdit"]').val()),
            Status: $('#editRoomModal select[name="status"]').val(),
            Description: $('#editRoomModal textarea[name="description"]').val()
        };

        $.ajax({
            url: '/Rooms/UpdateRoom/' + roomId,
            type: 'PUT',
            contentType: 'application/json',
            data: JSON.stringify(roomData),
            success: function (response) {
                Swal.fire({
                    icon: 'success',
                    title: 'Success',
                    text: 'Room information updated successfully.',
                    confirmButtonText: 'OK'
                }).then(() => {
                    location.reload(); // Reload the page
                });
            },
            error: function (xhr) {
                if (xhr.status === 409) { // Duplicate room number
                    Swal.fire({
                        icon: 'error',
                        title: 'Error!',
                        text: 'A room with this number already exists.',
                        confirmButtonText: 'Close'
                    });
                } else {
                    Swal.fire({
                        icon: 'error',
                        title: 'Error!',
                        text: xhr.responseJSON.message || 'An error occurred while updating the room information.',
                        confirmButtonText: 'Close'
                    });
                }
            }
        });
    });

    $('#addRoomForm').on('submit', function (event) {
        event.preventDefault();

        // Validate input before sending
        const roomID = 0;
        const roomNumber = $('input[name="RoomNumber"]').val();
        const roomType = $('select[name="room_type"]').val();
        const price = $('input[name="HourlyRate"]').val();
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

$(document).ready(function () {
    $('#roomSearch').on('keyup', function () {
        var value = $(this).val().toLowerCase();
        $('.datatable tbody tr').filter(function () {
            $(this).toggle($(this).children('td').eq(1).text().toLowerCase().indexOf(value) > -1)
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

function populateEditRoomModal(roomId, roomNumber, roomType, price, status, description) {
    $('#editRoomModal input[name="room_id"]').val(roomId);
    $('#editRoomModal input[name="room_number"]').val(roomNumber);
    $('#editRoomModal select[name="room_type"]').val(roomType);
    $('#editRoomModal input[name="HourlyRateEdit"]').val(price);
    $('#editRoomModal select[name="status"]').val(status);
    $('#editRoomModal textarea[name="description"]').val(description);
}

/*================================= Hàm xử lí cho invoice ===================================*/
$(document).ready(function () {
    $('#addInvoiceForm').on('submit', function (event) {
        event.preventDefault();

        // Validate input before sending
        const bookingID = $('select[name="BookingID"]').val();
        const duration = $('input[name="Duration"]').val();
        const paymentDate = $('input[name="PaymentDate"]').val();
        const totalAmount = $('input[name="TotalAmount"]').val();
        const paymentStatus = $('select[name="PaymentStatus"]').val();

        // Check if required fields are filled
        if (!bookingID || !paymentDate || !duration || !totalAmount || !paymentStatus) {
            Swal.fire({
                icon: 'error',
                title: 'Error',
                text: 'Booking ID, payment date, duration,total amount, and payment status are required.'
            });
            return;
        }

        // Collect data from form
        const invoiceData = {
            InvoiceID: 0,
            BookingID: bookingID,
            Duration: parseInt(duration),
            PaymentDate: paymentDate,
            TotalAmount: parseInt(totalAmount),
            PaymentStatus: paymentStatus
        };

        // Send data via API
        $.ajax({
            url: '/Invoice/CreateInvoice', // API URL
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(invoiceData),
            success: function (response) {
                Swal.fire({
                    icon: 'success',
                    title: 'Success',
                    text: 'Invoice added successfully!'
                }).then(() => {
                    location.reload(); // Reload the page
                });
            },
            error: function (xhr) {
                // Handle different types of errors
                if (xhr.status === 409) {
                    // Duplicate invoice error
                    Swal.fire({
                        icon: 'error',
                        title: 'Error',
                        text: 'An invoice with this ID already exists.'
                    });
                } else {
                    // Other errors
                    Swal.fire({
                        icon: 'error',
                        title: 'Error',
                        text: xhr.responseJSON?.message || 'An error occurred while adding the invoice.'
                    });
                }
            }
        });
        return false;
    });

    $('#editInvoiceForm').on('submit', function (event) {
        event.preventDefault();

        // Validate input before sending
        const invoiceID = $('input[name="invoice_id"]').val();
        const bookingID = $('select[name="booking_id"]').val();
        const duration = $('input[name="Duration"]').val();
        const paymentDate = $('input[name="payment_date"]').val();
        const totalAmount = $('input[name="total_amount"]').val();
        const paymentStatus = $('select[name="payment_status"]').val();

        // Check if required fields are filled
        if (!invoiceID || !bookingID || !paymentDate || !duration || !totalAmount || !paymentStatus) {
            Swal.fire({
                icon: 'error',
                title: 'Error',
                text: 'Invoice ID, booking ID, payment date, duration, total amount, and payment status are required.'
            });
            return;
        }

        // Collect data from form
        const invoiceData = {
            InvoiceID: parseInt(invoiceID),
            BookingID: parseInt(bookingID),
            Duration: parseInt(duration),
            PaymentDate: paymentDate,
            TotalAmount: parseInt(totalAmount),
            PaymentStatus: paymentStatus
        };

        // Send data via API
        $.ajax({
            url: `/Invoice/UpdateInvoice/${invoiceID}`, // API URL
            type: 'PUT',
            contentType: 'application/json',
            data: JSON.stringify(invoiceData),
            success: function (response) {
                Swal.fire({
                    icon: 'success',
                    title: 'Success',
                    text: 'Invoice updated successfully!'
                }).then(() => {
                    location.reload(); // Reload the page
                });
            },
            error: function (xhr) {
                // Handle different types of errors
                if (xhr.status === 409) {
                    // Duplicate invoice error
                    Swal.fire({
                        icon: 'error',
                        title: 'Error',
                        text: 'An invoice with this ID already exists.'
                    });
                } else {
                    // Other errors
                    Swal.fire({
                        icon: 'error',
                        title: 'Error',
                        text: xhr.responseJSON?.message || 'An error occurred while updating the invoice.'
                    });
                }
            }
        });
        return false;
    });
});

$(document).ready(function () {
    $('#BookingID').on('change', function () {
        const bookingId = $(this).val();
        if (bookingId) {
            console.log(`Received Booking ID: ${bookingId}`);
            $.ajax({
                url: `https://localhost:7287/Invoice/getroomprice/${bookingId}`,
                type: 'GET',
                success: function (roomPrice) {
                    console.log(`Room Price: ${roomPrice}`);
                    $('#HourlyRate').val(roomPrice);
                },
                error: function (xhr, status, error) {
                    console.error(`Error retrieving room price from API. Status Code: ${xhr.status}, Reason: ${xhr.statusText}`);
                }
            });
        }
    });
});

$(document).ready(function () {
    function calculateTotalAmount() {
        const hourlyRate = parseInt($('#HourlyRate').val());
        const duration = parseInt($('#Duration').val());
        if (!isNaN(hourlyRate) && !isNaN(duration)) {
            const totalAmount = hourlyRate * duration;
            $('#TotalAmount').val(totalAmount);
        }
    }

    $('#HourlyRate, #Duration').on('input', function () {
        calculateTotalAmount();
    });
});



function populateEditInvoiceModal(invoiceId, bookingId, duration, totalAmount, paymentStatus, paymentDate) {
    // Set the values in the edit modal
    $('#editInvoiceModal input[name="invoice_id"]').val(invoiceId);
    $('#editInvoiceModal select[name="booking_id"]').val(bookingId);
    $('#editInvoiceModal input[name="Duration"]').val(duration);
    $('#editInvoiceModal input[name="total_amount"]').val(totalAmount);
    $('#editInvoiceModal select[name="payment_status"]').val(paymentStatus);
    $('#editInvoiceModal input[name="payment_date"]').val(new Date(paymentDate).toISOString().split('T')[0]);

    // Show the modal
    $('#editInvoiceModal').modal('show');
}

function confirmInvoiceDelete(invoiceId) {
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
                url: '/Invoice/DeleteInvoice/' + invoiceId,
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
                    Swal.fire({
                        icon: 'error',
                        title: 'Error',
                        text: 'An error occurred while deleting the invoice.',
                        confirmButtonText: 'Close'
                    });
                }
            });
        }
    });
}

$(document).ready(function () {
    // Function to filter the datatable
    function filterTable() {
        const bookingId = $('#searchBookingInvoice').val();
        const fromDate = $('#fromDate').val();
        const toDate = $('#toDate').val();

        if (fromDate && toDate && new Date(fromDate) > new Date(toDate)) {
            Swal.fire({
                icon: 'error',
                title: 'Invalid Date Range',
                text: 'The "To" date cannot be earlier than the "From" date.'
            });
            return;
        }

        $('.datatable tbody tr').each(function () {
            const row = $(this);
            const rowBookingId = row.find('td:nth-child(2)').text().trim();
            const rowPaymentDate = row.find('td:nth-child(3)').text().trim();

            let showRow = true;

            if (bookingId && parseInt(rowBookingId) !== parseInt(bookingId)) {
                showRow = false;
            }

            if (fromDate && toDate) {
                const paymentDate = new Date(rowPaymentDate.split('/').reverse().join('-'));
                const from = new Date(fromDate);
                const to = new Date(toDate);

                if (paymentDate < from || paymentDate > to) {
                    showRow = false;
                }
            }

            if (showRow) {
                row.show();
            } else {
                row.hide();
            }
        });
    }

    // Event listeners for the input fields
    $('#searchBookingInvoice, #fromDate, #toDate').on('input change', function () {
        filterTable();
    });
});

/*================================= Hàm xử lí cho Food and Beverage ===================================*/
function deleteItem(serviceId) {
    // Show confirmation dialog using SweetAlert
    Swal.fire({
        title: 'Are you sure?',
        text: 'This action cannot be undone!',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!',
        cancelButtonText: 'Cancel'
    }).then((result) => {
        if (result.isConfirmed) {
            // Send delete request via Ajax
            $.ajax({
                url: '/FoodAndBeverage/DeleteFoodAndBeverage',
                type: 'POST',
                data: { id: serviceId },
                success: function (response) {
                    // Show success message using SweetAlert
                    Swal.fire({
                        icon: 'success',
                        title: 'Deleted!',
                        text: 'The item has been deleted successfully.',
                        confirmButtonText: 'OK'
                    }).then(() => {
                        location.reload(); // Reload the page
                    });
                },
                error: function (xhr, status, error) {
                    // Show error message using SweetAlert
                    Swal.fire(
                        'Error!',
                        'An error occurred while deleting: ' + xhr.responseText,
                        'error'
                    );
                }
            });
        }
    });
}


$(document).ready(function () {
    $('#addFoodForm').on('submit', function (e) {
        e.preventDefault();

        // Validate client-side
        var isValid = true;
        var errorMessages = [];

        // Kiểm tra tên
        var foodName = $('#foodName').val().trim();
        if (!foodName) {
            isValid = false;
            errorMessages.push("The item name cannot be empty");
        }

        // Kiểm tra category
        var category = $('#category').val();
        if (!category) {
            isValid = false;
            errorMessages.push("The item name cannot be empty");
        }

        // Kiểm tra giá
        var price = $('#price').val();
        if (!price || isNaN(price) || parseFloat(price) < 0) {
            isValid = false;
            errorMessages.push("The item name cannot be empty");
        }

        // Kiểm tra hình ảnh
        var imageFile = $('#ItemImage')[0].files[0];
        if (!imageFile) {
            isValid = false;
            errorMessages.push("Please select an image");
        }

        // Nếu có lỗi, hiển thị thông báo
        if (!isValid) {
            Swal.fire({
                icon: 'error',
                title: 'Input Error',
                html: errorMessages.map(msg => `<p>${msg}</p>`).join(''),
                confirmButtonText: 'Close'
            });
            return;
        }

        // Chuẩn bị FormData
        var formData = new FormData();
        formData.append('ItemName', foodName);
        formData.append('Category', category);
        formData.append('ItemPrice', price);
        formData.append('Description', $('#description').val() || '');
        formData.append('IsAvailable', $('#status').val());
        formData.append('ItemImage', imageFile);

        // Gửi AJAX request
        $.ajax({
            url: '/FoodAndBeverage/CreateFoodAndBeverage',
            type: 'POST',
            processData: false,  // Không xử lý dữ liệu
            contentType: false,  // Để trình duyệt tự động set Content-Type
            data: formData,
            success: function (response) {
                Swal.fire({
                    icon: 'success',
                    title: 'Success!',
                    text: 'A new food item has been added',
                    confirmButtonText: 'OK'
                }).then(() => {
                    // Làm mới danh sách hoặc reload trang
                    location.reload();
                });
            },
            error: function (xhr, status, error) {
                // Xử lý các lỗi khác nhau
                var errorMessage = 'An error occurred while adding the item';
                if (xhr.responseJSON && xhr.responseJSON.errors) {
                    // Xử lý lỗi validation từ server
                    var serverErrors = xhr.responseJSON.errors;
                    var errorList = [];
                    for (var field in serverErrors) {
                        errorList.push(serverErrors[field].join(', '));
                    }
                    errorMessage = errorList.join('<br>');
                } else if (xhr.responseJSON && xhr.responseJSON.message) {
                    errorMessage = xhr.responseJSON.message;
                }
                Swal.fire({
                    icon: 'error',
                    title: 'Error!',
                    html: errorMessage,
                    confirmButtonText: 'Close'
                });
            }
        });
    });
});

function editItem(serviceId) {
    $.ajax({
        url: '/FoodAndBeverage/EditFoodAndBeverage/' + serviceId,
        method: 'GET',
        success: function (response) {
            if (response.success) {
                // Populate modal with item details
                $('#editServiceID').val(response.item.serviceID);
                $('#editFoodName').val(response.item.itemName);
                $('#editCategory').val(response.item.category);
                $('#editPrice').val(response.item.itemPrice);
                $('#editDescription').val(response.item.description);
                $('#editStatus').val(response.item.isAvailable.toString());
                // Show modal
                $('#editFoodModal').modal('show');
            } else {
                Swal.fire({
                    icon: 'error',
                    title: 'Error',
                    text: response.message
                });
            }
        },
        error: function (xhr) {
            Swal.fire({
                icon: 'error',
                title: 'Error',
                text: 'An unexpected error occurred'
            });
        }
    });
}

// Handle form submission for editing
$('#editFoodForm').on('submit', function (e) {
    e.preventDefault();

    // Validate client-side
    var isValid = true;
    var errorMessages = [];

    // Kiểm tra tên
    var foodName = $('#editFoodName').val().trim();
    if (!foodName) {
        isValid = false;
        errorMessages.push("The item name cannot be empty");
    }

    // Kiểm tra giá
    var price = $('#editPrice').val();
    if (!price || isNaN(price) || parseFloat(price) < 0) {
        isValid = false;
        errorMessages.push("Invalid price");
    }

    // Nếu có lỗi, hiển thị thông báo
    if (!isValid) {
        Swal.fire({
            icon: 'error',
            title: 'Lỗi Nhập Liệu',
            html: errorMessages.map(msg => `<p>${msg}</p>`).join(''),
            confirmButtonText: 'Close'
        });
        return;
    }

    // Chuẩn bị FormData
    var formData = new FormData();
    formData.append('ServiceID', $('#editServiceID').val());
    formData.append('ItemName', foodName);
    formData.append('Category', $('#editCategory').val());
    formData.append('ItemPrice', price);
    formData.append('Description', $('#editDescription').val() || '');
    formData.append('IsAvailable', $('#editStatus').val());

    // Thêm hình ảnh nếu có
    var imageFile = $('#editItemImage')[0].files[0];
    if (imageFile) {
        formData.append('ItemImage', imageFile);
    }

    // Gửi AJAX request
    $.ajax({
        url: '/FoodAndBeverage/UpdateFoodAndBeverage',
        type: 'POST',
        processData: false,
        contentType: false,
        data: formData,
        success: function (response) {
            if (response.success) {
                Swal.fire({
                    icon: 'success',
                    title: 'Success!',
                    text: response.message
                }).then(() => {
                    // Làm mới danh sách hoặc reload trang
                    location.reload();
                });
            } else {
                Swal.fire({
                    icon: 'error',
                    title: 'Error!',
                    text: response.message
                });
            }
        },
        error: function (xhr) {
            Swal.fire({
                icon: 'error',
                title: 'Lỗi',
                text: 'An unexpected error occurred'
            });
        }
    });
});
document.addEventListener('DOMContentLoaded', function () {
    window.orderItems = [];
    window.addToOrder = addToOrder;
    window.removeFromOrder = removeFromOrder;
    window.submitOrder = submitOrder;
});

// Thêm món ăn vào đơn hàng
function addToOrder(item) {
    const normalizedItem = {
        ServiceID: item.ServiceID || item.serviceID,
        ItemName: item.ItemName || item.itemName,
        ItemPrice: item.ItemPrice || item.itemPrice
    };

    if (!normalizedItem.ServiceID) {
        console.error("Invalid item:", item);
        return;
    }

    const existingItemIndex = window.orderItems.findIndex(i => i.ServiceID === normalizedItem.ServiceID);

    if (existingItemIndex !== -1) {
        window.orderItems[existingItemIndex].quantity += 1;
    } else {
        window.orderItems.push({ ...normalizedItem, quantity: 1 });
    }

    updateOrderSummary();
}

// Cập nhật giao diện đơn hàng
function updateOrderSummary() {
    const orderItemsContainer = document.getElementById("orderItems");
    const orderTotalElement = document.getElementById("orderTotal");

    orderItemsContainer.innerHTML = "";
    let total = 0;

    window.orderItems.forEach((item, index) => {
        const itemTotal = item.ItemPrice * item.quantity;
        total += itemTotal;

        const itemRow = document.createElement("div");
        itemRow.className = "d-flex justify-content-between align-items-center mb-2";
        itemRow.innerHTML = `
            <div>
                <span>${item.ItemName}</span>
                <span class="ml-2 text-muted">x${item.quantity}</span>
            </div>
            <div>
                <span>${formatCurrency(itemTotal)}</span>
                <button class="btn btn-sm btn-outline-danger ml-2" onclick="removeFromOrder(${index})">
                    <i class="fas fa-trash"></i>
                </button>
            </div>
        `;
        orderItemsContainer.appendChild(itemRow);
    });

    orderTotalElement.textContent = formatCurrency(total);
}

// Xóa món ăn khỏi đơn hàng
function removeFromOrder(index) {
    Swal.fire({
        title: 'Are you sure you want to remove this item ?',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Yes, remove it!',
        cancelButtonText: 'Cancel'
    }).then((result) => {
        if (result.isConfirmed) {
            window.orderItems.splice(index, 1);
            updateOrderSummary();

            Swal.fire({
                title: 'Removed!',
                text: 'The item has been removed from the order',
                icon: 'success',
                timer: 1500,
                showConfirmButton: false
            });
        }
    });
}

// Gửi đơn hàng
async function submitOrder() {
    const bookingSelect = document.getElementById("bookingSelect");
    const selectedBooking = bookingSelect.value;

    if (!selectedBooking) {
        Swal.fire({
            title: 'Error!',
            text: 'Please select a booking',
            icon: 'error',
            confirmButtonText: 'OK'
        });
        return;
    }

    if (window.orderItems.length === 0) {
        Swal.fire({
            title: 'Error!',
            text: 'The order is empty!',
            icon: 'error',
            confirmButtonText: 'OK'
        });
        return;
    }

    const orderData = {
        BookingID: parseInt(selectedBooking),
        OrderItems: window.orderItems.map(item => ({
            ServiceID: item.ServiceID,
            Quantity: item.quantity
        }))
    };

    try {
        const response = await fetch("/FoodAndBeverage/SubmitOrder", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(orderData)
        });

        const result = await response.json();

        if (response.ok) {
            Swal.fire({
                title: 'Success!',
                text: result.message || 'The order has been submitted successfully!',
                icon: 'success',
                confirmButtonText: 'OK'
            }).then(() => {
                window.orderItems = [];
                updateOrderSummary();
            });
        } else {
            Swal.fire({
                title: 'Error!',
                text: result.message || 'Failed to submit the order',
                icon: 'error',
                confirmButtonText: 'OK'
            });
        }
    } catch (error) {
        console.error("Error submitting order:", error);
        Swal.fire({
            title: 'Error!',
            text: 'An error occurred while submitting the order',
            icon: 'error',
            confirmButtonText: 'OK'
        });
    }
}

// Định dạng tiền tệ
function formatCurrency(amount) {
    return new Intl.NumberFormat("vi-VN", { style: "currency", currency: "VND" }).format(amount);
}




// Sự kiện click cho nút phân trang
document.addEventListener('DOMContentLoaded', () => {
    const paginationLinks = document.querySelectorAll('.page-link');
    paginationLinks.forEach(link => {
        link.addEventListener('click', function (e) {
            e.preventDefault();
            const page = this.getAttribute('data-page');
            loadPage(page);
        });
    });
});

// Hàm load trang mới bằng AJAX
function loadPage(page) {
    $.ajax({
        url: '/FoodAndBeverage/ReceptionFoodAndBeverage',
        type: 'GET',
        data: { page: page },
        headers: {
            'X-Requested-With': 'XMLHttpRequest'
        },
        success: function (result) {
            $('#foodAndBeverageList').replaceWith(result);
        },
        error: function (xhr) {
            console.error('Error loading page:', xhr);
        }
    });
}

// Hàm xử lí Search Mode
function toggleView() {
    const toggleButton = document.getElementById('toggleButton');
    const foodList = document.getElementById('foodAndBeverageList');
    const searchResults = document.getElementById('searchResults');
    const searchContainer = document.getElementById('searchContainer');

    if (foodList.style.display === 'none') {
        // Hiển thị Food List, ẩn Search Results và khung tìm kiếm
        foodList.style.display = 'block';
        searchResults.style.display = 'none';
        searchContainer.style.display = 'none';
        toggleButton.textContent = 'Search Mode';
    } else {
        // Hiển thị Search Results và khung tìm kiếm, ẩn Food List
        foodList.style.display = 'none';
        searchResults.style.display = 'block';
        searchContainer.style.display = 'flex'; // Hiển thị dưới dạng flex để giữ đúng định dạng
        toggleButton.textContent = 'Food List Mode';
    }
}


function searchByBookingId() {
    const bookingId = document.getElementById('searchBookingId').value;

    if (!bookingId) {
        Swal.fire({
            icon: 'warning',
            title: 'Notice',
            text: 'Please enter a Booking ID'
        });
        return;
    }

    fetch(`/FoodAndBeverage/SearchByBookingId?bookingId=${bookingId}`, {
        method: 'GET',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
        }
    })
        .then(response => response.json())
        .then(result => {
            if (result.success) {
                displaySearchResults(result.data);
            } else {
                Swal.fire({
                    icon: 'error',
                    title: 'Not Found',
                    text: result.message || 'An error occurred while searching'
                });
            }
        })
        .catch(error => {
            console.error('Lỗi:', error);
            Swal.fire({
                icon: 'error',
                title: 'System Error',
                text: 'No services found'
            });
        });
}


function displaySearchResults(data) {
    const foodList = document.getElementById('foodAndBeverageList');
    const searchResults = document.getElementById('searchResults');
    const toggleButton = document.getElementById('toggleButton');

    if (!data.bookingFoodServiceDetails || data.bookingFoodServiceDetails.length === 0) {
        /*searchResults.innerHTML = `
            <div class="alert alert-info">
                Không có dịch vụ thức ăn nào cho Booking ID này.
            </div>
        `;*/
        Swal.fire({
            icon: 'error',
            title: 'Not Found',
            text: result.message || 'Not Found'
        });
    } else {
        let resultsHtml = `
            <table class="table table-striped">
                <thead>
                    <tr>
                        <th>Food/Beverage</th>
                        <th>Quantity</th>
                        <th>Price</th>
                        <th>Action</th>
                    </tr>
                </thead>
                <tbody>
        `;
        data.bookingFoodServiceDetails.forEach(detail => {
            resultsHtml += `
                <tr id="detail-${detail.bookingFoodServiceDetailID}">
                    <td>${detail.foodAndBeverageService.itemName}</td>
                    <td>${detail.quantity}</td>
                    <td>${detail.foodAndBeverageService.itemPrice.toLocaleString()} ₫</td>
                    <td>
                        <button class="btn btn-danger btn-sm" onclick="deleteFoodServiceDetail(${detail.bookingFoodServiceDetailID}, ${data.bookingFoodServiceID})">
                            <i class="fas fa-trash-alt m-r-5"></i> Delete
                        </button>
                    </td>
                </tr>
            `;
        });
        resultsHtml += `
                </tbody>
            </table>
            <div class="alert alert-info">
                Total Price: ${data.totalPrice.toLocaleString()} ₫
            </div>
        `;
        searchResults.innerHTML = resultsHtml;
    }

    foodList.style.display = 'none';
    searchResults.style.display = 'block';
    toggleButton.textContent = 'Food List Mode';
}

// Thêm hàm xử lý xóa chi tiết dịch vụ
function deleteFoodServiceDetail(detailId, bookingFoodServiceId) {
    // SweetAlert xác nhận trước khi xóa
    Swal.fire({
        title: 'Are you sure you want to delete ?',
        text: "Are you sure you want to delete ?",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Yes, delete it!',
        cancelButtonText: 'Cancel'
    }).then((result) => {
        if (result.isConfirmed) {
            // Người dùng nhấn "Có"
            fetch(`/FoodAndBeverage/DeleteFoodServiceDetail?detailId=${detailId}`, {
                method: 'POST',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json'
                }
            })
                .then(response => response.json())
                .then(result => {
                    if (result.success) {
                        // Xóa dòng khỏi bảng ngay lập tức
                        const rowToRemove = document.getElementById(`detail-${detailId}`);
                        if (rowToRemove) {
                            rowToRemove.remove();
                        }

                        // Hiển thị SweetAlert thành công
                        Swal.fire({
                            title: 'Deleted!',
                            text: 'The service detail has been deleted',
                            icon: 'success',
                            confirmButtonText: 'OK'
                        }).then(() => {
                            // Cập nhật lại thông tin tổng giá mà không cần load lại trang
                            fetch(`/FoodAndBeverage/SearchByBookingId?bookingId=${bookingFoodServiceId}`, {
                                method: 'GET',
                                headers: {
                                    'Accept': 'application/json',
                                    'Content-Type': 'application/json'
                                }
                            })
                                .then(response => response.json())
                                .then(result => {
                                    if (result.success) {
                                        // Cập nhật lại tổng giá trong giao diện
                                        const searchResults = document.getElementById('searchResults');
                                        const totalPriceElement = searchResults.querySelector('.alert-info');
                                        if (totalPriceElement) {
                                            totalPriceElement.innerHTML = `Total Price: ${result.data.totalPrice.toLocaleString()} ₫`;
                                        }
                                    }
                                })
                                .catch(error => {
                                    console.error('Error updating total price:', error);
                                });
                        });
                    } else {
                        // Xử lý trường hợp xóa không thành công
                        Swal.fire({
                            title: 'Error!',
                            text: result.message || 'Unable to delete detail',
                            icon: 'error',
                            confirmButtonText: 'OK'
                        });
                    }
                })
                .catch(error => {
                    console.error('Error:', error);
                    Swal.fire({
                        title: 'Error!',
                        text: 'An error occurred while deleting',
                        icon: 'error',
                        confirmButtonText: 'OK'
                    });
                });
        }
    });
}

document.getElementById("addFoodForm").addEventListener("submit", async function (e) {
    e.preventDefault();
    const formData = new FormData(this);

    // Gửi yêu cầu POST với FormData
    const response = await fetch('https://localhost:7287/FoodAndBeverage/UploadFoodAndBeverage', {
        method: 'POST',
        body: formData
    });

    const result = await response.json();
    alert(result.message || "Thành công!");
});

/*================================= Hàm xử lí cho Employee ===================================*/
$(document).ready(function () {
    // Add Employee
    $('#addEmployeeForm').on('submit', function (event) {
        event.preventDefault();
        
        const employeeData = {
            FullName: $('input[name="employee_name"]').val(),
            PhoneNumber: $('input[name="phone_number"]').val(),
            Email: $('input[name="email"]').val(),
            EmployeeID: employeeId
        };

        // Send data via API
        $.ajax({
            url: '/Employee/CreateEmployee', // API URL
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(employeeData),
            success: function (response) {
                Swal.fire({
                    icon: 'success',
                    title: 'Success',
                    text: 'Employee added successfully!'
                }).then(() => {
                    location.reload(); // Reload page
                });
            },
            error: function (xhr) {
                // Handle different types of errors
                if (xhr.status === 409) {
                    // Duplicate Employee ID error
                    Swal.fire({
                        icon: 'error',
                        title: 'Error',
                        text: 'An employee with this ID number already exists.'
                    });
                } else {
                    // Other errors
                    Swal.fire({
                        icon: 'error',
                        title: 'Error',
                        text: xhr.responseJSON?.message || 'An error occurred while adding the employee.'
                    });
                }
            }
        });
    });

    // Validate Employee ID input on input    

    // Handle Edit button click
    $('.edit-employee').on('click', function () {
        var row = $(this).closest('tr');

        // Fill modal with employee information
        $('#editEmployeeModal input[name="employee_id"]').val(row.find('td:first').text());
        $('#editEmployeeModal input[name="employee_name"]').val(row.find('td:nth-child(2)').text());
        $('#editEmployeeModal input[name="phone_number"]').val(row.find('td:nth-child(3)').text());
        $('#editEmployeeModal input[name="email"]').val(row.find('td:nth-child(4)').text());
    });

    // Handle form submit to update employee
    $('#editEmployeeForm').on('submit', function (e) {
        e.preventDefault();

        var employeeId = $('#editEmployeeModal input[name="employee_id"]').val();
        var employeeData = {
            EmployeeID: parseInt(employeeId),
            FullName: $('#editEmployeeModal input[name="employee_name"]').val(),
            PhoneNumber: $('#editEmployeeModal input[name="phone_number"]').val(),
            Email: $('#editEmployeeModal input[name="email"]').val()
        };

        $.ajax({
            url: '/Employee/UpdateEmployee/' + employeeId,
            type: 'PUT',
            contentType: 'application/json',
            data: JSON.stringify(employeeData),
            success: function (response) {
                Swal.fire({
                    icon: 'success',
                    title: 'Success',
                    text: 'Employee information updated successfully.',
                    confirmButtonText: 'OK'
                }).then(() => {
                    location.reload(); // Reload page
                });
            },
            error: function (xhr) {
                if (xhr.status === 409) { // Duplicate Employee ID
                    Swal.fire({
                        icon: 'error',
                        title: 'Error!',
                        text: 'An employee with this ID number already exists.',
                        confirmButtonText: 'Close'
                    });
                } else {
                    Swal.fire({
                        icon: 'error',
                        title: 'Error!',
                        text: xhr.responseJSON.message || 'An error occurred while updating the employee information.',
                        confirmButtonText: 'Close'
                    });
                }
            }
        });
    });

    // Employee search by name
    $('#employeeSearch').on('keyup', function () {
        var value = $(this).val().toLowerCase();
        $('.datatable tbody tr').filter(function () {
            $(this).toggle($(this).children('td').eq(1).text().toLowerCase().indexOf(value) > -1)
        });
    });

    // Confirm and delete employee
    function confirmEmployeeDelete(employeeId) {
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
                    url: '/Employee/DeleteEmployee/' + employeeId,
                    type: 'DELETE',
                    success: function (response) {
                        Swal.fire({
                            icon: 'success',
                            title: 'Deleted',
                            text: response.message,
                            confirmButtonText: 'OK'
                        }).then(() => {
                            location.reload(); // Reload page
                        });
                    },
                    error: function (xhr) {
                        if (xhr.status === 409) {
                            Swal.fire({
                                icon: 'error',
                                title: 'Error',
                                text: 'The employee cannot be deleted because the employee has already booked.'
                            });
                        } else {
                            Swal.fire({
                                icon: 'error',
                                title: 'Error!',
                                text: 'An error occurred while deleting the employee.',
                                confirmButtonText: 'Close'
                            });
                        }
                    }
                });
            }
        });
    }
});


