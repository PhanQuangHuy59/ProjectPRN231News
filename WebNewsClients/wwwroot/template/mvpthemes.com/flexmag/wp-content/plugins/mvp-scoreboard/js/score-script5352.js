jQuery(document).ready(function($) {
"use strict";

 	$('.carousel').elastislide({
 		imageW 	: 130,
 		minItems	: 1,
 		margin		: 2
	});

	$('.tabber-container').each(function() {
		$(this).find(".tabber-content").hide(); //Hide all content

		$(this).find(".tabber-content:first").show(); //Show first tab content
	});
	$(".score-nav-menu select").change(function(e) {
		$(this).parents('.tabber-container').find(".tabber-content").hide(); //Hide all tab content

		var activeTab = $(this).find("option:selected").val(); //Find the href attribute value to identify the active tab + content
		$(this).parents('.tabber-container').find(activeTab).fadeIn(); //Fade in the active ID content

		e.preventDefault();
	});
	$("select.tabs option a").change(function(e) {
		e.preventDefault();
	})

});