'use stric';

var CotorraAppURL = UX.UrlApp = location.origin;
var CotorraReportsAppURL = '../';

UX.Cotorra = {};

var CotorraNode = {
    IdentityID: "",
    SessionID: "",
    ServiceID: "",
    InstanceID: "",
    CompanyID: "",
    RFC: "",
    LicenseServiceID: "",
    LicenseCode: "",
    LicenseID: "",
    AppID: "",
    Alias: ""
};

var Report = {
    ID: '',
    Filters: {},
    Headers: {}
}

//Get CSS styles
$('<link/>', { rel: 'stylesheet', type: 'text/css', href: CotorraAppURL + '/assets/css/cotorra.min.css' }).appendTo('head');


//Set favicon
document.querySelector('link[rel="shortcut icon"]').setAttribute('href', CotorraAppURL + '/favicon.ico');

//Set title
document.title = document.title.split(' | ')[0] + ' | ' + 'Cotorria by COTORRAi®';

UX.Reports = {
    Do: (data, method = 'GET', action = 'Get', onSuccess, onError, onComplete) => {
        return $.ajax({
            method: method,
            url: '/' + 'Report' + '/' + action + '/',
            data: data,
            success: onSuccess,
            error: onError,
            complete: onComplete
        });
    },

    Initialize: function (params) {

        let reportParams = params
        //UX.Reports.Process(reportParams);
    },

    Process: function (params) {
        $('#designer').hide();
        $('#viewer').show();
        UX.Loaders.SetLoader('body');    
        UX.Reports.Do(
            {
                param: JSON.stringify(params.ReportParams),
                typename: params.ReportParamsType
            },
            'GET', 'GetData',
            (data) => {
                UX.Loaders.RemoveLoader('body');
                UX.Reports.PrintReport({ Data: data, ReportName: params.ReportName });
            },
            (error) => {
                UX.Common.ErrorModal(error);
                UX.Loaders.RemoveLoader('body');
            },
            (complete) => { UX.Loaders.RemoveLoader('body');});
    },

    PrintReport: function (params) {
        let data = params.Data.Data;
        Stimulsoft.Base.StiLicense.key = params.Data.Key;
       
        var dataSet = new Stimulsoft.System.Data.DataSet();
        dataSet.readJson(data);
        var report = new Stimulsoft.Report.StiReport();
        
        Stimulsoft.Base.Localization.StiLocalization.addLocalizationFile(CotorraAppURL + "/assets/js/thirdparty/stimulsoft/es.xml", false, "Spanish");
        Stimulsoft.Base.Localization.StiLocalization.cultureName = "Spanish";

        report.loadFile('/' + 'Report' + '/GetByName?name=' + params.ReportName);
        report.dictionary.databases.clear();
        report.regData(dataSet.dataSetName, "", dataSet);

        var optionsDesigner = new Stimulsoft.Designer.StiDesignerOptions();
        optionsDesigner.appearance.fullScreenMode = false;
       
        var designer = new Stimulsoft.Designer.StiDesigner(optionsDesigner, "StiDesigner", false);
        designer.report = report;

        // Assign the onSaveReport event function
        designer.onSaveReport = function (e) {
            //var jsonStr = e.report.saveToJsonString();
            alert("Save to JSON string complete.")
        }

        // Assign the onCreateReport event function
        designer.onCreateReport = function (e) {
            //var ds = new Stimulsoft.System.Data.DataSet("Demo");
            //ds.readJsonFile("../reports/Demo.json");
            //e.report.regData("Demo", "Demo", ds);
        }

        // Assign the onPreviewReport event function
        designer.onPreviewReport = function (e) {
            switch (e.format) {
                case Stimulsoft.Report.StiExportFormat.Html:
                    e.settings.zoom = 2; // Set HTML zoom factor to 200%
                    break;
            }
        }

        // Assign the onExit event function
        designer.onExit = function (e) {
            UX.Loaders.SetLoader('body');  
            $('#designer').hide();
            $('#viewer').show();
            viewer.renderHtml('viewer');         
            UX.Loaders.RemoveLoader('body');
        }       

        var options = new Stimulsoft.Viewer.StiViewerOptions();
        options.height = "100%";
        options.width = "100%";
        options.toolbar.showParametersButton = false;
        options.appearance.scrollbarsMode = true;
        options.toolbar.visible = true;
        options.toolbar.showDesignButton = true;
        options.toolbar._zoom = 150
        options.appearance.fullScreenMode = false;

        var viewer = new Stimulsoft.Viewer.StiViewer(options, 'StiViewer', false);
        viewer.report = report;

        viewer.onDesignReport = function (event) {
            $('#viewer').hide();
            $('#designer').show();
            UX.Loaders.SetLoader('body'); 
            setTimeout(() => {
                designer.renderHtml("designer");
                UX.Loaders.RemoveLoader('body');
            }, 250);
            
        };

        viewer.renderHtml('viewer');  
    },
}

$(document).ready(() => {

    UX.Cotorra.Catalogs = {};

    $.when(

        //Load common scripts
        $.cachedScript(CotorraAppURL + '/assets/js/' + 'common.js'),
        $.cachedScript(CotorraAppURL + '/assets/js/' + 'loaders.js'),

        //Load catalogs scripts
        $.cachedScript(CotorraAppURL + '/assets/js/catalogs/' + 'main.js'),

        //Third-party
        $.cachedScript(CotorraAppURL + '/assets/thirdparty/bootstrap-datepicker/js/' + 'bootstrap-datepicker.js'),

    ).done(function () {

        //Language for datepicker
        $.cachedScript(CotorraAppURL + '/assets/thirdparty/bootstrap-datepicker/js/locales/' + 'bootstrap-datepicker.es.js');

        //Get reports parameters
        var search = location.search.substring(1);
        let params = (new URL(document.location)).searchParams;
        CotorraNode = JSON.parse(atob(params.get('c')));
        let toDecode = search.substring(0, search.indexOf('&c='));
        let searchObject = JSON.parse('{"' + decodeURI(toDecode).replace(/"/g, '\\"').replace(/&/g, '","').replace(/=/g, '":"') + '"}');

        let reportParams = {
            ReportParams: searchObject,
            ReportName: searchObject['ReportName'],
            ReportParamsType: searchObject['ReportParamsType']
        };

        //Init headers setup
        $.ajaxSetup({
            xhrFields: { withCredentials: false },
            beforeSend: function (xhr) {
                xhr.setRequestHeader("gid", CotorraNode.GuidID);
                xhr.setRequestHeader("iid", CotorraNode.IdentityID);
                xhr.setRequestHeader("ssid", CotorraNode.SessionID);
                xhr.setRequestHeader("srid", CotorraNode.ServiceID);
                xhr.setRequestHeader("inid", CotorraNode.InstanceID);
                xhr.setRequestHeader("cid", CotorraNode.CompanyID);
                xhr.setRequestHeader("rfc", CotorraNode.RFC);
                xhr.setRequestHeader("lsid", CotorraNode.LicenseServiceID);
                xhr.setRequestHeader("lid", CotorraNode.LicenseID);
                xhr.setRequestHeader("aid", CotorraNode.AppID);
            }

        });

        //Load report
        //UX.Loaders.SetLoader('body');
        $.get('/reports/' + reportParams.ReportName + '.html', function (data) {
            $('.main-container').html(data);
            $('.main-container').initUIElements();
            //UX.Loaders.RemoveLoader('body');
            UX.Reports.Initialize(reportParams);
        });

    });

});
