'use strict';

UX.Cotorra.InfonavitInsurance = {

    UI: {

        CatalogName: 'InfonavitInsurance',
        ContainerSelector: '#np-farescatalogs',
        TitleModalsString: 'Seguro vivienda INFONAVIT',

        Init: function () {
            UX.Cotorra.GenericCatalog.UI.Init(this.CatalogName, this.ContainerSelector);
        },

        LoadGrid: function (data) {

            let cn = this.CatalogName.toLowerCase();

            //Set fields
            let fields = {
                ID: { type: 'string' },
                InitialDate: { type: 'date' },
                Value: { type: 'number' }
            };

            //Set columns
            let columns = [
                { field: 'InitialDate', title: 'Fecha de inicio', template: UX.Cotorra.Common.GridFormatDate('InitialDate') },
                { field: 'Value', title: 'Couta bimestral', template: UX.Cotorra.Common.GridFormatCurrency('Value') },
                {
                    title: ' ', width: 100,
                    template: kendo.template($('#' + cn + 'ActionTemplate').html())
                }
            ];

            //Init grid
            let $grid = UX.Common.InitGrid({ selector: '.np-' + cn + '-grid', data: data, fields: fields, columns: columns });

        }
    }

};