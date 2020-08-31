'use strict';

UX.Cotorra.Menu = {

    OpenDashboard: () => { UX.Pills.Add('np_home', 'Dashboard', CotorraAppURL + '/views/dashboard.html'); },

    OpenEmployees: () => { UX.Pills.Add('np_employees', 'Colaboradores', CotorraAppURL + '/views/employees/index.html'); },

    OpenPrePayroll: () => { UX.Pills.Add('np_prepayroll', 'Mis nóminas', CotorraAppURL + '/views/prepayroll/index.html'); },

    OpenCatalogs: () => { UX.Pills.Add('np_catalogs', 'Catálogos', CotorraAppURL + '/views/catalogs/index.html'); },

    OpenFaresCatalogs: () => { UX.Pills.Add('np_farescatalogs', 'Tablas y tarifas', CotorraAppURL + '/views/farescatalogs/index.html'); },

    OpenSettlement: () => { UX.Pills.Add('np_settlement', 'Finiquito', CotorraAppURL + '/views/settlement/index.html'); },

    OpenReports: (reportName) => { window.open(CotorraAppURL + '/reports/index?ReportName=' + reportName + '&c=' + btoa(JSON.stringify(CotorraNode))); },

    //NOM035
    OpenEvalPeriods: () => { UX.Pills.Add('np_nom_evalperiods', 'Periodos evaluación', CotorraAppURL + '/views/nom035/evalperiods.html'); },

    OpenNOMFulfillment: (type) => {
        UX.Pills.Add('np_nom_fulfillment_' + type, 'Cumplimiento NOM035', CotorraAppURL + '/views/nom035/fulfillment.html', null, function () {

        });
    },

    OpenEvalATS: () => { UX.Pills.Add('np_nom_evalats', 'Evaluaciones ATS', CotorraAppURL + '/views/nom035/evaluations.html', null, setNOMEvalATS); },

    OpenEvalRP: () => { UX.Pills.Add('np_nom_evalrp', 'Evaluaciones RP', CotorraAppURL + '/views/nom035/evaluations.html', null, setNOMEvalRP); },

    OpenEvalEEO: () => { UX.Pills.Add('np_nom_evaleo', 'Evaluaciones EO', CotorraAppURL + '/views/nom035/evaluations.html', null, setNOMEvalEO); },

    OpenSurveys: () => { UX.Pills.Add('np_nom_surveys', 'Evaluaciones NOM035', CotorraAppURL + '/views/nom035/surveys.html', null, null); },

    //OKRS
    OpenORGOKRS: () => { UX.Pills.Add('np_okrs', 'Objetivos Organizacionales', CotorraAppURL + '/views/okr/orgokrs/index.html'); },
    
    OpenCompanyConfiguration: () => { UX.Pills.Add('np_company_config', 'Configuración', CotorraAppURL + '/views/companyconfiguration/index.html'); },
};

var setNOMEvalATS = function () { setNOMEval('ATS'); };
var setNOMEvalRP = function () { setNOMEval('RP'); };
var setNOMEvalEO = function () { setNOMEval('EO'); };

var setNOMEval = function (evalType) {
    let evalTypeDescription = '';

    switch (evalType) {
        case 'ATS': evalTypeDescription = 'Acontecimientos traumáticos severos'; break;
        case 'RP': evalTypeDescription = 'Riesgos psicosociales'; break;
        case 'EO': evalTypeDescription = 'Entorno organizacional'; break;
    }

    $('#np-nomcatalogs').attr('id', 'np-nomcatalogs-' + evalType.toLowerCase());
    $('#np-nomcatalogs-' + evalType.toLowerCase() + ' #lblEvalType').html(evalType);
    $('#np-nomcatalogs-' + evalType.toLowerCase() + ' #lblEvalTypeDescription').html(evalTypeDescription);
    UX.Cotorra.Catalogs.Init(UX.Cotorra.Catalogs.NOM035Options, '#np-nomcatalogs-' + evalType.toLowerCase(),
        {
            nomEvalType: evalType
        });
};
