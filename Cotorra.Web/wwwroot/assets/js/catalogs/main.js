'use strict';

UX.Cotorra.Catalogs = {
    Get: (catalog, data, onSuccess, onError, onComplete) => {
        return $.ajax({
            method: 'GET',
            url: CotorraAppURL + '/' + catalog + '/Get/',
            data: data,
            success: onSuccess,
            error: onError,
            complete: onComplete
        });
    },

    GetByID: (catalog, data, onSuccess, onError, onComplete) => {
        return $.ajax({
            method: 'GET',
            url: CotorraAppURL + '/' + catalog + '/GetByID/',
            data: data,
            success: onSuccess,
            error: onError,
            complete: onComplete
        });
    },

    Save: (catalog, data, onSuccess, onError, onComplete) => {
        return $.ajax({
            method: 'PUT',
            url: CotorraAppURL + '/' + catalog + '/Save/',
            data: data,
            success: function (success) {
                UX.Cotorra.Catalogs.Reload({ catalog: catalog });
                onSuccess(success);
            },
            error: onError,
            complete: onComplete
        });
    },

    Delete: (catalog, data, onSuccess, onError, onComplete) => {
        return $.ajax({
            method: 'DELETE',
            url: CotorraAppURL + '/' + catalog + '/Delete/',
            data: data,
            success: onSuccess,
            error: onError,
            complete: onComplete
        });
    },

    GetDetails: (catalog, data, onSuccess, onError, onComplete) => {
        return $.ajax({
            method: 'GET',
            url: CotorraAppURL + '/' + catalog + '/GetDetails/',
            data: data,
            success: onSuccess,
            error: onError,
            complete: onComplete
        });
    },

    Do: (method, catalog, action, data, onSuccess, onError, onComplete) => {
        return $.ajax({
            method: method,
            url: CotorraAppURL + '/' + catalog + '/' + action + '/',
            data: data,
            success: onSuccess,
            error: onError,
            complete: onComplete
        });
    },

    Reload: (params = {}) => {
        if (UX.Cotorra.Catalogs[params.catalog]) {
            UX.Cotorra.Catalogs.Get(params.catalog,
                params.serviceparams,
                (data) => {
                    UX.Cotorra.Catalogs[params.catalog] = data;
                },
                (error) => { },
                (complete) => { });
        }
    },

    ReloadAll: async () => {
        let catalogsToReload = [];
        Object.entries(UX.Cotorra.Catalogs).forEach(entry => {

            if (Array.isArray(entry[1]) && entry[0] !== "Options" &&
                entry[0] !== "FaresOptions" &&
                entry[0] !== "NOM035Options") {
                catalogsToReload.push(entry[0]);
            }

        });
        await UX.Cotorra.Catalogs.Require({
            catalogs: catalogsToReload,
            forceLoad: true
        });
    },

    Require: async (params) => {

        let container = params.loader ? params.loader.container : null;
        let removeWhenFinish = (container !== null && params.loader.removeWhenFinish !== undefined) ? params.loader.removeWhenFinish : true;
        let forceLoad = params.forceLoad ? params.forceLoad : false;
        let catalogs = params.catalogs;

        let requests = [];
        if (container) { UX.Loaders.SetLoader(container); }

        for (let i = 0; i < catalogs.length; i++) {
            let catalog = catalogs[i];

            if (forceLoad || !UX.Cotorra.Catalogs[catalog]) {

                requests.push(
                    UX.Cotorra.Catalogs.Get(catalog, {},
                        (data) => {
                            UX.Cotorra.Catalogs[catalog] = data;
                        },
                        (error) => { },
                        (complete) => { })
                );
            }
        }

        if (params.requests && params.requests.length > 0) {
            for (let i = 0; i < params.requests.length; i++) {
                requests.push(params.requests[i]);
            }
        }

        await $.when.apply($, requests);

        if (container && removeWhenFinish) { UX.Loaders.RemoveLoader(container); }
    }

};

UX.Cotorra.Catalogs.Init = (options, selector, configParams = {}) => {

    //Get menu container
    let $menu = $(selector + ' ul#catalogs-options');
    let $catalogs = $(selector);

    //Draw options
    options.forEach((opt) => {

        //Add option
        let optHTML = '<li><a href="#" data-action="' + opt.Action + '">' + opt.Title + '</a></li>';
        $menu.append(optHTML);

        //Set open page on click
        $menu.children().last().find('a').off('click').on('click', function (ev) {

            UX.Loaders.SetLoader(selector);

            //Select the option
            $menu.find('*').removeClass('selected');
            $(this).addClass('selected');

            //Save lat selected option
            lastSelectedCatalog[selector] = $(this).parent().index();

            //Reset common top actions
            $catalogs.find('.quicksearch').attr('placeholder', 'Búsqueda rápida en ' + opt.Title.toLowerCase());
            $catalogs.find('.top-actions').html('');
            $catalogs.find('.top-action-filter:not(#searchContent)').remove();
            $catalogs.find('.catalog-content').html('');

            let action = $(this).data('action');
            let data = {};

            //Fix for concepts
            let conceptType = null;
            if (action === 'Perceptions' || action === 'Deductions' || action === 'Liabilities') {

                switch (action) {
                    case 'Perceptions': conceptType = 'P'; break;
                    case 'Deductions': conceptType = 'D'; break;
                    case 'Liabilities': conceptType = 'L'; break;
                }

                action = 'Concepts';
                data = { type: conceptType };
            }

            let viewFolder = 'catalogs';

            //Get view
            $.ajax({
                url: CotorraAppURL + '/views/' + viewFolder + '/' + action.toLowerCase() + '.html',
                method: 'GET',
                data: data,
                success: function (view) {

                    //Set html on view
                    $catalogs.find('.catalog-content').html(view);

                    //Check for top-actions
                    if (selector === '#np-catalogs') {
                        if ($('#np-catalogs .generic-catalog-actions .top-actions').length === 0) {
                            $('#np-catalogs .generic-catalog-actions').append('<div class="top-actions"></div>')
                        }
                    } else if (selector === '#np-farescatalogs') {
                        if ($('#np-farescatalogs .generic-catalog-actions .top-actions').length === 0) {
                            $('#np-farescatalogs .generic-catalog-actions').append('<div class="top-actions"></div>')
                        }
                    }

                    //Get particular top actions
                    var topActions = $catalogs.find('.catalog-content .top-actions').html();
                    $catalogs.find('.generic-catalog-actions > .top-actions').html(topActions);

                    //Remove top-action from template
                    $catalogs.find('.catalog-content .top-actions').remove();

                    //Get particular top filters
                    var topFilters = $catalogs.find('.catalog-content .top-filters').html();
                    if (topFilters) {
                        $catalogs.find('.generic-catalog-actions').prepend(topFilters);
                        $catalogs.find('.catalog-content .top-filters').remove();
                    }

                    //Delete top-actions if have no children
                    if ($catalogs.find('.generic-catalog-actions > .top-actions').children().length === 0) {
                        $catalogs.find('.generic-catalog-actions > .top-actions').remove();
                    }

                    //Init catalog
                    try {

                        if (conceptType) {
                            UX.Cotorra[action].UI.ConceptType = conceptType;
                            UX.Cotorra[action].UI.Init();
                        }
                        else if (configParams.nomEvalType) {
                            UX.Cotorra.NOM035[action].UI.Init(configParams.nomEvalType);
                        }
                        else {
                            UX.Cotorra[action].UI.Init();
                        }

                    } catch (e) {
                        console.log(e);
                    }

                    //Quick search
                    UX.Cotorra.Common.SetQuickSearch(selector + ' .quicksearch',
                        selector + ' .catalog-content .full-kendo-grid');

                },
                error: function (error) {
                    UX.Modals.Alert('ERROR', UX.Common.getMessageFromError(error).message, 'm', 'error', function () { });
                    UX.Loaders.RemoveLoader(selector);
                },
                complete: function (complete) {
                }
            });
        });

    });

    $(selector + ' .scrollbar-macosx').scrollbar();

    //Set last selected index
    let lsi = lastSelectedCatalog[selector] ? lastSelectedCatalog[selector] : 0;
    $($menu.children()[lsi]).find('a').click();
};

/*Options for catalogs */
UX.Cotorra.Catalogs.Options = [
    { Title: 'Registros patronales', Action: 'EmployerRegistration' },
    { Title: 'Certificados', Action: 'EmployerFiscalInformation' },
    { Title: 'Periodos', Action: 'Periods' },
    //{ Title: 'Tipos de periodo', Action: 'PeriodTypes' },
    { Title: 'Bancos', Action: 'Banks' },
    { Title: 'Turnos', Action: 'Shifts' },
    { Title: 'Puestos', Action: 'Positions' },
    { Title: 'Áreas', Action: 'Areas' },
    { Title: 'Departamentos', Action: 'Departments' },
    { Title: 'Centros de trabajo', Action: 'WorkCenters' },
    { Title: 'Tipos de incidencia', Action: 'IncidentTypes' },
    { Title: 'Tipos de acumulado', Action: 'AccumulatedTypes' },
    { Title: 'Percepciones', Action: 'Perceptions' },
    { Title: 'Deducciones', Action: 'Deductions' },
    { Title: 'Obligaciones', Action: 'Liabilities' },
    { Title: 'Prestaciones', Action: 'BenefitTypes' },

];

UX.Cotorra.Catalogs.FaresOptions = [
    { Title: 'Salarios mínimos', Action: 'MinimunSalaries' },
    { Title: 'ISR mensual', Action: 'MonthlyIncomeTax' },
    { Title: 'Subsidio al empleo mensual', Action: 'MonthlyEmploymentSubsidy' },
    { Title: 'ISR anual', Action: 'AnualIncomeTax' },
    { Title: 'Subsidio al empleo anual', Action: 'AnualEmploymentSubsidy' },
    { Title: 'Finiquito', Action: 'Settlement' },
    { Title: 'UMA', Action: 'UMA' },
    { Title: 'Topes SGDF', Action: 'SGDFLimits' },
    { Title: 'Tablas IMSS Patrón', Action: 'IMSSEmployerTable' },
    { Title: 'Tablas IMSS Empleado', Action: 'IMSSEmployeeTable' },
    { Title: 'UMI', Action: 'UMI' },
    { Title: 'Seguro vivienda INFONAVIT', Action: 'InfonavitInsurance' },
];

UX.Cotorra.Catalogs.NOM035Options = [
    { Title: 'Cuestionarios', Action: 'Questionnaries' },
    { Title: 'Secciones', Action: 'Sections' },
    { Title: 'Preguntas', Action: 'Questions' },
    { Title: 'Aplicaciones', Action: 'Applications' },
    { Title: 'Resultados', Action: 'Results' },
];

/*Generic catalog methods */
UX.Cotorra.GenericCatalog = {

    UI: {
        Init: function (catalogName, containerSelector, params = {}) {
            let data = null;

            //Show grid
            if (params.isNOM035) {
                UX.Cotorra.NOM035[catalogName].UI.LoadGrid({ data: [], evalType: params.nomEvalType });
                data = {
                    evalType: params.nomEvalType
                };
            } else {
                UX.Cotorra[catalogName].UI.LoadGrid([]);
            }

            UX.Loaders.SetLoader(containerSelector);

            //Fix for concepts
            //if (catalogName === 'Concepts') {
            //    data = { type: UX.Cotorra.Concepts.UI.ConceptType };
            //}

            //Get
            UX.Cotorra.Catalogs.Get(catalogName, data,
                (data) => {
                    if (params.isNOM035) {
                        UX.Cotorra.NOM035[catalogName].UI.LoadGrid({
                            data: data,
                            evalType: params.nomEvalType
                        });
                    } else {
                        UX.Cotorra[catalogName].UI.LoadGrid(data);
                        UX.Cotorra.Catalogs[catalogName] = data;
                    }
                },
                (error) => { UX.Common.ErrorModal(error); },
                (complete) => { UX.Loaders.RemoveLoader(containerSelector); });

            //Set top action behaviors

            if (containerSelector === '#np-catalogs') {
                $('#np-catalogs #np_btnAddRecord').off('click').on('click', function (ev) {
                    UX.Cotorra[catalogName].UI.OpenSave();
                });
            }

            if (containerSelector === '#np-farescatalogs') {
                $('#np-farescatalogs #np_btnAddRecord').off('click').on('click', function (ev) {
                    UX.Cotorra[catalogName].UI.OpenSave();
                });
            }

        },

        Delete: function (obj, title, desc, catalogName, containerSelector) {

            //Show modal
            UX.Modals.Confirm(title, desc, 'Sí, eliminar', 'No, espera',
                () => {
                    let row = UX.Cotorra.Common.GetRowData(obj);

                    UX.Loaders.SetLoader(containerSelector);
                    UX.Cotorra.Catalogs.Delete(
                        catalogName,
                        {
                            id: row.dataItem.ID
                        },
                        (data) => { $(row.el).fadeOut(function () { row.$kendoGrid.dataSource.remove(row.dataItem); }); },
                        (error) => { UX.Common.ErrorModal(error); },
                        (complete) => { UX.Loaders.RemoveLoader(containerSelector); }
                    );
                },
                () => {
                });

        }
    }

};


var lastSelectedCatalog = [];

