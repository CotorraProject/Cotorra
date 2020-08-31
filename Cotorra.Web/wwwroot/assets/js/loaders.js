'use stric';

//UX.Loaders.SetLoader = function (container) {

//    if ($(container + " .loader-container").length > 0) { return; }
//    let $container = $(container);

//    loader = ''; //'<div class="lds-ring"><div></div><div></div><div></div><div></div></div>';
//    let $loaderContainer = $("<div class='loader-container'></div>");
//    $loaderContainer.append(loader);

//    $container.append($loaderContainer);
//    $loaderContainer
//        .css('height', $loaderContainer.parent().outerHeight() + "px")
//        .css('width', $loaderContainer.parent().outerWidth() + "px");

//    //$loaderContainer.hide().fadeIn(180);

//    return $container.last();
//};

//UX.Loaders.RemoveLoader = function (container) {
//    //Get the loader
//    let loader = $(container).find(".loader-container, .loader-container-alt");

//    //If loader is visible, fade it out, if not, just remove it
//    if (loader.is(":visible")) {
//        loader.fadeOut(180, function () {
//            this.remove();
//        });
//    } else {
//        loader.remove();
//    }
//};