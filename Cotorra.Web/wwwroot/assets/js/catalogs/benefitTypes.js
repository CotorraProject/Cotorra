'use strict';

UX.Cotorra.BenefitTypes = {

    UI: {

        CatalogName: 'BenefitTypes',
        ContainerSelector: '#np-catalogs',
        TitleModalsString: 'Prestaciones',

        Init: function () {
            UX.Cotorra.GenericCatalog.UI.Init(this.CatalogName, this.ContainerSelector);
        },

        LoadGrid: function (data = [], setBenefitTypeName = true) {

            let cn = this.CatalogName.toLowerCase();

            //Set validity filter
            let selectedDropData = null;
            if (data.length > 0 && setBenefitTypeName) {

                //Set options and change behavior
                let $drpBenefitTypeName = $('#np_fc_drpBenefitTypeName');
                for (var i = 0; i < data.length; i++) { $drpBenefitTypeName.append(new Option(data[i][0].Name, i)); }
                $drpBenefitTypeName.off('change').on('change', function () { UX.Cotorra.BenefitTypes.UI.LoadGrid([data[parseInt($(this).val())]], false); });
                $drpBenefitTypeName.change();
                return;
            } else if (data.length === 1) {
                selectedDropData = data[0];
            }

            //Set fields
            let fields = {
                ID: { type: 'string' },
                Name: { type: 'string' },
                Antiquity: { type: 'number' },
                Holidays: { type: 'number' },
                HolidayPremiumPortion: { type: 'number' },
                DaysOfChristmasBonus: { type: 'number' },
                IntegrationFactor: { type: 'number' },
                DaysOfAntiquity: { type: 'number' },
                IsDeleteable: { type: 'boolean' }
            };

            //Set columns
            let columns = [
                { field: 'Name', title: 'Nombre', width: 150, },
                { field: 'Antiquity', title: 'Antigüedad', width: 150, },
                { field: 'Holidays', title: 'Días de vacaciones' },
                { field: 'HolidayPremiumPortion', title: 'Porc. prima vacacional' },
                { field: 'DaysOfChristmasBonus', title: 'Días de aguinaldo' },
                { field: 'IntegrationFactor', title: 'Factor de integración' },

                {
                    title: ' ', width: 100,
                    template: kendo.template($('#' + cn + 'ActionTemplate').html())
                }
            ];


            //Init grid
            let $grid = UX.Common.InitGrid({ selector: '.np-' + cn + '-grid', data: selectedDropData ? selectedDropData : data, fields: fields, columns: columns });
        },

        OpenSave: function (obj = null) {

            let catNam = this.CatalogName;
            let titStr = this.TitleModalsString;

            let row = UX.Cotorra.Common.GetRowData(obj);
            let containerID = 'record_save_wrapper';

            let modalID = UX.Modals.OpenModal(
                !row ? 'Nueva ' + titStr + '' : 'Editar ' + titStr + '', 's', '<div id="' + containerID + '"></div>',
                function () {

                    let $container = $('#' + containerID);

                    //Init template
                    var saveModel = UX.Cotorra.BenefitTypes.UI.SaveModel = new Ractive({
                        el: '#' + containerID,
                        template: '#' + catNam.toLowerCase() + '_save_template',
                        data: {
                            ID: row ? row.dataItem.ID : null,
                            Antiquity: row ? row.dataItem.Antiquity : $('.np-' + catNam.toLowerCase() + '-grid').data('kendoGrid').dataSource._data.length + 1,
                            Name: row ? row.dataItem.Name : "No espeficicado",
                            Holidays: row ? row.dataItem.Holidays : 0,
                            HolidayPremiumPortion: row ? row.dataItem.HolidayPremiumPortion : 25,
                            DaysOfChristmasBonus: row ? row.dataItem.DaysOfChristmasBonus : 15,
                            IntegrationFactor: row ? row.dataItem.IntegrationFactor : 0,
                            DaysOfAntiquity: row ? row.dataItem.DaysOfAntiquity : 0,
                        }
                    });

                    //Buttons
                    $('#np_btnCancelSaveRecord').on('click', function () { UX.Modals.CloseModal(modalID); });
                    $('#np_btnSaveRecord').on('click', function () { $('#np_cat_saverecord').data('formValidation').validate(); });

                    //Masks
                    $("#np_Holidays").inputmask("decimal", {
                        radixPoint: ".", groupSeparator: '', digits: 2, autoGroup: true, rightAlign: false
                    });
                    $("#np_HolidayPremiumPortion").inputmask("decimal", {
                        radixPoint: ".", groupSeparator: '', digits: 2, autoGroup: true, rightAlign: false
                    });
                    $("#np_DaysOfChristmasBonus").inputmask("decimal", {
                        radixPoint: ".", groupSeparator: '', digits: 2, autoGroup: true, rightAlign: false
                    });
                    $("#np_IntegrationFactor").inputmask("decimal", {
                        radixPoint: ".", groupSeparator: '', digits: 4, autoGroup: true, rightAlign: false
                    });
                    $("#np_DaysOfAntiquity").inputmask("decimal", {
                        radixPoint: ".", groupSeparator: '', digits: 2, autoGroup: true, rightAlign: false
                    });

                    //Set validations
                    $('#np_cat_saverecord').formValidation({
                        framework: 'bootstrap',
                        live: 'disabled',
                        fields: {
                            np_Antiquity: {
                                validators: {
                                    notEmpty: { message: 'Debes capturar los años de antigüedad' }
                                }
                            },
                            np_Holidays: {
                                validators: {
                                    notEmpty: { message: 'Debes capturar los días de vacaciones' }
                                }
                            },
                            np_HolidayPremiumPortion: {
                                validators: {
                                    notEmpty: { message: 'Debes capturar el porcentaje de prima vacacional' },
                                    between: {
                                        max: 100,
                                        min: 0,
                                        message: 'El valor debe ser entre 0 y 100'
                                    }
                                }
                            },
                            np_DaysOfChristmasBonus: {
                                validators: {
                                    notEmpty: { message: 'Debes capturar los días de aguinaldo' }
                                }
                            },
                            np_IntegrationFactor: {
                                validators: {
                                    notEmpty: { message: 'Debes capturar el factor de integración' },
                                    between: {
                                        max: 100,
                                        min: 0,
                                        message: 'El valor debe ser entre 0 y 100'
                                    }
                                }
                            },
                            np_DaysOfAntiquity: {
                                validators: {
                                    notEmpty: { message: 'Debes capturar los días de antigüedad' },
                                    between: {
                                        max: 100,
                                        min: 0,
                                        message: 'El valor debe ser entre 0 y 100'
                                    }
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

                                        //Insert
                                        let dataItem = grid.dataSource.insert(grid.dataSource._data.length, data);
                                        $('.k-grid-content').scrollTop(1000);
                                        setTimeout(function () {
                                            $('tr[data-uid="' + dataItem.uid + '"]').animateCSS('flash');
                                        }, 100);

                                    } else {
                                        //Update data
                                        let dataItem = row.dataItem;
                                        dataItem.Name = data.Name;
                                        dataItem.Antiquity = data.Antiquity;
                                        dataItem.Holidays = data.Holidays;
                                        dataItem.HolidayPremiumPortion = data.HolidayPremiumPortion;
                                        dataItem.DaysOfChristmasBonus = data.DaysOfChristmasBonus;
                                        dataItem.IntegrationFactor = data.IntegrationFactor;
                                        dataItem.DaysOfAntiquity = data.DaysOfAntiquity;

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

                    saveModel.observe('Holidays',
                        function (newValue, oldValue) {
                            UX.Cotorra.BenefitTypes.UI.CalculateIntegrationFactor();
                        },
                        { init: false, defer: true });

                    saveModel.observe('HolidayPremiumPortion',
                        function (newValue, oldValue) {
                            UX.Cotorra.BenefitTypes.UI.CalculateIntegrationFactor();
                        },
                        { init: false, defer: true });

                    saveModel.observe('DaysOfChristmasBonus',
                        function (newValue, oldValue) {
                            UX.Cotorra.BenefitTypes.UI.CalculateIntegrationFactor();
                        },
                        { init: false, defer: true });


                    //Get data (if applicable)
                });
        },

        Delete: function (obj) {
            let ts = this.TitleModalsString;
            UX.Cotorra.GenericCatalog.UI.Delete(obj, 'Eliminar ' + ts, '¿Deseas eliminar el ' + ts + '?',
                this.CatalogName, this.ContainerSelector);
        },

        CalculateIntegrationFactor: function (obj) {
            let holidays = UX.Cotorra.BenefitTypes.UI.SaveModel.get('Holidays') * 1;
            let holidayPremiumPortion = UX.Cotorra.BenefitTypes.UI.SaveModel.get('HolidayPremiumPortion') * 1;
            let daysOfChristmasBonus = UX.Cotorra.BenefitTypes.UI.SaveModel.get('DaysOfChristmasBonus') * 1;

            let calculatedIntegrationFactor = (((daysOfChristmasBonus + holidays * (holidayPremiumPortion / 100)) / 365) + 1).toFixed(4);
            UX.Cotorra.BenefitTypes.UI.SaveModel.set('IntegrationFactor', calculatedIntegrationFactor ? calculatedIntegrationFactor : 0);

        },
    }

};




