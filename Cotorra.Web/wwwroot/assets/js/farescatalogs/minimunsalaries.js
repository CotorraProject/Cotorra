'use strict';

UX.Cotorra.MinimunSalaries = {

    UI: {

        CatalogName: 'MinimunSalaries',
        ContainerSelector: '#np-farescatalogs',
        TitleModalsString: 'salario mínimo',

        Init: function () {
            UX.Cotorra.GenericCatalog.UI.Init(this.CatalogName, this.ContainerSelector);
        }, 

        LoadGrid: function (data) {

            let cn = this.CatalogName.toLowerCase();

            //Set fields
            let fields = {
                ID: { type: 'string' },
                ZoneA: { type: 'number' },
                ZoneB: { type: 'number' },
                ZoneC: { type: 'number' },
                ExpirationDate: { type: 'date' }
            };

            //Set columns
            let columns = [
                {
                    field: 'ExpirationDate', title: 'Vigencia', width: 150,
                    template: ' #= moment(ExpirationDate).format("DD/MM/YYYY") #'
                },
                { field: 'ZoneA', title: 'Zona A', template: UX.Cotorra.Common.GridFormatCurrency('ZoneA') },
                { field: 'ZoneB', title: 'Zona B', template: UX.Cotorra.Common.GridFormatCurrency('ZoneB') },
                { field: 'ZoneC', title: 'Zona C', template: UX.Cotorra.Common.GridFormatCurrency('ZoneC') },
                {
                    title: ' ', width: 100,
                    template: kendo.template($('#' + cn + 'ActionTemplate').html())
                }
            ];

            //Init grid
            let $grid = UX.Common.InitGrid({ selector: '.np-' + cn + '-grid', data: data, fields: fields, columns: columns });

        },

        OpenSave: function (obj = null) {

            let catNam = this.CatalogName;
            let titStr = this.TitleModalsString;

            let row = UX.Cotorra.Common.GetRowData(obj);
            let containerID = 'record_save_wrapper';

            let modalID = UX.Modals.OpenModal(
                !row ? 'Nuevo ' + titStr + '' : 'Editar ' + titStr + '', 's', '<div id="' + containerID + '"></div>',
                function () {

                    let $container = $('#' + containerID);

                    //Init template
                    var saveModel = new Ractive({
                        el: '#' + containerID,
                        template: '#' + catNam.toLowerCase() + '_save_template',
                        data: {
                            ID: row ? row.dataItem.ID : null,
                            ExpirationDate: moment(row ? row.dataItem.ExpirationDate : new Date).format("DD/MM/YYYY"),
                            ZoneA: row ? row.dataItem.ZoneA : '0.00',
                            ZoneB: row ? row.dataItem.ZoneB : '0.00',
                            ZoneC: row ? row.dataItem.ZoneC : '0.00'
                        }
                    });

                    //Buttons
                    $('#np_btnCancelSaveRecord').on('click', function () { UX.Modals.CloseModal(modalID); });
                    $('#np_btnSaveRecord').on('click', function () { $('#np_cat_saverecord').data('formValidation').validate(); });

                    //Masks
                    $('#np_minsal_expiration').mask('00/00/0000');

                    //Set validations
                    $('#np_cat_saverecord').formValidation({
                        framework: 'bootstrap',
                        live: 'disabled',
                        fields: {
                            np_minsal_expiration: {
                                validators: {
                                    notEmpty: { message: 'Debes ingresar la vigencia' },
                                    callback: { callback: UX.Common.ValidDateValidator }
                                }
                            },
                            np_minsal_zonea: {
                                validators: {
                                    notEmpty: { message: 'Debes ingresar el salario de la zona A' },
                                    greaterThan: { value: 0, message: 'El salario debe ser mayor o igual a 0.00' },
                                    lessThan: { value: 999999.99, message: 'El salario debe ser menor o igual a 999,999.99' }
                                }
                            },
                            np_minsal_zoneb: {
                                validators: {
                                    notEmpty: { message: 'Debes ingresar el salario de la zona B' },
                                    greaterThan: { value: 0, message: 'El salario debe ser mayor o igual a 0.00' },
                                    lessThan: { value: 999999.99, message: 'El salario debe ser menor o igual a 999,999.99' }
                                }
                            },
                            np_minsal_zonec: {
                                validators: {
                                    notEmpty: { message: 'Debes ingresar el salario de la zona C' },
                                    greaterThan: { value: 0, message: 'El salario debe ser mayor o igual a 0.00' },
                                    lessThan: { value: 999999.99, message: 'El salario debe ser menor o igual a 999,999.99' }
                                }
                            },
                        },
                        onSuccess: function (ev) {
                            ev.preventDefault();
                            UX.Common.ClearFocus();

                            //Set loader
                            UX.Loaders.SetLoader('#' + modalID);

                            //Save data
                            let data = saveModel.get();
                            UX.Cotorra.Catalogs.Save(
                                catNam,
                                data,
                                (id) => {

                                    let grid = row ? row.$kendoGrid : $('.np-' + catNam.toLowerCase() + '-grid').data('kendoGrid');

                                    if (!row) {
                                        //Set data
                                        data.ID = id;
                                        data.ExpirationDate = UX.Common.StrToDate(data.ExpirationDate);

                                        //Insert
                                        let dataItem = grid.dataSource.insert(0, data);
                                        $('tr[data-uid="' + dataItem.uid + '"]').animateCSS('flash');

                                    } else {
                                        //Update data
                                        let dataItem = row.dataItem;
                                        dataItem.ExpirationDate = UX.Common.StrToDate(data.ExpirationDate);
                                        dataItem.ZoneA = data.ZoneA;
                                        dataItem.ZoneB = data.ZoneB;
                                        dataItem.ZoneC = data.ZoneC;

                                        //Redraw
                                        UX.Common.KendoFastRedrawRow(grid, dataItem);
                                        $('tr[data-uid="' + dataItem.uid + '"]').animateCSS('flash');
                                    }

                                    UX.Modals.CloseModal(modalID);
                                },
                                (error) => {
                                    UX.Common.ErrorModal(error);
                                },
                                (complete) => {
                                    UX.Loaders.RemoveLoader('#' + modalID);
                                }
                            );
                        }
                    })
                        .on('err.validator.fv', function (e, data) { UX.Common.FVShowOneMessage(data); });

                    //Init elements
                    $container.initUIElements();

                    //Get data (if applicable)
                });
        },

        Delete: function (obj) {
            let ts = this.TitleModalsString;
            UX.Cotorra.GenericCatalog.UI.Delete(obj, 'Eliminar ' + ts, '¿Deseas eliminar el ' + ts + '?',
                this.CatalogName, this.ContainerSelector);
        }
    }

};