(function ($) {
	"use strict";
	var $wrapper = $('.main-wrapper');
	var $pageWrapper = $('.page-wrapper');
	var $slimScrolls = $('.slimscroll');
	var Sidemenu = function () {
		this.$menuItem = $('#sidebar-menu a');
	};

	function init() {
		var $this = Sidemenu;
		$('#sidebar-menu a').on('click', function (e) {
			if ($(this).parent().hasClass('submenu')) {
				e.preventDefault();
			}
			if (!$(this).hasClass('subdrop')) {
				$('ul', $(this).parents('ul:first')).slideUp(350);
				$('a', $(this).parents('ul:first')).removeClass('subdrop');
				$(this).next('ul').slideDown(350);
				$(this).addClass('subdrop');
			} else if ($(this).hasClass('subdrop')) {
				$(this).removeClass('subdrop');
				$(this).next('ul').slideUp(350);
			}
		});
		$('#sidebar-menu ul li.submenu a.active').parents('li:last').children('a:first').addClass('active').trigger('click');
	}
	init();
	$('body').append('<div class="sidebar-overlay"></div>');
	$(document).on('click', '#mobile_btn', function () {
		$wrapper.toggleClass('slide-nav');
		$('.sidebar-overlay').toggleClass('opened');
		$('html').addClass('menu-opened');
		return false;
	});
	$(".sidebar-overlay").on("click", function () {
		$wrapper.removeClass('slide-nav');
		$(".sidebar-overlay").removeClass("opened");
		$('html').removeClass('menu-opened');
	});
	if ($('.page-wrapper').length > 0) {
		var height = $(window).height();
		$(".page-wrapper").css("min-height", height);
	}
	$(window).resize(function () {
		if ($('.page-wrapper').length > 0) {
			var height = $(window).height();
			$(".page-wrapper").css("min-height", height);
		}
	});
	if ($('.select').length > 0) {
		$('.select').select2({
			minimumResultsForSearch: -1,
			width: '100%'
		});
	}
	if ($('.datetimepicker').length > 0) {
		$('.datetimepicker').datetimepicker({
			format: 'DD/MM/YYYY',
			icons: {
				up: "fa fa-angle-up",
				down: "fa fa-angle-down",
				next: 'fa fa-angle-right',
				previous: 'fa fa-angle-left'
			}
		});
		$('.datetimepicker').on('dp.show', function () {
			$(this).closest('.table-responsive').removeClass('table-responsive').addClass('temp');
		}).on('dp.hide', function () {
			$(this).closest('.temp').addClass('table-responsive').removeClass('temp')
		});
	}
	if ($('[data-toggle="tooltip"]').length > 0) {
		$('[data-toggle="tooltip"]').tooltip();
	}
	if ($('.datatable').length > 0) {
		$('.datatable').DataTable({
			"bFilter": false,
		});
	}
	if ($('.clickable-row').length > 0) {
		$(document).on('click', '.clickable-row', function () {
			window.location = $(this).data("href");
		});
	}
	$(document).on('click', '#check_all', function () {
		$('.checkmail').click();
		return false;
	});
	if ($('.checkmail').length > 0) {
		$('.checkmail').each(function () {
			$(this).on('click', function () {
				if ($(this).closest('tr').hasClass('checked')) {
					$(this).closest('tr').removeClass('checked');
				} else {
					$(this).closest('tr').addClass('checked');
				}
			});
		});
	}
	$(document).on('click', '.mail-important', function () {
		$(this).find('i.fa').toggleClass('fa-star').toggleClass('fa-star-o');
	});
	if ($('.summernote').length > 0) {
		$('.summernote').summernote({
			height: 200,
			minHeight: null,
			maxHeight: null,
			focus: false
		});
	}
	if ($('.proimage-thumb li a').length > 0) {
		var full_image = $(this).attr("href");
		$(".proimage-thumb li a").click(function () {
			full_image = $(this).attr("href");
			$(".pro-image img").attr("src", full_image);
			$(".pro-image img").parent().attr("href", full_image);
			return false;
		});
	}
	if ($('#pro_popup').length > 0) {
		$('#pro_popup').lightGallery({
			thumbnail: true,
			selector: 'a'
		});
	}
	if ($slimScrolls.length > 0) {
		$slimScrolls.slimScroll({
			height: 'auto',
			width: '100%',
			position: 'right',
			size: '7px',
			color: '#ccc',
			allowPageScroll: false,
			wheelStep: 10,
			touchScrollStep: 100
		});
		var wHeight = $(window).height() - 60;
		$slimScrolls.height(wHeight);
		$('.sidebar .slimScrollDiv').height(wHeight);
		$(window).resize(function () {
			var rHeight = $(window).height() - 60;
			$slimScrolls.height(rHeight);
			$('.sidebar .slimScrollDiv').height(rHeight);
		});
	}
	/*$(document).on('click', '#toggle_btn', function () {
		if ($('body').hasClass('mini-sidebar')) {
			$('body').removeClass('mini-sidebar');
			$('.subdrop + ul').slideDown();
		} else {
			$('body').addClass('mini-sidebar');
			$('.subdrop + ul').slideUp();
		}
		setTimeout(function () {
			mA.redraw();
			mL.redraw();
		}, 300);
		return false;
	});*/
	$(document).on('mouseover', function (e) {
		e.stopPropagation();
		if ($('body').hasClass('mini-sidebar') && $('#toggle_btn').is(':visible')) {
			var targ = $(e.target).closest('.sidebar').length;
			if (targ) {
				$('body').addClass('expand-menu');
				$('.subdrop + ul').slideDown();
			} else {
				$('body').removeClass('expand-menu');
				$('.subdrop + ul').slideUp();
			}
			return false;
		}
	});
	$(document).on('click', '.add-row', function () {
		var newRow = `<tr>
        <td class="text-center"></td>
                                                   <td>
                                                        <div class="form-group">
                                                            <div class="input-group">
                                                                <div class="input-group-prepend">
                                                                    <span class="input-group-text"><i class="fas fa-id-badge"></i></span>
                                                                </div>
                                                                <select class="form-control room-id">
                                                                    <option value="" disabled selected>Select</option>
                                                                    <option value="1">1</option>
                                                                    <option value="2">2</option>
                                                                    <option value="3">3</option>
                                                                </select>
                                                            </div>
                                                        </div>
                                                    </td>
                                                    <td>
                                                        <div class="form-group">
                                                            <div class="input-group">
                                                                <div class="input-group-prepend">
                                                                    <span class="input-group-text"><i class="fas fa-money-bill-wave"></i></span>
                                                                </div>
                                                                <input class="form-control unit-cost" readonly="" type="number" inputmode="numeric" placeholder="" required>
                                                            </div>
                                                        </div>
                                                    </td>
                                                    <td>
                                                        <div class="form-group">
                                                            <div class="input-group">
                                                                <div class="input-group-prepend">
                                                                    <span class="input-group-text"><i class="fas fa-clock"></i></span>
                                                                </div>
                                                                <input class="form-control quantity" type="number" inputmode="numeric" placeholder="Enter Duration" required>
                                                            </div>
                                                        </div>
                                                    </td>
                                                    <td>
                                                        <div class="form-group">
                                                            <div class="input-group">
                                                                <div class="input-group-prepend">
                                                                    <span class="input-group-text"><i class="fas fa-utensils"></i></span>
                                                                </div>
                                                                <input class="form-control services-fee" readonly="" type="number" inputmode="numeric" placeholder="" required>
                                                            </div>
                                                        </div>
                                                    </td>
                                                    <td>
                                                        <div class="form-group">
                                                            <div class="input-group">
                                                                <div class="input-group-prepend">
                                                                    <span class="input-group-text"><i class="fas fa-dollar-sign"></i></span>
                                                                </div>
                                                                <input class="form-control amount" readonly="" type="number" inputmode="numeric" placeholder="" required>
                                                            </div>
                                                        </div>
                                                    </td>
                                                    <td class="text-center"">
                                                        <a href="javascript:void(0)" class="text-success font-18 add-row" title="Add"><i class="fas fa-plus"></i></a>
														<a href="javascript:void(0)" class="text-danger font-18 remove-row" title="Remove"><i class="fas fa-trash"></i></a>
                                                    </td>                                               
    </tr>`;
		$('#invoiceTable tbody').append(newRow);
		updateRowActions();
	});

	$(document).on('click', '.remove-row', function () {
		$(this).closest('tr').remove();
		updateRowActions();
	});

	function updateRowActions() {
		$('#invoiceTable tbody tr').each(function (index, row) {
			$(row).find('td:first').text(index + 1);
			$(row).find('.add-row').show();
			$(row).find('.remove-row').show();
		});
		// Ensure the first row always has the remove button hidden
		$('#invoiceTable tbody tr:first').find('.remove-row').hide();
		// Ensure the last row always has the add button hidden
		if ($('#invoiceTable tbody tr').length > 1) {
			$('#invoiceTable tbody tr:last').find('.add-row').hide();
		}
		// Ensure the middle rows have add action
		$('#invoiceTable tbody tr').not(':first').not(':last').each(function () {
			$(this).find('.add-row').show();
			$(this).find('.remove-row').hide();
		});
	}
	// Initialize the table with the correct actions
	updateRowActions();

	$(document).ready(function () {
        // Hàm kiểm tra trạng thái mini-sidebar và ẩn/hiện nút
        function toggleButtons() {
            if ($('body').hasClass('mini-sidebar')) {
                $('.toggle_btn_1').show(); // Hiện nút Toggle 1
                $('.toggle_btn_2').hide(); // Ẩn nút Toggle 2
            } else {
                $('.toggle_btn_1').hide(); // Ẩn nút Toggle 1
                $('.toggle_btn_2').show(); // Hiện nút Toggle 2
            }
        }

        // Gọi hàm kiểm tra ngay khi load trang
        toggleButtons();

        // Thêm sự kiện click để thay đổi trạng thái mini-sidebar
		$(document).on('click', '#toggle_btn', function () {
			$('body').toggleClass('mini-sidebar'); // Thay đổi trạng thái mini-sidebar
			toggleButtons(); // Cập nhật trạng thái nút
		});
    });

    $(document).ready(function () {
        $('.datatable').DataTable();
	});
	function changeRoomStatus(element) {
		var newStatus = element.getAttribute('data-status');
		var dropdownToggle = element.closest('.dropdown').querySelector('.dropdown-toggle');
		dropdownToggle.textContent = newStatus;

		// Update the button class based on the new status
		dropdownToggle.classList.remove('bg-success-light', 'bg-danger-light', 'bg-warning-light');
		if (newStatus === 'Available') {
			dropdownToggle.classList.add('bg-success-light');
		} else if (newStatus === 'Occupied') {
			dropdownToggle.classList.add('bg-danger-light');
		} else if (newStatus === 'Under Maintainance/Cleaning') {
			dropdownToggle.classList.add('bg-warning-light');
		}

		// Add your AJAX call here to update the status in the backend if needed
	}

	// Ensure the function is globally accessible
	window.changeRoomStatus = changeRoomStatus;
})(jQuery);


function changeStatus(element) {
	var newStatus = element.getAttribute('data-status');
	var dropdownToggle = element.closest('.dropdown').querySelector('.dropdown-toggle');
	dropdownToggle.textContent = newStatus;

	// Update the button class based on the new status
	dropdownToggle.classList.remove('bg-success-light', 'bg-danger-light', 'bg-warning-light');
	if (newStatus === 'Paid' || newStatus === 'Confirm') {
		dropdownToggle.classList.add('bg-success-light');
	} else if (newStatus === 'Pending') {
		dropdownToggle.classList.add('bg-warning-light');
	} else if (newStatus === 'Cancelled') {
		dropdownToggle.classList.add('bg-danger-light');
	}
	// Add your AJAX call here to update the status in the backend if needed
}
// Ensure the function is globally accessible
window.changeStatus = changeStatus;

/*document.addEventListener('DOMContentLoaded', function () {
    // Lấy tất cả các link phân trang
    const pageLinks = document.querySelectorAll('.pagination .page-link');

    pageLinks.forEach(link => {
        link.addEventListener('click', function (e) {
            

            // Xóa active class từ tất cả các page item
            document.querySelectorAll('.page-item').forEach(item => {
                item.classList.remove('active');
            });

            // Thêm active class vào page item được click
            if (!this.parentElement.classList.contains('disabled')) {
                this.parentElement.classList.add('active');

                // Ở đây bạn có thể thêm code để load dữ liệu mới
                // Ví dụ: loadPage(this.textContent);
            }
        });
    });
});*/

document.addEventListener('DOMContentLoaded', function () {
	// Hàm load dữ liệu trang mới
	const isReception = document.body.getAttribute('data-is-reception') === 'true';
	const controllerEndpoint = isReception ? 'ReceptionFoodAndBeverage' : 'AdminFoodAndBeverage';
    function loadPage(page) {
        // Hiển thị loading indicator (nếu cần)
        const cardBody = document.querySelector('.booking_card');
        cardBody.style.opacity = '0.5';


		// Sử dụng endpoint tương ứng với view hiện tại
		fetch(`/FoodAndBeverage/${controllerEndpoint}?page=${page}`, {
			headers: {
				'X-Requested-With': 'XMLHttpRequest'
			}
		})
        .then(response => response.text())
        .then(html => {
            // Cập nhật nội dung
            cardBody.innerHTML = html;
            cardBody.style.opacity = '1';
            
            // Cập nhật URL mà không reload trang
			history.pushState({ page: page }, '', `/FoodAndBeverage/${controllerEndpoint}?page=${page}`);
            
            // Khởi tạo lại event listeners cho các nút phân trang mới
			initPaginationHandlers();

        })
        .catch(error => {
            console.error('Error:', error);
            cardBody.style.opacity = '1';
        });
    }

    // Hàm khởi tạo event handlers cho phân trang
    function initPaginationHandlers() {
        const pageLinks = document.querySelectorAll('.pagination .page-link');
        pageLinks.forEach(link => {
            link.addEventListener('click', function (e) {
                e.preventDefault();
                
                // Kiểm tra nếu nút không bị disable
                if (!this.parentElement.classList.contains('disabled')) {
                    const page = this.getAttribute('data-page');
                    if (page) {
                        loadPage(page);
                    }
                }
            });
        });
    }

    // Xử lý nút Back/Forward của trình duyệt
    window.addEventListener('popstate', function (e) {
        if (e.state && e.state.page) {
            loadPage(e.state.page);
        }
    });

    // Khởi tạo handlers lần đầu
    initPaginationHandlers();
});

let currentOrder = [];

function addToOrder(item) {
	// Convert JSON string back to an object
	const parsedItem = typeof item === 'string' ? JSON.parse(item) : item;

	const existingItem = currentOrder.find(i => i.id === parsedItem.id);
	if (existingItem) {
		existingItem.quantity = (existingItem.quantity || 1) + 1;
	} else {
		currentOrder.push({ ...parsedItem, quantity: 1 });
	}
	updateOrderDisplay();
}

function updateOrderDisplay() {
	const orderItemsDiv = document.getElementById('orderItems');
	const orderTotalElement = document.getElementById('orderTotal');
	let total = 0;

	orderItemsDiv.innerHTML = currentOrder.map(item => {
		const itemTotal = item.price * (item.quantity || 1);
		total += itemTotal;

		// Định dạng giá tiền sang VNĐ
		const formattedPrice = new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(itemTotal);

		return `
            <div class="d-flex justify-content-between align-items-center mb-2">
                <div>
                    <span class="fw-bold">${item.name}</span> x ${item.quantity}
                </div>
                <div>
                    ${formattedPrice}
                    <button class="btn btn-sm btn-danger ms-2" onclick="removeFromOrder(${item.id})">
                        <i class="fas fa-times"></i>
                    </button>
                </div>
            </div>
        `;
	}).join('');

	// Hiển thị tổng giá trị đơn hàng
	orderTotalElement.textContent = new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(total);
}


function removeFromOrder(itemId) {
	currentOrder = currentOrder.filter(item => item.id !== itemId);
	updateOrderDisplay();
}

function submitOrder() {
	if (currentOrder.length === 0) {
		alert('Please add items to your order first');
		return;
	}

	// Typically, you would send the order to the server here
	console.log('Submitting order:', currentOrder);
	alert('Order submitted successfully!');
	currentOrder = [];
	updateOrderDisplay();
}

document.addEventListener('DOMContentLoaded', function () {
	var today = new Date().toISOString().split('T')[0];
	document.getElementById('fromDate').value = today;
	document.getElementById('toDate').value = today;	
});

document.addEventListener('DOMContentLoaded', function () {
	var today = new Date().toISOString().split('T')[0];
	document.getElementById('invoiceDate').value = today;
});

document.addEventListener("DOMContentLoaded", function () {
	// Lấy ô input
	var checkInInput = document.querySelector('input[name="check_in"]');
	var checkOutInput = document.querySelector('input[name="check_out"]');
	// Lấy ngày hiện tại
	var today = new Date().toISOString().split('T')[0];
	// Gán giá trị mặc định cho ô input
	checkInInput.value = today;
	checkOutInput.value = today;
});



function confirmInvoiceDelete(invoiceId) {
	Swal.fire({
		title: 'Are you sure?',
		text: "You won't be able to revert this!",
		icon: 'error',
		showCancelButton: true,
		confirmButtonColor: '#2563EB',
		cancelButtonColor: '#d33',
		confirmButtonText: 'Yes, delete it!'
	}).then((result) => {
		if (result.isConfirmed) {
			// Call your delete function here
			deleteInvoice(invoiceId);
		}
	})
}

function deleteInvoice(invoiceId) {
	// Implement your delete logic here
	console.log("Deleting invoice with ID:", invoiceId);
	Swal.fire({
		title: 'Deleted!',
		text: 'The invoice has been deleted.',
		icon: 'success',
		confirmButtonText: 'OK',
		confirmButtonColor: '#2563EB'
	});
}

$(document).on('input', '.unit-cost, .quantity, .services-fee', function () {
	var $row = $(this).closest('tr');
	var unitCost = parseFloat($row.find('.unit-cost').val()) || 0;
	var quantity = parseFloat($row.find('.quantity').val()) || 0;
	var servicesFee = parseFloat($row.find('.services-fee').val()) || 0;
	var amount = (unitCost * quantity) + servicesFee;
	$row.find('.amount').val(amount.toFixed(2));
});

function calculateGrandTotal() {
	var totalAmount = 0;
	$('#invoiceTable tbody tr').each(function () {
		var amount = parseFloat($(this).find('.amount').val().replace(/,/g, '').replace('$', '')) || 0;
		totalAmount += amount;
	});

	var discount = parseFloat($('#discount').val()) || 0;
	if (isNaN(discount) || discount < 0 || discount > 100) {
		discount = '';
		alert('Discount must be between 0 and 100.');
		$('#discount').val(discount);
	}
	var grandTotal = totalAmount - (totalAmount * (discount / 100));

	$('#totalAmount').text(formatNumber(totalAmount.toFixed(2)) + ' VND');
	$('#grandTotal').text(formatNumber(grandTotal.toFixed(2)) + ' VND');
}
function formatNumber(num) {
	return new Intl.NumberFormat('vi-VN').format(num);
}

$(document).on('input', '#discount', function () {
	calculateGrandTotal();
});

$(document).ready(function () {
	function updateGrandTotal() {
		calculateGrandTotal();
		requestAnimationFrame(updateGrandTotal);
	}
	requestAnimationFrame(updateGrandTotal); // Start the update loop
});

function validateInvoiceForm(event) {
	event.preventDefault();
	var form = document.getElementById('invoiceForm');
	if (form.checkValidity()) {
		Swal.fire({
			title: 'Are you sure?',
			text: "Do you want to save the invoice?",
			icon: 'warning',
			showCancelButton: true,
			confirmButtonColor: '#3085d6',
			cancelButtonColor: '#d33',
			confirmButtonText: 'Yes, save it!'
		}).then((result) => {
			if (result.isConfirmed) {
				// Simulate form submission
				Swal.fire({
					title: 'Saved!',
					text: 'The invoice has been saved.',
					icon: 'success',
					confirmButtonText: 'OK',
					confirmButtonColor: '#2563EB'
				});
			}
		});
	} else {
		form.reportValidity();
	}
}

function confirmRoomDelete(roomId) {
	Swal.fire({
		title: 'Are you sure?',
		text: "You won't be able to revert this!",
		icon: 'error',
		showCancelButton: true,
		confirmButtonColor: '#2563EB',
		cancelButtonColor: '#d33',
		confirmButtonText: 'Yes, delete it!',
		cancelButtonText: 'No, cancel!',
	}).then((result) => {
		if (result.isConfirmed) {
			// Call your delete function here
			deleteRoom(roomId);
		}
	})
}


function deleteRoom(roomId) {
	// Implement your delete logic here
	console.log("Deleting room with ID:", roomId);
	Swal.fire({
		title: 'Deleted!',
		text: 'The room has been deleted.',
		icon: 'success',
		confirmButtonText: 'OK'
	});
}

function confirmSaveRoom(event, formId) {
	event.preventDefault();
	var form = document.getElementById(formId);
	if (form.checkValidity()) {
		Swal.fire({
			title: 'Are you sure?',
			text: "Do you want to save this room?",
			icon: 'warning',
			showCancelButton: true,
			confirmButtonColor: '#2563EB',
			cancelButtonColor: '#d33',
			confirmButtonText: 'Yes, save it!'
		}).then((result) => {
			if (result.isConfirmed) {
				// Simulate form submission
				Swal.fire({
					title: 'Saved!',
					text: 'The room has been saved.',
					icon: 'success',
					confirmButtonText: 'OK'
				}).then(() => {
					form.submit();
				});
			}
		});
	} else {
		form.reportValidity();
	}
}