import { WorkListComponent } from './work-list/work-list.component';
import { SettingWorkplanComponent } from './setting-workplan/setting-workplan.component';
import { PrintQrcodeGlueWorkplanComponent } from './schedule-detail-workplan/print-qrcode-glue-workplan/print-qrcode-glue-workplan.component';
import { ScheduleDetailWorkplanComponent } from './schedule-detail-workplan/schedule-detail-workplan.component';
import { ProcessComponent } from './process/process.component';
import { WorkplanComponent } from './workplan/workplan.component';
import { ChemicalPrintQrcodeComponent } from './chemical/chemical-print-qrcode/chemical-print-qrcode.component';
import { StockComponent } from './stock/stock.component';
import { TreamentWayComponent } from './treament-way/treament-way.component';
import { ScheduleStatusComponent } from './schedule-status/schedule-status.component';
import { ScheduleComponent } from './schedule/schedule.component';
import { ChemicalComponent } from './chemical/chemical.component';
import { InkComponent } from './ink/ink.component';
import { ScheduleDetailComponent } from './schedule-detail/schedule-detail.component';
import { SuppilerComponent } from './suppiler/suppiler.component';
import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { GlueComponent } from './glue/glue.component';
import { GlueResolver } from '../../_core/_resolvers/glue.resolver';
import { BuildingComponent } from './building/building.component';
import { BuildingUserComponent } from './building-user/building-user.component';
import { AccountComponent } from './account/account.component';
import { PrintQrcodeComponent } from './ink/print-qrcode/print-qrcode.component';
import { PrintQrcodeWorkplanComponent } from './schedule-detail-workplan/print-qrcode-workplan/print-qrcode-workplan.component';

const routes: Routes = [
  {
    path: '',
    data: {
      title: 'ec',
      breadcrumb: 'Home'
    },

    children: [

      // setting
      {
        path: 'setting',
        data: {
          title: 'setting',
          breadcrumb: 'Setting'
        },
        children: [
          {
            path: 'chemical',
            // component: ChemicalComponent,
            data: {
              title: 'Chemical',
              breadcrumb: 'Chemical',
              url: 'ink/setting/chemical'
            },
            children: [
              {
                path: '',
                component: ChemicalComponent,
              },
              {
                path: 'print-qrcode/:id/:code/:name',
                component: ChemicalPrintQrcodeComponent,
                data: {
                  title: 'Print QRCode',
                  breadcrumb: 'Print QRCode'
                }
              }
            ]
          },
          {
            path: 'ink',
            // component: IngredientComponent,
            data: {
              title: 'Ink',
              breadcrumb: 'Ink',
              url: 'ink/setting/ink'
            },
            children: [
              {
                path: '',
                component: InkComponent,
              },
              {
                path: 'print-qrcode/:id/:code/:name',
                component: PrintQrcodeComponent,
                data: {
                  title: 'Print QRCode',
                  breadcrumb: 'Print QRCode'
                }
              }
            ]
          },
          {
            path: 'treatment-way',
            component: TreamentWayComponent,
            data: {
              title: 'Treatment Way',
              breadcrumb: 'Treatment Way'
            }
          },
          {
            path: 'treatment',
            component: ProcessComponent,
            data: {
              title: 'Treatment',
              breadcrumb: 'Treatment'
            }
          },
          {
            path: 'account-1',
            component: AccountComponent,
            data: {
              title: 'account',
              breadcrumb: 'Account'
            }
          },
          {
            path: 'account-2',
            component: BuildingUserComponent,
            data: {
              title: 'Account 2',
              breadcrumb: 'Account 2'
            }
          },
          {
            path: 'building',
            component: BuildingComponent,
            data: {
              title: 'Building',
              breadcrumb: 'Building'
            }
          },
          {
            path: 'supplier',
            component: SuppilerComponent,
            data: {
              title: 'Suppiler',
              breadcrumb: 'Suppiler'
            }
          },

          {
            path: 'glue',
            component: GlueComponent,
            resolve: { glues: GlueResolver },
            data: {
              title: 'Glue',
              breadcrumb: 'Glue'
            }
          },
        ]
      },
      // end setting

      // establish
      {
        path: 'establish',
        data: {
          title: 'Establish',
          breadcrumb: 'Establish'
        },
        children: [
          {
            path: 'schedule',
            data: {
              title: 'Schedule',
              breadcrumb: 'Schedule'
            },
            children: [
              {
                path: '',
                component: ScheduleComponent
              },
              {
                path: 'detail',
                data: {
                  title: 'Detail',
                  breadcrumb: 'Detail'
                },
                children: [
                  {
                    path: '',
                    component: ScheduleDetailComponent,
                  },
                  {
                    path: ':id',
                    component: ScheduleDetailComponent,
                    // data: {
                    //   breadcrumb: ''
                    // }
                  }
                  
                ]
              },
              {
                path: 'detail-workplan',
                data: {
                  title: 'Detail Workplan',
                  breadcrumb: 'Detail Workplan'
                },
                children: [
                  {
                    path: '',
                    component: ScheduleDetailWorkplanComponent,
                  },
                  {
                    path: ':id',
                    component: ScheduleDetailWorkplanComponent,
                    // data: {
                    //   breadcrumb: ''
                    // }
                  },
                  {
                    path: ':id/:treatment',
                    component: ScheduleDetailWorkplanComponent,
                    children: [
                      
                    ]
                    // data: {
                    //   breadcrumb: ''
                    // }
                  },
                  {
                    path: ':id/:treatment/print-qrcode/:ink',
                    component: PrintQrcodeWorkplanComponent,
                  },
                  {
                    // path: ':id/:treatment/print-glue/:qty/:article',
                    path: ':id/:treatment/print-glue',
                    component: PrintQrcodeGlueWorkplanComponent,
                    // data: {
                    //   breadcrumb: ''
                    // }
                  }
                ]
              }
            ]
          },
          {
            path: 'schedule-status',
            data: {
              title: 'Schedule Status',
              breadcrumb: 'Schedule Status'
            },
            component: ScheduleStatusComponent
          }
        ]
      },
      // end establish

      // manage
      {
        path: 'manage',
        data: {
          title: 'Manage',
          breadcrumb: 'Manage'
        },
        children: [
          {
            path: 'workplan',
            data: {
              title: 'Workplan',
              breadcrumb: 'Work Plan'
            }
          },
        ]
      },
      // end manage

      // execution
      {
        path: 'execution',
        data: {
          title: 'Execution',
          breadcrumb: 'Execution'
        },
        children: [

          {
            path: 'stock',
            // component: SummaryComponent,
            data: {
              title: 'stock',
              breadcrumb: 'stock'
            },
            children: [
              {
                path: '',
                component: StockComponent,
              },
            ]
          },

          {
            path: 'work-plan',
            component: WorkplanComponent,
            data: {
              title: 'Work plan',
              breadcrumb: 'Work plan'
            },
          }

        ]
      },
      // end execution

      // dispatch
      {
        path: 'dispatch',
        data: {
          title: 'Dispatch',
          breadcrumb: 'Dispatch'
        },
        children: [

          {
            path: 'setting-workplan',
            // component: SummaryComponent,
            data: {
              title: 'Setting WorkPlan',
              breadcrumb: 'Setting WorkPlan'
            },
            children: [
              {
                path: '',
                component: SettingWorkplanComponent,
              },
            ]
          },

          {
            path: 'work-list',
            component: WorkListComponent,
            data: {
              title: 'Work list',
              breadcrumb: 'Work list'
            },
          }

        ]
      },

    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ECRoutingModule { }
