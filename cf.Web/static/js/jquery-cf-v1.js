jQuery(document).ready(function () {
    createThumbRolloverEffect();
    stopDuplicateFormSubmissions();
    addConfirmOnDelete();
    makeFooterMediaClickable();
    setQuickSearch();
    addQRModel();
    attachOpinionModal();

    $(".jBtn").button();
});

function dynamicLoadScript(url) {
    var script = document.createElement("script");
    script.setAttribute("type", "text/javascript");
    script.setAttribute("src", url);
    document.body.appendChild(script);
}

function createThumbRolloverEffect() 
{
    opacityRollover("img.tm", 0.67);
    opacityRollover(".media-roll img", 0.67);
}

function opacityRollover(selector, opacity) {
    $(selector).opacityrollover({
        mouseOutOpacity: opacity,
        mouseOverOpacity: 1.0,
        fadeSpeed: 'fast',
        exemptionSelector: '.selected'
    });
}

function closeModal() { $("#modal-placeholder").dialog('close'); $("#modal-placeholder").html(""); }

function attachOpinionModal() 
{
    if ($(".opinion").length > 0) {
        $('.opinion').click(function () {
            var objectID = $(this).attr('href').replace("#", "");
            $.ajax({ type: "Get",
                url: "/rate/" + objectID,
                dataType: "html",
                success: function (data) { initializeRatingForm(data); }
            });
        });
    }
}

function initializeRatingForm(data) {
    $("#modal-placeholder").html(data);
    $("#modal-placeholder").attr("title", "Contribute your opinion");
    $("#modal-placeholder").dialog({ minWidth: 640, modal: true });
    $("#stars-wrapper").stars({ inputType: "select", captionEl: $("#stars-cap") });

    reattachedValidationToStars("#stars-wrapper");

    $("#rating-form").submit(function () {
        //-- TODO(JSK): Seems like this is not firing properly...
        var valid = $("#rating-form").valid();
        if (valid) {
            $.post(
                "/rate/" + $("#RateObjectID").val(),
                $("#rating-form").serialize(),
                function (data) {
                    alert("Opinion saved!");
                    var loc = document.location.href;
                    var indexOfHash = loc.indexOf('#');
                    document.location = loc.substring(0, indexOfHash);
                }
            );
        }
        return false;
    });  
}

function reattachedValidationToStars(parentDivSelector) {
    //-- Make sure our validation fires!
    var rt = $(parentDivSelector + " input");
    
    //-- We have to set the value to nothing so the validation fires
    rt.val("");
    
    rt.removeAttr("disabled")

    rt.attr("data-val-required", "rating required");
    rt.attr("data-val-range-min", "1");
    rt.attr("data-val-range-max", "5");
    rt.attr("data-val-range", "rating required");
    rt.attr("data-val", "true");
}

function addConfirmOnDelete()
{
    $("a.delete").confirm();
    $("input.delete").confirm();
}

function addQRModel()
{
    if ($("#showQR").length > 0) {
        $('#showQR').click(function () {
            var qrUrl = $("#imageQR").attr("alt");
            $('#imageQR').attr('src', 'http://www.beqrious.com/generate_image.php?type=http://&text=' + qrUrl);
            $("#dialog-qr").dialog({ minWidth: 500 });
        });
    }
}

function makeFooterMediaClickable()
{
    //-- Make the thumbnails in the bottom clickable without making them links (better for SEO)
    if ($("#latest-media-roll").length > 0) {

        $("#latest-media-roll img").click(function () {
            document.location = '/new-rock-climbing-media';
        });
    }
}

function stopDuplicateFormSubmissions()
{
    $('.SubmitBtn').click(function () {
        var form = $(this).parents('form');
        form.validate();
        var formValid = form.valid();
        if ($(this).text() != 'processing') {
            if (formValid) {
                $(this).hide();
                $(this).after('<p class="processFeedback">Processing form</p>');
            }
        }
   });
}

function resetFormButtonOptions() {
    $("div.options .processFeedback").remove();
    $("div.options input").show();
}

function setQuickSearch() {
    if ($("#qsearch").length > 0) {

        $("#qsearch").autocomplete({
            minLength: 3,
            source: function (request, response) {
                $.ajax({
                    delay: 550,
                    type: "POST",
                    url: "/search",
                    dataType: "json",
                    data: "qsearch=" + $("#qsearch").val(),
                    success: function (data) {
                        var regex = new RegExp("(?![^&;]+;)(?!<[^<>]*)(" + request.term.replace(/([\^\$\(\)\[\]\{\}\*\.\+\?\|\\])/gi, "\\$1") + ")(?![^<>]*>)(?![^&;]+;)", "gi");

                        response($.map(data, function (item) {
                            var lbl = item.Title.replace(regex, "<strong>$1</strong>");
                            return { label: lbl, value: item.Title, url: item.Url, flg: item.Flag, type: cftypes[item.TypeID] }
                        }));
                    }
                });
            },
            select: function (event, ui) {
                $("#qsearch").val(ui.item.label);
                document.location = ui.item.url;
            }
        })
        .data("autocomplete")._renderItem = function (ul, item) {
            return $("<li></li>").data("item.autocomplete", item)
				        .append("<a href=''><img src='http://static.climbfind.com/flags/" + item.flg + "' /> " + item.label + " <i>" + item.type +"</i></a>")
				        .appendTo(ul);
        };

        disableEnterFormPost("#qsearch");
    }
}

function disableEnterFormPost(selector) {
    //-- Disable enter causing post to new page
    $(selector).keypress(function (e) { if (e.which == 13) { return false; } });
}

function placeAllTypesAutocomplete(searchSelector, callback) { placeAutocomplete(searchSelector, callback, "/search-places", "psearch"); }
function locationAutocomplete(searchSelector, callback) { placeAutocomplete(searchSelector, callback, "/search-locations", "lsearch"); }
function provinceAutocomplete(searchSelector, callback) { placeAutocomplete(searchSelector, callback, "/search-provinces", "psearch"); }
function climbingAreaAutocomplete(searchSelector, callback) { placeAutocomplete(searchSelector, callback, "/search-climbing-areas", "asearch"); }

function placeAutocomplete(searchSelector, callback, searchUrl, searchParameter) {
    $(searchSelector).autocomplete({
        minLength: 3,
        source: function (request, response) {
            $.ajax({
                delay: 550,
                type: "POST",
                url: searchUrl,
                dataType: "json",
                data: searchParameter + "=" + $(searchSelector).val(),
                success: function (data) {
                    var regex = new RegExp("(?![^&;]+;)(?!<[^<>]*)(" + request.term.replace(/([\^\$\(\)\[\]\{\}\*\.\+\?\|\\])/gi, "\\$1") + ")(?![^<>]*>)(?![^&;]+;)", "gi");

                    response($.map(data, function (item) {
                        var lbl = item.Title.replace(regex, "<strong>$1</strong>");
                        return { label: lbl, value: item.Title, url: item.Url, flg: item.Flag, id: item.ID, type: cftypes[item.TypeID] }
                    }));
                }
            });
        },
        select: function (event, ui) {
            if (ui.item.value != 'No matching places found') { // (JSK:2011.09.26) don't think this actually makes a difference
                $(searchSelector).val(ui.item.value);
            }
            if (callback != null) { callback(ui.item); }
        }
    })
    .data("autocomplete")._renderItem = function (ul, item) {
        return $("<li></li>").data("item.autocomplete", item)
				    .append("<a href=''><img src='http://static.climbfind.com/flags/" + item.flg + "' /> " + item.label + " <i>" + item.type + "</i></a>")
				    .appendTo(ul);
    };

    //-- Disable enter causing post to new page
    //$(searchSelector).keypress(function (e) { if (e.which == 13) { return false; } });
}

jQuery.fn.confirm = function () {
    return this.each(function () {
        $(this).click(function (event) {
            var href = $(event.currentTarget).attr("href");
            if (confirm("Are you 100% SURE you want to delete this?")) { window.location(href); }
            return false;
        });
    });
};

String.format = function () {
    var s = arguments[0];
    for (var i = 0; i < arguments.length - 1; i++) {
        var reg = new RegExp("\\{" + i + "\\}", "gm");
        s = s.replace(reg, arguments[i + 1]);
    }

    return s;
}

/* jQuery Watermark plugin Version 1.2 (7-DEC-2010) * Examples at: http://mario.ec/static/jq-watermark/ */
(function (a) { var k = a.browser.msie && a.browser.version < 8; a.watermarker = function () { }; a.extend(a.watermarker, { defaults: { color: "#999", left: 0, top: 0, fallback: false, animDuration: 300, minOpacity: 0.6 }, setDefaults: function (e) { a.extend(a.watermarker.defaults, e) }, checkVal: function (e, c) { e.length == 0 ? a(c).show() : a(c).hide(); return e.length > 0 }, html5_support: function () { return "placeholder" in document.createElement("input") } }); a.fn.watermark = function (e, c) { var i; c = a.extend(a.watermarker.defaults, c); i = this.filter("textarea, input:not(:checkbox,:radio,:file,:submit,:reset)"); if (!(c.fallback && a.watermarker.html5_support())) { i.each(function () { var b, f, j, g, d, h = 0; b = a(this); if (b.attr("data-jq-watermark") != "processed") { f = b.attr("placeholder") != undefined && b.attr("placeholder") != "" ? "placeholder" : "title"; j = e === undefined || e === "" ? a(this).attr(f) : e; g = a('<span class="watermark_container"></span>'); d = a('<span class="watermark">' + j + "</span>"); f == "placeholder" && b.removeAttr("placeholder"); g.css({ display: "inline-block", position: "relative" }); k && g.css({ zoom: 1, display: "inline" }); b.wrap(g).attr("data-jq-watermark", "processed"); if (this.nodeName.toLowerCase() == "textarea") { e_height = b.css("line-height"); e_height = e_height === "normal" ? parseInt(b.css("font-size")) : e_height; h = b.css("padding-top") != "auto" ? parseInt(b.css("padding-top")) : 0 } else { e_height = b.outerHeight(); if (e_height <= 0) { e_height = b.css("padding-top") != "auto" ? parseInt(b.css("padding-top")) : 0; e_height += b.css("padding-bottom") != "auto" ? parseInt(b.css("padding-bottom")) : 0; e_height += b.css("height") != "auto" ? parseInt(b.css("height")) : 0 } } h += b.css("margin-top") != "auto" ? parseInt(b.css("margin-top")) : 0; f = b.css("margin-left") != "auto" ? parseInt(b.css("margin-left")) : 0; f += b.css("padding-left") != "auto" ? parseInt(b.css("padding-left")) : 0; d.css({ position: "absolute", display: "block", fontFamily: b.css("font-family"), fontSize: b.css("font-size"), color: c.color, left: 4 + c.left + f, top: c.top + h, height: e_height, lineHeight: e_height + "px", textAlign: "left", pointerEvents: "none" }).data("jq_watermark_element", b); a.watermarker.checkVal(b.val(), d); d.click(function () { a(a(this).data("jq_watermark_element")).trigger("focus") }); b.before(d).bind("focus.jq_watermark", function () { a.watermarker.checkVal(a(this).val(), d) || d.stop().fadeTo(c.animDuration, c.minOpacity) }).bind("blur.jq_watermark change.jq_watermark", function () { a.watermarker.checkVal(a(this).val(), d) || d.stop().fadeTo(c.animDuration, 1) }).bind("keydown.jq_watermark", function () { a(d).hide() }).bind("keyup.jq_watermark", function () { a.watermarker.checkVal(a(this).val(), d) }) } }); return this } }; a(document).ready(function () { a(".jq_watermark").watermark() }) })(jQuery);

/*** jQuery Opacity Rollover plugin Copyright (c) 2009 Trent Foley (http://trentacular.com) Licensed under the MIT License:  http://www.opensource.org/licenses/mit-license.php */
; (function ($) { var defaults = { mouseOutOpacity: 0.67, mouseOverOpacity: 1.0, fadeSpeed: 'fast', exemptionSelector: '.selected' };

    $.fn.opacityrollover = function (settings) { // Initialize the effect
        $.extend(this, defaults, settings);
        var config = this;

        function fadeTo(element, opacity) {
            var $target = $(element);
            if (config.exemptionSelector) $target = $target.not(config.exemptionSelector);
            $target.fadeTo(config.fadeSpeed, opacity);
        }

        this.css('opacity', this.mouseOutOpacity)
			.hover(function () { fadeTo(this, config.mouseOverOpacity); },
				function () { fadeTo(this, config.mouseOutOpacity); });

        return this;
    };
})(jQuery);


var cftypes = [];
cftypes[null] = "";
cftypes[0] = "";
cftypes[1] = "Country";
cftypes[2] = "Province";
cftypes[3] = "City";
cftypes[7] = "Area";
cftypes[10] = "Indoor climbing wall";
cftypes[11] = "Sports center";
cftypes[12] = "Private indoor";
cftypes[23] = "Boulder";
cftypes[21] = "Outdoor rock wall";
cftypes[101] = "Climb";
cftypes[103] = "Climb";
cftypes[120] = "Climber";