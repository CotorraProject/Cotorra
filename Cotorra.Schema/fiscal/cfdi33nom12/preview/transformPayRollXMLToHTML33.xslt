<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:cfdi="http://www.sat.gob.mx/cfd/3" xmlns:nomina12="http://www.sat.gob.mx/nomina12" xmlns:tfd="http://www.sat.gob.mx/TimbreFiscalDigital" version="2.0" xmlns:catalogSat="urn:catalogSat" xmlns:convert="urn:convert" xmlns:dateFormat="urn:dateFormat"
    exclude-result-prefixes="xs">
  <xsl:output method="html" version="1.0" encoding="UTF-8" indent="no" omit-xml-declaration="yes"/>


  <xsl:param name="logoCotorraTemplate"></xsl:param>
  <xsl:param name="cbbUriTemplate"></xsl:param>
  <xsl:param name="originalstring"></xsl:param>
  <xsl:param name="overdraftID"></xsl:param>
  <xsl:param name="stamporiginalstring"></xsl:param>
  <xsl:param name="logo" ></xsl:param>
  <xsl:param name="waterMark" ></xsl:param>

  <xsl:template match="/">
    <!--<!DOCTYPE html >-->
    <html>
      <head>
        <meta charset='UTF-8' xmlns:cfdi='http://www.sat.gob.mx/cfd/3' xmlns:tfd='http://www.sat.gob.mx/TimbreFiscalDigital'/>
        <link rel='stylesheet' href='https://maxcdn.bootstrapcdn.com/bootstrap/3.3.6/css/bootstrap.min.css'  id='previewer-styles-link'></link>
        <link async="" href="https://maxcdn.bootstrapcdn.com/font-awesome/4.7.0/css/font-awesome.min.css" rel="stylesheet" />
      </head>
      <style>
        <![CDATA[
.container-previewer .title-fields{
          color:#000000;          
          font-size: 14px !important;
          }
          .container-previewer .fields-results{
          color:#000000; 
          font-size: 14px;         
          }
         .container-previewer .square-logo{
          border-style: none;
          color: #BDBDBD;
          height: 110px;
          text-align: center;
          font-size: 30px;
          vertical-align: middle;
          display: table-cell;
          padding-left: 20px;
          padding-right: 20px;
          }
          .container-previewer hr{
          display: block;
          height: 1px;
          border: 0;
          border-top: 1px solid #BDBDBD;
          margin: 1em 0;
          padding: 0;
          }
          .container-previewer .block-title{
            background: #F5B54E;
            text-align: center;
            padding-top: 3px;         
            margin-left: 5px;
          }
           .container-previewer .block-title-cfdiversion{
            background: #F7E3C4;
            text-align: center;
            margin-left: 5px;
          }
          .container-previewer .issuing-name{
              color: #000;
              font-size: 18px;
              font-weight: bold;
          }
          .container-previewer .issuing-rfc{
              color: #000;
              font-size: 16px;
              font-weight: bold;
          }
          .container-previewer #datosEmisor{
              border-top: 2px solid #000;
              border-left: 4px solid #000;
              border-bottom: 2px solid #000;
              border-right: 4px solid #000;
              padding: 7px;
              background-color: #E9EDEC;
          }
          .container-previewer .issuing-details{
              margin-left: 20px;
          }
          .container-previewer .date-block{
            text-align: right;
          }
          .container-previewer .detail-date{
            padding-right:40px
          }
          .container-previewer .detail-hour{
           padding-right:56px
          }
          .container-previewer .text-bold{
            font-weight: bold;
          }
          .container-previewer .title-cfdi{
          color: #000000;
          font-size: small;
          font-weight: bolder;
          }
           .container-previewer .title-version-cfdi{
          color: #ffffff;
          font-size: medium;
          font-weight: bolder;
          }
          .container-previewer .container-logo{
          margin-left: 10px;
          }
          .container-previewer .separator-line{
          margin-top: 10px;
          }
          .container-previewer .emisor{         
           padding-bottom: 20px;
           padding-left: 0px;
           padding-right: 0px;
          }
          .container-previewer .subtitle{
            font-size: 18px;
            color: #000000;
            margin-bottom: 10px;
            background-color: #F8EABA;
          }
          .container-previewer .section-emisor-receptor{         
          margin-top: 20px;
          padding-left: 20px;
          }
          .container-previewer .receptor{                      
             padding-left: 0px;
             padding-right: 0px;
             border: 2px solid #989D9C;
          }        
          .container-previewer #DatosReceptor-2{
            border-left: 2px solid #989D9C;
            padding-bottom: 23px;
          }
         .container-previewer  .title-table{
            border: 2px solid #989D9C;
            background-color: #E9EDEC;
            margin-top: 5px;
            margin-left: 5px;
            text-align: center;
            font-weight: bold;
            margin-bottom: 5px;
         }
         .container-previewer .subtitle-deducciones{
           border-left: 2px solid #989D9C;
         }
         .container-previewer .col-table-header{
            padding-left: 0px !important;
            padding-rigth: 0px !important;           
            float: left;
            position: relative;
            min-height: 1px;           
         }
          .container-previewer .col-table-header-percepciones{
            width: 50%;
          }
           .container-previewer .col-table-header-deducciones{
            width: 50%;
          }
         .container-previewer .section-table{
            margin-left:5px;
            margin-bottom: 10px;
         }      
         .container-previewer .th-notable{
          text-align: initial !important;
         }
         .container-previewer th{
            border:none !important;
             background: #F2F2F2;           
         }
         .container-previewer .totals{
           text-align: end;
            border: 2px solid #000;
            font-weight: bold;
         }              
         .container-previewer thead tr{
            background-color: #E9EDEC !important;
            page-break-inside:avoid;
            page-break-after:auto;         
          }
          .container-previewer tbody tr{
           background-color: #ffffff !important;
          }
          .container-previewer .title-center{
             text-align:center;
          }
          .container-previewer .title-end{
             text-align: end !important;
          }
          .container-previewer .title-start{
             text-align: start !important;
          }
          .container-previewer .total-color{
            color:#2D2FFA
          }
          .container-previewer .section-details-doc{
            margin-left: 5px;
            margin-top: 10px;
          }
           .container-previewer .document-details{
              border: 2px solid #989D9C;
           }
          .container-previewer .fiscal-details{
            border-top: 2px solid #989D9C;
           }
           .container-previewer .label-cfdi{
             background-color: #E9EDEC 
           }
          .container-previewer  .fiscal-details-title{
             border-right: 2px solid #989D9C;
           }
           .container-previewer .title-total-letter{
             background-color: #F8EABA;
           }
           .container-previewer .block-sign{
            margin-top: 75px;
           }
           .container-previewer .title-sign{
              border-top: 4px solid #F8EABA;
              padding-left: 5px;
              padding-right: 5px;
           }
           .container-previewer .title-sign-2{
              border-top: 2px solid #F8EABA;
              padding-left: 10px;
              padding-right: 10px;
              font-size: 8px;
           }
           .container-previewer .border-table{
            border: 2px solid #000;
           }
           .container-previewer .table-bordered{
            border:none !important;
           }
           .container-previewer .border-right{
            border-right:2px solid #000;
           }
            .container-previewer .text-right{
               text-align: right;
            }
          .container-previewer #datostimbre{
            padding-left: 35px;
          }
          .container-previewer{
          /*border: 1px solid #bdbdbd;*/
          width: 1080px !important;
          }
          .container-previewer .header-concepts{
          padding-left: 20px;
          padding-top: 10px;
          }
          .container-previewer .table{
          max-width: 100% !important;
          }                                  
          .container-previewer .table{
            text-align: center;
            page-break-inside: auto;
           /* border: 2px solid #000;*/
          }
          .container-previewer td {
          font-size: 14px;
          border: none !important;
          }         
          .container-previewer .total-field{
          padding-right: 0px !important;
          border-bottom: 1px solid #bdbdbd;
          }
          .container-previewer .symbol-field{
          padding-left: 0px !important;
          }
          .container-previewer .total-result{
          padding-left: 0px !important;
          }
          .container-previewer .conceptos{
          border-bottom: 1px solid #bdbdbd;

          }
          .container-previewer .conceptos div br{
          display:inline;
          }
          .container-previewer .footer{
          padding-top: 10px;
          }
         .container-previewer  .other-totals{
            padding-bottom: 20px;
            border: 2px solid #000;
          }
          .container-previewer .stamping{
            overflow-wrap: break-Word;
            Word-wrap: break-Word;
            Word-break: break-Word;
            -ms-hyphens: auto;
            -moz-hyphens: auto;
            -webkit-hyphens: auto;
            hyphens: auto;
            font-size: 12px;
          }
          .container-previewer #selloemisor{
          padding-top: 10px;
          }
          .container-previewer #sellosat{
          padding-top: 10px;
          }
          .container-previewer .table-row br{
          display: none !important;
          }
          
        
          .container-previewer img{
          width: 200px;
          }
          .container-previewer thead { display:table-header-group }
          .container-previewer tfoot { display:table-footer-group }

          .container-previewer #QR{
            margin: auto;
            padding: 10px;
          }
          .container-previewer #cotorraiLogo{
         
          }
          .container-previewer #logo {
            width: 120px;
        }
        .container-previewer #taxestable > tr, td{
          font-size: 13px !important;
        }
        .container-previewer .taxestd{
         font-size: 13px !important;
        }
        .container-previewer #backgroundforwatermark{
          position:absolute;
          z-index:0;
          background:white;
          display:block;
          min-height:50%; 
          min-width:50%;
          color:yellow;
        }
        .container-previewer watermark{        
          color:lightgrey;
          font-size:120px;
          transform:rotate(300deg);
          -webkit-transform:rotate(300deg);
        }
        .container-previewer table tbody{
          background-color: #f4f4f4;
        }
        .container-previewer thead th{
          background-color: #E9EDEC !important;        
          font-weight: bold !important;
          color: #000 !important;
        }
             .container-previewer .span-long-text{
              text-overflow: ellipsis;
              display: block;
              max-width: 100%;
              word-wrap: break-word;
              } 
              .container-previewer .td-long-text-200{
              max-width: 200px;
              }
              .container-previewer .td-long-text-100{
              max-width: 100px;
              }
              .container-previewer .td-long-text-80{
              max-width: 80px;
              }
              .container-previewer .td-long-text-150{
              max-width: 100px;
              }
              .container-previewer .td-long-text-110{
              max-width: 110px;
              }             
        ]]>
      </style>
      <body>
        <div class="container-previewer">
          <xsl:apply-templates select="/cfdi:Comprobante"/>
        </div>
      </body>
    </html>

  </xsl:template>
  <!--
  Aquí iniciamos el procesamiento de los datos incluidos en el comprobante 
-->
  <xsl:template match="cfdi:Comprobante">
    <div class="row block-title" >
      <span class="title-cfdi" >COMPROBANTE FISCAL DIGITAL POR INTERNET</span>
    </div>
    <div class="row block-title-cfdiversion" >
      <span class="title-cfdi" >Versión CFDI 3.3 y Complemento de Nóminas 1.2</span>
    </div>
    <div class="row section-emisor-receptor">
      <div class="col-xs-12 emisor">
        <xsl:apply-templates select="./cfdi:Emisor"/>
      </div>
      <div class="col-xs-12 receptor">
        <xsl:apply-templates select="./cfdi:Receptor"/>
      </div>
    </div>

    <!--Percepciones-->
    <div class="row title-table">
      <div class="col-xs-6">
        <span>Percepciones</span>
        <table class="table table-striped  table-condensed">
          <thead>
            <tr class="title-center">
              <th style="width: 0%;" >
                Ag. SAT
              </th>
              <th style="width: 8%;" class="text-right">
                No.
              </th>
              <th style="width: 33.2%;" class="title-center">
                Concepto
              </th>
              <th class="text-right">
                Gravado
              </th>
              <th style="width: 16.6%;" class="text-right">
                Exento
              </th>
              <th class="text-right">
                Total
              </th>
            </tr>
          </thead>
          <tbody id="Conceptos">
            <xsl:for-each select="./cfdi:Complemento/nomina12:Nomina/nomina12:Percepciones" >
              <!--<tr>-->
              <xsl:apply-templates select="." />
              <!--</tr>-->
            </xsl:for-each>
          </tbody>
        </table>
        <span>Otros pagos</span>
        <table class="table table-striped  table-condensed">
          <thead>
            <tr class="title-center">
              <th style="width: 0%;" >
                Ag. SAT
              </th>
              <th style="width: 8%;" class="text-right">
                No.
              </th>
              <th style="width: 33.2%;" class="title-center">
                Concepto
              </th>
              <th class="text-right">
                Gravado
              </th>
              <th style="width: 16.6%;" class="text-right">
                Exento
              </th>
              <th class="text-right">
                Total
              </th>
            </tr>
          </thead>
          <tbody id="Conceptos">
            <xsl:for-each select="./cfdi:Complemento/nomina12:Nomina/nomina12:OtrosPagos" >
              <!--<tr>-->
              <xsl:apply-templates select="." />
              <!--</tr>-->
            </xsl:for-each>
          </tbody>
        </table>
      </div>

      <!--Deducciones-->
      <div class="col-xs-6 subtitle-deducciones">
        <span>Deducciones</span>
        <table class="table table-striped table-condensed">
          <thead>
            <tr class="title-center">
              <th style="width: 0%;">
                Ag. SAT
              </th>
              <th style="width: 8%;" class="text-right">
                No.
              </th>
              <th style="width: 55%;" class="title-center">
                Concepto
              </th>
              <th class="text-right">
                Total
              </th>
            </tr>
          </thead>
          <tbody id="Conceptos">
            <xsl:for-each select="./cfdi:Complemento/nomina12:Nomina/nomina12:Deducciones" >
              <!--<tr>-->
              <xsl:apply-templates select="." />
              <!--</tr>-->
            </xsl:for-each>
          </tbody>
        </table>
      </div>
    </div>

    <div class="row section-table">
      <div class="col-table-header col-table-header-percepciones">
        <div class="table-row ">
          <table class="table table-striped  table-condensed border-table">
            <thead>
              <tr>
                <th class="col-xs-6 th-notable" style="width:50%;" >
                  <span>Total Percep. más Otros Pagos $</span>
                </th>
                <th class="title-end" style="width:30%;">
                  <span>
                    <xsl:variable name="percepTotalGravado" select="./cfdi:Complemento/nomina12:Nomina/nomina12:Percepciones/@TotalGravado" />
                    <xsl:value-of select="convert:AmountSeparator( $percepTotalGravado)"/>
                  </span>
                </th>
                <th class="title-end" style="width: 5%;">
                  <span>
                    <xsl:variable name="percepTotalExento" select="./cfdi:Complemento/nomina12:Nomina/nomina12:Percepciones/@TotalExento" />
                    <xsl:value-of select="convert:AmountSeparator( $percepTotalExento)"/>
                  </span>
                </th>
                <th class="title-end" style="width: 15%;" >
                  <span>
                    <xsl:variable name="subTotalPercep" select="./@SubTotal" />
                    <xsl:value-of select="convert:AmountSeparator( $subTotalPercep)"/>
                  </span>
                </th>
              </tr>
            </thead>

          </table>

        </div>

      </div>
      <div class="col-table-header col-table-header-deducciones totals">
        <div class="row">
          <div class="col-xs-6 th-notable" style="margin-left:10px">Subtotal $</div>
          <div class="col-xs-4">
            <xsl:variable name="subTotalTotal" select="./@SubTotal" />
            <xsl:value-of select="convert:AmountSeparator( $subTotalTotal)"/>
          </div>
        </div>
        <div class="row">
          <div class="col-xs-6 th-notable" style="margin-left:10px">Descuentos $</div>
          <div class="col-xs-4">
            <xsl:variable name="descuentoTotal" select="./@Descuento" />
            <xsl:value-of select="convert:AmountSeparator( $descuentoTotal)"/>
          </div>
        </div>
        <div class="row">
          <div class="col-xs-6 th-notable" style="margin-left:10px">Retenciones $</div>
          <div class="col-xs-4">
            <xsl:variable name="totalImpuestosRetenidosTotal" select="./cfdi:Complemento/nomina12:Nomina/nomina12:Deducciones/@TotalImpuestosRetenidos" />
            <xsl:value-of select="convert:AmountSeparator( $totalImpuestosRetenidosTotal)"/>
          </div>
        </div>
        <div class="row total-color">
          <div class="col-xs-6 th-notable" style="margin-left:10px">Total $</div>
          <div class="col-xs-4">
            <xsl:variable name="totalTotal" select="./@Total" />
            <xsl:value-of select="convert:AmountSeparator( $totalTotal)"/>
          </div>
        </div>
      </div>
    </div>
    <div class="row section-details-doc" >
      <div class="col-xs-7 document-details">
        <div class="row label-cfdi title-center">
          <span>Este documento es una representación impresa de un CFDI</span>
        </div>
        <div class="row title-center">
          <span>
            <xsl:value-of select="./@MetodoPago"/>
            <xsl:variable name="metodoPago" select="./@MetodoPago" />
            &#160;-&#160;
            <xsl:value-of select="catalogSat:GetPaymentMethodByCode( $metodoPago)"/>
          </span>
        </div>
        <div class="row title-center">
          <span>
            Forma de pago:
            <xsl:value-of select="./@FormaPago"/>
            <xsl:variable name="formaPago" select="./@FormaPago" />
            &#160;-&#160;
            <xsl:value-of select="catalogSat:GetPaymentFormByCode($formaPago)"/>
          </span>
        </div>
        <div class="row fiscal-details">
          <div class="col-xs-6 fiscal-details-title title-end ">
            <div class="row ">
              <span>Serie del Certificado del emisor:</span>
            </div>
            <div class="row">
              <span>Folio Fiscal UUID:</span>
            </div>
            <div class="row">
              <span>No. de serie del Certificado del SAT:</span>
            </div>
            <div class="row">
              <span>Fecha y hora de certificación:</span>
            </div>
          </div>
          <div class="col-xs-6">
            <div class="row">
              <span>
                <xsl:value-of select="./@NoCertificado"/>
              </span>
            </div>
            <div class="row">
              <span>
                <xsl:value-of select="./cfdi:Complemento/tfd:TimbreFiscalDigital/@UUID"/>
              </span>
            </div>
            <div class="row">
              <span>
                <xsl:value-of select="./cfdi:Complemento/tfd:TimbreFiscalDigital/@NoCertificadoSAT"/>
              </span>
            </div>
            <div class="row">
              <span>
                <xsl:value-of select="./cfdi:Complemento/tfd:TimbreFiscalDigital/@FechaTimbrado"/>
              </span>
            </div>
          </div>
        </div>
      </div>
      <div class="col-xs-5 title-center">
        <div class="row title-total-letter">
          <span>Importe con letra</span>
        </div>
        <div class="row">
          <xsl:variable name="claveMoneda" select="./@Moneda" />
          <xsl:variable name="totalMoneda" select="./@Total"/>

          <span>
            <xsl:value-of select="convert:ConvertAmountToWordsByCurrencyCode($claveMoneda,  $totalMoneda)"/>
          </span>
        </div>
        <div class="row block-sign">
          <span class="title-sign">Firma del empleado</span>
        </div>
        <div>
          <span class="title-sign-2">Se puso a mi disposición el archivo XML correspondiente y recibí de la empresa arriba mencionada la cantidad </span>
          <span class="title-sign-2">neta a que este documento se refiere estando conforme con las percepciones y deducciones que en él</span>
          <span class="title-sign-2">aparecen especificados.</span>
        </div>

      </div>

    </div>
    <div class="row footer">
      <div class="col-xs-4">
        <div id="QR">
          <xsl:choose>
            <xsl:when test="$cbbUriTemplate!= '' ">
              <img>
                <xsl:attribute name="src">
                  <xsl:value-of select="$cbbUriTemplate"/>
                </xsl:attribute>
              </img>
            </xsl:when>
            <xsl:otherwise>
              <span>&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;</span>
            </xsl:otherwise>
          </xsl:choose>
        </div>
        <div id="cotorraiLogo">
          <img>
            <xsl:attribute name="src">
              <xsl:value-of select="$logoCotorraTemplate"/>
            </xsl:attribute>
          </img>
        </div>
      </div>
      <div class="col-xs-8 stamping">
        <div id="cadenatimbre">
          <div class="subtitle">
            <span>Cadena Original del complemento de certificación digital del SAT</span>
          </div>
          <div>
            <xsl:value-of select="$originalstring"/>
          </div>

        </div>
        <div id="selloemisor">
          <div class="subtitle">
            <span>Sello digital del emisor (CFDI)</span>
          </div>
          <div style="overflow-wrap: break-Word;">
            <xsl:value-of select="./@Sello"/>
          </div>
        </div>
        <div id="sellosat">
          <div class="subtitle">
            <span>Sello digital del SAT</span>
          </div>
          <div>
            <xsl:value-of select="./cfdi:Complemento/tfd:TimbreFiscalDigital/@SelloSAT"/>
          </div>
        </div>
      </div>
    </div>
  </xsl:template>

  <!--  Manejador de nodos tipo Emisor  -->
  <xsl:template match="cfdi:Emisor">
    <!--Iniciamos el tratamiento de los atributos del Emisor -->
    <div id="DatosEmisor">
      <div class="row">
        <div class="col-xs-10">
          <span class="issuing-name">
            <xsl:value-of select="./@Nombre"/>
          </span>

        </div>
        <div class="col-xs-2 date-block">
          <span class="detail-date">Fecha:</span>
          <span>
            <xsl:variable name="fecha" select="../@Fecha" />
            <xsl:value-of select="dateFormat:GetDate( $fecha)"/>
          </span>
        </div>
      </div>
      <div class="issuing-details">
        <div class="row">
          <div class="col-xs-4">
            <span class="issuing-rfc">
              RFC:
            </span>
            <span class="issuing-rfc">
              <xsl:value-of select="./@Rfc"/>
            </span>
          </div>
          <div class="col-xs-4">
            <span>Reg Pat: </span>
            <xsl:apply-templates select="/cfdi:Comprobante/cfdi:Complemento/nomina12:Nomina/nomina12:Emisor" />
          </div>
          <div class="col-xs-4 date-block">
            <span class="detail-hour">Hora:</span>
            <span>
              <xsl:variable name="fecha" select="../@Fecha" />
              <xsl:value-of select="dateFormat:GetHour( $fecha)"/>
            </span>
          </div>

        </div>
        <div class="row">
          <div class="col-xs-12">
            <span class="title-fields">
              Régimen fiscal:
            </span>
            <span class="fields-results">
              <xsl:value-of select="./@RegimenFiscal"/>
              <xsl:variable name="claveRegimenFiscal" select="./@RegimenFiscal" />
              &#160;-&#160;
              <xsl:value-of select="catalogSat:GetTaxRegimeByCode($overdraftID, $claveRegimenFiscal)"/>
            </span>

          </div>

        </div>
        <div class="row">
          <div class="col-xs-12">
            <span>Lugar de expedición: </span>
            <span>
              <xsl:value-of select="../@LugarExpedicion"/>
            </span>
            <xsl:variable name="lugarExp" select="../@LugarExpedicion" />
            <span>
              &#160; <xsl:value-of select="catalogSat:GetTownShipByZipCode($lugarExp)"/>
            </span>
            <span>
              &#160; <xsl:value-of select="catalogSat:GetStateByZipCode($lugarExp)"/>
            </span>
          </div>
        </div>
      </div>

    </div>

  </xsl:template>
  <!--  Manejador de nodos tipo Receptor  -->
  <xsl:template match="cfdi:Receptor">
    <!--
 Iniciamos el tratamiento de los atributos del Receptor 
-->
    <xsl:apply-templates select="../cfdi:Complemento/nomina12:Nomina/nomina12:Receptor" />

  </xsl:template>
  <xsl:template match="nomina12:Nomina">
    <!--  Iniciamos el manejo de los elementos hijo en la secuencia -->
    <xsl:for-each select="./nomina12">
      <xsl:apply-templates select="./nomina12:Emisor"/>
    </xsl:for-each>
  </xsl:template>
  <xsl:template match="nomina12:Emisor">
    <xsl:value-of select="./@RegistroPatronal"/>
  </xsl:template>
  <xsl:template match="nomina12:Receptor">
    <div class="col-xs-6" id="DatosReceptor-1">
      <div class="row">
        <div class="col-xs-12">
          <span class="issuing-rfc">
            <xsl:value-of select="./@NumEmpleado"/> -  <xsl:apply-templates select="/cfdi:Comprobante/cfdi:Receptor/@Nombre" />
          </span>
        </div>

      </div>
      <div class="row">
        <div class="col-xs-6">
          <span class="title-fields">
            RFC:
          </span>
        </div>
        <div class="col-xs-6">
          <span class="fields-results">
            <xsl:apply-templates select="/cfdi:Comprobante/cfdi:Receptor/@Rfc" />
          </span>
        </div>
      </div>
      <div class="row">
        <div class="col-xs-6">
          <span class="title-fields">
            CURP:
          </span>
        </div>
        <div class="col-xs-6">
          <span>
            <xsl:value-of select="./@Curp"/>
          </span>
        </div>
      </div>
      <div class="row">
        <div class="col-xs-6">
          <span class="title-fields">
            Fecha Ini Relación Lab:
          </span>
        </div>
        <div class="col-xs-6">
          <span>
            <xsl:variable name="fechaIniLab" select="./@FechaInicioRelLaboral" />
            <xsl:value-of select="dateFormat:GetDate( $fechaIniLab)"/>
          </span>
        </div>
      </div>
      <div class="row">
        <div class="col-xs-6">
          <span class="title-fields">
            Jornada:
          </span>
        </div>
        <div class="col-xs-6">
          <span>
            <xsl:value-of select="./@TipoJornada"/>
            <xsl:variable name="tipoJornada" select="./@TipoJornada" />
            &#160;-&#160;
            <xsl:value-of select="catalogSat:GetTypeDayByCode($overdraftID, $tipoJornada)"/>
          </span>

        </div>
      </div>
      <div class="row">
        <div class="col-xs-6">
          <span class="title-fields">
            NSS:
          </span>
        </div>
        <div class="col-xs-6">
          <span>
            <xsl:value-of select="./@NumSeguridadSocial"/>
          </span>
        </div>
      </div>
    </div>
    <div class="col-xs-6" id="DatosReceptor-2">
      <div class="row">
        <div class="col-xs-6">
          <span class="title-fields text-bold">
            Periodo
            &#160;
            <xsl:value-of select="./@PeriodicidadPago"/>
            &#160;
            <xsl:variable name="periodicidadPago" select="./@PeriodicidadPago" />
            &#160;
            <xsl:value-of select="catalogSat:GetPaymentPeriodByCode( $periodicidadPago)"/>
          </span>

        </div>
        <div class="col-xs-6 date-block">
          <span class="fields-results text-bold">

          </span>
          <span>
            <xsl:variable name="fechaIniPago" select="/cfdi:Comprobante/cfdi:Complemento/nomina12:Nomina/@FechaInicialPago" />
            <xsl:value-of select="dateFormat:GetDate( $fechaIniPago)"/>
          </span> -
          <span>
            <xsl:variable name="fechaFinalPago" select="/cfdi:Comprobante/cfdi:Complemento/nomina12:Nomina/@FechaFinalPago" />
            <xsl:value-of select="dateFormat:GetDate( $fechaFinalPago)"/>
          </span>
        </div>
      </div>
      <div class="row">
        <div class="col-xs-3">
          <span class="title-fields">
            Días de Pago:
          </span>
        </div>
        <div class="col-xs-8">
          <span class="fields-results">
            <xsl:apply-templates select="/cfdi:Comprobante/cfdi:Complemento/nomina12:Nomina/@NumDiasPagados" />
          </span>
        </div>
      </div>
      <div class="row">
        <div class="col-xs-3">
          <span class="title-fields">
            Fecha de pago:
          </span>
        </div>
        <div class="col-xs-8">
          <span>
            <xsl:variable name="fechaPago" select="/cfdi:Comprobante/cfdi:Complemento/nomina12:Nomina/@FechaPago" />
            <xsl:value-of select="dateFormat:GetDate( $fechaPago)"/>
          </span>
        </div>
      </div>
      <div class="row">
        <div class="col-xs-3">
          <span class="title-fields">
            Puesto:
          </span>
        </div>
        <div class="col-xs-8">
          <span>
            <xsl:value-of select="./@Puesto"/>
          </span>
        </div>
      </div>
      <div class="row">
        <div class="col-xs-3">
          <span class="title-fields">
            Depto:
          </span>
        </div>
        <div class="col-xs-8">
          <span>
            <xsl:value-of select="./@Departamento"/>
          </span>
        </div>
      </div>
      <div class="row">
        <div class="col-xs-3">
          <span class="title-fields">
            SBC: $
          </span>
        </div>
        <div class="col-xs-8">
          <span>
            <xsl:variable name="sbcApor" select="./@SalarioBaseCotApor" />
            <xsl:value-of select="convert:AmountSeparator( $sbcApor)"/>
          </span>
        </div>
      </div>

    </div>
  </xsl:template>
  <xsl:template match="nomina12:Percepciones">
    <xsl:for-each select="./nomina12:Percepcion">
      <xsl:apply-templates select="."/>
    </xsl:for-each>

  </xsl:template>
  <xsl:template match="nomina12:Percepcion">
    <tr>
      <td>
        <xsl:value-of select="./@TipoPercepcion"/>
      </td>
      <td>
        <xsl:value-of select="./@Clave"/>
        <xsl:variable name="tipoPercepcion" select="./@TipoPercepcion" />
      </td>
      <td class="title-start">
        <xsl:value-of select="./@Concepto"/>
      </td>
      <td class="text-right">
        <xsl:variable name="importeGravado" select="./@ImporteGravado" />
        <xsl:value-of select="convert:AmountSeparator( $importeGravado)"/>
      </td>
      <td class="text-right">
        <xsl:variable name="importeExento" select="./@ImporteExento" />
        <xsl:value-of select="convert:AmountSeparator( $importeExento)"/>
      </td>
      <xsl:variable name="importeGravado" select="./@ImporteGravado" />
      <xsl:variable name="importeExento" select="./@ImporteExento" />
      <td class="text-right">
        <xsl:value-of select="convert:AmountSeparator(sum($importeGravado | $importeExento))"/>
      </td>
    </tr>

  </xsl:template>
  <xsl:template match="nomina12:Deducciones">
    <xsl:for-each select="./nomina12:Deduccion">
      <xsl:apply-templates select="."/>
    </xsl:for-each>

  </xsl:template>
  <xsl:template match="nomina12:Deduccion">
    <tr>
      <td>
        <xsl:value-of select="./@TipoDeduccion"/>
        <xsl:variable name="tipoDeduccion" select="./@TipoDeduccion" />
      </td>
      <td>
        <xsl:value-of select="./@Clave"/>
      </td>
      <td class="title-start">
        <xsl:value-of select="./@Concepto"/>
      </td>
      <td class="text-right">
        <xsl:variable name="deduccionImporte" select="./@Importe" />
        <xsl:value-of select="convert:AmountSeparator( $deduccionImporte)"/>
      </td>
    </tr>

  </xsl:template>
  <xsl:template match="nomina12:OtrosPagos">
    <xsl:for-each select="./nomina12:OtroPago">
      <xsl:apply-templates select="."/>
    </xsl:for-each>

  </xsl:template>
  <xsl:template match="nomina12:OtroPago">
    <tr>
      <td>
        <xsl:value-of select="./@TipoOtroPago"/>
        <xsl:variable name="tipoOtroPago" select="./@TipoOtroPago" />
      </td>
      <td>
        <xsl:value-of select="./@Clave"/>
      </td>
      <td class="title-start">
        <xsl:value-of select="./@Concepto"/>
      </td>
      <td class="text-right">
        <xsl:variable name="importeGravadoOP" select="./@ImporteGravado" />
        <xsl:value-of select="convert:AmountSeparator( $importeGravadoOP)"/>
      </td>
      <td class="text-right">
        <xsl:variable name="importeExentoOP" select="./@ImporteExento" />
        <xsl:value-of select="convert:AmountSeparator( $importeExentoOP)"/>
      </td>
      <td class="text-right">
        <xsl:variable name="importeOP" select="./@Importe" />
        <xsl:value-of select="convert:AmountSeparator( $importeOP)"/>
      </td>
    </tr>

  </xsl:template>
</xsl:stylesheet>