'use strict';

UX.Cotorra.Accumulated = {
    UI: {
        CatalogName: 'Accumulated',
        ContainerSelector: '#np-accumulated',
        TitleModalsString: 'Acumulados',
        ActualRow: null,

        Init: (params = {}) => {

            let modalID = UX.Modals.OpenModal(
                'Acumulados del colaborador',
                'xm', '<div id="np-editaccumulated"></div>',
                function () {

                    let editModel = new Ractive({
                        el: '#np-editaccumulated',
                        template: '#np-editaccumulated-template',
                        data: {
                            Employee: params.Employee

                        },
                        modifyArrays: true
                    });

                    //Buttons
                    $('#np_btnCancelSaveRecord').on('click', function (ev) {
                        ev.preventDefault();
                        UX.Modals.CloseModal(modalID);
                    });
                    $('#np_btnSaveRecord').on('click', function (ev) {
                        ev.preventDefault();
                        UX.Cotorra.Accumulated.UI.Update();
                    });

                    // GetData
                    UX.Cotorra.Accumulated.UI.EditModel = editModel;

                    //Load empty grid
                    UX.Cotorra.Accumulated.UI.LoadGrid([]);

                    (async function () {
                        //Load data
                        let getAccumulated = UX.Cotorra.Catalogs.Get(
                            'Accumulated',
                            { employeeID: params.Employee.ID },
                            (data) => { UX.Cotorra.Accumulated.UI.LoadGrid(data); },
                            (error) => { UX.Common.ErrorModal(error); },
                            (complete) => { }
                        );

                        await UX.Cotorra.Catalogs.Require({
                            loader: {
                                container: '#' + modalID,
                                removeWhenFinish: true
                            },
                            catalogs: ['Periods'],
                            requests: [getAccumulated],
                            forceLoad: false
                        });
                    })();
                });
        },

        GridFormatCurrencyField: (field) => {
            let template =
                '<div class="form-group textfield currency accumulated-field-container" style="margin: 0 !important;">' +
                '    <input data-atid="#= AccumulatedTypeID #" data-aeid="#= AccumulatedEmployeeID #" type="text" class="form-control currency-input accumulated-field" autocomplete="off" value="#= ' + field + '#" />' +
                '</div>';
            return kendo.template(template);
        },

        GridFormatCurrency: (field) => {
            let template = '<div # if(' + field + ' == 0) { # style="opacity:0.62;" # } # class="currency-grid-value # if(' + field + ' < 0) { # negative # } #"> #= UX.Cotorra.Common.FormatCurrency(' + field + ') # </div>';
            return kendo.template(template);
        },

        LoadGrid: function (data) {
            let cn = this.CatalogName.toLowerCase();

            //Set fields
            let fields = {
                AccumulatedTypeID: { type: 'string' },
                AccumulatedEmployeeID: { type: 'string' },
                PreviousExerciseAccumulated: { type: 'string' },
                InitialExerciseAmount: { type: 'string' },
                January: { type: 'string' },
                February: { type: 'string' },
                March: { type: 'string' },
                April: { type: 'string' },
                May: { type: 'string' },
                June: { type: 'string' },
                July: { type: 'string' },
                August: { type: 'string' },
                September: { type: 'string' },
                October: { type: 'string' },
                November: { type: 'string' },
                December: { type: 'string' },
            };

            //Set columns
            let columns = [
                //{ field: 'AccumulatedTypeID', title: 'AccumulatedTypeID', width: 250, },
                //{ field: 'AccumulatedEmployeeID', title: 'AccumulatedEmployeeID', width: 250, },
                { field: 'Name', title: 'Nombre', width: 230, },
                { field: 'PreviousExerciseAccumulated', title: 'Acumulado <br> ejercicio anterior', width: 130, template: UX.Cotorra.Accumulated.UI.GridFormatCurrency('PreviousExerciseAccumulated') },
                { field: 'InitialExerciseAmount', title: 'Importe inicial <br>del ejercicio', width: 150, template: UX.Cotorra.Accumulated.UI.GridFormatCurrencyField('InitialExerciseAmount') },
                { field: 'January', title: 'Enero', width: 150, template: UX.Cotorra.Accumulated.UI.GridFormatCurrency('January') },
                { field: 'February', title: 'Febrero', width: 150, template: UX.Cotorra.Accumulated.UI.GridFormatCurrency('February') },
                { field: 'March', title: 'Marzo', width: 150, template: UX.Cotorra.Accumulated.UI.GridFormatCurrency('March') },
                { field: 'April', title: 'Abril', width: 150, template: UX.Cotorra.Accumulated.UI.GridFormatCurrency('April') },
                { field: 'May', title: 'Mayo', width: 150, template: UX.Cotorra.Accumulated.UI.GridFormatCurrency('May') },
                { field: 'June', title: 'Junio', width: 150, template: UX.Cotorra.Accumulated.UI.GridFormatCurrency('June') },
                { field: 'July', title: 'Julio', width: 150, template: UX.Cotorra.Accumulated.UI.GridFormatCurrency('July') },
                { field: 'August', title: 'Agosto', width: 150, template: UX.Cotorra.Accumulated.UI.GridFormatCurrency('August') },
                { field: 'September', title: 'Septiembre', width: 150, template: UX.Cotorra.Accumulated.UI.GridFormatCurrency('September') },
                { field: 'October', title: 'Octubre', width: 150, template: UX.Cotorra.Accumulated.UI.GridFormatCurrency('October') },
                { field: 'November', title: 'Noviembre', width: 150, template: UX.Cotorra.Accumulated.UI.GridFormatCurrency('November') },
                { field: 'December', title: 'Diciembre', width: 150, template: UX.Cotorra.Accumulated.UI.GridFormatCurrency('December') }
            ];

            //Init grid
            let $grid = UX.Common.InitGrid({
                selector: '.np-' + cn + '-grid', data: data, fields: fields, columns: columns,
                pageable: { refresh: false, alwaysVisible: false, pageSizes: 500, },
                dataBound: function () {
                    $('.np-accumulated-grid').initUIElements();

                    $('.np-accumulated-grid .accumulated-field').off('focus').on('focus', function (ev) {
                        setTimeout(function () {
                            $(this).select();
                        }, 100);
                    });

                    $('.np-accumulated-grid .accumulated-field').off('change').on('change', function (ev) {
                        if ($(this).val() === '') { $(this).val(0); }
                        $(this).addClass('modified');
                    });
                }
            });
        },

        Update: function () {

            UX.Common.ClearFocus();
            let inputs = $('.np-accumulated-grid input.accumulated-field.modified');
            let data = [];
            let year = UX.Cotorra.Catalogs.Periods.find(x => { return x.ID === UX.Cotorra.PrePayroll.UI.Header.PeriodID }).Year;
            let employeeID = UX.Cotorra.Accumulated.UI.EditModel.get('Employee.ID');

            for (var i = 0; i < inputs.length; i++) {
                let $input = $(inputs[i]);

                data.push({
                    AccumulatedID: $input.data('atid'),
                    AccumulatedEmployeeID: $input.data('aeid'),
                    InitialExerciseAmount: $input.val(),
                    ExerciseFiscalYear: year,
                    EmployeeID: employeeID
                });

            }

            UX.Loaders.SetLoader('#np-accumulated');
            UX.Cotorra.Catalogs.Save('Accumulated',
                { aem: data },
                (data) => { $('#np-editaccumulated #np_btnCancelSaveRecord').click(); },
                (error) => { UX.Common.ErrorModal(error); },
                (complete) => { UX.Loaders.RemoveLoader('#np-accumulated'); }
            );
        },
    }
};