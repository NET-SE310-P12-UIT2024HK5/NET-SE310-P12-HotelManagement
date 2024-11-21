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
	$(document).on('click', '#toggle_btn', function () {
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
	});
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
        <td></td>
                                                    <td>
                                                        <div class="form-group">
                                                            <div class="input-group">
                                                                <div class="input-group-prepend">
                                                                    <span class="input-group-text"><i class="fas fa-id-badge"></i></span>
                                                                </div>
                                                                <input class="form-control room-id" type="text" placeholder="Enter Room ID">
                                                            </div>
                                                        </div>
                                                    </td>
                                                    <td>
                                                        <div class="form-group">
                                                            <div class="input-group">
                                                                <div class="input-group-prepend">
                                                                    <span class="input-group-text"><i class="fas fa-coins"></i></span>
                                                                </div>
                                                                <input class="form-control unit-cost" type="text" placeholder="Enter Unit Cost">
                                                            </div>
                                                        </div>
                                                    </td>
                                                    <td>
                                                        <div class="form-group">
                                                            <div class="input-group">
                                                                <div class="input-group-prepend">
                                                                    <span class="input-group-text"><i class="fas fa-hashtag"></i></span>
                                                                </div>
                                                                <input class="form-control quantity" type="number" placeholder="Enter Quantity">
                                                            </div>
                                                        </div>
                                                    </td>
                                                    <td>
                                                        <div class="form-group">
                                                            <div class="input-group">
                                                                <div class="input-group-prepend">
                                                                    <span class="input-group-text"><i class="fas fa-dollar-sign"></i></span>
                                                                </div>
                                                                <input class="form-control amount" readonly="" type="text" placeholder="Amount">
                                                            </div>
                                                        </div>
                                                    </td>
                                                    <td>
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

	$(document).on('input', '.unit-cost, .quantity', function () {
		var $row = $(this).closest('tr');
		var calculatorunitCost = parseFloat($row.find('.unit-cost').val()) || 0;
		var calculatorquantity = parseInt($row.find('.quantity').val()) || 0;


		var amount = calculatorunitCost * calculatorquantity;
		$row.find('.amount').val(formatNumber(amount.toFixed(2)) + ' $');
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

		$('#totalAmount').text('$ ' +formatNumber(totalAmount.toFixed(2)));
		$('#grandTotal').text('$ ' + formatNumber(grandTotal.toFixed(2)));
	}
	function formatNumber(num) {
		return new Intl.NumberFormat('en-US').format(num);
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

	$(document).ready(function () {
		$('.dropdown-menu .dropdown-item').on('click', function (e) {
			e.preventDefault();
			var newStatus = $(this).data('status');
			var dropdownToggle = $(this).closest('.dropdown').find('.dropdown-toggle');
			dropdownToggle.text(newStatus);
			dropdownToggle.removeClass('bg-success-light bg-warning-light bg-danger-light');
			if (newStatus === 'Paid') {
				dropdownToggle.addClass('bg-success-light');
			} else if (newStatus === 'Pending') {
				dropdownToggle.addClass('bg-warning-light');
			} else if (newStatus === 'Cancelled') {
				dropdownToggle.addClass('bg-danger-light');
			}
			// Add your AJAX call here to update the status in the backend
		});
	});
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
	function navigateToEdit() {
		window.location.href = '/Rooms/Edit';
	}
})(jQuery);
