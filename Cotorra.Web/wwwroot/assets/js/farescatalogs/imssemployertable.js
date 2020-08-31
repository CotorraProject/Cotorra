'use strict';

UX.Cotorra.IMSSEmployerTable = {

    UI: {

        CatalogName: 'IMSSEmployerTable',
        ContainerSelector: '#np-farescatalogs',
        TitleModalsString: 'tablas IMSS patrón',

        Init: function () {
            UX.Cotorra.GenericCatalog.UI.Init(this.CatalogName, this.ContainerSelector);
        },

        LoadGrid: function (data = []) {

            let cn = this.CatalogName.toLowerCase();

            //Set fields
            let fields = {
                ID: { type: 'string' },
                ValidityDate: { type: 'date' },
                EG_Especie_GastosMedicos_1: { type: 'number' },
                EG_Especie_Fija_2: { type: 'number' },
                EG_Especie_mas_3SMDF_3: { type: 'number' },
                EG_Prestaciones_en_Dinero_4: { type: 'number' },
                Invalidez_y_vida_5: { type: 'number' },
                Cesantia_y_vejez_6: { type: 'number' },
                Guarderias_7: { type: 'number' },
                Retiro_8: { type: 'number' },
            };

            //Set columns
            let columns = [
                {
                    field: 'ValidityDate', title: 'Vigencia', width: 150,
                    template: ' #= moment(ValidityDate).format("DD/MM/YYYY") #'
                },
                { field: 'EG_Especie_GastosMedicos_1', title: 'EG Especie gastos médicos', width: 150 },
                { field: 'EG_Especie_Fija_2', title: 'EG Especie fija', width: 150 },
                { field: 'EG_Especie_mas_3SMDF_3', title: 'EG Expecie mas 3 SMDF', width: 150 },
                { field: 'EG_Prestaciones_en_Dinero_4', title: 'EG Prestaciones en dinero', width: 150 },
                { field: 'Invalidez_y_vida_5', title: 'Invalidez y vida', width: 150 },
                { field: 'Cesantia_y_vejez_6', title: 'Cesantia y vejez', width: 150 },
                { field: 'Guarderias_7', title: 'Guarderías', width: 150 },
                { field: 'Retiro_8', title: 'Retiro', width: 150 },
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




