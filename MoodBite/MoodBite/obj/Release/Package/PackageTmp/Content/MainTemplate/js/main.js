(function ($) {
    "use strict";
    
    // Back to top button
    $(window).scroll(function () {
        if ($(this).scrollTop() > 200) {
            $('.back-to-top').fadeIn('slow');
        } else {
            $('.back-to-top').fadeOut('slow');
        }
    });
    $('.back-to-top').click(function () {
        $('html, body').animate({scrollTop: 0}, 1500, 'easeInOutExpo');
        return false;
    });

    $(document).ready(function () {
        $("#show-hide-password a").on('click', function (event) {
            event.preventDefault();
            var $passwordInput = $('#show-hide-password input');
            var $eyeIcon = $('#show-hide-password i');

            if ($passwordInput.attr("type") == "text") {
                $passwordInput.attr('type', 'password');
                $eyeIcon.addClass("fa-eye-slash").removeClass("fa-eye");
                console.log('working if');
            } else if ($passwordInput.attr("type") == "password") {
                $passwordInput.attr('type', 'text');
                $eyeIcon.addClass("fa-eye").removeClass("fa-eye-slash");
                console.log('working else');
            }
        });
    });

    
    // Sticky Navbar
    $(window).scroll(function () {
        if ($(this).scrollTop() > 0) {
            $('.navbar').addClass('nav-sticky');
        } else {
            $('.navbar').removeClass('nav-sticky');
        }
    });
    
    
    // Dropdown on mouse hover
    $(document).ready(function () {
        function toggleNavbarMethod() {
            if ($(window).width() > 992) {
                $('.navbar .dropdown').on('mouseover', function () {
                    $('.dropdown-toggle', this).trigger('click');
                }).on('mouseout', function () {
                    $('.dropdown-toggle', this).trigger('click').blur();
                });
                console.log("inside if");
            } else {
                $('.navbar .dropdown').off('mouseover').off('mouseout');
                console.log("inside else");
            }
        }
        toggleNavbarMethod();
        $(window).resize(toggleNavbarMethod);
        console.log("working");
    });

    
    // Main carousel
    $(".carousel .owl-carousel").owlCarousel({
        autoplay: true,
        animateOut: 'fadeOut',
        animateIn: 'fadeIn',
        items: 1,
        smartSpeed: 400,
        dots: false,
        loop: true,
        nav : false
    });
    
    // Modal Video
    $(document).ready(function () {
        var $videoSrc;
        $('.btn-play').click(function () {
            $videoSrc = $(this).data("src");
        });
        console.log($videoSrc);

        $('#videoModal').on('shown.bs.modal', function (e) {
            $("#video").attr('src', $videoSrc + "?autoplay=1&amp;modestbranding=1&amp;showinfo=0");
        })

        $('#videoModal').on('hide.bs.modal', function (e) {
            $("#video").attr('src', $videoSrc);
        })
    });
    
    
    // Date and time picker
    //$('#date').datetimepicker({
    //    format: 'L'
    //});
    //$('#time').datetimepicker({
    //    format: 'LT'
    //});

    $(function () {
        $('#date').datetimepicker({
            format: 'MM/DD/YYYY'
        });
    });


    // Testimonials carousel
    $(".testimonials-carousel").owlCarousel({
        center: true,
        autoplay: true,
        dots: true,
        loop: false,
        responsive: {
            0:{
                items:1
            },
            576:{
                items:1
            },
            768:{
                items:2
            },
            992:{
                items:3
            }
        }
    });
    
    
    // Related post carousel
    $(".related-slider").owlCarousel({
        autoplay: true,
        dots: false,
        loop: false,
        nav : true,
        navText : [
            '<i class="fa fa-angle-left" aria-hidden="true"></i>',
            '<i class="fa fa-angle-right" aria-hidden="true"></i>'
        ],
        responsive: {
            0:{
                items:1
            },
            576:{
                items:1
            },
            768:{
                items:2
            }
        }
    });

    //$(document).ready(function () {
    //    $('.form-check-input').change(function () {
    //        var isChecked = $(this).is(':checked');
    //        var ingredientName = $(this).siblings('.form-check-label').find('.ingredient-name');

    //        if (isChecked) {
    //            ingredientName.addClass('crossed-out');
    //        } else {
    //            ingredientName.removeClass('crossed-out');
    //        }
    //    });
    //});
})(jQuery);

function showLoader() {
    $('#loaderContainer').show();
    $('html').css('pointer-events', 'none');
    $('html').css('cursor', 'progress !important');
}

function hideLoader() {
    $('#loaderContainer').hide();
    $('html').css('pointer-events', 'auto');
    $('html').css('cursor', 'auto');
}

$(document).ready(function () {
    $('.newpwd').click(function () {
        var passwordField = $('#show-hide-new-password input');
        var icon = $(this).find('i');

        if (passwordField.attr('type') === 'password') {
            passwordField.attr('type', 'text');
            icon.removeClass('fa-eye').addClass('fa-eye-slash');
        } else {
            passwordField.attr('type', 'password');
            icon.removeClass('fa-eye-slash').addClass('fa-eye');
        }
    });
});
$(document).ready(function () {
    $('.oldpwd').click(function () {
        var passwordField = $('#show-hide-old-password input');
        var icon = $(this).find('i');

        if (passwordField.attr('type') === 'password') {
            passwordField.attr('type', 'text');
            icon.removeClass('fa-eye').addClass('fa-eye-slash');
        } else {
            passwordField.attr('type', 'password');
            icon.removeClass('fa-eye-slash').addClass('fa-eye');
        }
    });
});

