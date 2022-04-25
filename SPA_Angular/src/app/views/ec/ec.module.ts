import { WorkListComponent } from './work-list/work-list.component';
import { SettingWorkplanComponent } from './setting-workplan/setting-workplan.component';
import { TotalChemicalModalComponent } from './schedule-detail-workplan/totalChemicalModal/totalChemicalModal.component';
import { TotalInkModalComponent } from './schedule-detail-workplan/totalInkModal/totalInkModal.component';
import { PrintQrcodeGlueWorkplanComponent } from './schedule-detail-workplan/print-qrcode-glue-workplan/print-qrcode-glue-workplan.component';
import { PrintQrcodeWorkplanComponent } from './schedule-detail-workplan/print-qrcode-workplan/print-qrcode-workplan.component';
import { DetailDirective } from './../../_core/_directive/detail.directive';
import { ScheduleDetailWorkplanComponent } from './schedule-detail-workplan/schedule-detail-workplan.component';
import { ProcessComponent } from './process/process.component';
import { ChemicalModalComponent } from './chemical/chemical-modal/chemical-modal.component';
// Angular
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgModule } from '@angular/core';
import { NgxSpinnerModule } from 'ngx-spinner';
// Components Routing
import { ECRoutingModule } from './ec-routing.module';
import { NgSelectModule } from '@ng-select/ng-select';

import { GlueComponent } from './glue/glue.component';
import { GlueModalComponent } from './glue/glue-modal/glue-modal.component';
import { DropDownListModule } from '@syncfusion/ej2-angular-dropdowns';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
// Import ngx-barcode module
import { BarcodeGeneratorAllModule, DataMatrixGeneratorAllModule } from '@syncfusion/ej2-angular-barcode-generator';
import { ChartAllModule, AccumulationChartAllModule, RangeNavigatorAllModule } from '@syncfusion/ej2-angular-charts';
import { SwitchModule, RadioButtonModule } from '@syncfusion/ej2-angular-buttons';
import { GridAllModule } from '@syncfusion/ej2-angular-grids';
import { TreeGridAllModule } from '@syncfusion/ej2-angular-treegrid';
import { ButtonModule } from '@syncfusion/ej2-angular-buttons';

import { SuppilerComponent } from './suppiler/suppiler.component';
import { BuildingComponent } from './building/building.component';
import { BuildingUserComponent } from './building-user/building-user.component';
import { DatePickerModule } from '@syncfusion/ej2-angular-calendars';
import { AccountComponent } from './account/account.component';
import { BuildingModalComponent } from './building/building-modal/building-modal.component';
import { QRCodeGeneratorAllModule } from '@syncfusion/ej2-angular-barcode-generator';
import { MaskedTextBoxModule } from '@syncfusion/ej2-angular-inputs';
import { HttpClient } from '@angular/common/http';
import { TranslateModule, TranslateLoader } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { ToolbarModule } from '@syncfusion/ej2-angular-navigations';
// AoT requires an exported function for factories
export function HttpLoaderFactory(http: HttpClient) {
  return new TranslateHttpLoader(http, './assets/i18n/', '.json');
}

import { AutofocusDirective } from './focus.directive';
import { AutoSelectDirective } from './select.directive';
import { SearchDirective } from './search.directive';
import { SelectTextDirective } from './select.text.directive';
import { TooltipModule } from '@syncfusion/ej2-angular-popups';
import { TagInputModule } from 'ngx-chips';
import { TimePickerModule } from '@syncfusion/ej2-angular-calendars';
import { DateTimePickerModule } from '@syncfusion/ej2-angular-calendars';
import { Ng2SearchPipeModule } from 'ng2-search-filter';
import { SelectQrCodeDirective } from './select.qrcode.directive';
import { ScheduleDetailComponent } from './schedule-detail/schedule-detail.component';
import { InkComponent } from './ink/ink.component';
import { InkModalComponent } from './ink/ink-modal/ink-modal.component';
import { ChemicalComponent } from './chemical/chemical.component';
import { ScheduleComponent } from './schedule/schedule.component';
import { CheckBoxModule } from '@syncfusion/ej2-angular-buttons';
import { NgxPrettyCheckboxModule } from 'ngx-pretty-checkbox';
import { ScheduleStatusComponent } from './schedule-status/schedule-status.component';
import { TreamentWayComponent } from './treament-way/treament-way.component';
import { StockComponent } from './stock/stock.component';
import { PrintQrcodeComponent } from './ink/print-qrcode/print-qrcode.component';
import { ChemicalPrintQrcodeComponent } from './chemical/chemical-print-qrcode/chemical-print-qrcode.component';
import { WorkplanComponent } from './workplan/workplan.component';
import { ColorPickerModule } from 'ngx-color-picker';
import { TextareaAutosizeModule } from 'ngx-textarea-autosize';
const lang = localStorage.getItem('lang');
let defaultLang: any;
if (lang) {
  defaultLang = lang;
} else {
  defaultLang = 'vi';
}
@NgModule({
  imports: [
    ColorPickerModule,
    TextareaAutosizeModule,
    ToolbarModule,
    TagInputModule,
    NgxPrettyCheckboxModule,
    ButtonModule,
    CheckBoxModule ,
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    NgxSpinnerModule,
    ECRoutingModule,
    NgSelectModule,
    DropDownListModule,
    NgbModule,
    ChartAllModule,
    AccumulationChartAllModule,
    RangeNavigatorAllModule,
    BarcodeGeneratorAllModule,
    QRCodeGeneratorAllModule,
    DataMatrixGeneratorAllModule,
    SwitchModule,
    MaskedTextBoxModule,
    DatePickerModule,
    TreeGridAllModule,
    GridAllModule,
    RadioButtonModule,
    TooltipModule,
    TimePickerModule ,
    Ng2SearchPipeModule,
    DateTimePickerModule,
    TranslateModule.forChild({
      loader: {
        provide: TranslateLoader,
        useFactory: HttpLoaderFactory,
        deps: [HttpClient]
      },
      defaultLanguage: defaultLang
    }),
  ],
  declarations: [
    GlueComponent,
    GlueModalComponent,
    SuppilerComponent,
    BuildingComponent,
    BuildingModalComponent,
    BuildingUserComponent,
    AccountComponent,
    AutofocusDirective,
    SelectTextDirective,
    DetailDirective,
    AutoSelectDirective,
    SearchDirective,
    SelectQrCodeDirective,
    ScheduleDetailComponent,
    ScheduleDetailWorkplanComponent,
    InkComponent,
    InkModalComponent,
    ChemicalComponent,
    ChemicalModalComponent,
    ScheduleComponent,
    ScheduleStatusComponent,
    TreamentWayComponent,
    StockComponent,
    PrintQrcodeComponent,
    ChemicalPrintQrcodeComponent,
    WorkplanComponent,
    ProcessComponent,
    PrintQrcodeWorkplanComponent,
    PrintQrcodeGlueWorkplanComponent,
    TotalInkModalComponent,
    TotalChemicalModalComponent,
    SettingWorkplanComponent,
    WorkListComponent
  ]
})
export class ECModule { }
