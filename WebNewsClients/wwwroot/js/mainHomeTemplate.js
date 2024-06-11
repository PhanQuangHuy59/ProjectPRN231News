jQuery(document).ready(function () {
	jQuery("#post-right-col,#post-sidebar-wrap,.home-mid-col,.home-right-col,#arch-right-col").theiaStickySidebar({ "containerSelector": "", "additionalMarginTop": "65", "additionalMarginBottom": "15", "updateSidebarHeight": false, "minWidth": "767", "sidebarBehavior": "modern" });
});
jQuery(document).ready(function ($) {

	// Back to Top Button
	var duration = 500;
	$('.back-to-top').click(function (event) {
		event.preventDefault();
		$('html, body').animate({ scrollTop: 0 }, duration);
		return false;
	});

	// Main Menu Dropdown Toggle
	$('.menu-item-has-children a').click(function (event) {
		event.stopPropagation();
		location.href = this.href;
	});

	$('.menu-item-has-children').click(function () {
		$(this).addClass('toggled');
		if ($('.menu-item-has-children').hasClass('toggled')) {
			$(this).children('ul').toggle();
			$('.fly-nav-menu').getNiceScroll().resize();
		}
		$(this).toggleClass('tog-minus');
		return false;
	});

	// Main Menu Scroll
	$('.fly-nav-menu').niceScroll({ cursorcolor: "#888", cursorwidth: 7, cursorborder: 0, zindex: 999999 });


	// Infinite Scroll
	$('.infinite-content').infinitescroll({
		navSelector: ".nav-links",
		nextSelector: ".nav-links a:first",
		itemSelector: ".infinite-post",
		loading: {
			msgText: "Loading more posts...",
			finishedMsg: "Sorry, no more posts"
		},
		errorCallback: function () { $(".inf-more-but").css("display", "none") }
	});
	$(window).unbind('.infscr');
	$(".inf-more-but").click(function () {
		$('.infinite-content').infinitescroll('retrieve');
		return false;
	});
	if ($('.nav-links a').length) {
		$('.inf-more-but').css('display', 'inline-block');
	} else {
		$('.inf-more-but').css('display', 'none');
	}

	// The slider being synced must be initialized first
	$('.post-gallery-bot').flexslider({
		animation: "slide",
		controlNav: false,
		animationLoop: true,
		slideshow: false,
		itemWidth: 80,
		itemMargin: 10,
		asNavFor: '.post-gallery-top'
	});

	$('.post-gallery-top').flexslider({
		animation: "fade",
		controlNav: false,
		animationLoop: true,
		slideshow: false,
		prevText: "&lt;",
		nextText: "&gt;",
		sync: ".post-gallery-bot"
	});

});


(function () {
	var c = document.body.className;
	c = c.replace(/woocommerce-no-js/, 'woocommerce-js');
	document.body.className = c;
})();
