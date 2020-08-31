/// <reference path="thirdparty/aes.js" />
'use stric';

$.fx.speeds._default = 175;

var testModel = {};
var CotorraAppURL = UX.UrlApp;

UX.Cotorra = {};

//Get CSS styles
$('<link/>', { rel: 'stylesheet', type: 'text/css', href: UX.UrlApp + '/assets/css/cotorra.min.css' }).appendTo('head');

//Set initial loader
$('<div class="np-initial-loader"><img src="//cdn.Cotorria.mx/assets/img/logo-beta-1.png" /></div>').appendTo('body').hide().fadeIn(1000);

//Set favicon
document.querySelector('link[rel="shortcut icon"]').setAttribute('href', '//cdn.Cotorria.mx/assets/img/favicon.ico');

//Set title
document.title = document.title.split(' | ')[0] + ' | ' + 'Cotorria';

//Init session
var initSessionReq = $.ajax({
    url: CotorraAppURL + '/Home/InitSession/',
    success: function (data) { console.log('Cotorria - Build date: ' + data); }
});

$(document).ready(() => {

    //Declare namespaces
    UX.Cotorra.Menu = {};
    UX.Cotorra.Catalogs = {};
    UX.Cotorra.SATGroupCodes = {};
    UX.Cotorra.NOM035 = {};

    $.when(

        initSessionReq,

        //Load common html templates
        $.get(UX.UrlApp + '/views/common/' + 'address.html', (html) => { $('body').append(html); }),
        $.get(UX.UrlApp + '/views/common/' + 'periodtypemodal.html', (html) => { $('body').append(html); }),

        //Prepayroll html templates
        $.get(UX.UrlApp + '/views/prepayroll/' + 'fonacot.html', (html) => { $('body').append(html); }),
        $.get(UX.UrlApp + '/views/prepayroll/' + 'incidents.html', (html) => { $('body').append(html); }),
        $.get(UX.UrlApp + '/views/prepayroll/' + 'infonavit.html', (html) => { $('body').append(html); }),
        $.get(UX.UrlApp + '/views/prepayroll/' + 'inhabilities.html', (html) => { $('body').append(html); }),
        $.get(UX.UrlApp + '/views/prepayroll/' + 'overdraft.html', (html) => { $('body').append(html); }),
        $.get(UX.UrlApp + '/views/prepayroll/' + 'permanentmovements.html', (html) => { $('body').append(html); }),
        $.get(UX.UrlApp + '/views/prepayroll/' + 'vacations.html', (html) => { $('body').append(html); }),
        $.get(UX.UrlApp + '/views/prepayroll/' + 'accumulated.html', (html) => { $('body').append(html); }),

        //SettlementProcess
        $.get(UX.UrlApp + '/views/employees/' + 'settlement.html', (html) => { $('body').append(html); }),

        //Load common scripts
        $.cachedScript(UX.UrlApp + '/assets/js/' + 'common.js'),
        $.cachedScript(UX.UrlApp + '/assets/js/' + 'menu.js'),
        $.cachedScript(UX.UrlApp + '/assets/js/' + 'loaders.js'),

        //Load employee scripts
        $.cachedScript(UX.UrlApp + '/assets/js/employees/' + 'main.js'),
        $.cachedScript(UX.UrlApp + '/assets/js/employees/' + 'salaryupdate.js'),
        $.cachedScript(UX.UrlApp + '/assets/js/employees/' + 'sbcupdate.js'),
        $.cachedScript(UX.UrlApp + '/assets/js/employees/' + 'settlement.js'),

        //Load catalogs scripts
        $.cachedScript(UX.UrlApp + '/assets/js/catalogs/' + 'main.js'),
        $.cachedScript(UX.UrlApp + '/assets/js/catalogs/' + 'employerregistration.js'),
        $.cachedScript(UX.UrlApp + '/assets/js/catalogs/' + 'employerfiscalinformation.js'),
        $.cachedScript(UX.UrlApp + '/assets/js/catalogs/' + 'banks.js'),
        $.cachedScript(UX.UrlApp + '/assets/js/catalogs/' + 'periodtypes.js'),
        $.cachedScript(UX.UrlApp + '/assets/js/catalogs/' + 'periods.js'),
        $.cachedScript(UX.UrlApp + '/assets/js/catalogs/' + 'shifts.js'),
        $.cachedScript(UX.UrlApp + '/assets/js/catalogs/' + 'positions.js'),
        $.cachedScript(UX.UrlApp + '/assets/js/catalogs/' + 'departments.js'),
        $.cachedScript(UX.UrlApp + '/assets/js/catalogs/' + 'areas.js'),
        $.cachedScript(UX.UrlApp + '/assets/js/catalogs/' + 'workcenters.js'),
        $.cachedScript(UX.UrlApp + '/assets/js/catalogs/' + 'concepts.js'),
        $.cachedScript(UX.UrlApp + '/assets/js/catalogs/' + 'accumulatedtypes.js'),
        $.cachedScript(UX.UrlApp + '/assets/js/catalogs/' + 'incidenttypes.js'),
        $.cachedScript(UX.UrlApp + '/assets/js/catalogs/' + 'workcenters.js'),
        $.cachedScript(UX.UrlApp + '/assets/js/catalogs/' + 'benefitTypes.js'),

        //Load prepayroll
        $.cachedScript(UX.UrlApp + '/assets/js/prepayroll/' + 'accumulated.js'),
        $.cachedScript(UX.UrlApp + '/assets/js/prepayroll/' + 'fonacot.js'),
        $.cachedScript(UX.UrlApp + '/assets/js/prepayroll/' + 'incidents.js'),
        $.cachedScript(UX.UrlApp + '/assets/js/prepayroll/' + 'infonavit.js'),
        $.cachedScript(UX.UrlApp + '/assets/js/prepayroll/' + 'inhability.js'),
        $.cachedScript(UX.UrlApp + '/assets/js/prepayroll/' + 'overdraft.js'),
        $.cachedScript(UX.UrlApp + '/assets/js/prepayroll/' + 'permanentmovements.js'),
        $.cachedScript(UX.UrlApp + '/assets/js/prepayroll/' + 'prepayroll.js'),
        $.cachedScript(UX.UrlApp + '/assets/js/prepayroll/' + 'vacation.js'),

        //Load tables and fares
        $.cachedScript(UX.UrlApp + '/assets/js/farescatalogs/' + 'anualemploymentsubsidy.js'),
        $.cachedScript(UX.UrlApp + '/assets/js/farescatalogs/' + 'anualincometax.js'),
        $.cachedScript(UX.UrlApp + '/assets/js/farescatalogs/' + 'imssemployeetable.js'),
        $.cachedScript(UX.UrlApp + '/assets/js/farescatalogs/' + 'imssemployertable.js'),
        $.cachedScript(UX.UrlApp + '/assets/js/farescatalogs/' + 'minimunsalaries.js'),
        $.cachedScript(UX.UrlApp + '/assets/js/farescatalogs/' + 'monthlyincometax.js'),
        $.cachedScript(UX.UrlApp + '/assets/js/farescatalogs/' + 'monthlyemploymentsubsidy.js'),
        $.cachedScript(UX.UrlApp + '/assets/js/farescatalogs/' + 'settlement.js'),
        $.cachedScript(UX.UrlApp + '/assets/js/farescatalogs/' + 'sgdflimits.js'),
        $.cachedScript(UX.UrlApp + '/assets/js/farescatalogs/' + 'uma.js'),
        $.cachedScript(UX.UrlApp + '/assets/js/farescatalogs/' + 'umi.js'),
        $.cachedScript(UX.UrlApp + '/assets/js/farescatalogs/' + 'infonavitinsurance.js'),

        //Company configuration
        $.cachedScript(UX.UrlApp + '/assets/js/companyconfiguration/' + 'main.js'),

        //Fixed catalogs
        $.cachedScript(UX.UrlApp + '/assets/js/fixedcatalogs/' + 'satconcepts.js'),
        $.cachedScript(UX.UrlApp + '/assets/js/fixedcatalogs/' + 'typesofaccumulated.js'),
        $.cachedScript(UX.UrlApp + '/assets/js/fixedcatalogs/' + 'employeelists.js'),
        $.cachedScript(UX.UrlApp + '/assets/js/fixedcatalogs/' + 'disabilitytypes.js'),

        //Third-party
        $.cachedScript(UX.UrlApp + '/assets/js/thirdparty/' + 'aes.js'),
        $.cachedScript(UX.UrlApp + '/assets/thirdparty/bootstrap-datepicker/js/' + 'bootstrap-datepicker.js'),

        //NOM035
        //$.cachedScript(UX.UrlApp + '/assets/js/nom035/' + 'evalperiods.js'),       

    ).done(function () {

        //Language for datepicker
        $.cachedScript(UX.UrlApp + '/assets/thirdparty/bootstrap-datepicker/js/locales/' + 'bootstrap-datepicker.es.js');

        //Override styles
        $('.header > .logo > img').attr('src', '//cdn.Cotorria.mx/assets/img/logo-beta-3.png');
        $('.menu').prepend($('.header > .logo'));
        //$('.top-menu').remove();

        setTimeout(() => {
            $('body').css('background-image', 'url(//cdn.Cotorria.mx/assets/img/Cotorria-bg-beta-3.jpg)');

            $('.app-t').show();
            $('.np-initial-loader').fadeOut(1000, () => {
                $('.np-initial-loader').remove();

                //Set inline manual widget and tracking

                window.inlineManualTracking = {
                    uid: '',
                    email: CotorraNode.Username,
                    username: CotorraNode.Username,
                    name: CotorraNode.UserDisplayName,
                    created: '',
                    updated: '',
                    roles: '',
                    group: '',
                    plan: ''
                };
                $.cachedScript('https://inlinemanual.com/embed/player.a1f6b5c6abf94fd393eaece0c5de9b21.js');
            });
        }, 800);

        //Remove useless top icons
        $('#btnOpenSupportChat').parent().remove();
        $('#um-btnCompanyView').parent().remove();

        //Set help button
        $('#um-btnGuideHelp > svg').remove();
        $('#um-btnGuideHelp').append('<i class="fas fa-question"></i>');
        $('#um-btnGuideHelp').parent().removeClass('dropdown').find('ul').remove();
        $('#um-btnGuideHelp').attr('title', 'Centro de ayuda');
        $('#um-btnGuideHelp').attr('data-original-title', 'Centro de ayuda');
        $('#um-btnGuideHelp').off('click').on('click', function () {
            setTimeout(function () { $('#um-btnGuideHelp').blur() }, 100);
            $('.inmplayer-trigger').click();
        });

        //Set refresh icon
        $('#btnRefeshActualView > svg').remove();
        $('#btnRefeshActualView').append('<i class="fas fa-sync-alt"></i>');

        //Set notifications icon
        $('#um-btnNotifications > svg').remove();
        $('#um-btnNotifications').append('<i class="far fa-bell"></i>');

        //Set notifications icon
        $('#btnOpenCompany > svg').remove();
        $('#btnOpenCompany').append('<i class="far fa-window-alt"></i>');

        //Create user avatar
        $('.username-option > a > img').remove();
        $('.username-option > a > svg').remove();
        $('.username-option > a').append('<div class="useravatar">' + CotorraNode.UserDisplayName.substr(0, 1) + '</div>');

        //Create dark mode switch
        $('.username-option > ul').append(
            '<li class="dark-mode-switch">' +
            '<div class="toggle-switch" data-ts-color="blue">' +
            '    <input id="np-dark-mode-switch" type="checkbox" hidden="hidden">' +
            '        <label for="np-dark-mode-switch" class="ts-helper"></label>' +
            '        <label for="np-dark-mode-switch" class="ts-label">Lado oscuro</label>' +
            '    </div>' +
            '</li>');

        $('.username-option > ul .dark-mode-switch').off('click').on('click', function (ev) {
            ev.stopPropagation();
            $('.username-option').addClass('open');

            if ($(ev.originalEvent.target).hasClass('ts-helper') || $(ev.originalEvent.target).hasClass('ts-label')) {

                //Check switch future
                let goDark = !$('#np-dark-mode-switch').is(':checked');

                //Set loader
                $('body').append('<div class="theme-loader"></div>');

                if (goDark) {

                    $('.theme-loader').addClass('black');
                    $('.theme-loader').addClass('left');
                    $('.theme-loader').animate({
                        opacity: 1,
                        backgroundColor: 'black',
                        width: '100%'
                    }, 800, function () {

                        //Set css class and logo
                        $('body')[goDark ? 'addClass' : 'removeClass']('dark-mode');
                        $('.logo img').attr('src', '//cdn.Cotorria.mx/assets/img/logo-beta-3' + (goDark ? '-dark' : '') + '.png');

                        //Set to right
                        $('.theme-loader').removeClass('left');
                        $('.theme-loader').addClass('right');

                        setTimeout(function () {

                            $('.username-option').removeClass('open');
                            //Hide loader
                            $('.theme-loader').animate({
                                opacity: 0.92,
                                width: '0%'
                            }, 800, function () {
                                $('.theme-loader').remove();
                            });
                        }, 100);
                    });

                } else {

                    $('.theme-loader').addClass('white');
                    $('.theme-loader').addClass('right');
                    $('.theme-loader').animate({
                        opacity: 1,
                        width: '100%'
                    }, 800, function () {

                        //Set css class and logo
                        $('body')[goDark ? 'addClass' : 'removeClass']('dark-mode');
                        $('.logo img').attr('src', '//cdn.Cotorria.mx/assets/img/logo-beta-3' + (goDark ? '-dark' : '') + '.png');

                        //Set to right
                        $('.theme-loader').removeClass('right');
                        $('.theme-loader').addClass('left');

                        setTimeout(function () {

                            $('.username-option').removeClass('open');

                            //Hide loader
                            $('.theme-loader').animate({
                                opacity: 0.92,
                                width: '0%'
                            }, 800, function () {
                                $('.theme-loader').remove();
                            });
                        }, 100);
                    });

                }


                return;
            }
        });

        //Create new container
        $('.app-c').prepend('<div class="app-container"></div>');
        $('.app-container').prepend($('.app-main'));

        //Add logout
        $('.menu').append('<div class="options options-logout"><ul><li><a id="mo_Logout" href="#"><i class="far fa-sign-out-alt"></i></a><li><ul></div>')
        $('#mo_Logout').off('click').on('click', () => { $('#btnLogOffApp').click(); });

        //Remove vesion tag
        $('.menu .version').remove();

        //Set fullscreen
        $('#btnFullScreen').off('click').on('click', function () {

            let hasFullscreen = $('.app-container').hasClass('fullscreen');
            $('.app-container')[hasFullscreen ? 'removeClass' : 'addClass']('fullscreen');
            $('#btnFullScreen').html('<i class="zmdi zmdi-fullscreen"></i>' + (hasFullscreen ? 'Ampliar aplicación' : 'Reducir aplicación'));

            setTimeout(function () {
                $(window).trigger('resize');
                if ($('.full-kendo-grid').length > 0) {
                    $('.full-kendo-grid').data('kendoGrid').dataSource.read();
                }
            }, 650);

        });

        //Create footer for company data and licensing data
        $('.app-main').append('<div class="app-footer"><label>' + CotorraNode.RFC + '</label><label>' + CotorraNode.SocialReason + '</label></div>');

        //Load other stuff
        UX.Cotorra.PeriodTypesRequest =
            UX.Cotorra.Catalogs.Do('GET', 'PrePayroll', 'GetActivePeriodTypes', {},
                (data) => { UX.Cotorra.PeriodTypesOptions = data }, (error) => { }, (complete) => { });


        //Get global payroll company configuration
        UX.Cotorra.Catalogs.Get('PayrollCompanyConfiguration', {},
            (data) => { UX.Cotorra.CompanyConfiguration.Data = data; },
            (error) => { console.log(UX.Common.getMessageFromError(error)); },
            () => { },
        );


        //Open dashboard
        UX.Cotorra.Menu.OpenDashboard();
    });


});


