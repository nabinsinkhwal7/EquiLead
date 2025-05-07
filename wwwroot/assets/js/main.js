/* ============ Main JS ============ */

(function ($) {
    "use strict";

    var windowOn = $(window);
    

    /*======================================
    Preloader activation
    ========================================*/
    $(window).on("load", function (event) {
        $("#preloader").delay(1000).fadeOut(500);
    });

    $(".preloader-close").on("click", function () {
        $("#preloader").delay(0).fadeOut(500);
    });

    $(document).ready(function () {

        if (navigator.userAgent.toLowerCase().indexOf('firefox') > -1) {
            $('body').addClass('firefox');
        }

        var header = $(".header"),
            stickyHeader = $(".primary-header");

        function menuSticky(w) {
            if (w.matches) {

                $(window).on("scroll", function () {
                    var scroll = $(window).scrollTop();
                    if (scroll >= 110) {
                        stickyHeader.addClass("fixed");
                    } else {
                        stickyHeader.removeClass("fixed");
                    }
                });
                if ($(".header").length > 0) {
                    var headerHeight = document.querySelector(".header"),
                        setHeaderHeight = headerHeight.offsetHeight;
                    header.each(function () {
                        $(this).css({
                            'height': setHeaderHeight + 'px'
                        });
                    });
                }
            }
        }

        var minWidth = window.matchMedia("(min-width: 992px)");
        if (header.hasClass("sticky-active")) {
            menuSticky(minWidth);
        }

        //Mobile Menu Js
        $(".mobile-menu-items").meanmenu({
            meanMenuContainer: ".side-menu-wrap",
            meanScreenWidth: "992",
            meanMenuCloseSize: "30px",
            meanRemoveAttrs: true,
            meanExpand: ['<i class="fa-solid fa-caret-down"></i>'],
        });

        // Mobile Sidemenu
        $(".mobile-side-menu-toggle").on("click", function () {
            $(".mobile-side-menu, .mobile-side-menu-overlay").toggleClass("is-open");
        });

        $(".mobile-side-menu-close, .mobile-side-menu-overlay").on("click", function () {
            $(".mobile-side-menu, .mobile-side-menu-overlay").removeClass("is-open");
        });

        // Popup Search Box
        $(function () {
            $("#popup-search-box").removeClass("toggled");

            $(".dl-search-icon").on("click", function (e) {
                e.stopPropagation();
                $("#popup-search-box").toggleClass("toggled");
                $("#popup-search").focus();
            });

            $("#popup-search-box input").on("click", function (e) {
                e.stopPropagation();
            });

            $("#popup-search-box, body").on("click", function () {
                $("#popup-search-box").removeClass("toggled");
            });
        });

        // Popup Sidebox
        function sideBox() {
            $("body").removeClass("open-sidebar");
            $(document).on("click", ".sidebar-trigger", function (e) {
                e.preventDefault();
                $("body").toggleClass("open-sidebar");
            });
            $(document).on("click", ".sidebar-trigger.close, #sidebar-overlay", function (e) {
                e.preventDefault();
                $("body.open-sidebar").removeClass("open-sidebar");
            });
        }

        // Nice Select Js
        $(".nice-select select").niceSelect();

        sideBox();

        // Venobox Video
        new VenoBox({
            selector: ".video-popup, .img-popup",
            bgcolor: "transparent",
            numeration: true,
            infinigall: true,
            spinner: "plane",
        });

        // Data Background
        $("[data-background").each(function () {
            $(this).css("background-image", "url( " + $(this).attr("data-background") + "  )");
        });

        // Custom Cursor
        // $("body").append('<div class="mt-cursor"></div>');
        // var cursor = $(".mt-cursor"),
        //     linksCursor = $("a, .swiper-nav, button, .cursor-effect"),
        //     crossCursor = $(".cross-cursor");

        // $(window).on("mousemove", function (e) {
        //     cursor.css({
        //         transform: "translate(" + (e.clientX - 15) + "px," + (e.clientY - 15) + "px)",
        //         visibility: "inherit",
        //     });
        // });

        /* Odometer */
        $(".odometer").waypoint(
            function () {
                var odo = $(".odometer");
                odo.each(function () {
                    var countNumber = $(this).attr("data-count");
                    $(this).html(countNumber);
                });
            },
            {
                offset: "80%",
                triggerOnce: true,
            }
        );

        // Wow JS Active
        new WOW().init();

        // Isotop
        $(".filter-items").imagesLoaded(function () {
            // Add isotope click function
            $(".project-filter li").on("click", function () {
                $(".project-filter li").removeClass("active");
                $(this).addClass("active");

                var selector = $(this).attr("data-filter");
                $(".filter-items").isotope({
                    filter: selector,
                    animationOptions: {
                        duration: 750,
                        easing: "linear",
                        queue: false,
                    },
                });
                return false;
            });

            $(".filter-items").isotope({
                itemSelector: ".single-item",
                layoutMode: "fitRows",
                fitRows: {
                    gutter: 0,
                },
            });
        });

        // Sponsor Carousel
        var swiperSponsor = new Swiper(".sponsor-carousel", {
            slidesPerView: 5,
            spaceBetween: 50,
            slidesPerGroup: 1,
            loop: true,
            autoplay: true,
            grabcursor: true,
            speed: 400,
            breakpoints: {
                320: {
                    slidesPerView: 1,
                    slidesPerGroup: 1,
                    spaceBetween: 25,
                },
                767: {
                    slidesPerView: 3,
                    slidesPerGroup: 1,
                    spaceBetween: 30,
                },
                1024: {
                    slidesPerView: 5,
                    slidesPerGroup: 1,
                },
            },
        });

        // Sponsor Carousel
        var swiperSponsor = new Swiper(".sponsor-carousel-6", {
            slidesPerView: 5,
            spaceBetween: 45,
            slidesPerGroup: 1,
            loop: true,
            autoplay: true,
            grabcursor: true,
            speed: 400,
            breakpoints: {
                320: {
                    slidesPerView: 1,
                    slidesPerGroup: 1,
                    spaceBetween: 25,
                },
                767: {
                    slidesPerView: 3,
                    slidesPerGroup: 1,
                    spaceBetween: 30,
                },
                1024: {
                    slidesPerView: 5,
                    slidesPerGroup: 1,
                },
            },
        });

        // Testimonial Carousel
        var swiperTesti = new Swiper(".testi-carousel", {
            slidesPerView: 1,
            spaceBetween: 30,
            slidesPerGroup: 1,
            loop: true,
            autoplay: true,
            grabcursor: true,
            speed: 400,
            pagination: {
                el: ".swiper-pagination",
                clickable: true,
            },
        });
        // Testimonial Carousel
        var swiperTesti = new Swiper(".testi-carousel-4", {
            slidesPerView: 4,
            spaceBetween: 30,
            slidesPerGroup: 1,
            loop: true,
            autoplay: true,
            grabcursor: true,
            speed: 400,
            pagination: {
                el: ".swiper-pagination",
                clickable: true,
            },
            breakpoints: {
                320: {
                    slidesPerView: 1,
                    slidesPerGroup: 1,
                    spaceBetween: 25,
                },
                767: {
                    slidesPerView: 4,
                    slidesPerGroup: 1,
                    spaceBetween: 25,
                },
                1400: {
                    slidesPerView: 4,
                    slidesPerGroup: 1,
                    spaceBetween: 25,
                },
                1600: {
                    slidesPerView: 4,
                    slidesPerGroup: 1,
                    spaceBetween: 30,
                },
            },
        });
        // Testimonial Carousel
        var swiperTesti = new Swiper(".community-platform-carousel", {
            slidesPerView: 2,
            spaceBetween: 30,
            slidesPerGroup: 1,
            loop: true,
            autoplay: true,
            grabcursor: true,
            speed: 400,
            pagination: {
                el: ".swiper-pagination",
                clickable: true,
            },
            breakpoints: {
                320: {
                    slidesPerView: 1,
                    slidesPerGroup: 1,
                    spaceBetween: 25,
                },
                767: {
                    slidesPerView: 2,
                    slidesPerGroup: 1,
                    spaceBetween: 25,
                },
                1400: {
                    slidesPerView: 2,
                    slidesPerGroup: 1,
                    spaceBetween: 25,
                },
                1600: {
                    slidesPerView: 2,
                    slidesPerGroup: 1,
                    spaceBetween: 30,
                },
            },
        });
        // Testimonial Carousel
        var swiperTesti = new Swiper(".blog-carousel-new", {
            slidesPerView: 3,
            spaceBetween: 30,
            slidesPerGroup: 1,
            loop: false,
            autoplay: true,
            grabcursor: false,
            allowTouchMove: false,
            speed: 500,
            // navigation: {
            //     nextEl: '.swiper-button-next',
            //     prevEl: '.swiper-button-prev',
            //   },
            pagination: {
                el: ".swiper-pagination",
                clickable: true,
            },
            breakpoints: {
                320: {
                    slidesPerView: 1,
                    slidesPerGroup: 1,
                    spaceBetween: 25,
                },
                767: {
                    slidesPerView: 2,
                    slidesPerGroup: 1,
                    spaceBetween: 25,
                },
                1300: {
                    slidesPerView: 3,
                    slidesPerGroup: 1,
                    spaceBetween: 25,
                },
                1400: {
                    slidesPerView: 3,
                    slidesPerGroup: 1,
                    spaceBetween: 25,
                },
                1600: {
                    slidesPerView: 3,
                    slidesPerGroup: 1,
                    spaceBetween: 30,
                },
            },
        });
        // team Carousel
        var swiperTesti = new Swiper(".team-carousel-new", {
            slidesPerView: 4,
            spaceBetween: 30,
            slidesPerGroup: 1,
            loop: false,
            autoplay: false,
            grabcursor: false,
            allowTouchMove: false,
            // autoHeight: true,
            speed: 400,
            navigation: {
                nextEl: '.swiper-button-next',
                prevEl: '.swiper-button-prev',
              },
            pagination: {
                el: ".swiper-pagination",
                clickable: true,
            },
            breakpoints: {
                320: {
                    slidesPerView: 1,
                    slidesPerGroup: 1,
                    spaceBetween: 25,
                },
                767: {
                    slidesPerView: 2,
                    slidesPerGroup: 1,
                    spaceBetween: 25,
                },
                1300: {
                    slidesPerView: 3,
                    slidesPerGroup: 1,
                    spaceBetween: 25,
                },
                1400: {
                    slidesPerView: 4,
                    slidesPerGroup: 1,
                    spaceBetween: 25,
                },
                1600: {
                    slidesPerView: 4,
                    slidesPerGroup: 1,
                    spaceBetween: 30,
                },
            },
        });
        // Testimonial Carousel
        var swiperTesti = new Swiper(".testi-carousel-5", {
            slidesPerView: 3,
            spaceBetween: 30,
            slidesPerGroup: 1,
            loop: true,
            autoplay: true,
            grabcursor: true,
            speed: 400,
            pagination: {
                el: ".swiper-pagination",
                clickable: true,
            },
            breakpoints: {
                320: {
                    slidesPerView: 1,
                    slidesPerGroup: 1,
                    spaceBetween: 25,
                },
                767: {
                    slidesPerView: 2,
                    slidesPerGroup: 1,
                    spaceBetween: 30,
                },
                1024: {
                    slidesPerView: 3,
                    slidesPerGroup: 1,
                },
            },
        });
        // Testimonial Carousel
        var swiperTesti = new Swiper(".testi-carousel-6", {
            slidesPerView: 2,
            spaceBetween: 30,
            slidesPerGroup: 1,
            loop: true,
            autoplay: true,
            grabcursor: true,
            speed: 400,
            pagination: {
                el: ".swiper-pagination",
                clickable: true,
            },
            breakpoints: {
                320: {
                    slidesPerView: 1,
                    slidesPerGroup: 1,
                    spaceBetween: 25,
                },
                767: {
                    slidesPerView: 2,
                    slidesPerGroup: 1,
                    spaceBetween: 30,
                },
            },
            navigation: {
                nextEl: ".slider-arrow .slider-prev",
                prevEl: ".slider-arrow .slider-next",
            },
        });
        // Testimonial Carousel
        var swiperTesti = new Swiper(".testi-carousel-2", {
            slidesPerView: 1,
            spaceBetween: 30,
            slidesPerGroup: 1,
            loop: true,
            autoplay: true,
            grabcursor: true,
            speed: 400,
            navigation: {
                nextEl: ".testi-carousel-wrap .swiper-prev",
                prevEl: ".testi-carousel-wrap .swiper-next",
            },
        });

        // Project Carousel
        var swiperProject = new Swiper(".project-carousel", {
            slidesPerView: 3,
            spaceBetween: 30,
            slidesPerGroup: 1,
            loop: true,
            autoplay: false,
            grabcursor: true,
            centeredSlides: true,
            centeredSlidesBounds: true,
            speed: 600,
            scrollbar: {
                el: ".swiper-scrollbar",
                hide: false,
                draggable: true,
            },
            breakpoints: {
                320: {
                    slidesPerView: 1,
                    slidesPerGroup: 1,
                    spaceBetween: 25,
                },
                767: {
                    slidesPerView: 2,
                    slidesPerGroup: 1,
                    spaceBetween: 30,
                },
                1024: {
                    slidesPerView: 3,
                    slidesPerGroup: 1,
                },
            },
        });

        // Project Carousel-5
        var swiperProject = new Swiper(".project-carousel-5", {
            slidesPerView: 2,
            spaceBetween: 30,
            slidesPerGroup: 1,
            loop: true,
            autoplay: false,
            grabcursor: true,
            speed: 600,
            scrollbar: {
                el: ".swiper-scrollbar",
                hide: false,
                draggable: true,
            },
            breakpoints: {
                320: {
                    slidesPerView: 1,
                    slidesPerGroup: 1,
                    spaceBetween: 25,
                },
                767: {
                    slidesPerView: 2,
                    slidesPerGroup: 1,
                    spaceBetween: 30,
                },
            },
        });

        // Blog Carousel
        var swiperBlog = new Swiper(".blog-carousel", {
            slidesPerView: 3,
            spaceBetween: 30,
            slidesPerGroup: 1,
            loop: true,
            autoplay: true,
            grabcursor: true,
            speed: 600,
            pagination: {
                el: ".swiper-pagination",
                clickable: true,
            },
            breakpoints: {
                320: {
                    slidesPerView: 1,
                    slidesPerGroup: 1,
                    spaceBetween: 25,
                },
                767: {
                    slidesPerView: 2,
                    slidesPerGroup: 1,
                    spaceBetween: 30,
                },
                1024: {
                    slidesPerView: 3,
                    slidesPerGroup: 1,
                },
            },
        });


        // Scroll To Top
        var scrollTop = $("#scrollup");
        $(window).on("scroll", function () {
            var topPos = $(this).scrollTop();
            if (topPos > 100) {
                $("#scrollup").removeClass("hide");
                $("#scrollup").addClass("show");
            } else {
                $("#scrollup").removeClass("show");
                $("#scrollup").addClass("hide");
            }
        });

        $(scrollTop).on("click", function () {
            $("html, body").animate(
                {
                    scrollTop: 0,
                },
                0
            );
            return false;
        });
    });

    // brand-8__silder H8
    var swiper = new Swiper(".brand-8__silder", {
        slidesPerView: 5,
        spaceBetween: 30,
        loop: true,
        centeredSlides: false,
        freemode: true,
        speed: 4000,
        allowTouchMove: false,
        autoplay: {
            delay: 1,
            disableOnInteraction: true,
        },
        breakpoints: {
            1201: {
                slidesPerView: 5,
            },
            1024: {
                slidesPerView: 4,
            },
            575: {
                slidesPerView: 3,
            },
            370: {
                slidesPerView: 2,
            },
            0: {
                slidesPerView: 1,
            },
        },
    });
    // brand-10__silder H8
    var swiper = new Swiper(".brand-10__silder", {
        slidesPerView: 5,
        spaceBetween: 30,
        loop: true,
        centeredSlides: false,
        freemode: true,
        speed: 4000,
        allowTouchMove: false,
        autoplay: {
            delay: 1,
            disableOnInteraction: true,
        },
        breakpoints: {
            1201: {
                slidesPerView: 5,
            },
            1024: {
                slidesPerView: 4,
            },
            575: {
                slidesPerView: 3,
            },
            370: {
                slidesPerView: 2,
            },
            0: {
                slidesPerView: 1,
            },
        },
    });
    // brand-9__silder H9
    var swiper = new Swiper(".logo-9__silder", {
        slidesPerView: 5,
        spaceBetween: 30,
        loop: true,
        centeredSlides: false,
        freemode: true,
        speed: 4000,
        allowTouchMove: false,
        autoplay: {
            delay: 1,
            disableOnInteraction: true,
        },
        breakpoints: {
            1201: {
                slidesPerView: 5,
            },
            1024: {
                slidesPerView: 4,
            },
            575: {
                slidesPerView: 3,
            },
            370: {
                slidesPerView: 2,
            },
            0: {
                slidesPerView: 1,
            },
        },
    });


    // portfolio-10__active
    var swiperport = new Swiper(".portfolio-10__active", {
        slidesPerView: 'auto',
        spaceBetween: 30,
        centeredSlides: false,
        centerMode: false,
        // autoplay: true,
        loop: true,
        speed: 600,
        navigation: {
            nextEl: '.portfolio-10__arrow__next',
            prevEl: '.portfolio-10__arrow__prev',
        },
    }); 


    var swiper1 = new Swiper(".customer-feedback__slider-1", {
        direction: "vertical",
        slidesPerView: "auto",
        spaceBetween: 10,
        speed: 7e3,
        loop: !0,
        freemode: true,
        autoplay: {
            delay: 0.9,
            pauseOnMouseEnter:true, 
            disableOnInteraction: !1
        }
    }),
        swiper4 = new Swiper(".customer-feedback__slider-2", {
            direction: "vertical",
            spaceBetween: 10,
            speed: 8e3,
            loop: !0,
            slidesPerView: "auto",
            freemode: true,
            autoplay: {
                delay: 0.9,
                pauseOnMouseEnter:true, 
                disableOnInteraction: !1
            }
        }),

        swiper3 = new Swiper(".customer-feedback__slider-3", {
            direction: "vertical",
            slidesPerView: "auto",
            spaceBetween: 10,
            speed: 9e3,
            loop: !0,
            freemode: true,
            autoplay: {
                delay: 0.9,
                pauseOnMouseEnter:true, 
                disableOnInteraction: !1
            }
        })



    var swiper1 = new Swiper(".customer-feedback-11__slider-1", {
        slidesPerView: 'auto',
        spaceBetween: 30,
        centeredSlides: true,
        speed: 7e3,
        loop: !0,
        freemode: true,
        autoplay: {
            delay: 0.1,
            disableOnInteraction: !1
        }
    }),
        swiper4 = new Swiper(".customer-feedback-11__slider-2", {
            slidesPerView: 'auto',
            spaceBetween: 30,
            centeredSlides: true,
            speed: 8e3,
            loop: !0,
            freemode: true,
            autoplay: {
                delay: 0.9,
                reverseDirection: true,
                disableOnInteraction: !1
            }
        })

    swiper4 = new Swiper(".heading-slider-11__active", {
        slidesPerView: 'auto',
        spaceBetween: 30,
        centeredSlides: true,
        speed: 8e3,
        loop: !0,
        freemode: true,
        autoplay: {
            delay: 0.9,
            reverseDirection: true,
            disableOnInteraction: !1
        }
    })     

    // latest-blog-9-active
    var swiperProject = new Swiper(".latest-blog-9-active", {
        slidesPerView: 5,
        spaceBetween: 30,
        slidesPerGroup: 1,
        loop: true,
        autoplay: true,
        grabcursor: true,
        speed: 600,
        scrollbar: {
            el: ".swiper-scrollbar",
            hide: false,
            draggable: true,
        },
        breakpoints: {
            320: {
                slidesPerView: 1,
                slidesPerGroup: 1,
                spaceBetween: 25,
            },
            767: {
                slidesPerView: 2,
                slidesPerGroup: 1,
                spaceBetween: 30,
            },
        },
    });


    // section sticky js ------------
    $(document).ready(function () {
        $(window).scroll(reOrder);
        $(window).resize(reOrder);
        reOrder(); // Initial call to set the correct state
    });

    function reOrder() {
        var mq = window.matchMedia("(min-width: 768px)");
        if (mq.matches) {
            $('.sticky__left__child').addClass('customm');

            var $sticky = $('.our-work-11__sticky'),
                $right = $('.sticky__right'),
                $leftChild = $('.sticky__left__child');

            if ($sticky.length && $right.length && $leftChild.length) {
                var scroll = $(window).scrollTop(),
                    topContent = $sticky.position().top - 100,
                    sectionHeight = $right.height(),
                    rightHeight = $leftChild.height(),
                    bottomContent = topContent + sectionHeight - rightHeight - 0;

                if (scroll > topContent && scroll < bottomContent) {
                    $leftChild.removeClass('posAbs').addClass('posFix');
                } else if (scroll > bottomContent) {
                    $leftChild.removeClass('posFix').addClass('posAbs');
                } else if (scroll < topContent) {
                    $leftChild.removeClass('posFix');
                }
            }
        } else {
            $('.sticky__left__child').removeClass('customm posAbs posFix');
        }
    }




})(jQuery);